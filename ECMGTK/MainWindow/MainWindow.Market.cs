//  
//  MainWindow.Market.cs
//  
//  Author:
//       Dan Silk <silkypantsdan@gmail.com>
// 
//  Copyright (c) 2012 Dan Silk
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
using ECMGTK;
using Gtk;
using ECM;

public partial class MainWindow : Gtk.Window
{
    TreeStore marketStore = new TreeStore(typeof(Gdk.Pixbuf), typeof(string), typeof(long), typeof(bool), typeof(bool));
    ListStore itemStore = new ListStore(typeof(string), typeof(long));
    TreeModelFilter marketSearchFilter;
    ViewItemRender m_ViewRender = new ViewItemRender();
    ViewItemDetails m_ViewDetails = new ViewItemDetails();

    readonly Colour m_Untrainable = new Colour(128, 0, 0, 128);
    readonly Colour m_Trainable = new Colour(255, 128, 0, 128);
    readonly Colour m_Useable = new Colour(0, 128, 0, 128);

    protected void RowCollapsed(object sender, Gtk.RowCollapsedArgs args)
    {
        TreeView tv = sender as TreeView;

        if (tv != null)
        {
            tv.ColumnsAutosize();
        }
    }

    protected void SearchTextChanged(object sender, System.EventArgs e)
    {
        marketSearchFilter.Refilter();
    }

    protected void RowActivated(object o, Gtk.RowActivatedArgs args)
    {
        TreeIter iter;

        if (marketStore.GetIter(out iter, args.Path))
        {
            if (marketStore.IterHasChild(iter))
            {
                if (trvMarket.GetRowExpanded(args.Path))
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

        marketSearchFilter = new TreeModelFilter(itemStore, null);
        marketSearchFilter.VisibleFunc = new TreeModelFilterVisibleFunc(HandleMarketSearchFilter);

        TreeModelSort sortedFilter = new TreeModelSort(marketSearchFilter);

        sortedFilter.SetSortColumnId(0, SortType.Ascending);

        trvSearchItems.Model = sortedFilter;

        Console.WriteLine("Market Loaded");
    }

    void trvSelectionChanged(object sender, EventArgs e)
    {
        TreeIter iter;
        TreeModel model;
        if (trvMarket.Selection.GetSelected(out model, out iter))
        {
            bool hasItems = (bool)model.GetValue(iter, 3);

            if (model.GetValue(iter, 0) == null)
            {
                long ID = Convert.ToInt64(model.GetValue(iter, 2));
                ECM.EveItem item = ECM.ItemDatabase.Items[ID];

                // Add item's market group to the list
                TreeIter parentIter;

                // get Parent iterator
                if (model.IterParent(out parentIter, iter))
                {
                    ShowMarketGroupItems(model, parentIter);

                    // Open Item Market Details
                    ShowItemMarketDetails(item, model, iter);
                }
            }
            else if (hasItems)
            {
                ShowMarketGroupItems(model, iter);
            }
        }
    }

    void SelectItemInMarket (ECM.EveBase item)
    {
        Gtk.TreeIter iter;

        if (marketStore.GetIter(out iter, item.TreeReference.Path))
        {
            ntbPages.CurrentPage = 5;

            TreeModelSort sortedMarket = trvMarket.Model as TreeModelSort;

            trvMarket.CollapseAll();
            trvMarket.ExpandToPath(sortedMarket.ConvertChildPathToPath(item.TreeReference.Path));
            trvMarket.Selection.SelectIter(sortedMarket.ConvertChildIterToIter(iter));
            trvSelectionChanged(null, null);
        }
    }

    void ShowItemMarketDetails (ECM.EveItem item, TreeModel model, TreeIter iter)
    {
        ntbMarketDetails.CurrentPage = 0;

        foreach (Widget w in hbxItemPath.Children) 
        {
            hbxItemPath.Remove(w);
            w.Destroy();
        }

        // First work out the tree path
        TreeIter parentIter;
        string path = "";
        long ID = 0;
        while (model.IterParent(out parentIter, iter))
        {
            iter = parentIter;
            path = model.GetValue(parentIter, 1).ToString();
            ID = Convert.ToInt64(model.GetValue(iter, 2));

            ECM.EveMarketGroup g = ECM.ItemDatabase.MarketGroups[ID];

            Button btn = new Button(new Label(path));
            btn.Relief = ReliefStyle.None;

            btn.Clicked += delegate(object sender, EventArgs e) 
            {
                SelectItemInMarket(g);
            };

            hbxItemPath.PackEnd(new Label("\\"));
            hbxItemPath.PackEnd(btn);
        }

        hbxItemPath.ShowAll();

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

        //lblItemTreeDetails.Visible = true;
        imgItemIconDetails.Visible = true;
        btnItemInfo.Visible = true;
        vbxBuySell.Visible = true;
        frmItemImage.ShadowType = ShadowType.EtchedOut;
    }

    protected void ShowItemRender(object sender, System.EventArgs e)
    {
        long itemID;

        if (long.TryParse(btnShowRender.Name, out itemID))
        {
            m_ViewRender.ShowItemRender(ECM.ItemDatabase.Items[itemID]);
        }
    }

    protected void ShowItemDetails (object sender, EventArgs e)
    {
        long itemID;

        if (long.TryParse(btnShowRender.Name, out itemID))
        {
            m_ViewDetails.ShowItemDetails(ECM.ItemDatabase.Items[itemID]);
        }
    }

    void ShowMarketGroupItems(TreeModel model, TreeIter iter)
    {
        // Clear the VBox
        while (vbbMarketGroups.Children.Length > 0)
        {
            vbbMarketGroups.Remove(vbbMarketGroups.Children[0]);
        }

        TreeIter childIter;
        // get children iterator
        if (model.IterChildren(out childIter, iter))
        {
            do
            {
                long ID = Convert.ToInt64(model.GetValue(childIter, 2));
                ECM.EveItem item = ECM.ItemDatabase.Items[ID];

                AddItemToCurrentMarketGroup(item, model, childIter);
            }
            while (model.IterNext(ref childIter));

            ntbMarketDetails.CurrentPage = 1;
        }
    }

    void AddItemToCurrentMarketGroup(ECM.EveItem item, TreeModel model, TreeIter iter)
    {
        if (vbbMarketGroups.IsRealized == false)
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

        EveItemUseability useability = Core.CurrentCharacter.GetItemUseability(item);

        if (useability == EveItemUseability.Untrainable)
            buf.Fill(m_Untrainable.ToUint());
        else if (useability == EveItemUseability.Trainable)
            buf.Fill(m_Trainable.ToUint());
        else
            buf.Fill(m_Useable.ToUint());
        
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
        btnInfo.Clicked += delegate(object sender, EventArgs e) 
        {
            m_ViewDetails.ShowItemDetails(item);           
        };

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

        Button viewDets = new Button(new Label("View Market Details"));
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

    private bool HandleMarketSearchFilter(TreeModel model, TreeIter iter)
    {
        string itemName = model.GetValue(iter, 0).ToString();

        if (txtMarketFilter.Text == "")
            return false;

        if (itemName.Contains(txtMarketFilter.Text))
            return true;
        else
            return false;
    }
}
