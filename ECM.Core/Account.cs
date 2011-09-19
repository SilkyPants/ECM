using System;
using EveApi;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ECM.Core
{
    public class Account
    {
        AuthorisedApiRequest<ApiKeyInfo> m_accountKeyInfo;
        AuthorisedApiRequest<AccountStatus> m_accountStatus;
        ApiKeyMask m_KeyAccess;
        List<Character> m_Characters = new List<Character>();

        public ApiKeyMask KeyAccess
        {
            get { return m_KeyAccess; }
            private set
            {
                m_KeyAccess = value;
                m_accountStatus.Enabled = m_KeyAccess.HasFlag(ApiKeyMask.AccountStatus);
            }
        }

        public DateTime Expires
        {
         get;
         set;
        }

        public AccountStatus Status
        {
            get;
            private set;
        }

        public string KeyID
        {
            get;
            private set;
        }

        public string VCode
        {
            get;
            private set;
        }

        public List<Character> Characters
        {
            get { return m_Characters; }
        }

        public delegate void AccountUpdatedHandler(Account account, IApiResult result);
        public event AccountUpdatedHandler AccountUpdated;

        private void OnAccountUpdated(IApiResult result)
        {
            if (AccountUpdated != null)
            {
                AccountUpdated(this, result);
            }
        }

        private Account(string keyID, string vCode)
        {
            KeyID = keyID;
            VCode = vCode;

            m_accountKeyInfo = new AuthorisedApiRequest<ApiKeyInfo>(keyID, vCode);
            m_accountKeyInfo.OnRequestUpdate += AccountKeyInfoUpdate;
            m_accountKeyInfo.Enabled = true;

            m_accountStatus = new AuthorisedApiRequest<AccountStatus>(keyID, vCode);
            m_accountStatus.OnRequestUpdate += AccountStatusUpdate;
        }

        public Account (string keyID, string vCode, ApiKeyMask access, DateTime expires)
            : this (keyID, vCode)
        {
            KeyAccess = access;
            Expires = expires;
        }

        void AccountKeyInfoUpdate (ApiResult<ApiKeyInfo> result)
        {
            if(result != null && result.Error == null)
            {
                KeyAccess = result.Result.Key.AccessMask;
                Expires = result.Result.Key.Expires;

                foreach (CharacterListItem character in result.Result.Key.Characters)
                {
                    m_Characters.Add(new Character(KeyID, character.CharacterID, character.Name));
                }

                OnAccountUpdated(result);
            }
        }

        void AccountStatusUpdate (ApiResult<AccountStatus> result)
        {
            if(result != null && result.Error == null)
            {
                Status = result.Result;

                OnAccountUpdated(result);
            }
        }

        public void UpdateOnHeartbeat()
        {
            m_accountKeyInfo.UpdateOnSecTick();
            m_accountStatus.UpdateOnSecTick();
        }
    }
}

