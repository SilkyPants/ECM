using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ECM.API.EVE;

namespace ECM
{
    public static class Core
    {
        static Dictionary<string, Account> m_Accounts = new Dictionary<string, Account>();
        static Dictionary<long, Character> m_Characters = new Dictionary<long, Character>();
        static List<IApiRequest> m_ApiRequests = new List<IApiRequest>();
        static Character m_CurrentCharacter = null;
        static long m_FirstCharID = -1;
        static bool m_Loaded = false;

        static ApiRequest<ServerStatus> m_TQServerStatus;

        #region Events
        public delegate void ServerStatusUpdated(ServerStatus status);

        public static event EventHandler OnCharacterChanged;
        public static event EventHandler OnUpdateGui;
        public static event ServerStatusUpdated OnTQServerUpdate;

        public static void CharacterChanged()
        {
            if(OnCharacterChanged != null && m_Loaded)
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

        public static bool IsNetworkActivity
        {
            get
            {
                foreach (IApiRequest request in m_ApiRequests)
                {
                    if (request.IsUpdating)
                        return true;
                }

                return API.ImageApi.IsRetrieving;
            }
        }

        #region Resources
        private static Stream GetResource(string resourceName)
        {
            Assembly ass = Assembly.GetExecutingAssembly();

            if (ass != null)
            {
                return ass.GetManifestResourceStream(resourceName);
            }

            return null;
        }

        public static Stream MarketGroupPNG
        {
            get { return GetResource("ECM.Core.Icons.MarketGroupPNG"); }
        }

        public static Stream ItemUnknownPNG
        {
            get { return GetResource("ECM.Core.Icons.ItemUnknownPNG"); }
        }

        public static Stream Skillbook22PNG
        {
            get { return GetResource("ECM.Core.Icons.Skillbook22PNG"); }
        }

        public static Stream SkillbookPNG
        {
            get { return GetResource("ECM.Core.Icons.SkillbookPNG"); }
        }

        public static Stream Info16PNG
        {
            get { return GetResource("ECM.Core.Icons.Info16PNG"); }
        }

        public static Stream LoadingSpinnerGIF
        {
            get { return GetResource("ECM.Core.Icons.LoadingSpinnerGIF"); }
        }

        public static Stream LoadingSpinnerGIF16
        {
            get { return GetResource("ECM.Core.Icons.Loading16GIF"); }
        }

        public static Stream NoPortraitJPG
        {
            get { return GetResource("ECM.Core.NoPortraitJPG"); }
        }

        public static Stream CertGrade0PNG
        {
            get { return GetResource("ECM.Core.Icons.CertGrade0PNG"); }
        }

        public static Stream CertGrade1PNG
        {
            get { return GetResource("ECM.Core.Icons.CertGrade1PNG"); }
        }

        public static Stream CertGrade2PNG
        {
            get { return GetResource("ECM.Core.Icons.CertGrade2PNG"); }
        }

        public static Stream CertGrade3PNG
        {
            get { return GetResource("ECM.Core.Icons.CertGrade3PNG"); }
        }

        public static Stream CertGrade4PNG
        {
            get { return GetResource("ECM.Core.Icons.CertGrade4PNG"); }
        }

        public static Stream CertGrade5PNG
        {
            get { return GetResource("ECM.Core.Icons.CertGrade5PNG"); }
        }

        public static Stream ClaimablePNG
        {
            get { return GetResource("ECM.Core.Icons.ClaimablePNG"); }
        }

        public static Stream TrainedPNG
        {
            get { return GetResource("ECM.Core.Icons.TrainedPNG"); }
        }

        public static Stream TrainablePNG
        {
            get { return GetResource("ECM.Core.Icons.TrainablePNG"); }
        }

        public static Stream UntrainablePNG
        {
            get { return GetResource("ECM.Core.Icons.UntrainablePNG"); }
        }

        public static Stream Up16PNG
        {
            get { return GetResource("ECM.Core.Icons.Up16PNG"); }
        }

        public static Stream Down16PNG
        {
            get { return GetResource("ECM.Core.Icons.Down16PNG"); }
        }

        public static Stream Left16PNG
        {
            get { return GetResource("ECM.Core.Icons.Left16PNG"); }
        }

        public static Stream Right16PNG
        {
            get { return GetResource("ECM.Core.Icons.Right16PNG"); }
        }

        public static Stream Close16PNG
        {
            get { return GetResource("ECM.Core.Icons.Close16PNG"); }
        }

        public static string RowsetXSLT
        {
            get 
            {
                Stream s = GetResource("ECM.Core.RowsetXSLT");
                byte[] buffer = new byte[s.Length];

                s.Read(buffer, 0, buffer.Length);

                return System.Text.Encoding.UTF8.GetString(buffer); 
            }
        }

        #endregion

        public static void Init()
        {
            m_TQServerStatus = new ApiRequest<ServerStatus>();
            m_TQServerStatus.OnRequestUpdate += TQServerStatusUpdate;
            m_TQServerStatus.Enabled = true;

            m_ApiRequests.Add(m_TQServerStatus);

            LoadAccounts();

            m_Loaded = true;
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
            if (!m_Loaded) return;

            foreach (IApiRequest request in m_ApiRequests)
            {
                request.UpdateOnSecTick();
            }
        }

        public static void SaveAccounts ()
        {
            foreach(Account acc in m_Accounts.Values)
            {
                acc.SaveToDatabase();
            }
        }

        private static void LoadAccounts()
        {
            Console.WriteLine("Loading Accounts");
            List<Account> accounts = AccountDatabase.GetAllAccounts();

            foreach(Account acc in accounts)
            {
                AddAccount(acc);
            }
        }

        public static void AddAccount(Account toAdd)
        {
            m_Accounts.Add(toAdd.KeyID, toAdd);

            m_ApiRequests.Add(toAdd.AccountKeyInfo);
            m_ApiRequests.Add(toAdd.AccountStatus);

            foreach (Character character in toAdd.Characters)
            {
                AddCharacter(character);
            }

            // HACK: to get first character - need to find better way (and store settings!)
            if(CurrentCharacter == null && m_Characters.Count > 0)
                CurrentCharacter = m_Characters[m_FirstCharID];

            UpdateGui();
        }

        private static void AddCharacter(Character charToAdd)
        {
            if (m_FirstCharID == -1)
                m_FirstCharID = charToAdd.ID;

            m_Characters.Add(charToAdd.ID, charToAdd);

            m_ApiRequests.Add(charToAdd.CharInfoRequest);
            m_ApiRequests.Add(charToAdd.CharSheetRequest);
            m_ApiRequests.Add(charToAdd.SkillQueueRequest);
            m_ApiRequests.Add(charToAdd.AssetListRequest);
            m_ApiRequests.Add(charToAdd.StandingsRequest);

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

