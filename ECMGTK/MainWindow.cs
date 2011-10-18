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
using GLib;

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
        hpnMarket.Position = 250;
        Hide();

        while (Gtk.Application.EventsPending ())
            Gtk.Application.RunIteration ();

        GLib.ExceptionManager.UnhandledException += OnUnhandledException;

		FillTabsWithImages();
        
		BackgroundWorker worker = new BackgroundWorker();
		worker.DoWork += delegate 
        {
            LoadMarket();
            LoadSkills();
            ECM.Core.Init();
        };
		
		worker.RunWorkerCompleted += HandleWorkerRunWorkerCompleted;
		hpnMarket.Sensitive = false;
		worker.RunWorkerAsync();

        imgNetworkIndicator.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF16);
        imgNetworkIndicator.Visible = false;

        SetupGui();

        ECM.Core.OnUpdateGui += new EventHandler(UpdateGui);
        ECM.Core.OnCharacterChanged += CharacterChanged;
        ECM.Core.OnTQServerUpdate += TQServerUpdate;

        Timer heartbeat = new Timer(1000);
        heartbeat.AutoReset = true;
        heartbeat.Elapsed += new ElapsedEventHandler(heartbeat_Elapsed);
        heartbeat.Start();

        ntbPages.CurrentPage = 0;
    }

    void heartbeat_Elapsed(object sender, ElapsedEventArgs e)
    {
        ECM.Core.UpdateOnHeartbeat();
        ECM.Core.SaveAccounts();
    }

    #region Event Handlers

    void OnUnhandledException (UnhandledExceptionArgs args)
    {
        Console.WriteLine(args.ExceptionObject);
    }

    void TQServerUpdate (EveApi.ServerStatus status)
    {
        string serverStatus = string.Empty;

        if(status.ServerOnline)
        {
            serverStatus = string.Format("Tranquility is online with {0:0,0} pilots", status.NumberOfPlayers);
        }
        else
        {
            serverStatus = "Tranquility is offline";
        }

        stbStatus.Pop(0);
        stbStatus.Push(0, serverStatus);
    }

    bool OnHeartbeat()
    {
        ECM.Core.UpdateOnHeartbeat();
        ECM.Core.SaveAccounts();

        return true;
    }

    void CharacterChanged (object sender, EventArgs e)
    {
        Gtk.Application.Invoke(delegate
        {
            ShowCharacterSheet(ECM.Core.CurrentCharacter);

            ntbPages.CurrentPage = 1;
        });
    }

    void UpdateGui(object sender, EventArgs e)
    {
        Gtk.Application.Invoke(delegate
        {
            FillAccounts();

            vbxCharSheet.Sensitive = ECM.Core.CurrentCharacter != null;

            if(ECM.Core.CurrentCharacter != null)
                ShowCharacterSheet(ECM.Core.CurrentCharacter);
        });
    }

    void HandleWorkerRunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
    {
        Gtk.Application.Invoke(delegate
        {
            hpnMarket.Sensitive = true;
            Show();
            FillAccounts();
        });
    }
    
    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {
        ECM.Core.SaveAccounts();
        Application.Quit ();
        a.RetVal = true;
    }

    #endregion

    #region Gui Setup

    public void SetupGui ()
    {
        SetupMarket();
        SetupCharacterSheet();
    }

	public void FillTabsWithImages ()
	{
		ntbPages.SetTabLabelPacking(vbxOverview, false, false, PackType.Start);
		ntbPages.SetTabLabel(vbxOverview, CreateTabLabel("Overview", "ECMGTK.Resources.Icons.Home.png", true));
		
		ntbPages.SetTabLabelPacking(vbxCharSheet, false, false, PackType.Start);
		ntbPages.SetTabLabel(vbxCharSheet, CreateTabLabel("Character Sheet", "ECMGTK.Resources.Icons.CharSheet.png", true));
     
        ntbPages.SetTabLabelPacking(tmpContacts, false, false, PackType.Start);
        ntbPages.SetTabLabel(tmpContacts, CreateTabLabel("People & Places", "ECMGTK.Resources.Icons.Contacts.png", true));
        
        ntbPages.SetTabLabelPacking(tmpMail, false, false, PackType.Start);
        ntbPages.SetTabLabel(tmpMail, CreateTabLabel("Mail", "ECMGTK.Resources.Icons.Mail.png", true));
        
        ntbPages.SetTabLabelPacking(tmpFittings, false, false, PackType.Start);
        ntbPages.SetTabLabel(tmpFittings, CreateTabLabel("Fittings", "ECMGTK.Resources.Icons.Fitting.png", true));
		
		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);
		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png", true));

		ntbPages.SetTabLabelPacking(tmpResearch, false, false, PackType.Start);
		ntbPages.SetTabLabel(tmpResearch, CreateTabLabel("Science & Industry", "ECMGTK.Resources.Icons.Research.png", true));

		ntbPages.SetTabLabelPacking(tmpContracts, false, false, PackType.Start);
		ntbPages.SetTabLabel(tmpContracts, CreateTabLabel("Contracts", "ECMGTK.Resources.Icons.Contracts.png", true));

		ntbPages.SetTabLabelPacking(tmpMap, false, false, PackType.Start);
		ntbPages.SetTabLabel(tmpMap, CreateTabLabel("Map", "ECMGTK.Resources.Icons.Map.png", true));

		ntbPages.SetTabLabelPacking(tmpCorporations, false, false, PackType.Start);
		ntbPages.SetTabLabel(tmpCorporations, CreateTabLabel("Corporations", "ECMGTK.Resources.Icons.Corporations.png", true));

        ntbPages.SetTabLabelPacking(tmpAssets, false, false, PackType.Start);
        ntbPages.SetTabLabel(tmpAssets, CreateTabLabel("Assets", "ECMGTK.Resources.Icons.Assets.png", true));
        
        ntbPages.SetTabLabelPacking(tmpMoney, false, false, PackType.Start);
        ntbPages.SetTabLabel(tmpMoney, CreateTabLabel("Money", "ECMGTK.Resources.Icons.Money.png", true));
        
        ntbPages.SetTabLabelPacking(tmpNews, false, false, PackType.Start);
        ntbPages.SetTabLabel(tmpNews, CreateTabLabel("News", "ECMGTK.Resources.Icons.News.png", true));
        
        ntbPages.SetTabLabelPacking(tmpHelp, false, false, PackType.Start);
        ntbPages.SetTabLabel(tmpHelp, CreateTabLabel("Help & Settings", "ECMGTK.Resources.Icons.Help.png", true));

        // Character Sheet
        ntbCharSheetPages.SetTabLabelPacking(vbxSkills, false, false, PackType.Start);
        ntbCharSheetPages.SetTabLabel(vbxSkills, CreateTabLabel("Skills", "ECMGTK.Resources.Icons.Skills.png", false));

        ntbCharSheetPages.SetTabLabelPacking(vbxCertificates, false, false, PackType.Start);
        ntbCharSheetPages.SetTabLabel(vbxCertificates, CreateTabLabel("Certificates", "ECMGTK.Resources.Icons.Certificates.png", false));

        ntbCharSheetPages.SetTabLabelPacking(scwMedals, false, false, PackType.Start);
        ntbCharSheetPages.SetTabLabel(scwMedals, CreateTabLabel("Decorations", "ECMGTK.Resources.Icons.Medal.png", false));

        ntbCharSheetPages.SetTabLabelPacking(scwAttributes, false, false, PackType.Start);
        ntbCharSheetPages.SetTabLabel(scwAttributes, CreateTabLabel("Attributes", "ECMGTK.Resources.Icons.Attributes.png", false));

        ntbCharSheetPages.SetTabLabelPacking(scwImplants, false, false, PackType.Start);
        ntbCharSheetPages.SetTabLabel(scwImplants, CreateTabLabel("Augmentations", "ECMGTK.Resources.Icons.Implants.png", false));

        ntbCharSheetPages.SetTabLabelPacking(scwEmployment, false, false, PackType.Start);
        ntbCharSheetPages.SetTabLabel(scwEmployment, CreateTabLabel("Employment History", "ECMGTK.Resources.Icons.Corporations.png", false));

        ntbCharSheetPages.SetTabLabelPacking(scwStandings, false, false, PackType.Start);
        ntbCharSheetPages.SetTabLabel(scwStandings, CreateTabLabel("Standings", "ECMGTK.Resources.Icons.Standings.png", false));

        ntbCharSheetPages.SetTabLabelPacking(scwCombatLogs, false, false, PackType.Start);
        ntbCharSheetPages.SetTabLabel(scwCombatLogs, CreateTabLabel("Combat Log", "ECMGTK.Resources.Icons.KillLogs.png", false));
	}

	public Widget CreateTabLabel (string title, string imageResource, bool iconAfterText)
	{
		HBox box = new HBox();
		Image icon = new Image(null, imageResource);
		Label label = new Label(title);
		label.Xalign = 0;

        if(iconAfterText)
            box.PackEnd(icon, false, false, 0);
        else
		    box.PackStart(icon, false, false, 0);

		box.PackStart(label, true, true, 6);
		
		box.ShowAll();
		
		return box;
	}

    public void SetupMarket ()
    {
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
        
        trvMarket.ColumnsAutosize();

        trvMarket.Selection.Changed += trvSelectionChanged;

        // Search Item Tree
        mainColumn = new TreeViewColumn();
        mainColumn.Title = "Groups";
        
        label = new CellRendererText();
        label.Xalign = 0;
        mainColumn.PackStart(label, false);
        mainColumn.AddAttribute(label, "text", 0);

        trvSearchItems.AppendColumn(mainColumn);

        trvSearchItems.ColumnsAutosize();
    }

    private void SetupCharacterSheet()
    {
        imgCharPortrait.Pixbuf = EveApi.ImageApi.StreamToPixbuf(ECM.Core.NoPortraitJPG).ScaleSimple(198,198,Gdk.InterpType.Hyper);
        evtCharPortrait.ButtonPressEvent += UpdateCharacterPortrait;

        #region Attributes Treeview
        trvAttributes.EnableGridLines = TreeViewGridLines.Horizontal;

        TreeViewColumn attributeColumn = new TreeViewColumn();
        attributeColumn.Title = "Attribute";

        CellRendererPixbuf attributeImg = new CellRendererPixbuf();
        attributeImg.Xalign = 0;

        Gtk.CellRendererText attributeName = new Gtk.CellRendererText();

        attributeColumn.PackStart(attributeImg, false);
        attributeColumn.PackStart (attributeName, true);

        attributeColumn.AddAttribute(attributeImg, "pixbuf", 0);

        trvAttributes.AppendColumn(attributeColumn);

        attributeColumn.SetCellDataFunc (attributeName, new Gtk.TreeCellDataFunc (RenderAttribute));

        trvAttributes.Selection.Changed += ClearSelection;
        #endregion

        #region Skills Treeview
        TreeViewColumn skillColumn = new TreeViewColumn();
        skillColumn.Title = "Skill";

        CellRendererCharSkill skillCell = new CellRendererCharSkill();

        skillColumn.PackStart(skillCell, true);

        skillColumn.AddAttribute(skillCell, "SkillName", SkillNameColumn);
        skillColumn.AddAttribute(skillCell, "SkillRank", SkillRankColumn);
        skillColumn.AddAttribute(skillCell, "SkillCurrSP", SkillCurrSPColumn);
        skillColumn.AddAttribute(skillCell, "SkillNextSP", SkillNextSPColumn);
        skillColumn.AddAttribute(skillCell, "SkillLevlSP", SkillLevlSPColumn);
        skillColumn.AddAttribute(skillCell, "SkillLevel", SkillLevelColumn);
        skillColumn.AddAttribute(skillCell, "SkillMinsToNext", SkillTimeToNextColumn);

        trvSkills.AppendColumn(skillColumn);
        trvSkills.RowActivated += HandleTrvSkillsRowActivated;

        trvSkills.EnableTreeLines = false;
        trvSkills.ShowExpanders = false;
        trvSkills.Selection.Changed += ExpandSkillGroup;
        //trvSkills.LevelIndentation = -32;

        #endregion

        #region Certificates Treeview

        #endregion
    }

    void HandleTrvSkillsRowActivated (object o, RowActivatedArgs args)
    {
        Console.WriteLine("Click");
    }

    void UpdateCharacterPortrait (object o, ButtonPressEventArgs args)
    {
        if(args.Event.Button == 3)
        {
            Menu m = new Menu();

            MenuItem update = new MenuItem("Update Character Portrait");
            m.Add(update);

            update.ButtonPressEvent += delegate(object sender, ButtonPressEventArgs e)
            {
                if(e.Event.Button == 1)
                {
                    imgCharPortrait.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF);

                    BackgroundWorker tmp = new BackgroundWorker();
                    tmp.DoWork += delegate
                    {
                        ECM.Core.CurrentCharacter.UpdateCharacterPortrait();
                        ECM.Core.UpdateGui();

                    };

                    tmp.RunWorkerAsync();
                }
            };

            m.ShowAll();
            m.Popup();
        }
    }

    private void RenderAttribute (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
    {
        if(cell is Gtk.CellRendererText)
        {
            CellRendererText realCell = cell as Gtk.CellRendererText;
            string[] attribute = model.GetValue (iter, 1).ToString().Split('\n');
            realCell.Markup = string.Format("<span size=\"smaller\" weight=\"bold\">{0}</span>\n<span size=\"small\">{1}</span>", attribute[0], attribute[1]);
        }
    }
    #endregion
    
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

        ECM.ItemDatabase.LoadMarket(marketStore, itemStore);
        
        TreeModelSort sortedMarket = new TreeModelSort(marketStore);
        
        sortedMarket.SetSortColumnId(1, SortType.Ascending);

        trvMarket.Model = sortedMarket;

        marketFilter = new TreeModelFilter(itemStore, null);
        marketFilter.VisibleFunc = new TreeModelFilterVisibleFunc(HandleMarketFilter);

        TreeModelSort sortedFilter = new TreeModelSort(marketFilter);
        
        sortedFilter.SetSortColumnId(0, SortType.Ascending);
        
        trvSearchItems.Model = sortedFilter;

        Console.WriteLine("Market Loaded");
    }

    public void LoadSkills()
    {
        charSkillStore.Clear();

        ECM.ItemDatabase.LoadSkills(charSkillStore);

        skillsFilter = new TreeModelFilter(charSkillStore, null);
        skillsFilter.VisibleFunc = new TreeModelFilterVisibleFunc(HandleCharSkillsFilter);

        TreeModelSort skillsSorted = new TreeModelSort(skillsFilter);
        skillsSorted.SetSortColumnId(SkillNameColumn, SortType.Ascending);

        trvSkills.Model = skillsSorted;

        Console.WriteLine("Skills Loaded");
    }

    void ExpandSkillGroup(object sender, EventArgs e)
    {
        TreePath[] paths = trvSkills.Selection.GetSelectedRows();
        if(paths.Length > 0)
        {
            if(trvSkills.GetRowExpanded(paths[0]))
                trvSkills.CollapseRow(paths[0]);
            else
                trvSkills.ExpandRow(paths[0], false);
        }

        trvSkills.Selection.UnselectAll();
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
                ECM.EveItem item = ECM.ItemDatabase.Items[ID];
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

    void ShowItemMarketDetails (ECM.EveItem item, TreeModel model, TreeIter iter)
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
        lblItemNameDetails.Markup = string.Format("<b>{0}</b>", item.Name);
        imgItemIconDetails.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF);

        BackgroundWorker fetchImage = new BackgroundWorker();
        fetchImage.DoWork += delegate(object sender, DoWorkEventArgs e)
        {
            imgItemIconDetails.Pixbuf = EveApi.ImageApi.GetItemImageGTK(item.ID, EveApi.ImageApi.ImageRequestSize.Size64x64);
        };

        btnShowRender.Name = item.ID.ToString();
        btnShowRender.Sensitive = true;

        fetchImage.RunWorkerAsync();

        lblItemTreeDetails.Visible = true;
        imgItemIconDetails.Visible = true;
        btnItemInfo.Visible = true;
        vbxBuySell.Visible = true;
        frmItemImage.ShadowType = ShadowType.EtchedOut;
    }

    protected void ShowItemRender (object sender, System.EventArgs e)
    {
        long itemID;

        if (long.TryParse(btnShowRender.Name, out itemID))
        {
            ViewItemRender viewRender = new ViewItemRender(ECM.ItemDatabase.Items[itemID]);

            viewRender.ShowAll();
        }
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
                ECM.EveItem item = ECM.ItemDatabase.Items[ID];

                AddItemToCurrentMarketGroup(item, model, childIter);
            }
            while(model.IterNext(ref childIter));

            ntbMarketDetails.CurrentPage = 1;
        }
    }

    void AddItemToCurrentMarketGroup (ECM.EveItem item, TreeModel model, TreeIter iter)
    {
        if(vbbMarketGroups.IsRealized == false)
            vbbMarketGroups.Realize();

        Image itemPic = new Image();
        itemPic.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF);
        itemPic.WidthRequest = 64;
        itemPic.HeightRequest = 64;

        BackgroundWorker fetchImage = new BackgroundWorker();
        fetchImage.DoWork += delegate(object sender, DoWorkEventArgs e)
        {
            itemPic.Pixbuf = EveApi.ImageApi.GetItemImageGTK(item.ID, EveApi.ImageApi.ImageRequestSize.Size64x64);
        };
        
        fetchImage.RunWorkerAsync();

        Gdk.Pixbuf buf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 22, 22);
        Gdk.Pixbuf book = new Gdk.Pixbuf(ECM.Core.Skillbook22PNG);

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

        Image infoPic = new Image(ECM.Core.Info16PNG);
        infoPic.WidthRequest = 16;
        infoPic.HeightRequest = 16;

        Button btnInfo = new Button();
        btnInfo.Relief = ReliefStyle.None;
        btnInfo.Add(infoPic);

        HBox itemNameHeader = new HBox();
        itemNameHeader.PackStart(itemName, false, false, 0);
        itemNameHeader.PackStart(btnInfo, false, false, 3);

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
        viewDets.Clicked += delegate(object sender, EventArgs e)
        {
            ShowItemMarketDetails(item, model, iter);
        };

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

    private bool HandleMarketFilter(TreeModel model, TreeIter iter)
    {
        string itemName = model.GetValue(iter, 0).ToString();

        if (txtMarketFilter.Text == "")
            return false;

        if (itemName.Contains(txtMarketFilter.Text))
            return true;
        else
            return false;
    }
    #endregion
	
	#region Overview

	protected void AddNewKey (object sender, System.EventArgs e)
	{
		ECMGTK.AddApiKey addKey = new ECMGTK.AddApiKey();
		
		addKey.Run();
	}

    protected Widget CreateAccountWidget (ECM.Account account)
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

        Button editAccBtn = new Button();
        editAccBtn.Relief = ReliefStyle.None;
        editAccBtn.TooltipText = "Edit Account";
        editAccBtn.Add(new Image("gtk-edit", IconSize.Menu));

        HBox accHeader = new HBox();
        accHeader.Spacing = 2;
        accHeader.PackStart(accKey, false, false, 0);
        accHeader.PackStart(sep, true, true, 0);
        accHeader.PackStart(editAccBtn, false, false, 0);
        accHeader.PackStart(delAccBtn, false, false, 0);

        accWidget.PackStart(accHeader, true, false, 0);

        TextView accStats = new TextView();
        accWidget.PackStart(accStats, true, false, 0);

        accStats.Editable = false;
        accStats.Sensitive = false;

        DateTime paidUntilLocal = account.PaidUntil.ToLocalTime();
        TimeSpan playTime = new TimeSpan(0, account.LogonMinutes, 0);

        accStats.Buffer.Text = string.Format("Paid Until: {0}\nTime spent playing: {1}",
            paidUntilLocal.ToString(), ECM.Helper.GetDurationInWords(playTime));

        foreach(ECM.Character ecmChar in account.Characters)
        {
            if(ecmChar.AutoUpdate)
                accWidget.PackStart(CreateCharacterButton(ecmChar), true, false, 0);
        }

        accWidget.ShowAll();

        return accWidget;
    }

    protected Widget CreateCharacterButton (ECM.Character character)
    {
        HBox box = new HBox();

        Frame frm = new Frame();
        frm.Shadow = ShadowType.EtchedOut;
        frm.BorderWidth = 3;
        
        Image img = new Image();
        img.WidthRequest = 64;
        img.HeightRequest = 64;

        if(character.Portrait != null)
            img.Pixbuf = EveApi.ImageApi.StreamToPixbuf(character.Portrait).ScaleSimple(64, 64, Gdk.InterpType.Bilinear);
        else
            img.Pixbuf = EveApi.ImageApi.StreamToPixbuf(ECM.Core.NoPortraitJPG).ScaleSimple(64, 64, Gdk.InterpType.Bilinear);

        frm.Add(img);

        box.PackStart(frm, false, false, 3);

        Label text = new Label();
        text.UseMarkup = true;
        text.Markup = string.Format("<span size=\"larger\" weight=\"bold\">{0}</span>\n<span size=\"small\">{1}\n{2:0,0.00} ISK\nLocation: {3}</span>",
                            character.Name, character.Background, character.AccountBalance, character.LastKnownLocation);
        text.Xalign = 0;
        text.Yalign = 0;

        box.PackStart(text, true, true, 0);
        Button btn = new Button(box);

        btn.Name = string.Format("btn{0}", character.ID);
        btn.Clicked += delegate(object sender, EventArgs e)
        {
            ECM.Core.CurrentCharacter = character;
        };

        btn.ShowAll();
        return btn;

    }

    public void FillAccounts ()
    {
        while (vbxAccounts.Children.Length > 0)
        {
            vbxAccounts.Remove(vbxAccounts.Children[0]);
        }

        foreach(ECM.Account account in ECM.Core.Accounts.Values)
        {
            vbxAccounts.PackStart(CreateAccountWidget(account), false, false, 0);
        }
    }
	#endregion

    #region Character Sheet

    static int SkillNameColumn = 0;
    static int SkillRankColumn = 1;
    static int SkillCurrSPColumn = 2;
    static int SkillNextSPColumn = 3;
    static int SkillLevelColumn = 4;
    static int SkillTimeToNextColumn = 5;
    static int SkillLearntColumn = 6;
    static int SkillIdColumn = 7;
    static int SkillLevlSPColumn = 8;

    ListStore charAttributeStore = new ListStore(typeof(Gdk.Pixbuf), typeof(string));
    TreeStore charSkillStore = new TreeStore(typeof(string), typeof(int), typeof(int), typeof(int), typeof(int), typeof(double), typeof(bool), typeof(long), typeof(int));
    TreeModelFilter skillsFilter = null;

    void ShowCharacterSheet (ECM.Character currentCharacter)
    {
        Console.WriteLine("****** Changing Character");
        // unhook models
        trvAttributes.Model = null;

        charAttributeStore.Clear();

        lblCharName.Markup = string.Format("<b>{0}</b>", currentCharacter.Name);

        if(currentCharacter.Portrait != null)
            imgCharPortrait.Pixbuf = EveApi.ImageApi.StreamToPixbuf(currentCharacter.Portrait).ScaleSimple(160,160,Gdk.InterpType.Bilinear);
        else
            imgCharPortrait.Pixbuf = EveApi.ImageApi.StreamToPixbuf(ECM.Core.NoPortraitJPG).ScaleSimple(160,160,Gdk.InterpType.Bilinear);

        lblCurrentLocation.Markup = string.Format("<b>{0}</b>", currentCharacter.LastKnownLocation);
        lblBackground.Markup = string.Format("<b>{0}</b>", currentCharacter.Background);
        lblSkillpoints.Markup = string.Format("<b>{0}</b>", currentCharacter.SkillPoints.ToString("#0,0"));
        lblCone.Markup = string.Format("<b>{0} ({1:0,0})</b>", currentCharacter.CloneName, currentCharacter.CloneSkillPoints);
        lblDoB.Markup = string.Format("<b>{0}</b>", currentCharacter.Birthday.ToString("dd.MM.yyyy HH:mm:ss"));
        lblSecStatus.Markup = string.Format("<b>{0}</b>", currentCharacter.SecurityStatus.ToString("#0.00"));

        if(string.IsNullOrEmpty(currentCharacter.Corporation))
        {
            lblCorporation.Text = string.Empty;
            lblAlliance.Text = string.Empty;
        }
        else
        {
            lblCorporation.Markup = string.Format("<b>{0}</b>", currentCharacter.Corporation);

            if (string.IsNullOrEmpty(currentCharacter.Alliance))
            {
                lblAlliance.Text = string.Empty;
            }
            else
            {
                lblAlliance.Markup = string.Format("<b>{0}</b>", currentCharacter.Alliance);
            }
        }

        lblCorporation.Visible = !string.IsNullOrEmpty(lblCorporation.Text);
        lblAlliance.Visible = !string.IsNullOrEmpty(lblAlliance.Text);

        lblAllianceTag.Visible = lblAlliance.Visible;
        lblCorporationTag.Visible = lblCorporation.Visible;

        // Load Attributes
        int charisma = currentCharacter.Attributes.Charisma + currentCharacter.Implants.Charisma.Amount;
        int intelligence = currentCharacter.Attributes.Intelligence + currentCharacter.Implants.Intelligence.Amount;
        int memory = currentCharacter.Attributes.Memory + currentCharacter.Implants.Memory.Amount;
        int perception = currentCharacter.Attributes.Perception + currentCharacter.Implants.Perception.Amount;
        int willpower = currentCharacter.Attributes.Willpower + currentCharacter.Implants.Willpower.Amount;

        charAttributeStore.AppendValues(new Gdk.Pixbuf(null, "ECMGTK.Resources.Icons.IntelligenceBrain"), string.Format("INTELLIGENCE\n{0} points", intelligence));
        charAttributeStore.AppendValues(new Gdk.Pixbuf(null, "ECMGTK.Resources.Icons.PerceptionBrain"), string.Format("PERCEPTION\n{0} points", perception));
        charAttributeStore.AppendValues(new Gdk.Pixbuf(null, "ECMGTK.Resources.Icons.CharismaBrain"), string.Format("CHARISMA\n{0} points", charisma));
        charAttributeStore.AppendValues(new Gdk.Pixbuf(null, "ECMGTK.Resources.Icons.WillpowerBrain"), string.Format("WILLPOWER\n{0} points", willpower));
        charAttributeStore.AppendValues(new Gdk.Pixbuf(null, "ECMGTK.Resources.Icons.MemoryBrain"), string.Format("MEMORY\n{0} points", memory));

        trvAttributes.Model = charAttributeStore;

        // Traverse character skills tree and update the values
        TreeIter iter;
        charSkillStore.GetIterFirst(out iter);
        bool cont = true;

        while(cont)
        {
            // These are all the main categories
            if (charSkillStore.IterHasChild(iter))
            {
                charSkillStore.SetValue(iter, SkillLearntColumn, false);

                TreeIter child;
                charSkillStore.IterChildren(out child, iter);

                while (cont)
                {
                    long id = (long)charSkillStore.GetValue(child, SkillIdColumn);
                    ECM.EveSkill skill = ECM.ItemDatabase.Items[id] as ECM.EveSkill;
                    bool learnt = currentCharacter.Skills.ContainsKey(id);
                    int level = 0;
                    int points = 0;
                    double minsToNext = 0;

                    if (learnt)
                    {
                        level = currentCharacter.Skills[id].Level;
                        points = currentCharacter.Skills[id].Skillpoints;

                        charSkillStore.SetValue(iter, SkillLearntColumn, learnt);
                    }

                    int pointsAtNext = skill.PointsAtLevel(level + 1);
                    int pointsAtCurr = skill.PointsAtLevel(level);

                    if (level < 5)
                    {
                        long diff = pointsAtNext - points;
                        double spPerMin = ECM.Core.CurrentCharacter.SkillpointsPerMinute(skill.PrimaryAttribute, skill.SecondaryAttribute);
    
                        minsToNext = diff / spPerMin;
                    }

                    charSkillStore.SetValue(child, SkillLevelColumn, level);
                    charSkillStore.SetValue(child, SkillTimeToNextColumn, minsToNext);

                    charSkillStore.SetValue(child, SkillLearntColumn, learnt);

                    charSkillStore.SetValue(child, SkillCurrSPColumn, points);
                    charSkillStore.SetValue(child, SkillNextSPColumn, pointsAtNext);
                    charSkillStore.SetValue(child, SkillLevlSPColumn, pointsAtCurr);

                    cont = charSkillStore.IterNext(ref child);
                }
            }

            cont = charSkillStore.IterNext(ref iter);
        }

        skillsFilter.Refilter();
    }

    private bool HandleCharSkillsFilter(TreeModel model, TreeIter iter)
    {
        bool skillLearnt = (bool)charSkillStore.GetValue(iter, SkillLearntColumn);

        return skillLearnt;
    }
    #endregion

    #region Mail

    #endregion

    #region Helper Events

    // Used to stop selection on a treeview
    // Must be manually tied to the TreeView.Selection.Changed event
    protected void ClearSelection (object sender, EventArgs e)
    {
        if(sender is Gtk.TreeSelection)
        {
            TreeSelection ts = sender as TreeSelection;
            ts.UnselectAll();
        }
    }

    #endregion
}
