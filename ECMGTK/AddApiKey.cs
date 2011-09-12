using System;
using EveApi;
using Gtk;

namespace ECMGTK
{
	public partial class AddApiKey : Gtk.Dialog
	{
		ListStore accCharacters = new ListStore(typeof(bool), typeof(string));
		
		public AddApiKey ()
		{
			this.Build ();
			
			SetupTreeview();
		}

		public void SetupTreeview ()
		{
			trvCharacters.Model = accCharacters;
			
			CellRendererToggle crt = new CellRendererToggle();
			crt.Activatable = true;
			crt.Toggled += ImportCharacterChanged;
			trvCharacters.AppendColumn ("Import", crt, "active", 0);
			
			trvCharacters.AppendColumn("Name", new CellRendererText(), "text", 1);
		}

		void ImportCharacterChanged (object o, ToggledArgs args)
		{
			TreeIter iter;

			if (accCharacters.GetIter (out iter, new TreePath(args.Path))) 
			{
				bool old = (bool) accCharacters.GetValue(iter,0);
				accCharacters.SetValue(iter,0,!old);
			}
		}

		protected void NeedKeyClick (object o, Gtk.ButtonPressEventArgs args)
		{
			throw new System.NotImplementedException ();
		}

		protected void RetrieveApiInfo (object sender, System.EventArgs e)
		{
	        AuthorisedApiRequest<ApiKeyInfo> apiKeyInfo = new AuthorisedApiRequest<ApiKeyInfo>(txtApiKeyID.Text, txtVerificationCode.Text);
			apiKeyInfo.OnRequestUpdate += HandleApiKeyInfoOnRequestUpdate;
			apiKeyInfo.Enabled = true;
			apiKeyInfo.UpdateOnSecTick();
		}

		void HandleApiKeyInfoOnRequestUpdate (ApiResult<ApiKeyInfo> result)
		{
			if (result != null && result.Error == null)
	        {
				accCharacters.Clear();
				
	            foreach (CharacterListItem character in result.Result.Key.Characters)
					accCharacters.AppendValues(true, character.Name);
			}
		}

		protected void ImportKey (object sender, System.EventArgs e)
		{
		}
	}
}

