using System;using EveApi;namespace ECM.Core{    public class Character    {        CharacterApiRequest<CharacterSheet> m_charSheetRequest;        public Account Account        {            get;            private set;        }

        public long ID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }        public CharacterSheet Sheet        {            get;            private set;        }        public Character (Account characterAccount, long characterID, string name)        {
            Account = characterAccount;
            ID = characterID;
            Name = name;            m_charSheetRequest = new CharacterApiRequest<CharacterSheet>(characterID, Account.KeyID, Account.VCode);            m_charSheetRequest.OnRequestUpdate += SheetRequestUpdated;        }        void SheetRequestUpdated (ApiResult<CharacterSheet> result)        {            if(result != null && result.Error == null)            {                Sheet = result.Result;            }        }        public void UpdateOnHeartbeat()        {            m_charSheetRequest.UpdateOnSecTick();        }    }}