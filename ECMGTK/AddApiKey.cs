using System;
using EveApi;
using Gtk;
using System.ComponentModel;
using System.Threading;

namespace ECMGTK
{
	public partial class AddApiKey : Gtk.Dialog
	{
		ListStore accCharacters = new ListStore(typeof(bool), typeof(string), typeof(long));
        ECM.Account apiAccount = null;
		
		public AddApiKey ()
		{
			this.Build ();
			
			SetupTreeview();
            Deletable = false;
            vbxKeyInfo.Sensitive = false;
            ntbInfoAndLoading.CurrentPage = 0;
            imgSpinner.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF);
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
            ntbInfoAndLoading.CurrentPage = 1;
	        apiAccount = new ECM.Account(txtApiKeyID.Text, txtVerificationCode.Buffer.Text);
            apiAccount.AccountUpdated += HandleApiAccountAccountUpdated;

            apiAccount.UpdateOnHeartbeat();
		}

        void HandleApiAccountAccountUpdated (ECM.Account account, IApiResult result)
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

                    foreach (ECM.Character character in apiAccount.Characters)
                    {
                        accCharacters.AppendValues(true, character.Name, character.ID);
                    }

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

                    ntbInfoAndLoading.CurrentPage = 0;
                }
            }
        }

        int charactersUpdated = 0;
        void CharacterUpdated(object sender, EventArgs e)
        {
            charactersUpdated++;
        }

		protected void ImportKey (object sender, System.EventArgs e)
		{
            if(apiAccount != null)
            {
                ntbInfoAndLoading.CurrentPage = 1;

                BackgroundWorker fetchCharDetails = new BackgroundWorker();

                fetchCharDetails.DoWork += new DoWorkEventHandler(FetchCharacterDetails);
                fetchCharDetails.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CharacterDetailsFetched);

                foreach (ECM.Character character in apiAccount.Characters)
                {
                    character.CharacterUpdated += new EventHandler(CharacterUpdated);
                }

                charactersUpdated = 0;
                fetchCharDetails.RunWorkerAsync();                
            }
		}

        void FetchCharacterDetails(object sender, DoWorkEventArgs e)
        {
            TreeIter it = new TreeIter();
            accCharacters.GetIterFirst(out it);
            int index = 0;
            while (accCharacters.IterIsValid(it))
            {
                bool selected = (bool)accCharacters.GetValue(it, 0);

                apiAccount.Characters[index].AutoUpdate = selected;

                if(selected)
                    apiAccount.Characters[index].DoInitialUpdate();

                index++;

                accCharacters.IterNext(ref it);
            }

            bool allDone = false;
            while (!allDone)
            {
                Thread.Sleep(500);

                allDone = charactersUpdated == apiAccount.Characters.Count;
            }

            Console.WriteLine("Fetched Character Details");
        }

        void CharacterDetailsFetched(object sender, RunWorkerCompletedEventArgs e)
        {
            ECM.Core.AddAccount(apiAccount);
            ECM.Core.UpdateGui();
            this.Destroy();
        }

        protected void CancelClicked (object sender, System.EventArgs e)
        {
            this.Destroy();
        }
	}
}

