//  
//  ViewItemDetails.cs
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
using System.Collections.Generic;
using Gtk;

namespace ECMGTK
{
    public partial class ViewItemDetails : Gtk.Window
    {
        Dictionary<long, Widget> m_CurrentItems = new Dictionary<long, Widget>();

        public ViewItemDetails () : 
                base(Gtk.WindowType.Toplevel)
        {
            this.Build ();
        }

        public void ShowItemDetails (ECM.EveItem item)
        {
            if (m_CurrentItems.ContainsKey (item.ID)) {
                ntbItems.CurrentPage = ntbItems.PageNum(m_CurrentItems [item.ID]);
            } 
            else 
            {
                // Create new page
                Widget page = CreateNewPage(item);

                // Add item to dictionary
                m_CurrentItems.Add(item.ID, page);

                // Actually create the page
                ntbItems.CurrentPage = ntbItems.AppendPage(page, CreateTabLabel(item, page));
            }

            ShowAll();
        }

        private Widget CreateNewPage(ECM.EveItem item)
        {
            VBox itemDetails = new VBox();

            HBox detailsTop = new HBox();
            Frame imageFrame = new Frame();
            Image itemImage = new Image();
            Label itemName = new Label(item.Name);

            itemImage.WidthRequest = 64;
            itemImage.HeightRequest = 64;
            imageFrame.Add(itemImage);

            imageFrame.Shadow = ShadowType.EtchedIn;
            detailsTop.PackStart(imageFrame);
            detailsTop.PackEnd(itemName);

            itemName.Yalign = 0.1f;


            itemDetails.ShowAll();

            return itemDetails;
        }

        private Widget CreateTabLabel (ECM.EveItem item, Widget page)
        {
            HBox box = new HBox();
            Image icon = new Image("gtk-close", IconSize.Menu);
            Button btnClose = new Button(icon);
            btnClose.Relief = ReliefStyle.None;
            btnClose.Clicked += delegate(object sender, EventArgs e)
            {
                ntbItems.RemovePage(ntbItems.PageNum(page));
                m_CurrentItems.Remove(item.ID);

                // If all pages are gone - hide
                if(ntbItems.NPages == 0)
                    Hide ();
            };

            Label label = new Label(item.Name);
            label.Xalign = 0;

            box.PackEnd(btnClose, false, false, 3);

            box.PackStart(label, true, true, 6);
            
            box.ShowAll();
            
            return box;
        }       

        protected void OnDelete (object o, DeleteEventArgs args)
        {            
            // Remove all pages
            while(ntbItems.NPages > 0)
                ntbItems.RemovePage(0);

            // Clear the dictionary
            m_CurrentItems.Clear();

            // Then Hide
            Hide ();

            args.RetVal = true;
        }


    }
}

