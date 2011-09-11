//  
//  MainWindow.cs
//  
//  Author:
//       Dan Silk <silkypantsdan@gmail.com>
// 
//  Copyright (c) 2011 Dan Silk
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using Gtk;
using System.Data.SQLite;

using SqlConn = System.Data.SQLite.SQLiteConnection;
using SqlCmd = System.Data.SQLite.SQLiteCommand;
using SqlReader = System.Data.SQLite.SQLiteDataReader;

public partial class MainWindow: Gtk.Window
{    
    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();
        
		// Hide the tabs so that we can make the app look awesome :D
		//ntbPages.ShowTabs = false;
        //ntbPages.CurrentPage = 0;
        
		LoadMarket();
		Visible = true;
    }
    
    private Button CreateCharacterButton()
    {
        Button btnChar = new Button();
        Image imgPortrait = new Image(null, "ECMGTK.Resources.NoPortrait_64.png");
        Image imgRecycle = new Image(null, "ECMGTK.Resources.Icons.Recycle.png");
        Button btnRecycle = new Button(imgRecycle);
        Label lblStatus = new Label();
        HBox hbxContainer = new HBox(false, 1);
		        
        lblStatus.Text = "This is some text\nIt spans multiple lines\nSo we can show cool stuff. This is to push the button out more, I can make it wrap with the WidthChar member";
        lblStatus.WidthChars = 40;
        lblStatus.Wrap = true;
        
        imgPortrait.WidthRequest = 64;
        imgPortrait.HeightRequest = 64;
        
        hbxContainer.PackStart(imgPortrait);
        hbxContainer.PackStart(lblStatus);
        hbxContainer.PackStart(btnRecycle);
        
        btnChar.Add(hbxContainer);
        
        btnChar.ShowAll();
		
		btnChar.Clicked += delegate {
			Console.WriteLine("Called Char Button Press");
		};
		
		btnRecycle.Clicked += delegate {
			Console.WriteLine("Called Char Recycle Button Press");
		};
        
        return btnChar;
    }
    
    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {
		CloseDatabase();
		
        Application.Quit ();
        a.RetVal = true;
    }

    protected void AddNewApiKey (object sender, System.EventArgs e)
    {
        //vbbCharacters.PackStart(CreateCharacterButton());
		
		ECMGTK.AddApiKey addKey = new ECMGTK.AddApiKey();
		
		addKey.Run();
    }

	protected void ChangePage (object sender, System.EventArgs e)
	{
		if(sender == HomeAction)
		{
			ntbPages.CurrentPage = 0;
		}
		else if(sender == CharSheetAction)
		{
			ntbPages.CurrentPage = 1;
		}
		else if(sender == MarketAction)
		{
			ntbPages.CurrentPage = 2;
		}
	}

    protected void RowCollapsed (object sender, Gtk.RowCollapsedArgs args)
    {
        TreeView tv = sender as TreeView;
        
        if(tv != null)
        {
         tv.ColumnsAutosize();
        }
    }
    
    protected void SearchTextChanged (object sender, System.EventArgs e)
    {
         marketFilter.Refilter();
    }

    protected void RowActivated (object o, Gtk.RowActivatedArgs args)
    {
        TreeIter iter;
        
        if(marketStore.GetIter(out iter, args.Path))
        {
            if(marketStore.IterHasChild(iter) == false)
            {
                // Item or empty group
            }
            else
            {
                if(trvMarket.GetRowExpanded(args.Path))
                    trvMarket.CollapseRow(args.Path);
                else
                    trvMarket.ExpandRow(args.Path, false);
            }
        }
    }
    
    #region Database
    SqlConn sqlConnection = null;
    TreeStore marketStore = new TreeStore(typeof(Gdk.Pixbuf), typeof(string), typeof(long));
    TreeModelFilter marketFilter;
    
    static string itemDatabasePath = "Resources/Database/eveItems.db";
    static string skillsDatabasePath = "Resources/Database/eveSkills.db";
    
    public bool OpenDatabase()
    {
        Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
        sqlConnection = new SqlConn(string.Format("Data Source={0};version=3", itemDatabasePath));
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
    
        cmd.Dispose();
     
        return true;
    }
    
    public void CloseDatabase()
    {
        if(sqlConnection.State != System.Data.ConnectionState.Open) return;
        
        sqlConnection.Close();
    }
    
    public void LoadMarket()
    {
        OpenDatabase();
        
        marketStore.Clear();
        
        TreeViewColumn mainColumn = new TreeViewColumn();
        mainColumn.Title = "Groups";
        
        CellRendererPixbuf pixBuf = new CellRendererPixbuf();
        pixBuf.Xalign = 0;
        mainColumn.PackStart(pixBuf, false);
        mainColumn.AddAttribute(pixBuf, "pixbuf", 0);
        
        CellRendererText label = new CellRendererText();
        label.Xalign = 0;
        mainColumn.PackStart(label, false);
        mainColumn.AddAttribute(label, "text", 1);
        
        trvMarket.AppendColumn(mainColumn);
        
        LoadMarketGroupsForID(-1, TreeIter.Zero);
        
        trvMarket.ColumnsAutosize();
        
        marketFilter = new TreeModelFilter(marketStore, null);
        marketFilter.VisibleFunc = HandleMarketFilter;
        
        TreeModelSort sorted = new TreeModelSort(marketFilter);
        
        sorted.SetSortColumnId(1, SortType.Ascending);
        
        trvMarket.Model = sorted;
        
        CloseDatabase();
    }
    
    private bool HandleMarketFilter (TreeModel model, TreeIter iter)
    {
     string itemName = model.GetValue (iter, 1).ToString ();
    
     if (txtMarketFilter.Text == "")
         return true;
     
     if(model.IterHasChild(iter))
         return true;
    
     if (itemName.Contains(txtMarketFilter.Text))
         return true;
     else
         return false;
    }
    
    private void LoadMarketGroupsForID(long parentGroupID, TreeIter parentIter)
    {
     string selectCmd = "SELECT marketGroupID, marketGroupName, hasTypes FROM invMarketGroups WHERE parentGroupID ";
     selectCmd += parentGroupID >= 0 ? string.Format("= {0}", parentGroupID) : "IS NULL";
     
        SqlCmd cmd = sqlConnection.CreateCommand();
        cmd.CommandText = selectCmd;
        SqlReader row = cmd.ExecuteReader();
        
        while(row.Read())
        {
            string groupName = row[1].ToString();
            long groupID = Convert.ToInt64(row[0]);
            int hasTypes = Convert.ToInt32(row[2]);
         TreeIter groupIter;
         
         if(parentIter.Equals(TreeIter.Zero))
             groupIter = marketStore.AppendValues(new Gdk.Pixbuf(null, "ECMGTK.Resources.Icons.MarketGroup.png"), groupName);
         else
             groupIter = marketStore.AppendValues(parentIter, new Gdk.Pixbuf(null, "ECMGTK.Resources.Icons.MarketGroup.png"), groupName);
             
         LoadMarketGroupsForID(groupID, groupIter);
         
         if(hasTypes == 1)
                 LoadMarketItemsForID(groupID, groupIter);
        }
    }
    
    private void LoadMarketItemsForID(long marketGroupID, TreeIter parentIter)
    {
        SqlCmd cmd = sqlConnection.CreateCommand();
        cmd.CommandText = string.Format("SELECT typeID, typeName FROM invTypes WHERE marketGroupID = {0} UNION SELECT typeID, typeName FROM invSkills WHERE marketGroupID = {0}", marketGroupID);
        SqlReader row = cmd.ExecuteReader();
        
        while(row.Read())
        {
            string itemName = row[1].ToString();
            long typeID = Convert.ToInt64(row[0]);
    
            marketStore.AppendValues(parentIter, null, itemName, typeID);
        }
    }
    #endregion
}
