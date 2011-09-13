//  //  MainWindow.cs//  //  Author://       Dan Silk <silkypantsdan@gmail.com>// //  Copyright (c) 2011 Dan Silk// //  This program is free software: you can redistribute it and/or modify//  it under the terms of the GNU General Public License as published by//  the Free Software Foundation, either version 3 of the License, or//  (at your option) any later version.// //  This program is distributed in the hope that it will be useful,//  but WITHOUT ANY WARRANTY; without even the implied warranty of//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the//  GNU General Public License for more details.// //  You should have received a copy of the GNU General Public License//  along with this program.  If not, see <http://www.gnu.org/licenses/>.using System;using Gtk;using System.ComponentModel;using System.Diagnostics;using System.Timers;public partial class MainWindow: Gtk.Window{        public MainWindow (): base (Gtk.WindowType.Toplevel)    {        Build ();                ntbPages.CurrentPage = 0;				FillTabsWithImages();        		BackgroundWorker worker = new BackgroundWorker();		worker.DoWork += delegate {			LoadMarket();		};				worker.RunWorkerCompleted += HandleWorkerRunWorkerCompleted;				worker.RunWorkerAsync();				Visible = true;        Timer heartbeat = new Timer(1000);        heartbeat.AutoReset = true;        heartbeat.Elapsed += delegate        {            ECM.Core.Data.UpdateOnHeartbeat();        };        heartbeat.Start();    }    void HandleWorkerRunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)    {    }	public void FillTabsWithImages ()	{		ntbPages.SetTabLabelPacking(vbxOverview, false, false, PackType.Start);		ntbPages.SetTabLabel(vbxOverview, CreateTabLabel("Overview", "ECMGTK.Resources.Icons.Home.png"));				ntbPages.SetTabLabelPacking(vbxCharSheet, false, false, PackType.Start);		ntbPages.SetTabLabel(vbxCharSheet, CreateTabLabel("Character Sheet", "ECMGTK.Resources.Icons.CharSheet.png"));				ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));		//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));//		//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));//		//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));//		//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));//		//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));//		//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));//		//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));//		//		ntbPages.SetTabLabelPacking(hpnMarket, false, false, PackType.Start);//		ntbPages.SetTabLabel(hpnMarket, CreateTabLabel("Market", "ECMGTK.Resources.Icons.Market.png"));	}	public Widget CreateTabLabel (string title, string imageResource)	{		HBox box = new HBox();		Image icon = new Image(null, imageResource);		Label label = new Label(title);		label.Xalign = 0;				box.PackEnd(icon, false, false, 0);		box.PackStart(label, true, true, 0);				box.ShowAll();				return box;	}        private Button CreateCharacterButton()    {        Button btnChar = new Button();        Image imgPortrait = new Image(null, "ECMGTK.Resources.NoPortrait_64.png");        Image imgRecycle = new Image(null, "ECMGTK.Resources.Icons.Recycle.png");        Button btnRecycle = new Button(imgRecycle);        Label lblStatus = new Label();        HBox hbxContainer = new HBox(false, 1);		                lblStatus.Text = "This is some text\nIt spans multiple lines\nSo we can show cool stuff. This is to push the button out more, I can make it wrap with the WidthChar member";        lblStatus.WidthChars = 40;        lblStatus.Wrap = true;                imgPortrait.WidthRequest = 64;        imgPortrait.HeightRequest = 64;                hbxContainer.PackStart(imgPortrait);        hbxContainer.PackStart(lblStatus);        hbxContainer.PackStart(btnRecycle);                btnChar.Add(hbxContainer);                btnChar.ShowAll();				btnChar.Clicked += delegate {			Console.WriteLine("Called Char Button Press");		};				btnRecycle.Clicked += delegate {			Console.WriteLine("Called Char Recycle Button Press");		};                return btnChar;    }        protected void OnDeleteEvent (object sender, DeleteEventArgs a)    {		        Application.Quit ();        a.RetVal = true;    }    protected void RowCollapsed (object sender, Gtk.RowCollapsedArgs args)    {        TreeView tv = sender as TreeView;                if(tv != null)        {         tv.ColumnsAutosize();        }    }        protected void SearchTextChanged (object sender, System.EventArgs e)    {         marketFilter.Refilter();    }    protected void RowActivated (object o, Gtk.RowActivatedArgs args)    {        TreeIter iter;                if(marketStore.GetIter(out iter, args.Path))        {            if(marketStore.IterHasChild(iter) == false)            {                // Item or empty group            }            else            {                if(trvMarket.GetRowExpanded(args.Path))                    trvMarket.CollapseRow(args.Path);                else                    trvMarket.ExpandRow(args.Path, false);            }        }    }        #region Database
    TreeStore marketStore = new TreeStore(typeof(Gdk.Pixbuf), typeof(string), typeof(long));
    ListStore itemStore = new ListStore(typeof(string), typeof(long));    TreeModelFilter marketFilter;        public void LoadMarket()    {        		Stopwatch sw = Stopwatch.StartNew();		        marketStore.Clear();
        itemStore.Clear();                TreeViewColumn mainColumn = new TreeViewColumn();        mainColumn.Title = "Groups";                CellRendererPixbuf pixBuf = new CellRendererPixbuf();        pixBuf.Xalign = 0;        mainColumn.PackStart(pixBuf, false);        mainColumn.AddAttribute(pixBuf, "pixbuf", 0);                CellRendererText label = new CellRendererText();        label.Xalign = 0;        mainColumn.PackStart(label, false);        mainColumn.AddAttribute(label, "text", 1);                trvMarket.AppendColumn(mainColumn);				ECM.Core.ItemDatabase.LoadMarket(marketStore, itemStore);                trvMarket.ColumnsAutosize();                TreeModelSort sorted = new TreeModelSort(marketStore);                sorted.SetSortColumnId(1, SortType.Ascending);                trvMarket.Model = sorted;

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
        
        trvSearchItems.Model = sorted;    }

    void trvSelectionChanged (object sender, EventArgs e)
    {
        TreeIter iter;        TreeModel model;
        if(trvMarket.Selection.GetSelected(out model, out iter))
        {
            if(model.GetValue(iter, 0) == null)
            {
                long ID = Convert.ToInt64(model.GetValue(iter, 2));
                string name = model.GetValue(iter, 1).ToString();

                Button btn = new Button(new Label(name + ID.ToString()));
                btn.ShowAll();

                vbbItems.Add(btn);
            }
        }
    }        private bool HandleMarketFilter (TreeModel model, TreeIter iter)    {     string itemName = model.GetValue (iter, 0).ToString ();     if (txtMarketFilter.Text == "")         return false;         if (itemName.Contains(txtMarketFilter.Text))         return true;     else         return false;    }    #endregion		#region Overview	protected void AddNewKey (object sender, System.EventArgs e)	{		ECMGTK.AddApiKey addKey = new ECMGTK.AddApiKey();				addKey.Run();        foreach(ECM.Core.Character character in ECM.Core.Data.Characters.Values)        {            vbbCharacters.Add(CreateCharacterButton(character));        }	}
    protected Widget CreateCharacterButton (ECM.Core.Character character)    {        HBox box = new HBox();

        Frame frm = new Frame();
        frm.Shadow = ShadowType.EtchedOut;
        frm.BorderWidth = 3;                Image img = new Image(null, "ECMGTK.Resources.NoPortrait_64.png");        img.WidthRequest = 64;        img.HeightRequest = 64;

        frm.Add(img);        box.PackStart(frm, false, false, 3);        Label text = new Label(character.Name);        text.Xalign = 0;        box.PackStart(text, true, true, 0);


        Button btn = new Button(box);

        btn.ShowAll();        return btn;    }	#endregion}