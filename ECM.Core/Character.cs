using System;using EveApi;namespace ECM.Core{    public class Character    {        CharacterApiRequest<CharacterSheet> m_charSheetRequest;        public string AccountKeyID        {            get;            private set;        }        public Account Account        {            get            {                return Data.RetrieveAccount(AccountKeyID);            }        }

        public long ID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }        public CharacterSheet Sheet        {            get;            private set;        }        public Character (string accountID, long characterID, string name)        {
            AccountKeyID = accountID;
            ID = characterID;
            Name = name;            Sheet = new CharacterSheet();            m_charSheetRequest = new CharacterApiRequest<CharacterSheet>(characterID, Account.KeyID, Account.VCode);            m_charSheetRequest.OnRequestUpdate += SheetRequestUpdated;        }        void SheetRequestUpdated (ApiResult<CharacterSheet> result)        {            if(result != null && result.Error == null)            {                Sheet = result.Result;            }        }        public void UpdateOnHeartbeat()        {            m_charSheetRequest.UpdateOnSecTick();        }    }}