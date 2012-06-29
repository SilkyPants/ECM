//  
//  MainWindow.Overview.cs
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
using Gtk;

public partial class MainWindow : Gtk.Window
{

    protected void AddNewKey(object sender, System.EventArgs e)
    {
        ECMGTK.AddApiKey addKey = new ECMGTK.AddApiKey();

        addKey.Run();
    }

    protected Widget CreateAccountWidget(ECM.Account account)
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
        delAccBtn.Clicked += delegate(object sender, EventArgs e)
        {
            //TODO: Make this work
            Console.WriteLine(string.Format("Deleting Account #{0}", account.KeyID));
        };

        Button editAccBtn = new Button();
        editAccBtn.Relief = ReliefStyle.None;
        editAccBtn.TooltipText = "Edit Account";
        editAccBtn.Add(new Image("gtk-edit", IconSize.Menu));
        editAccBtn.Clicked += delegate(object sender, EventArgs e)
        {
            //TODO: Make this work
            Console.WriteLine(string.Format("Editing Account #{0}", account.KeyID));
        };

        HBox accHeader = new HBox();
        accHeader.Spacing = 2;
        accHeader.PackStart(accKey, false, false, 0);
        accHeader.PackStart(sep, true, true, 0);
        accHeader.PackStart(editAccBtn, false, false, 0);
        accHeader.PackStart(delAccBtn, false, false, 0);

        accWidget.PackStart(accHeader, true, false, 0);

        TextView accStats = new TextView();
        accWidget.PackStart(accStats, true, false, 0);

        TextTag expired = new TextTag("expired");
        expired.Weight = Pango.Weight.Bold;
        expired.Foreground = "red";

        accStats.Editable = false;
        accStats.Sensitive = false;

        TimeSpan playTime = new TimeSpan(0, account.LogonMinutes, 0);
        string paidUntilString = account.PaidUntil.ToLocalTime().ToString();
        string accStatus = "Paid Until:";

        if (account.PaidUntilExpired)
        {
            accKey.UseMarkup = true;
            accKey.Markup = string.Format("<span foreground=\"red\" font_weight=\"bold\">{0}</span>", accKey.Text);

            accStatus = "Expired:";
            accStats.Buffer.TagTable.Add(expired);
        }

        accStats.Buffer.Text = string.Format("Time spent playing: {0}", ECM.Helper.GetDurationInWords(playTime));

        TextIter itr = accStats.Buffer.GetIterAtLine(0);
        accStats.Buffer.InsertWithTags(ref itr, string.Format("{0} {1}\n", accStatus, paidUntilString), expired);

        foreach (ECM.Character ecmChar in account.Characters)
        {
            if (ecmChar.AutoUpdate)
                accWidget.PackStart(CreateCharacterButton(ecmChar), true, false, 0);
        }

        accWidget.ShowAll();

        return accWidget;
    }

    protected Widget CreateCharacterButton(ECM.Character character)
    {
        HBox box = new HBox();

        Frame frm = new Frame();
        frm.Shadow = ShadowType.EtchedOut;
        frm.BorderWidth = 3;

        Image img = new Image();
        img.WidthRequest = 64;
        img.HeightRequest = 64;

        if (character.Portrait != null)
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

    public void FillAccounts()
    {
        while (vbxAccounts.Children.Length > 0)
        {
            vbxAccounts.Remove(vbxAccounts.Children[0]);
        }

        foreach (ECM.Account account in ECM.Core.Accounts.Values)
        {
            vbxAccounts.PackStart(CreateAccountWidget(account), false, false, 0);
        }
    }
}
