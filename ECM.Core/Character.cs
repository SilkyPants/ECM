using System;
using EveApi;
using EveApi.Api.Interfaces;
using System.Collections.Generic;

namespace ECM.Core
{
    public class Character : ICharacterInfo, ICharacterSheet
    {
        CharacterApiRequest<CharacterSheet> m_charSheetRequest;
        CharacterApiRequest<CharacterInfo> m_charInfoRequest;
        bool m_AutoUpdate = true;

        public Account Account
        {
            get;
            private set;
        }

        public bool AutoUpdate
        {
            get { return m_AutoUpdate && Account != null; }
            set
            {
                m_AutoUpdate = value;
                m_charInfoRequest.Enabled = value && Account.KeyAccess.HasFlag(ApiKeyMask.CharacterSheet);
                m_charSheetRequest.Enabled = value && (Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPublic) || Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPrivate));
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
        }

        public Character (Account account, long characterID, string name)
        {
            Account = account;
            ID = characterID;
            Name = name;

            m_charSheetRequest = new CharacterApiRequest<CharacterSheet>(characterID, Account.KeyID, Account.VCode);
            m_charSheetRequest.OnRequestUpdate += SheetRequestUpdated;
            m_charSheetRequest.Enabled = Account.KeyAccess.HasFlag(ApiKeyMask.CharacterSheet);

            m_charInfoRequest = new CharacterApiRequest<CharacterInfo>(characterID, Account.KeyID, Account.VCode);
            m_charInfoRequest.OnRequestUpdate += InfoRequestUpdated;
            m_charInfoRequest.Enabled = Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPublic) || Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPrivate);

            AutoUpdate = true;
        }

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
            Name = characterSheet.Name;
            Race = characterSheet.Race;
            Skills = characterSheet.Skills;

            // we want to store implants with their TypeIDs
            Implants = characterSheet.Implants;
        }

        public void UpdateOnHeartbeat()
        {
            m_charSheetRequest.UpdateOnSecTick();
            m_charInfoRequest.UpdateOnSecTick();
        }
    }
}

