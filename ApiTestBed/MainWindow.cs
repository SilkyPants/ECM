using System;
using Gtk;
using System.Collections.Generic;

using EveApi;

public partial class MainWindow: Gtk.Window
{	
	List<IApiRequest> requests = new List<IApiRequest>();
	
	System.Timers.Timer heartbeat = new System.Timers.Timer(1000);
	
	ListStore accCharacters = new ListStore(typeof(string), typeof(string), typeof(string));
	ListStore chaQueue = new ListStore(typeof(string), typeof(string), typeof(string));
	ListStore mapKills = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));
	
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

        AuthorisedApiRequest<ApiKeyInfo> charList = new AuthorisedApiRequest<ApiKeyInfo>(userID, apiKey);
		charList.OnRequestUpdate += HandleCharListOnRequestUpdate;
		charList.Enabled = true;
		
		requests.Add(charList);
				
		ApiRequest<ServerStatus> serverStatus = new ApiRequest<ServerStatus>();
		serverStatus.OnRequestUpdate += HandleServerStatusOnRequestUpdate;
		serverStatus.Enabled = true;
		
		requests.Add(serverStatus);
				
		ApiRequest<MapKills> mapKills = new ApiRequest<MapKills>();
		mapKills.OnRequestUpdate += HandleMapKillsOnRequestUpdate;
		mapKills.Enabled = true;
		
		requests.Add(mapKills);
	}

	void HandleMapKillsOnRequestUpdate (ApiResult<MapKills> result)
	{
		if(result != null && result.Error == null)
		{
			mapKills.Clear();
			
			MapKills list = result.Result;
			
			foreach(SystemKills info in list.SystemKills)
				mapKills.AppendValues(info.SolarSystemID.ToString(), info.ShipKills.ToString(), info.FactionKills.ToString(), info.PodKills.ToString());			
		}
	}
	
	public void CreateCharacterRequests()
	{
		string userID = txtUserID.Text;
		string apiKey = txtApiKey.Text;
		int charID = int.Parse(txtCharID.Text);
		
		CharacterApiRequest<CharacterSheet> charSheet = new CharacterApiRequest<CharacterSheet>(charID, userID, apiKey);
		charSheet.OnRequestUpdate += HandleCharSheetOnRequestUpdate;
		charSheet.Enabled = true;
		
		requests.Add(charSheet);
		
		CharacterApiRequest<SkillQueue> charQueue = new CharacterApiRequest<SkillQueue>(charID, userID, apiKey);
		charQueue.OnRequestUpdate += HandleCharQueueOnRequestUpdate;
		charQueue.Enabled = true;
		
		requests.Add(charQueue);
		
		CharacterApiRequest<SkillInTraining> charTraining = new CharacterApiRequest<SkillInTraining>(charID, userID, apiKey);
		charTraining.OnRequestUpdate += HandleCharTrainingOnRequestUpdate;
		charTraining.Enabled = true;

        requests.Add(charTraining);

        CharacterApiRequest<AssetList> asset = new CharacterApiRequest<AssetList>(charID, userID, apiKey);
        asset.OnRequestUpdate += new ApiRequest<AssetList>.RequestUpdated(asset_OnRequestUpdate);
        asset.Enabled = true;

        requests.Add(asset);
	}

    void asset_OnRequestUpdate(ApiResult<AssetList> result)
    {
        if (result != null && result.Error == null)
        {
            Console.WriteLine(result.XmlDocument.InnerXml);
        }
    }

	void HandleCharTrainingOnRequestUpdate (ApiResult<SkillInTraining> result)
	{
		if(result != null && result.Error == null)
		{
			lblCurrTraining.Text = string.Format("Currently Training: {0} {1} -> {2}", result.Result.TypeID, result.Result.StartTimeXML, result.Result.EndTimeXML);
		}
	}

	void HandleCharQueueOnRequestUpdate (ApiResult<SkillQueue> result)
	{
		if(result != null && result.Error == null)
		{
			chaQueue.Clear();
			
			SkillQueue list = result.Result;
			
			foreach(SkillQueueInfo info in list.Queue)
				chaQueue.AppendValues(info.TypeID.ToString(), info.StartTimeXML, info.EndTimeXML);			
		}
	}

	void HandleServerStatusOnRequestUpdate (ApiResult<ServerStatus> result)
	{
		if(result != null && result.Error == null)
		{
			if(result.Result.ServerOnline)
			{
				lblServerStatus.Text = string.Format("Tranquility online with {0} pilots.", result.Result.NumberOfPlayers);
			}
			else
			{
				lblServerStatus.Text = "Tranquility is offline.";
			}
		}
	}

	void HandleCharSheetOnRequestUpdate (ApiResult<CharacterSheet> result)
	{
		if(result != null)
		{
			if(result.Error == null)
				txtCharSheet.Buffer.Text = result.XmlDocument.InnerXml;
			else
			{
				requests[2].Enabled = false;
			}
		}
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
		
		
		column = new Gtk.TreeViewColumn ();
		column.Title = "Type ID";
		cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 0);
		trvSkillQueue.AppendColumn (column);
		
		column = new Gtk.TreeViewColumn ();
		column.Title = "Start";
		cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 1);
		trvSkillQueue.AppendColumn (column);
		
		column = new Gtk.TreeViewColumn ();
		column.Title = "End";
		cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 2);
		trvSkillQueue.AppendColumn (column);
		
		trvSkillQueue.Model = chaQueue;
		
		
		column = new Gtk.TreeViewColumn ();
		column.Title = "System ID";
		cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 0);
		trvMapKills.AppendColumn (column);
		
		column = new Gtk.TreeViewColumn ();
		column.Title = "Ship Kills";
		cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 1);
		trvMapKills.AppendColumn (column);
		
		column = new Gtk.TreeViewColumn ();
		column.Title = "Faction Kills";
		cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 2);
		trvMapKills.AppendColumn (column);
		
		column = new Gtk.TreeViewColumn ();
		column.Title = "Pod Kills";
		cell = new Gtk.CellRendererText ();
		column.PackStart(cell, true);
		column.AddAttribute(cell, "text", 3);
		trvMapKills.AppendColumn (column);
		
		trvMapKills.Model = mapKills;
	}

    void HandleCharListOnRequestUpdate(ApiResult<ApiKeyInfo> result)
	{
		if (result != null && result.Error == null)
        {
			accCharacters.Clear();
			
            foreach (CharacterListItem character in result.Result.Characters)
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

	protected void UpdateCharacter (object sender, System.EventArgs e)
	{
		CreateCharacterRequests();
	}
}
