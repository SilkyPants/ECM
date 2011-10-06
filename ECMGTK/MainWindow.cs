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
using System.Diagnostics;
using System.Timers;
using ECMGTK;

public partial class MainWindow: Gtk.Window
{
    struct Colour
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public Colour(byte red, byte green, byte blue, byte alpha)
        {
            r = red;
            g = green;
            b = blue;
            a = alpha;
        }

        public uint ToUint ()
        {
            return (uint)a | (uint)b << 8 | (uint)g << 16 | (uint)r << 24;
        }
    }

    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();
        
        ntbPages.CurrentPage = 0;
        hpnMarket.Position = 300;
		
		FillTabsWithImages();
        
		BackgroundWorker worker = new BackgroundWorker();
		worker.DoWork += delegate {
			LoadMarket();
		};
		
		worker.RunWorkerCompleted += HandleWorkerRunWorkerCompleted;
		hpnMarket.Sensitive = false;
		worker.RunWorkerAsync();
		
		Visible = true;

        ECM.Core.Data.LoadAccounts();

        FillAccounts();

        Timer heartbeat = new Timer(1000);
        heartbeat.AutoReset = true;
        heartbeat.Elapsed += delegate
        {
            ECM.Core.Data.UpdateOnHeartbeat();
        };
        heartbeat.Start();
    }

    void HandleWorkerRunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
    {
        hpnMarket.Sensitive = true;
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
    
    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {		
        Application.Quit ();
        a.RetVal = true;
    }
    
    #region Market
    TreeStore marketStore = new TreeStore(typeof(Gdk.Pixbuf), typeof(string), typeof(long), typeof(bool));
    ListStore itemStore = new ListStore(typeof(string), typeof(long));
    TreeModelFilter marketFilter;

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
            if(marketStore.IterHasChild(iter) )
            {
                if(trvMarket.GetRowExpanded(args.Path))
                    trvMarket.CollapseRow(args.Path);
                else
                    trvMarket.ExpandRow(args.Path, false);
            }
        }
    }
    
    public void LoadMarket()
    {
        marketStore.Clear();
        itemStore.Clear();
        
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
		
		ECM.Core.ItemDatabase.LoadMarket(marketStore, itemStore);
        
        trvMarket.ColumnsAutosize();
        
        TreeModelSort sorted = new TreeModelSort(marketStore);
        
        sorted.SetSortColumnId(1, SortType.Ascending);
        
        trvMarket.Model = sorted;

        trvMarket.Selection.Changed += trvSelectionChanged;

        // Search Item Tree
        marketFilter = new TreeModelFilter(itemStore, null);
        marketFilter.VisibleFunc = new TreeModelFilterVisibleFunc(HandleMarketFilter);

        mainColumn = new TreeViewColumn();
        mainColumn.Title = "Groups";
        
        label = new CellRendererText();
        label.Xalign = 0;
        mainColumn.PackStart(label, false);
        mainColumn.AddAttribute(label, "text", 0);

        trvSearchItems.AppendColumn(mainColumn);

        trvSearchItems.ColumnsAutosize();
        
        sorted = new TreeModelSort(marketFilter);
        
        sorted.SetSortColumnId(0, SortType.Ascending);
        
        trvSearchItems.Model = sorted;
    }

    void trvSelectionChanged (object sender, EventArgs e)
    {
        TreeIter iter;
        TreeModel model;
        if(trvMarket.Selection.GetSelected(out model, out iter))
        {
            bool hasItems = (bool)model.GetValue(iter, 3);

            if(model.GetValue(iter, 0) == null)
            {
                long ID = Convert.ToInt64(model.GetValue(iter, 2));
                ECM.Core.EveItem item = ECM.Core.ItemDatabase.Items[ID];
                // Item selected -
                // TODO: need to do this better ^^

                // Add item's market group to the list
                TreeIter parentIter;
                // get children iterator
                if(model.IterParent(out parentIter, iter))
                {
                    ShowMarketGroupItems(model, parentIter);

                    // TODO: Open Item Market Details
                    ShowItemMarketDetails(item, model, iter);
                }
            }
            else if(hasItems)
            {
                ShowMarketGroupItems(model, iter);
            }
        }
    }

    void ShowItemMarketDetails (ECM.Core.EveItem item, TreeModel model, TreeIter iter)
    {
        ntbMarketDetails.CurrentPage = 0;

        // First work out the tree path
        TreeIter parentIter;
        string path = "";
        while(model.IterParent(out parentIter, iter))
        {
            iter = parentIter;
            path = model.GetValue(parentIter, 1).ToString() + " \\ " + path;
        }

        lblItemTreeDetails.Text = path;

        lblItemNameDetails.Text = item.Name;
    }

    void ShowMarketGroupItems (TreeModel model, TreeIter iter)
    {
        // Clear the VBox
        while(vbbMarketGroups.Children.Length > 0)
        {
            vbbMarketGroups.Remove(vbbMarketGroups.Children[0]);
        }

        TreeIter childIter;
        // get children iterator
        if(model.IterChildren(out childIter, iter))
        {
            do
            {
                // TODO: Need check to make sure it's an item
                long ID = Convert.ToInt64(model.GetValue(childIter, 2));
                ECM.Core.EveItem item = ECM.Core.ItemDatabase.Items[ID];

                AddItemToCurrentMarketGroup(item);
            }
            while(model.IterNext(ref childIter));

            ntbMarketDetails.CurrentPage = 1;
        }
    }

    void AddItemToCurrentMarketGroup (ECM.Core.EveItem item)
    {
        if(vbbMarketGroups.IsRealized == false)
            vbbMarketGroups.Realize();

        Image itemPic = new Image(ECM.Core.ItemDatabase.ItemUnknownPNG);
        itemPic.WidthRequest = 64;
        itemPic.HeightRequest = 64;

        Gdk.Pixbuf buf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 22, 22);
        Gdk.Pixbuf book = new Gdk.Pixbuf(ECM.Core.ItemDatabase.Skillbook22PNG);

        Colour col = new Colour(128, 0, 0, 128);
        buf.Fill(col.ToUint());
        book.Composite(buf, 0, 0, buf.Width, buf.Height, 0, 0, 1, 1, Gdk.InterpType.Hyper, 255);

        Image skillsMet = new Image(buf);
        skillsMet.WidthRequest = 22;
        skillsMet.HeightRequest = 22;
        skillsMet.Xalign = 0;

        Pango.FontDescription font = new Pango.FontDescription();
        font.Size = 24;

        Label itemName = new Label();
        itemName.UseMarkup = true;
        itemName.Markup = string.Format("<span size=\"large\" weight=\"bold\">{0}</span>", item.Name);
        itemName.Xalign = 0;
        //itemName.ModifyFont(font);

        Image infoPic = new Image(ECM.Core.ItemDatabase.Info16PNG);
        infoPic.WidthRequest = 16;
        infoPic.HeightRequest = 16;

        HBox itemNameHeader = new HBox();
        itemNameHeader.PackStart(itemName, false, false, 0);
        itemNameHeader.PackStart(infoPic, false, false, 3);

        WrapLabel itemDesc = new WrapLabel(item.Description);

        Frame picFrame = new Frame();
        picFrame.Shadow = ShadowType.Out;
        picFrame.Add(itemPic);

        VBox heading = new VBox();
        heading.Spacing = 6;
        heading.PackEnd(itemNameHeader, false, false, 0);
        heading.PackEnd(skillsMet, false, false, 0);

        HBox inner = new HBox();
        inner.PackStart(picFrame, false, false, 0);
        inner.PackStart(heading, true, true, 1);

        Button viewDets = new Button(new Label("View Details"));

        HButtonBox itemButtons = new HButtonBox();
        itemButtons.Layout = ButtonBoxStyle.End;
        itemButtons.BorderWidth = 3;
        itemButtons.Add(viewDets);
        itemButtons.ShowAll();

        HSeparator sep = new HSeparator();

        VBox itemBlock = new VBox();
        itemBlock.Spacing = 10;
        itemBlock.PackStart(inner, false, false, 0);
        itemBlock.PackStart(itemDesc, true, true, 0);
        itemBlock.PackEnd(itemButtons, false, false, 0);

        itemBlock.ShowAll();
        sep.ShowAll();

        vbbMarketGroups.PackStart(itemBlock, false, false, 3);
        vbbMarketGroups.PackStart(sep, false, false, 3);
    }
    
    private bool HandleMarketFilter (TreeModel model, TreeIter iter)
    {
     string itemName = model.GetValue (iter, 0).ToString ();

     if (txtMarketFilter.Text == "")
         return false;
    
     if (itemName.Contains(txtMarketFilter.Text))
         return true;
     else
         return false;
    }
    #endregion

    #region Move Later To Helper Class?
    public static string GetDurationInWords( TimeSpan aTimeSpan )
    {
        string timeTaken = string.Empty;
    
        if( aTimeSpan.Days > 0 )
            timeTaken += aTimeSpan.Days + " day" + ( aTimeSpan.Days > 1 ? "s" : "" );
    
        if( aTimeSpan.Hours > 0 )
        {
            if( !string.IsNullOrEmpty( timeTaken ) )
               timeTaken += " ";
            timeTaken += aTimeSpan.Hours + " hour" + ( aTimeSpan.Hours > 1 ? "s" : "" );
        }
    
        if( aTimeSpan.Minutes > 0 )
        {
           if( !string.IsNullOrEmpty( timeTaken ) )
               timeTaken += " ";
           timeTaken += aTimeSpan.Minutes + " minute" + ( aTimeSpan.Minutes > 1 ? "s" : "" );
        }
    
        if( aTimeSpan.Seconds > 0 )
        {
           if( !string.IsNullOrEmpty( timeTaken ) )
               timeTaken += " ";
           timeTaken += aTimeSpan.Seconds + " second" + ( aTimeSpan.Seconds > 1 ? "s" : "" );
        }
    
        if( string.IsNullOrEmpty( timeTaken ) )
            timeTaken = "0 seconds.";
    
         return timeTaken;
    }
    #endregion
	
	#region Overview

	protected void AddNewKey (object sender, System.EventArgs e)
	{
		ECMGTK.AddApiKey addKey = new ECMGTK.AddApiKey();
		
		addKey.Run();

        FillAccounts();
	}

    protected Widget CreateAccountWidget (ECM.Core.Account account)
    {
        VBox accWidget = new VBox();
        accWidget.Spacing = 3;

        Label accKey = new Label(string.Format("Account #{0}", account.KeyID));
        accKey.Xalign = 0;

        HSeparator sep = new HSeparator();

        Button delAccBtn = new Button();
        delAccBtn.Relief = ReliefStyle.None;
        delAccBtn.TooltipText = "Delete Account";
        delAccBtn.Add(new Image("gtk-delete", IconSize.Menu));

        HBox accHeader = new HBox();
        accHeader.Spacing = 2;
        accHeader.PackStart(accKey, false, false, 0);
        accHeader.PackStart(sep, true, true, 0);
        accHeader.PackStart(delAccBtn, false, false, 0);

        accWidget.PackStart(accHeader, true, false, 0);

        TextView accStats = new TextView();
        accWidget.PackStart(accStats, true, false, 0);

        accStats.Editable = false;
        accStats.Sensitive = false;

        DateTime paidUntilLocal = account.PaidUntil.ToLocalTime();
        TimeSpan playTime = new TimeSpan(0, account.LogonMinutes, 0);

        accStats.Buffer.Text = string.Format("Paid Until: {0}\nTime spent playing: {1}",
            paidUntilLocal.ToString(), GetDurationInWords(playTime));

        foreach(ECM.Core.Character ecmChar in account.Characters)
        {
            accWidget.PackStart(CreateCharacterButton(ecmChar), true, false, 0);
        }

        accWidget.ShowAll();

        return accWidget;
    }

    protected Widget CreateCharacterButton (ECM.Core.Character character)
    {
        HBox box = new HBox();

        Frame frm = new Frame();
        frm.Shadow = ShadowType.EtchedOut;
        frm.BorderWidth = 3;
        
        Image img = new Image(null, "ECMGTK.Resources.NoPortrait_64.png");
        img.WidthRequest = 64;
        img.HeightRequest = 64;

        BackgroundWorker imgWorker = new BackgroundWorker();

        imgWorker.DoWork += delegate(object sender, DoWorkEventArgs e)
        {
            img.Pixbuf = EveApi.ImageApi.GetCharacterPortraitGTK(character.ID, EveApi.ImageApi.ImageRequestSize.Size64x64);
        };

        imgWorker.RunWorkerAsync();

        frm.Add(img);

        box.PackStart(frm, false, false, 3);

        Label text = new Label();
        text.UseMarkup = true;
        text.Markup = string.Format("<span size=\"larger\" weight=\"bold\">{0}</span>\n<span size=\"small\">{1} - {2} - {3}\n{4:0,0.00} ISK\nLocation: {5}</span>",
                            character.Name, character.Race, character.Bloodline, character.Ancestry, character.AccountBalance, character.LastKnownLocation);
        text.Xalign = 0;
        text.Yalign = 0;

        box.PackStart(text, true, true, 0);
        Button btn = new Button(box);

        btn.ShowAll();
        return btn;

    }

    public void FillAccounts ()
    {
        while (vbxAccounts.Children.Length > 0)
        {
            vbxAccounts.Remove(vbxAccounts.Children[0]);
        }

        foreach(ECM.Core.Account account in ECM.Core.Data.Accounts.Values)
        {
            vbxAccounts.PackStart(CreateAccountWidget(account), false, false, 0);
        }
    }
	#endregion
}
