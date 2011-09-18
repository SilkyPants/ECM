using System;
using EveApi;

namespace ECM.Core
{
    public class Account
    {
        AuthorisedApiRequest<ApiKeyInfo> m_accountKeyInfo;
        AuthorisedApiRequest<AccountStatus> m_accountStatus;
        ApiKeyMask m_KeyAccess;

        public ApiKeyMask KeyAccess
        {
            get { return m_KeyAccess; }
            private set
            {
                m_KeyAccess = value;
                m_accountStatus.Enabled = false;//m_KeyAccess.HasFlag(ApiKeyMask.AccountStatus);
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

        private Account(string keyID, string vCode)
        {
            KeyID = keyID;
            VCode = vCode;

            m_accountKeyInfo = new AuthorisedApiRequest<ApiKeyInfo>(keyID, vCode);
            m_accountKeyInfo.OnRequestUpdate += AccountKeyInfoUpdate;
            m_accountKeyInfo.Enabled = false;

            m_accountStatus = new AuthorisedApiRequest<AccountStatus>(keyID, vCode);
            m_accountStatus.OnRequestUpdate += AccountStatusUpdate;            m_accountKeyInfo.Enabled = false;
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
            }
        }

        void AccountStatusUpdate (ApiResult<AccountStatus> result)
        {
            if(result != null && result.Error == null)
            {
                Status = result.Result;
            }
        }

        public void UpdateOnHeartbeat()
        {
            m_accountKeyInfo.UpdateOnSecTick();
            m_accountStatus.UpdateOnSecTick();
        }
    }
}

