using System;
using Gtk;
using System.Collections.Generic;

using EveApi;

public partial class MainWindow: Gtk.Window
{	
	List<IApiRequest> requests = new List<IApiRequest>();
	
	System.Timers.Timer heartbeat = new System.Timers.Timer(1000);
	
	ListStore accCharacters = new ListStore(typeof(string), typeof(string), typeof(string));
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		
		heartbeat.AutoReset = true;
		heartbeat.Elapsed += HandleHeartbeatElapsed;
		heartbeat.Start();
		
		CreateRequests();
		
		SetupTrees();
	}

	void HandleHeartbeatElapsed (object sender, System.Timers.ElapsedEventArgs e)
	{
		foreach(IApiRequest request in requests)
			request.UpdateOnSecTick();
	}

	public void CreateRequests ()
	{
		string userID = txtUserID.Text;
		string apiKey = txtApiKey.Text;
		
		requests.Clear();
		
		AuthorisedApiRequest<AccountStatus> accStatus = new AuthorisedApiRequest<AccountStatus>(userID, apiKey);
		accStatus.OnRequestUpdate += HandleAccStatusOnRequestUpdate;
		accStatus.Enabled = true;
		
		requests.Add(accStatus);
		
		AuthorisedApiRequest<CharacterList> charList = new AuthorisedApiRequest<CharacterList>(userID, apiKey);
		charList.OnRequestUpdate += HandleCharListOnRequestUpdate;
		charList.Enabled = true;
		
		requests.Add(charList);
	}

	public void SetupTrees ()
	{
		// Create a column for the artist name
		Gtk.TreeViewColumn column = new Gtk.TreeViewColumn ();
		column.Title = "Character ID";
		Gtk.CellRendererText cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 0);
		trvCharacters.AppendColumn (column);
		
		column = new Gtk.TreeViewColumn ();
		column.Title = "Character Name";
		cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 1);
		trvCharacters.AppendColumn (column);
		
		column = new Gtk.TreeViewColumn ();
		column.Title = "Corporation";
		cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 2);
		trvCharacters.AppendColumn (column);
		
		trvCharacters.Model = accCharacters;
	}

	void HandleCharListOnRequestUpdate (ApiResult<CharacterList> result)
	{
		if (result != null && result.Error == null)
        {
			CharacterList list = result.Result;
			
			foreach(CharacterListItem character in list.Characters)
				accCharacters.AppendValues(character.CharacterID.ToString(), character.Name, character.CorporationName);
		}
	}

	void HandleAccStatusOnRequestUpdate (ApiResult<AccountStatus> result)
	{
		if (result != null && result.Error == null)
        {
			lblCreated.Text = "Created: " + result.Result.CreateDate.ToString("dd/MM/yyyy HH:mm:ss");
			lblPaidUntil.Text = "Paid Until: " + result.Result.PaidUntil.ToString("dd/MM/yyyy HH:mm:ss");
			lblLogonTimes.Text = "Number of logins: " + result.Result.LogonCount;
			lblMinutesPlayed.Text = "Minutes spent playing: " + result.Result.LogonMinutes;
		}
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
