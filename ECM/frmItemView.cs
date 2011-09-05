using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;

namespace ECM
{
    public partial class frmItemView : Form
    {
        long m_TypeID;

        public frmItemView(long typeID)
        {
            m_TypeID = typeID;

            InitializeComponent();

            //pictureBox1.Image = EveApi.ImageApi.GetItemImage(typeID, EveApi.ImageApi.ImageRequestSize.Size64x64);

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

            cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("SELECT typeName FROM invTypes WHERE typeID = {0} UNION SELECT typeName FROM invSkills WHERE typeID = {0}", typeID);

            object res = cmd.ExecuteScalar();

            if(res != null)
                label1.Text = res.ToString();

            SQLiteDataAdapter adp = new SQLiteDataAdapter(string.Format("SELECT attributeID, valueFloat, valueInt FROM dgmTypeAttributes WHERE typeID = {0}", typeID), conn);
            DataTable resTable = new DataTable();
            adp.Fill(resTable);
            adp.Dispose();

            foreach (DataRow row in resTable.Rows)
            {
                ListViewItem newItem = new ListViewItem(listView1.Groups[0]);
                double val = row[1] is DBNull ? Convert.ToDouble(row[2]) : Convert.ToDouble(row[1]);

                cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT displayName FROM dgmAttributeTypes WHERE attributeID = {0}", row[0]);
                res = cmd.ExecuteScalar();

                if (res == null || string.IsNullOrEmpty(res.ToString()))
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandText = string.Format("SELECT attributeName FROM dgmAttributeTypes WHERE attributeID = {0}", row[0]);
                    res = cmd.ExecuteScalar();
                }

                newItem.Text = res.ToString();
                newItem.SubItems.Add(val.ToString());

                listView1.Items.Add(newItem);
            }

            adp = new SQLiteDataAdapter(string.Format("SELECT effectID FROM dgmTypeEffects WHERE typeID = {0}", typeID), conn);
            resTable = new DataTable();
            adp.Fill(resTable);
            adp.Dispose();

            foreach (DataRow row in resTable.Rows)
            {
                ListViewItem newItem = new ListViewItem(listView1.Groups[1]);

                cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT effectName FROM dgmEffects WHERE effectID = {0}", row[0]);
                res = cmd.ExecuteScalar();

                if (res == null || string.IsNullOrEmpty(res.ToString()))
                {
                    res = "NULL";
                }

                newItem.Text = res.ToString();

                listView1.Items.Add(newItem);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            conn.Close();
            conn.Dispose();
        }

        private void ShowItemRender(object sender, EventArgs e)
        {
            frmItemRenderView renderView = new frmItemRenderView(m_TypeID, EveApi.ImageApi.ImageRequestSize.Size512x512);
            renderView.Show();
        }
    }
}
