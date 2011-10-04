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

            vbxKeyInfo.Sensitive = false;
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

		protected void NeedKeyClick (object o, System.EventArgs args)
		{

		}

		protected void RetrieveApiInfo (object sender, System.EventArgs e)
		{
	        apiAccount = new ECM.Core.Account(txtApiKeyID.Text, txtVerificationCode.Buffer.Text);
            apiAccount.AccountUpdated += HandleApiAccountAccountUpdated;

            apiAccount.UpdateOnHeartbeat();
		}

        void HandleApiAccountAccountUpdated (ECM.Core.Account account, IApiResult result)
        {
            if (result != null && result.Error == null)
            {
                if(result is ApiResult<ApiKeyInfo>)
                {
                    vbxKeyInfo.Sensitive = true;
                    btnImport.Sensitive = true;

                    ApiResult<ApiKeyInfo> keyInfo = result as ApiResult<ApiKeyInfo>;
                    ApiKeyData keyData = keyInfo.Result.Key;

                    accCharacters.Clear();

                    apiAccount.UpdateOnHeartbeat();
    
                    foreach (CharacterListItem character in keyData.Characters)
                        accCharacters.AppendValues(true, character.Name, character.CharacterID);

                    // Show key access
                    imgAccBalance.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.AccountBalance);
                    imgAccountStatus.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.AccountStatus);
                    imgAssetList.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.AssetList);
                    imgCalEvents.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.CalendarEventAttendees) && keyData.AccessMask.HasFlag(ApiKeyMask.UpcomingCalendarEvents);
                    imgCharInfo.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.CharacterInfoPrivate) || keyData.AccessMask.HasFlag(ApiKeyMask.CharacterInfoPublic);
                    imgCharSheet.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.CharacterSheet);
                    imgContactNotifications.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.ContactNotifications);
                    imgContacts.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.ContactList);
                    imgContracts.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.Contracts);
                    imgFacWarStats.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.FacWarStats);
                    imgIndJobs.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.IndustryJobs);
                    imgKillLog.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.KillLog);
                    // TODO: What about mailing lists?
                    imgMail.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.MailBodies) && keyData.AccessMask.HasFlag(ApiKeyMask.MailMessages);
                    imgMarketOrders.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.MarketOrders);
                    imgMedals.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.Medals);
                    ImgNotifications.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.Notifications) && keyData.AccessMask.HasFlag(ApiKeyMask.NotificationTexts);
                    imgReseach.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.Research);
                    imgSkillQueue.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.SkillQueue);
                    imgSkillTraining.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.SkillInTraining);
                    imgStandings.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.Standings);
                    imgWallJournal.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.WalletJournal);
                    imgWallTransactions.Sensitive = keyData.AccessMask.HasFlag(ApiKeyMask.WalletTransactions);
                }
            }
        }

		protected void ImportKey (object sender, System.EventArgs e)
		{
            if(apiAccount != null)
            {
                TreeIter it = new TreeIter ();
                accCharacters.GetIterFirst (out it);
                int index = 0;
                while (accCharacters.IterIsValid (it))
                {
                    bool selected = (bool) accCharacters.GetValue (it, 0);

                    apiAccount.Characters[index].AutoUpdate = selected;
    
                    accCharacters.IterNext (ref it);
                }

                ECM.Core.Data.AddAccount(apiAccount);
    
                this.Destroy();
            }
		}
	}
}

