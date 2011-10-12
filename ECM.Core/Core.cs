using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EveApi;

namespace ECM
{
    public static class Core
    {
        static Dictionary<string, Account> m_Accounts = new Dictionary<string, Account>();
        static Dictionary<long, Character> m_Characters = new Dictionary<long, Character>();
        static Character m_CurrentCharacter = null;

        static ApiRequest<ServerStatus> m_TQServerStatus;

        #region Events
        public delegate void ServerStatusUpdated(ServerStatus status);

        public static event EventHandler OnCharacterChanged;
        public static event EventHandler OnUpdateGui;
        public static event ServerStatusUpdated OnTQServerUpdate;

        public static void CharacterChanged()
        {
            if(OnCharacterChanged != null)
                OnCharacterChanged(null, null);
        }

        public static void UpdateGui()
        {
            if (OnUpdateGui != null)
                OnUpdateGui(null, null);
        }
        
        #endregion

        public static Character CurrentCharacter
        {
            get { return m_CurrentCharacter; }
            set
            {
                m_CurrentCharacter = value;

                CharacterChanged();
            }
        }

        public static Dictionary<long, Character> Characters
        {
            get { return m_Characters; }
        }

        public static Dictionary<string, Account> Accounts
        {
            get { return m_Accounts; }
        }

        #region Resources
        public static Stream MarketGroupPNG
        {
            get
            {
                Assembly ass = Assembly.GetExecutingAssembly();

                if (ass != null)
                {
                    return ass.GetManifestResourceStream("ECM.Core.Icons.MarketGroupPNG");
                }

                return null;
            }
        }

        public static Stream ItemUnknownPNG
        {
            get
            {
                Assembly ass = Assembly.GetExecutingAssembly();

                if (ass != null)
                {
                    return ass.GetManifestResourceStream("ECM.Core.Icons.ItemUnknownPNG");
                }

                return null;
            }
        }

        public static Stream Skillbook22PNG
        {
            get
            {
                Assembly ass = Assembly.GetExecutingAssembly();

                if (ass != null)
                {
                    return ass.GetManifestResourceStream("ECM.Core.Icons.Skillbook22PNG");
                }

                return null;
            }
        }

        public static Stream Info16PNG
        {
            get
            {
                Assembly ass = Assembly.GetExecutingAssembly();

                if (ass != null)
                {
                    return ass.GetManifestResourceStream("ECM.Core.Icons.Info16PNG");
                }

                return null;
            }
        }

        public static Stream LoadingSpinnerGIF
        {
            get
            {
                Assembly ass = Assembly.GetExecutingAssembly();

                if (ass != null)
                {
                    return ass.GetManifestResourceStream("ECM.Core.Icons.LoadingSpinnerGIF");
                }

                return null;
            }
        }

        public static Stream LoadingSpinnerGIF16
        {
            get
            {
                Assembly ass = Assembly.GetExecutingAssembly();

                if (ass != null)
                {
                    return ass.GetManifestResourceStream("ECM.Core.Icons.Loading16GIF");
                }

                return null;
            }
        }

        #endregion

        public static void Init()
        {
            m_TQServerStatus = new ApiRequest<ServerStatus>();
            m_TQServerStatus.OnRequestUpdate += TQServerStatusUpdate;
            m_TQServerStatus.Enabled = true;

            LoadAccounts();
        }

        static void TQServerStatusUpdate (ApiResult<ServerStatus> result)
        {
            if (result != null && result.Error == null)
            {
                ServerStatus status = result.Result;

                if(OnTQServerUpdate != null)
                    OnTQServerUpdate(status);
            }
        }

        public static void UpdateOnHeartbeat()
        {
            m_TQServerStatus.UpdateOnSecTick();

            foreach(Account account in m_Accounts.Values)
            {
                account.UpdateOnHeartbeat();
            }
        }

        private static void LoadAccounts()
        {
            List<Account> accounts = AccountDatabase.GetAllAccounts();

            foreach(Account acc in accounts)
            {
                AddAccount(acc);
            }

            // HACK: to get first character - need to find better way (and store settings!)
            CurrentCharacter = new List<Character>(m_Characters.Values)[0];
        }

        public static void AddAccount(Account toAdd)
        {
            m_Accounts.Add(toAdd.KeyID, toAdd);

            foreach (Character character in toAdd.Characters)
            {
                AddCharacter(character);
            }

            AccountDatabase.AddAccount(toAdd);
        }

        private static void AddCharacter(Character charToAdd)
        {
            m_Characters.Add(charToAdd.ID, charToAdd);
            AccountDatabase.AddCharacter(charToAdd);

            charToAdd.CharacterUpdated += delegate
            {
                if(charToAdd == CurrentCharacter)
                    UpdateGui();
            };
        }

        public static void RemoveAccount(Account toRemove)
        {
            m_Accounts.Remove(toRemove.KeyID);
            AccountDatabase.RemoveAccount(toRemove);
        }

        public static Account RetrieveAccount(string keyID)
        {
            return m_Accounts[keyID];
        }
    }
}

