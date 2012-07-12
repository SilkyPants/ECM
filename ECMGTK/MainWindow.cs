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
using System.ComponentModel;
using System.Timers;
using ECMGTK;
using GLib;
using Gtk;
using ECM.API.EVE;

public partial class MainWindow: Gtk.Window
{

    #region Constants
    const int HEARTBEAT_RATE = 100;
    const int MARKET_PANE_DEFAULT = 270;
    const int STARTING_PAGE = 0;
    #endregion

    #region Structures
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
    #endregion

    StatusIcon m_TrayIcon;

    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();

        // Creation of the Tray Icon
        m_TrayIcon = new StatusIcon(Gdk.Pixbuf.LoadFromResource("ECMGTK.Resources.Icons.Corpse.png"));
        m_TrayIcon.Visible = false;

        // Show/Hide the window (even from the Panel/Taskbar) when the TrayIcon has been clicked.
        m_TrayIcon.Activate += OnTrayIconPopup; ;

        // Show a pop up menu when the icon has been right clicked.
        m_TrayIcon.PopupMenu += OnTrayIconPopup;

        // A Tooltip for the Icon
        m_TrayIcon.Tooltip = "Eve Character Monitor";

        ntbPages.CurrentPage = STARTING_PAGE;
        hpnMarket.Position = MARKET_PANE_DEFAULT;

        while (Gtk.Application.EventsPending ())
            Gtk.Application.RunIteration ();

        GLib.ExceptionManager.UnhandledException += OnUnhandledException;

		FillTabsWithImages();
        
		BackgroundWorker worker = new BackgroundWorker();
		worker.DoWork += delegate 
        {
            LoadMarket();
            SetupCharacterTrees();
            ECM.MapDatabase.LoadMap();
            ECM.Core.Init();

            Gtk.Application.Invoke(delegate
            {
                Show();
                m_TrayIcon.Visible = true;
            });
        };
		
		worker.RunWorkerCompleted += HandleWorkerRunWorkerCompleted;
		hpnMarket.Sensitive = false;
		worker.RunWorkerAsync();

