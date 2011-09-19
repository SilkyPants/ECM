using System;using EveApi;
using EveApi.Api.Interfaces;
using System.Collections.Generic;namespace ECM.Core{
    public class Character : ICharacterInfo, ICharacterSheet
    {
        CharacterApiRequest<CharacterSheet> m_charSheetRequest;
        CharacterApiRequest<CharacterInfo> m_charInfoRequest;
        bool m_AutoUpdate = true;        public string AccountKeyID        {            get;            private set;        }        public Account Account        {            get            {                return Data.RetrieveAccount(AccountKeyID);            }        }

        public bool AutoUpdate
        {
            get { return m_AutoUpdate; }
            set
            {
                m_AutoUpdate = value;
                m_charInfoRequest.Enabled = value;
                m_charSheetRequest.Enabled = value;
            }
        }

        public long ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
        public string Race
        {
            get;
            set;
        }

        public string Bloodline
        {
            get;
            set;
        }

        public decimal AccountBalance
        {
            get;
            set;
        }

        public int SkillPoints
        {
            get;
            set;
        }

        public string ShipName
        {
            get;
            set;
        }

        public long ShipTypeID
        {
            get;
            set;
        }

        public string ShipTypeName
        {
            get;
            set;
        }

        public long CorporationID
        {
            get;
            set;
        }

        public string Corporation
        {
            get;
            set;
        }

        public DateTime CorporationDate
        {
            get;
            set;
        }

        public long AllianceID
        {
            get;
            set;
        }

        public string Alliance
        {
            get;
            set;
        }

        public DateTime AllianceDate
        {
            get;
            set;
        }

        public string LastKnownLocation
        {
            get;
            set;
        }

        public double SecurityStatus
        {
            get;
            set;
        }

        public DateTime Birthday
        {
            get;
            set;
        }

        public string Ancestry
        {
            get;
            set;
        }

        public string Gender
        {
            get;
            set;
        }

        public string CloneName
        {
            get;
            set;
        }

        public long CloneSkillPoints
        {
            get;
            set;
        }

        public ImplantSet Implants
        {
            get;
            set;
        }

        public CharacterAttributes Attributes
        {
            get;
            set;
        }

        public List<CharacterSkills> Skills
        {
            get;
            set;
        }

        public List<CharacterCertificates> Certificates
        {
            get;
            set;
        }        public Character (string accountID, long characterID, string name)        {
            AccountKeyID = accountID;
            ID = characterID;
            Name = name;
            AutoUpdate = true;

            m_charSheetRequest = new CharacterApiRequest<CharacterSheet>(characterID, Account.KeyID, Account.VCode);
            m_charSheetRequest.OnRequestUpdate += SheetRequestUpdated;
            m_charSheetRequest.Enabled = Account.KeyAccess.HasFlag(ApiKeyMask.CharacterSheet);

            m_charInfoRequest = new CharacterApiRequest<CharacterInfo>(characterID, Account.KeyID, Account.VCode);
            m_charInfoRequest.OnRequestUpdate += InfoRequestUpdated;
            m_charInfoRequest.Enabled = Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPublic) || Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPrivate);        }

        void SheetRequestUpdated(ApiResult<CharacterSheet> result)
        {
            if (result != null && result.Error == null)
            {
                UpdateCharacter(result.Result);
            }
        }

        void InfoRequestUpdated(ApiResult<CharacterInfo> result)
        {
            if (result != null && result.Error == null)
            {
                UpdateCharacter(result.Result);
            }
        }

        private void UpdateCharacter(CharacterInfo characterInfo)
        {
            AccountBalance = characterInfo.AccountBalance;
            Alliance = characterInfo.Alliance;
            AllianceDate = characterInfo.AllianceDate;
            AllianceID = characterInfo.AllianceID;
            Bloodline = characterInfo.Bloodline;
            Corporation = characterInfo.Corporation;
            CorporationDate = characterInfo.CorporationDate;
            CorporationID = characterInfo.CorporationID;
            ID = characterInfo.ID;
            LastKnownLocation = characterInfo.LastKnownLocation;
            Name = characterInfo.Name;
            Race = characterInfo.Race;
            SecurityStatus = characterInfo.SecurityStatus;
            ShipName = characterInfo.ShipName;
            ShipTypeID = characterInfo.ShipTypeID;
            ShipTypeName = characterInfo.ShipTypeName;
            SkillPoints = characterInfo.SkillPoints;
            
        }

        private void UpdateCharacter(CharacterSheet characterSheet)
        {
            AccountBalance = characterSheet.AccountBalance;
            Alliance = characterSheet.Alliance;
            AllianceID = characterSheet.AllianceID;
            Ancestry = characterSheet.Ancestry;
            Attributes = characterSheet.Attributes;
            Birthday = characterSheet.Birthday;
            Bloodline = characterSheet.Bloodline;
            Certificates = characterSheet.Certificates;
            CloneName = characterSheet.CloneName;
            CloneSkillPoints = characterSheet.CloneSkillPoints;
            CorporationID = characterSheet.CorporationID;
            Gender = characterSheet.Gender;
            ID = characterSheet.ID;
            Implants = characterSheet.Implants;
            Name = characterSheet.Name;
            Race = characterSheet.Race;
            Skills = characterSheet.Skills;
        }        public void UpdateOnHeartbeat()        {            m_charSheetRequest.UpdateOnSecTick();        }
    }}