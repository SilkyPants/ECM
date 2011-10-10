using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ECM
{
    public static class Core
    {
        static Dictionary<string, Account> m_Accounts = new Dictionary<string, Account>();
        static Dictionary<long, Character> m_Characters = new Dictionary<long, Character>();

        #region Events
        public static event EventHandler OnCharacterChanged;
        public static event EventHandler OnUpdateGui;

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

        #endregion

        public static void UpdateOnHeartbeat()
        {
            foreach(Account account in m_Accounts.Values)
            {
                account.UpdateOnHeartbeat();
            }
        }

        public static void LoadAccounts()
        {
            List<Account> accounts = AccountDatabase.GetAllAccounts();

            foreach(Account acc in accounts)
            {
                m_Accounts.Add(acc.KeyID, acc);
            }
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

