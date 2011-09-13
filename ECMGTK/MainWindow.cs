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
using System.ComponentModel;

public partial class MainWindow: Gtk.Window
{    
    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();
        
        ntbPages.CurrentPage = 0;
		
		FillTabsWithImages();
        
		BackgroundWorker worker = new BackgroundWorker();
		worker.DoWork += delegate {
			LoadMarket();
		};
		
		worker.RunWorkerCompleted += HandleWorkerRunWorkerCompleted;
		
		worker.RunWorkerAsync();
		
		Visible = true;
    }

    void HandleWorkerRunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
    {
    }

	public void FillTabsWithImages ()
	{
		ntbPages.SetTabLabelPacking(vbxOverview, false, false, PackType.Start);
		ntbPages.SetTabLabel(vbxOverview, CreateTabLabel("Overview", "ECMGTK.Resources.Icons.Home.png"));
		
		ntbPages.SetTabLabelPacking(vbxCharSheet, false, false, PackType.Start);
		ntbPages.SetTabLabel(vbxCharSheet, CreateTabLabel("Character Sheet", "ECMGTK.Resources.Icons.CharSheet.png"));
		
		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));
		
//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));
//		
//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));
//		
//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));
//		
//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));
//		
//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));
//		
//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));
//		
//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));
//		
//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));
	}

	public Widget CreateTabLabel (string title, string imageResource)
	{
		HBox box = new HBox();
		Image icon = new Image(null, imageResource);
		Label label = new Label(title);
		label.Xalign = 0;
		
		box.PackEnd(icon, false, false, 0);
		box.PackStart(label, true, true, 0);
		
		box.ShowAll();
		
		return box;
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
        Application.Quit ();
        a.RetVal = true;
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
    TreeStore marketStore = new TreeStore(typeof(Gdk.Pixbuf), typeof(string), typeof(long));
    TreeModelFilter marketFilter;
    
    public void LoadMarket()
    {        
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
		
		ECM.Core.ItemDatabase.LoadMarket(marketStore);
        
        trvMarket.ColumnsAutosize();
        
        marketFilter = new TreeModelFilter(marketStore, null);
        marketFilter.VisibleFunc = HandleMarketFilter;
        
        TreeModelSort sorted = new TreeModelSort(marketFilter);
        
        sorted.SetSortColumnId(1, SortType.Ascending);
        
        trvMarket.Model = sorted;
		
		Console.WriteLine("Market should be loaded");
    }
    
    private bool HandleMarketFilter (TreeModel model, TreeIter iter)
    {
     //string itemName = model.GetValue (iter, 1).ToString ();
    
     //if (txtMarketFilter.Text == "")
         return true;
     
     //if(model.IterHasChild(iter))
     //    return true;
    
     //if (itemName.Contains(txtMarketFilter.Text))
     //    return true;
     //else
     //    return false;
    }

	protected void SelectRow (object o, Gtk.SelectCursorRowArgs args)
	{
        Console.WriteLine("SelectRow");
	}
    #endregion
	
	#region Overview
	protected void AddNewKey (object sender, System.EventArgs e)
	{
		ECMGTK.AddApiKey addKey = new ECMGTK.AddApiKey();
		
		addKey.Run();
	}
	#endregion
}
