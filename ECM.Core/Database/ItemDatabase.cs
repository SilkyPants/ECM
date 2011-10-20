using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using System.IO;

using SqlConn = System.Data.SQLite.SQLiteConnection;
using SqlCmd = System.Data.SQLite.SQLiteCommand;
using SqlReader = System.Data.SQLite.SQLiteDataReader;
using System.Data;

namespace ECM
{
	public class ItemDatabase
	{		
		static SqlConn sqlConnection = null;
    
	    static string itemDatabasePath = "Resources/Database/eveItems.db";
        static string skillsDatabasePath = "Resources/Database/eveSkills.db";
        static string certsDatabasePath = "Resources/Database/eveCertificates.db";
		
		static Dictionary<long, EveMarketGroup> m_MarketGroups = new Dictionary<long, EveMarketGroup>();
        static Dictionary<long, EveItem> m_Items = new Dictionary<long, EveItem>();

        public static Dictionary<long, EveItem> Items
        {
            get
            {
                return m_Items;
            }
        }

        public static Dictionary<long, EveMarketGroup> MarketGroups
        {
            get
            {
                return m_MarketGroups;
            }
        }
	    
	    private static bool OpenDatabase()
	    {
	        sqlConnection = new SqlConn(string.Format("Data Source={0};version=3;", itemDatabasePath));
	        sqlConnection.Open();
	    
	        SqlCmd cmd = sqlConnection.CreateCommand();
	        cmd.CommandText = "PRAGMA cache_size=5000";
	        cmd.ExecuteNonQuery();

	        cmd = sqlConnection.CreateCommand();
	        cmd.CommandText = "PRAGMA count_changes=OFF";
	        cmd.ExecuteNonQuery();
	    
	        cmd = sqlConnection.CreateCommand();
	        cmd.CommandText = "PRAGMA temp_store=MEMORY";
	        cmd.ExecuteNonQuery();
	    
	        cmd = sqlConnection.CreateCommand();
	        cmd.CommandText = string.Format("ATTACH DATABASE \'{0}\' AS {1}", skillsDatabasePath, 
	                                     System.IO.Path.GetFileNameWithoutExtension(skillsDatabasePath));
	        cmd.ExecuteNonQuery();
     
            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = string.Format("ATTACH DATABASE \'{0}\' AS {1}", certsDatabasePath,
                                      System.IO.Path.GetFileNameWithoutExtension(certsDatabasePath));
            cmd.ExecuteNonQuery();
	    
	        cmd.Dispose();
	     
	        return true;
	    }

	    private static void CloseDatabase()
	    {
	        if(sqlConnection.State != System.Data.ConnectionState.Open) return;
	        
	        sqlConnection.Close();
	    }
	    
	    private static void LoadMarket()
	    {
	        OpenDatabase();
	        
	        LoadMarketGroups();
            LoadItems();
	        
	        CloseDatabase();
	    }
	    
	    private static void LoadMarketGroups()
	    {
	     	string selectCmd = "SELECT marketGroupID, marketGroupName, parentGroupID, hasTypes, iconFile FROM invMarketGroups ORDER BY hasTypes, parentGroupID, marketGroupID";
	     
	        SqlCmd cmd = sqlConnection.CreateCommand();
	        cmd.CommandText = selectCmd;
	        SqlReader row = cmd.ExecuteReader();

	        while(row.Read())
	        {
	            string groupName = row[1].ToString();
	            long groupID = Convert.ToInt64(row[0]);
	            long parentID = row[2] is DBNull ? -1 : Convert.ToInt64(row[2]);
                bool hasItems = row[3] is DBNull ? false : Convert.ToInt32(row[3]) == 1;
                string iconFile = row[4] is DBNull ? string.Empty : row[4].ToString();
				
				EveMarketGroup newGroup = new EveMarketGroup();
				newGroup.Name = groupName;
				newGroup.ID = groupID;
				newGroup.ParentID = parentID;
                newGroup.HasItems = hasItems;
                newGroup.IconString = iconFile;
				
				m_MarketGroups.Add(groupID, newGroup);
	        }
	    }
	    
	    private static void LoadItems()
	    {
	        SqlCmd cmd = sqlConnection.CreateCommand();
            cmd.CommandText = string.Format("SELECT typeID, typeName, marketGroupID, description FROM invTypes");
	        SqlReader row = cmd.ExecuteReader();
	        
	        while(row.Read())
	        {
                string itemName = row[1].ToString();
                long typeID = Convert.ToInt64(row[0]);
                long mgID = row[2] is DBNull ? -1 : Convert.ToInt64(row[2]);
                string itemDesc = row[3].ToString();

                EveItem newItem = new EveItem();
				newItem.Name = itemName;
                newItem.Description = itemDesc;
				newItem.ID = typeID;
                newItem.MarketGroupID = mgID;
				newItem.IconString = "TYPEID";
				
				m_Items.Add(typeID, newItem);
	        }

            row.Close();

            // Load skills seperately so we can give them all their data
            LoadSkills();
	    }