        imgNetworkIndicator.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF16);
        //imgNetworkIndicator.Visible = false;

        SetupGui();

        ECM.Core.OnUpdateGui += new EventHandler(UpdateGui);
        ECM.Core.OnCharacterChanged += CharacterChanged;
        ECM.Core.OnTQServerUpdate += TQServerUpdate;

        Timer heartbeat = new Timer(HEARTBEAT_RATE);
        heartbeat.AutoReset = true;
        heartbeat.Elapsed += new ElapsedEventHandler(heartbeat_Elapsed);
        heartbeat.Start();
    }

    void OnTrayIconPopup (object sender, EventArgs e)
    {
        this.Visible = !this.Visible;
    }

    #region Event Handlers

    void heartbeat_Elapsed(object sender, ElapsedEventArgs e)
    {
        ECM.Core.UpdateOnHeartbeat();
        ECM.Core.SaveAccounts();
        imgNetworkIndicator.Visible = ECM.Core.IsNetworkActivity;
    }

    void OnUnhandledException (UnhandledExceptionArgs args)
    {
        Console.WriteLine(args.ExceptionObject);
    }

    void TQServerUpdate (ECM.API.EVE.ServerStatus status)
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
        m_TrayIcon.Visible = false;
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
        SetupAssets();
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

        ntbPages.SetTabLabelPacking(vbxAssets, false, false, PackType.Start);
        ntbPages.SetTabLabel(vbxAssets, CreateTabLabel("Assets", "ECMGTK.Resources.Icons.Assets.png", true));
        
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

    void SetupAssets ()
    {
        TreeViewColumn mainColumn = new TreeViewColumn();
        mainColumn.Title = "Assets";

        CellRendererEveTree itemCell = new CellRendererEveTree();
        itemCell.RenderInfo = false;

        mainColumn.PackStart(itemCell, true);

        mainColumn.AddAttribute(itemCell, "Text", 0);
        mainColumn.AddAttribute(itemCell, "IsHeading", 2);
        
        trvAssets.AppendColumn(mainColumn);
        trvAssets.ColumnsAutosize();

        trvAssets.EnableTreeLines = false;
        trvAssets.ShowExpanders = false;
        trvAssets.LevelIndentation = 16;
        trvAssets.Selection.Changed += ExpandGroup;
    }

    public void SetupMarket ()
    {
        TreeViewColumn mainColumn = new TreeViewColumn();
        mainColumn.Title = "Groups";

        CellRendererEveTree itemCell = new CellRendererEveTree();

        mainColumn.PackStart(itemCell, true);

        mainColumn.AddAttribute(itemCell, "Icon", 0);
        mainColumn.AddAttribute(itemCell, "Text", 1);
        mainColumn.AddAttribute(itemCell, "IsHeading", 4);
        
        trvMarket.AppendColumn(mainColumn);

//        trvMarket.TooltipColumn = 1;
//        trvMarket.HasTooltip = true;

        trvMarket.ColumnsAutosize();
        
        trvMarket.EnableTreeLines = false;
        trvMarket.ShowExpanders = false;
        trvMarket.LevelIndentation = 16;
        trvMarket.Selection.Changed += trvSelectionChanged;
        trvMarket.Selection.Changed += ExpandGroup;
        trvMarket.ButtonPressEvent += onMarketClick;

        // Search Item Tree
        mainColumn = new TreeViewColumn();
        mainColumn.Title = "Groups";
        
        CellRendererText label = new CellRendererText();
        label.Xalign = 0;
        mainColumn.PackStart(label, false);
        mainColumn.AddAttribute(label, "text", 0);

        trvSearchItems.AppendColumn(mainColumn);

        trvSearchItems.ColumnsAutosize();
    }

    [GLib.ConnectBefore]
    void onMarketClick (object o, ButtonPressEventArgs args)
    {
        args.RetVal = false;
        TreeView tree = o as TreeView;

        if (tree == null) return;

        // Check we have an item selected
        TreePath path;
        TreeIter iter;
        TreeViewColumn col;
        int cellX, cellY;
        TreeModel model = tree.Model;

        // Get path to node under mouse
        tree.GetPathAtPos((int)args.Event.X, (int)args.Event.Y, out path, out col, out cellX, out cellY);
        Gdk.Rectangle cellArea = tree.GetCellArea(path, col);

        // Convert to iter
        if (model.GetIter(out iter, path)) 
        {
            bool isItem = !Convert.ToBoolean(model.GetValue(iter, 4));

            if (isItem)
            {
                long ID = Convert.ToInt64(model.GetValue(iter, 2));
                ECM.EveItem item = ECM.ItemDatabase.Items[ID];

                // Right mouse click
                if (args.Event.Button == 3)
                {
                    Menu m = new Menu();

                    MenuItem view = new MenuItem("View Item Details");

                    m.Add(view);

                    view.ButtonPressEvent += delegate(object sender, ButtonPressEventArgs e)
                    {
                        if (e.Event.Button == 1)
                        {
                            // Show selected item details
                            m_ViewDetails.ShowItemDetails(item);
                        }
                    };

                    m.ShowAll();
                    m.Popup();
                }
                else if (args.Event.Button == 1 && cellX >= (cellArea.Width + cellArea.X) - 16)
                {
                    m_ViewDetails.ShowItemDetails(item);

                    args.RetVal = true;
                }
            }
        }
    }

    private void SetupCharacterSheet()
    {
        imgCharPortrait.Pixbuf = ECM.API.ImageApi.StreamToPixbuf(ECM.Core.NoPortraitJPG).ScaleSimple(198,198,Gdk.InterpType.Hyper);
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

        skillColumn.AddAttribute(skillCell, "Text", SkillNameColumn);
        skillColumn.AddAttribute(skillCell, "IsHeading", SkillIsHeadingColumn);
        skillColumn.AddAttribute(skillCell, "SkillRank", SkillRankColumn);
        skillColumn.AddAttribute(skillCell, "SkillCurrSP", SkillCurrSPColumn);
        skillColumn.AddAttribute(skillCell, "SkillNextSP", SkillNextSPColumn);
        skillColumn.AddAttribute(skillCell, "SkillLevlSP", SkillLevlSPColumn);
        skillColumn.AddAttribute(skillCell, "SkillLevel", SkillLevelColumn);
        skillColumn.AddAttribute(skillCell, "SkillMinsToNext", SkillTimeToNextColumn);

        trvSkills.AppendColumn(skillColumn);

        trvSkills.EnableTreeLines = false;
        trvSkills.ShowExpanders = false;
        trvSkills.Selection.Changed += ExpandGroup;

        #endregion

        #region Certificates Treeview
        TreeViewColumn column = new TreeViewColumn();
        skillColumn.Title = "Cert";

        CellRendererCertificate cell = new CellRendererCertificate();

        column.PackStart(cell, true);

        column.AddAttribute(cell, "Text", 0);
        column.AddAttribute(cell, "CertGrade", 1);
        column.AddAttribute(cell, "IsHeading", 3);

        trvCertificates.AppendColumn(column);

        trvCertificates.EnableTreeLines = false;
        trvCertificates.ShowExpanders = false;
        trvCertificates.Selection.Changed += ExpandGroup;
        #endregion

        #region Standings Treeview
        column = new TreeViewColumn();
        skillColumn.Title = "Standings";

        CellRendererEveTree standingsCell = new CellRendererEveTree();

        column.PackStart(standingsCell, true);

        column.AddAttribute(standingsCell, "Text", 0);
        column.AddAttribute(standingsCell, "AnimatedIcon", 1);
        column.AddAttribute(standingsCell, "IsHeading", 2);

        trvStandings.AppendColumn(column);

        trvStandings.EnableTreeLines = false;
        trvStandings.ShowExpanders = false;
        trvStandings.Selection.Changed += ExpandGroup;
        #endregion
    }

    void ExpandGroup(object sender, EventArgs e)
    {
        TreeSelection selection = sender as TreeSelection;

        if (selection != null)
        {
            TreeView tree = selection.TreeView;
            TreePath[] paths = selection.GetSelectedRows();
            if (paths.Length > 0)
            {
                bool expanded = tree.GetRowExpanded(paths[0]);

                if (expanded)
                    tree.CollapseRow(paths[0]);
                else
                    tree.ExpandRow(paths[0], false);
            }

            selection.UnselectAll();
        }
    }

    void UpdateCharacterPortrait (object o, ButtonPressEventArgs args)
    {
        if(args.Event.Button == 3)
        {
            Menu m = new Menu();

            MenuItem update = new MenuItem("Update Character Portrait");
            MenuItem render = new MenuItem("View Larger Render");

            m.Add(update);
            m.Add(render);

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

            render.ButtonPressEvent += delegate(object sender, ButtonPressEventArgs e)
            {
                if(e.Event.Button == 1)
                {
                    m_ViewRender.ShowCharacterRender(ECM.Core.CurrentCharacter, ECM.API.ImageApi.ImageRequestSize.Size512x512);
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
