using System;
using System.Collections.Generic;

namespace ECM.Core
{
    public static class Data
    {
        static Dictionary<string, Account> m_Accounts = new Dictionary<string, Account>();
        static Dictionary<long, Character> m_Characters = new Dictionary<long, Character>();

        #region Events
        public static event EventHandler CharacterChanged;

        public static void OnCharacterChanged()
        {
            if(CharacterChanged != null)
                CharacterChanged(null, null);
        }
        
        #endregion
        public static Dictionary<long, Character> Characters
        {
            get { return m_Characters; }
        }

        public static void UpdateOnHeartbeat()
        {
            foreach(Account account in m_Accounts.Values)
            {
                account.UpdateOnHeartbeat();
            }

            foreach(Character character in m_Characters.Values)
            {
                character.UpdateOnHeartbeat();
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
            AccountDatabase.AddAccount(toAdd);
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

