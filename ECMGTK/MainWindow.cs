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

public partial class MainWindow: Gtk.Window
{    
    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();
        
		// Hide the tabs so that we can make the app look awesome :D
		ntbPages.ShowTabs = false;
        ntbPages.CurrentPage = 0;
		
		lblBackground.UseMarkup = true;
		lblBackground.Markup = "Go to the <a href=\"http://www.gtk.org\" title=\"&lt;i&gt;Our&/i&gt; website\">GTK+ website</a> for more...";
		lblBackground.Text = "<b>Hello</b>";
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
	}
}
