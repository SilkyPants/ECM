using System;
using EveApi;
using Gtk;

namespace ECMGTK
{
	public partial class AddApiKey : Gtk.Dialog
	{
		ListStore accCharacters = new ListStore(typeof(bool), typeof(string), typeof(long));
        ECM.Core.Account apiAccount = null;
		
		public AddApiKey ()
		{
			this.Build ();
			
			SetupTreeview();
		}

		public void SetupTreeview ()
		{
			trvCharacters.Model = accCharacters;
			
			TreeViewColumn col = new TreeViewColumn();
			col.Title = "Character";
			
			CellRendererToggle crt = new CellRendererToggle();
			crt.Activatable = true;
			crt.Toggled += ImportCharacterChanged;
			col.PackStart(crt, false);
			col.AddAttribute(crt, "active", 0);
			
			CellRendererText txt = new CellRendererText();
			col.PackStart(txt, true);
			col.AddAttribute(txt, "text", 1);
			
			trvCharacters.AppendColumn(col);
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
	        apiAccount = new ECM.Core.Account(txtApiKeyID.Text, txtVerificationCode.Text);
            apiAccount.AccountUpdated += HandleApiAccountAccountUpdated;

            apiAccount.UpdateOnHeartbeat();
		}

        void HandleApiAccountAccountUpdated (ECM.Core.Account account, IApiResult result)
        {
            if (result != null && result.Error == null)
            {
                if(result is ApiResult<ApiKeyInfo>)
                {
                    ApiResult<ApiKeyInfo> keyInfo = result as ApiResult<ApiKeyInfo>;
                    ApiKeyData keyData = keyInfo.Result.Key;

                    accCharacters.Clear();
    
                    foreach (CharacterListItem character in keyData.Characters)
                        accCharacters.AppendValues(true, character.Name, character.CharacterID);
                }
            }
        }

		protected void ImportKey (object sender, System.EventArgs e)
		{
            if(apiAccount != null)
            {
                ECM.Core.Data.AddAccount(apiAccount);

                TreeIter it = new TreeIter ();
                accCharacters.GetIterFirst (out it);
                while (accCharacters.IterIsValid (it))
                {
                    bool selected = (bool) accCharacters.GetValue (it, 0);
                    string name = (string) accCharacters.GetValue (it, 1);
                    long id = (long) accCharacters.GetValue (it, 2);
    
                    if(selected)
                    {
                        ECM.Core.Character newChar = new ECM.Core.Character(apiAccount, id, name);
                        ECM.Core.Data.Characters.Add(id, newChar);
                        newChar.UpdateOnHeartbeat();
                    }
    
                    accCharacters.IterNext (ref it);
                }
    
                this.Destroy();
            }
		}
	}
}

