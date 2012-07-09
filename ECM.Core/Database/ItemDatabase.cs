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
        static Dictionary<long, EveCertificate> m_Certificates = new Dictionary<long, EveCertificate>();
        static Dictionary<long, EveCertGroup> m_CertGroups = new Dictionary<long, EveCertGroup>();

        public static Dictionary<long, EveCertGroup> CertificateGroups
        {
            get
            {
                return m_CertGroups;
            }
        }

        public static Dictionary<long, EveCertificate> Certificates
        {
            get
            {
                return m_Certificates;
            }
        }

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

            LoadCertificates();
	        
	        CloseDatabase();
	    }

        private static void LoadCertificates()
        {
            // Get Categories
            string selectCmd = "SELECT * FROM crtCategories";

            SqlCmd cmd = sqlConnection.CreateCommand();
            cmd.CommandText = selectCmd;
            SqlReader row = cmd.ExecuteReader();

            while (row.Read())
            {
                long groupID = Convert.ToInt64(row[0]);
                string groupDesc = row[1].ToString();
                string groupName = row[2].ToString();

                EveCertGroup newGroup = new EveCertGroup();
                newGroup.Name = groupName;
                newGroup.ID = groupID;
                newGroup.Description = groupDesc;

                m_CertGroups.Add(newGroup.ID, newGroup);
            }

            row.Close();

            // Get Certificates
            selectCmd = @"SELECT crtCertificates.certificateID, crtCertificates.categoryID, crtClasses.className, crtCertificates.grade, crtCertificates.corpID, crtCertificates.description, crtCertificates.classID
                            FROM crtCertificates 
                            INNER JOIN crtClasses ON crtCertificates.classID = crtClasses.classID";

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = selectCmd;
            row = cmd.ExecuteReader();

            while (row.Read())
            {
                long certID = Convert.ToInt64(row[0]);
                long groupID = Convert.ToInt64(row[1]);
                string certName = row[2].ToString();
                int certGrade = Convert.ToInt32(row[3]);
                long corpID = Convert.ToInt64(row[4]);
                string certDesc = row[5].ToString();
                long classID = Convert.ToInt64(row[6]);

                EveCertificate newCert = new EveCertificate();
                newCert.Name = certName;
                newCert.ID = certID;
                newCert.GroupID = groupID;
                newCert.Grade = certGrade;
                newCert.CorpID = corpID;
                newCert.Description = certDesc;
                newCert.ClassID = classID;

                m_Certificates.Add(newCert.ID, newCert);
            }

            row.Close();

            // Link Certificate Requirements
            selectCmd = "SELECT parentID, parentTypeID, parentLevel, childID FROM crtRelationships";

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = selectCmd;
            row = cmd.ExecuteReader();

            while (row.Read())
            {
                long parentID = -1, childID = -1;
                int parentLevel = -1;

                bool isSkill = row[0] is DBNull;
                childID = Convert.ToInt64(row[3]);

                if (isSkill)
                {
                    parentID = Convert.ToInt64(row[1]);
                    parentLevel = Convert.ToInt32(row[2]);
                }
                else
                {
                    parentID = Convert.ToInt64(row[0]);
                }

                EveCertificateRequirement newReq = new EveCertificateRequirement();
                EveCertificate cert = m_Certificates[childID];

                newReq.RequirementID = parentID;
                newReq.RequirementLevel = parentLevel;
                newReq.RequirementIsSkill = isSkill;

                cert.Requirements.Add(newReq);
            }

            row.Close();

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

            row.Close();
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

                GetRequiredSkills(newItem);
				
				m_Items.Add(typeID, newItem);
	        }

            row.Close();

            // Load skills seperately so we can give them all their data
            LoadSkills();
	    }

        static void LoadSkills ()
        {
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

                GetRequiredSkills(newSkill);

                skills.Add(newSkill);
            }
            
            row.Close();

            foreach(EveSkill skill in skills)
            {
                // Get Skill Attributes
                // Should maybe think of a better way to do this
                // Maybe some type of attribute?
                int attID = 0;
                try
                {
                    cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = string.Format("SELECT * FROM dgmTypeAttributes WHERE typeID = {0} AND (attributeId = 275 OR attributeId = 180 OR attributeId = 181)", skill.ID);
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
        }

        public static void LoadCertificateTree(Gtk.TreeStore certStore)
        {
            foreach (EveCertGroup group in m_CertGroups.Values)
            {
                Gtk.TreeIter groupIter = certStore.AppendValues(group.Name, -1, group.ID, true);
                group.TreeReference = new Gtk.TreeRowReference(certStore, certStore.GetPath(groupIter));
            }

            foreach (EveCertificate cert in m_Certificates.Values)
            {
                EveCertGroup group = m_CertGroups[cert.GroupID];
                Gtk.TreeIter groupIter;
                Gtk.TreeIter certIter;

                if (certStore.GetIter(out groupIter, group.TreeReference.Path))
                {
                    certIter = certStore.AppendValues(groupIter, cert.Name, cert.Grade, cert.ID, false);
                    cert.TreeReference = new Gtk.TreeRowReference(certStore, certStore.GetPath(certIter));
                }
            }
        }

		public static void LoadMarket (Gtk.TreeStore marketStore, Gtk.ListStore itemStore)
		{
			LoadMarket();

			foreach(EveMarketGroup group in m_MarketGroups.Values)
			{
                Gtk.TreeIter groupIter;
                Gtk.TreeIter  parentIter;

                if (group.ParentID > -1 && marketStore.GetIter(out parentIter, m_MarketGroups[group.ParentID].TreeReference.Path))
                {
                    groupIter = marketStore.AppendNode(parentIter);
                }
                else
                {
                    groupIter = marketStore.AppendNode();
                }

                group.TreeReference = new Gtk.TreeRowReference(marketStore, marketStore.GetPath(groupIter));
				marketStore.SetValues(groupIter, new Gdk.Pixbuf(ItemDatabase.GetMarketIconStream(group.IconString)), group.Name, group.ID, group.HasItems, true);
			}

            foreach (EveItem item in m_Items.Values)
            {
                if (item.MarketGroupID > -1)
                {
                    Gtk.TreeIter parentIter;
                    if (marketStore.GetIter(out parentIter, m_MarketGroups[item.MarketGroupID].TreeReference.Path))
                    {
                        Gtk.TreeIter childIter = marketStore.AppendValues(parentIter, null, item.Name, item.ID, false, false);
                        itemStore.AppendValues(item.Name, item.ID);

                        item.TreeReference = new Gtk.TreeRowReference(marketStore, marketStore.GetPath(childIter));
                    }
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
                    skillsStore.SetValues(groupIter, group.Name, 0, 0, 0, -1, 0, false, group.ID, 0, true);
                    skillGroups.Add(group.ID, groupIter);
                }
            }

            Console.WriteLine("Groups added");

            foreach (EveItem item in m_Items.Values)
            {
                //Console.WriteLine("Checking item {0}", item.Name);
                if (skillGroups.ContainsKey(item.MarketGroupID))
                {
                    EveSkill skill = item as EveSkill;
                    Gtk.TreeIter parentIter = skillGroups[item.MarketGroupID];

                    skillsStore.AppendValues(parentIter, skill.Name, skill.Rank, -1, -1, 0, 0, false, skill.ID, 0, false);
                }
            }

            sw.Stop ();
            Console.WriteLine("Skill TreeStore created in {0}", sw.Elapsed);
        }

        static void GetRequiredSkills (EveItem item)
        {            
            SqlCmd cmd = sqlConnection.CreateCommand();
            cmd.CommandText = string.Format(@"SELECT * FROM dgmTypeAttributes INNER JOIN dgmAttributeTypes ON dgmTypeAttributes.attributeID = dgmAttributeTypes.attributeID 
                                                WHERE typeId = {0} AND categoryId = 8", item.ID);
            SqlReader row = cmd.ExecuteReader();

            while(row.Read())
            {
                string attributeName = row["attributeName"].ToString();
                int reqSkillIdx = attributeName.Replace("requiredSkill", "")[0] - 49;

                string value = row["valueInt"] is DBNull ? row["valueFloat"].ToString() : row["valueInt"].ToString();

                if(attributeName.Contains("Level"))
                {
                    // Required Skill Level
                    item.RequiredSkills[reqSkillIdx].SkillLevel = int.Parse(value);
                }
                else
                {
                    // Required Skill ID
                    item.RequiredSkills[reqSkillIdx].SkillID = long.Parse(value);
                }
            }

            row.Close();
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

