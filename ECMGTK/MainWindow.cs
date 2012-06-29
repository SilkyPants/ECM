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

public partial class MainWindow: Gtk.Window
{
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

    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();
        
        ntbPages.CurrentPage = 0;
        hpnMarket.Position = 250;

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

            Gtk.Application.Invoke(delegate
            {
                Show();
            });
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

    #region Event Handlers

    void heartbeat_Elapsed(object sender, ElapsedEventArgs e)
    {
        ECM.Core.UpdateOnHeartbeat();
        ECM.Core.SaveAccounts();
    }

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

        trvSkills.EnableTreeLines = false;
        trvSkills.ShowExpanders = false;
        trvSkills.Selection.Changed += ExpandSkillGroup;

        #endregion

        #region Certificates Treeview

        #endregion
    }

    void ExpandSkillGroup(object sender, EventArgs e)
    {
        TreePath[] paths = trvSkills.Selection.GetSelectedRows();
        if(paths.Length > 0)
        {
            bool expanded = trvSkills.GetRowExpanded(paths[0]);
            if(trvSkills.GetRowExpanded(paths[0]))
                trvSkills.CollapseRow(paths[0]);
            else
                trvSkills.ExpandRow(paths[0], false);
        }

        trvSkills.Selection.UnselectAll();
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
