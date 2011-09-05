using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;

namespace ECM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch watch = Stopwatch.StartNew();
            SQLiteConnection conn = new SQLiteConnection("Data Source=eveItems.db");
            conn.Open();

            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA cache_size=5000";
            cmd.ExecuteNonQuery();

            cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA count_changes=OFF";
            cmd.ExecuteNonQuery();

            cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA temp_store=MEMORY";
            cmd.ExecuteNonQuery();

            cmd = conn.CreateCommand();
            cmd.CommandText = "ATTACH DATABASE \'eveSkills.db\' AS eveSkills";
            cmd.ExecuteNonQuery();

            cmd.Dispose();

            SQLiteDataAdapter adp = new SQLiteDataAdapter("SELECT marketGroupID, marketGroupName FROM invMarketGroups WHERE parentGroupID IS NULL", conn);
            DataTable res = new DataTable();
            adp.Fill(res);
            adp.Dispose();

            foreach (DataRow row in res.Rows)
            {
                TreeNode group = new TreeNode(row[1].ToString());
                long groupID = Convert.ToInt64(row[0]);

                group.Tag = groupID;

                group.Nodes.AddRange(GetMarketGroups(conn, groupID));
                group.Nodes.AddRange(GetMarketItems(conn, groupID));

                treeView1.Nodes.Add(group);
            }

            res.Dispose();

            conn.Close();
            watch.Stop();

            conn.Dispose();

            MessageBox.Show(watch.Elapsed.ToString());

            GC.Collect();
        }

        private TreeNode[] GetMarketGroups(SQLiteConnection conn, long parentGroupID)
        {
            List<TreeNode> nodes = new List<TreeNode>();

            SQLiteDataAdapter adp = new SQLiteDataAdapter(string.Format("SELECT marketGroupID, marketGroupName FROM invMarketGroups WHERE parentGroupID = {0}", parentGroupID), conn);
            DataTable res = new DataTable();
            adp.Fill(res);
            adp.Dispose();

            foreach (DataRow row in res.Rows)
            {
                TreeNode group = new TreeNode(row[1].ToString());
                long groupID = Convert.ToInt64(row[0]);

                group.Tag = groupID;

                group.Nodes.AddRange(GetMarketGroups(conn, groupID));
                group.Nodes.AddRange(GetMarketItems(conn, groupID));

                nodes.Add(group);
            }
            res.Dispose();

            return nodes.ToArray();
        }

        private TreeNode[] GetMarketItems(SQLiteConnection conn, long marketGroupID)
        {
            List<TreeNode> nodes = new List<TreeNode>();

            SQLiteDataAdapter adp = new SQLiteDataAdapter(string.Format("SELECT typeID, typeName FROM invTypes WHERE marketGroupID = {0} UNION SELECT typeID, typeName FROM invSkills WHERE marketGroupID = {0}", marketGroupID), conn);
            DataTable res = new DataTable();
            adp.Fill(res);
            adp.Dispose();

            foreach (DataRow row in res.Rows)
            {
                TreeNode item = new TreeNode(row[1].ToString());
                long typeID = Convert.ToInt64(row[0]);

                item.Tag = typeID;

                nodes.Add(item);
            }
            res.Dispose();

            return nodes.ToArray();
        }

        private void SelectItem(object sender, TreeNodeMouseClickEventArgs e)
        {
            long typeID = (long)e.Node.Tag;
            frmItemView newItem = new frmItemView(typeID);
            newItem.Show();
        }
    }
}