        static void LoadSkills ()
        {
            Console.WriteLine("Loading skills");
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            List<EveSkill> skills = new List<EveSkill>();

            SqlCmd cmd = sqlConnection.CreateCommand();
            cmd.CommandText = string.Format("SELECT typeID, typeName, marketGroupID, description FROM invSkills");
            SqlReader row = cmd.ExecuteReader();

            while(row.Read())
            {
                string itemName = row[1].ToString();
                long typeID = Convert.ToInt64(row[0]);
                long mgID = row[2] is DBNull ? -1 : Convert.ToInt64(row[2]);
                string itemDesc = row[3].ToString();
            
                EveSkill newSkill = new EveSkill();
                newSkill.Name = itemName;
                newSkill.Description = itemDesc;
                newSkill.ID = typeID;
                newSkill.MarketGroupID = mgID;
                newSkill.IconString = "TYPEID";

                skills.Add(newSkill);
            }
            
            row.Close();
            sw.Stop ();
            Console.WriteLine("Skills loaded in {0}", sw.Elapsed);

            Console.WriteLine("Filling skill attributes");
            sw = System.Diagnostics.Stopwatch.StartNew();
            foreach(EveSkill skill in skills)
            {
                // Get Skill Attributes
                // Should maybe think of a better way to do this
                // Maybe some type of attribute?
                int attID = 0;
                try
                {
                    if(skill.ID == 3313)
                        Console.WriteLine("stop");

                    cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = string.Format("SELECT * FROM dgmTypeAttributes WHERE typeID = {0}", skill.ID);
                    row = cmd.ExecuteReader();

                    if(row.HasRows)
                    {
                        while(row.Read())
                        {
                            object val = row[2] is DBNull ? row[3] : row[2];
                            attID = Convert.ToInt32(row[1]);
    
                            if(attID == 275)
                            {
                                skill.Rank = Convert.ToInt32(val);
                            }
                            else if (attID == 180)
                            {
                                skill.PrimaryAttribute = (SkillAttributes)(Convert.ToInt32(val));
                            }
                            else if (attID == 181)
                            {
                                skill.SecondaryAttribute = (SkillAttributes)(Convert.ToInt32(val));
                            }
                        }
        
                        m_Items.Add(skill.ID, skill);
                    }
                    else
                    {
                        Console.WriteLine("Skill {0} ({1}) has no attributes!", skill.Name, skill.ID);
                    }

                    row.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} ({1} ID:{2})", e.Message, attID, skill.ID);
                }
            }

            sw.Stop ();
            Console.WriteLine("Skill attributes loaded in {0}", sw.Elapsed);
        }

		public static void LoadMarket (Gtk.TreeStore marketStore, Gtk.ListStore itemStore)
		{
			LoadMarket();

			foreach(EveMarketGroup group in m_MarketGroups.Values)
			{
                Gtk.TreeIter groupIter;

                if (group.ParentID > -1)
                {
                    Gtk.TreeIter parentIter = (Gtk.TreeIter)m_MarketGroups[group.ParentID].Tag;
                    groupIter = marketStore.AppendNode(parentIter);
                }
                else
                {
                    groupIter = marketStore.AppendNode();
                }

				marketStore.SetValues(groupIter, new Gdk.Pixbuf(ItemDatabase.GetMarketIconStream(group.IconString)), group.Name, group.ID, group.HasItems);
				group.Tag = groupIter;
			}

            foreach (EveItem item in m_Items.Values)
            {
                if (item.MarketGroupID > -1)
                {
                    Gtk.TreeIter parentIter = (Gtk.TreeIter)m_MarketGroups[item.MarketGroupID].Tag;
                    marketStore.AppendValues(parentIter, null, item.Name, item.ID);
                    itemStore.AppendValues(item.Name, item.ID);
                }
            }
		}

        public static void LoadSkills(Gtk.TreeStore skillsStore)
        {
            Console.WriteLine("Creating Skill TreeStore");
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Dictionary<long, Gtk.TreeIter> skillGroups = new Dictionary<long, Gtk.TreeIter>();
            foreach (EveMarketGroup group in m_MarketGroups.Values)
            {
                if (group.ParentID == 150)
                {
                    Gtk.TreeIter groupIter = skillsStore.AppendNode();
                    skillsStore.SetValues(groupIter, group.Name, 0, 0, 0, -1, 0, false, group.ID);
                    skillGroups.Add(group.ID, groupIter);
                }
            }

            foreach (EveItem item in m_Items.Values)
            {
                if (skillGroups.ContainsKey(item.MarketGroupID))
                {
                    EveSkill skill = item as EveSkill;
                    Gtk.TreeIter parentIter = skillGroups[item.MarketGroupID];

                    skillsStore.AppendValues(parentIter, skill.Name, skill.Rank, -1, -1, 0, 0, false, skill.ID);
                }
            }

            sw.Stop ();
            Console.WriteLine("Skill TreeStore created in {0}", sw.Elapsed);
        }

        private static Stream GetMarketIconStream(string iconFile)
        {
            Assembly ass = Assembly.GetExecutingAssembly();

            if (ass != null && string.IsNullOrEmpty(iconFile) == false)
            {
                Stream s = ass.GetManifestResourceStream(string.Format("ECM.Resources.Icons.MarketIcons.icon{0}.png", iconFile));

                if(s == null)
                {
                    Console.WriteLine("Error: Cannot load ECM.Resources.Icons.MarketIcons.icon{0}.png", iconFile);
                    return Core.MarketGroupPNG;
                }

                return s;
            }

            return Core.MarketGroupPNG;
        }
	}
}

