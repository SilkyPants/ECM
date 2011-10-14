using System;
using EveApi;
using EveApi.Api.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace ECM
{
    public class Character : DatabaseBase, ICharacterInfo, ICharacterSheet
    {
        CharacterApiRequest<CharacterSheet> m_charSheetRequest;
        CharacterApiRequest<CharacterInfo> m_charInfoRequest;
        bool m_AutoUpdate = false;

        public event EventHandler CharacterUpdated;

        #region Properties
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

        public string Background
        {
            get { return string.Format("{0} - {1} - {2}", Race, Bloodline, Ancestry); }
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

        public double AccountBalance
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

        public bool IsUpdating
        {
            get
            {
                return m_charInfoRequest.IsUpdating || m_charSheetRequest.IsUpdating;
            }
        }

        public MemoryStream Portrait
        {
            get;
            set;
        }
        #endregion

        public Character (Account account, long characterID, string name)
        {
            Account = account;
            ID = characterID;
            Name = name;

            Attributes = new CharacterAttributes();
            Implants = new ImplantSet();
            Skills = new List<CharacterSkills>();
            Certificates = new List<CharacterCertificates>();

            m_charSheetRequest = new CharacterApiRequest<CharacterSheet>(characterID, Account.KeyID, Account.VCode);
            m_charSheetRequest.OnRequestUpdate += ApiRequestUpdate;
            //m_charSheetRequest.Enabled = Account.KeyAccess.HasFlag(ApiKeyMask.CharacterSheet);

            m_charInfoRequest = new CharacterApiRequest<CharacterInfo>(characterID, Account.KeyID, Account.VCode);
            m_charInfoRequest.OnRequestUpdate += ApiRequestUpdate;
            //m_charInfoRequest.Enabled = Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPublic) || Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPrivate);
        }

        void ApiRequestUpdate(IApiResult result)
        {
            if (result != null && result.Error == null)
            {
                if (result is ApiResult<CharacterSheet>)
                {
                    ApiResult<CharacterSheet> charSheet = result as ApiResult<CharacterSheet>;
                    UpdateCharacter(charSheet.Result);
                }
                else if (result is ApiResult<CharacterInfo>)
                {
                    ApiResult<CharacterInfo> charInfo = result as ApiResult<CharacterInfo>;
                    UpdateCharacter(charInfo.Result);
                }

                if (!IsUpdating && CharacterUpdated != null)
                {
                    CharacterUpdated(this, null);
                }
            }
        }

        public void DoInitialUpdate()
        {
            // Do a heartbeat
            UpdateOnHeartbeat();

            // Get the characters Portrait
            UpdateCharacterPortrait();
        }

        public void UpdateCharacterPortrait ()
        {
            Portrait = EveApi.ImageApi.GetCharacterPortrait(ID, ImageApi.ImageRequestSize.Size200x200);
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

        #region implemented abstract members of ECM.DatabaseBase
        protected override void WriteToDatabase ()
        {
            AccountDatabase.AddCharacter(this);
        }
        #endregion
    }
}

