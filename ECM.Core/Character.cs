using System;
using EveApi;
using EveApi.Api.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ECM
{
    public class Character : DatabaseBase, ICharacterInfo, ICharacterSheet
    {
        CharacterApiRequest<CharacterSheet> m_charSheetRequest;
        CharacterApiRequest<CharacterInfo> m_charInfoRequest;
        CharacterApiRequest<SkillQueue> m_skillQueueRequest;
        CharacterApiRequest<AssetList> m_AssetListRequest;
        CharacterApiRequest<CharacterStandings> m_StandingsRequest;

        bool m_AutoUpdate = false;

        public event EventHandler CharacterUpdated;

        #region Properties
        internal CharacterApiRequest<CharacterStandings> StandingsRequest
        {
            get { return m_StandingsRequest; }
        }
        internal CharacterApiRequest<AssetList> AssetListRequest
        {
            get { return m_AssetListRequest; }
        }

        internal CharacterApiRequest<CharacterSheet> CharSheetRequest
        {
            get { return m_charSheetRequest; }
        }

        internal CharacterApiRequest<CharacterInfo> CharInfoRequest
        {
            get { return m_charInfoRequest; }
        }

        internal CharacterApiRequest<SkillQueue> SkillQueueRequest
        {
            get { return m_skillQueueRequest; }
        }

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
                if(SetProperty<bool>("AutoUpdate", ref m_AutoUpdate, value))
                {
                    m_charInfoRequest.Enabled = value && Account.KeyAccess.HasFlag(ApiKeyMask.CharacterSheet);
                    m_charSheetRequest.Enabled = value && (Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPublic) || Account.KeyAccess.HasFlag(ApiKeyMask.CharacterInfoPrivate));

                    m_skillQueueRequest.Enabled = value && Account.KeyAccess.HasFlag(ApiKeyMask.SkillQueue);

                    m_AssetListRequest.Enabled = value && Account.KeyAccess.HasFlag(ApiKeyMask.AssetList);

                    m_StandingsRequest.Enabled = value && Account.KeyAccess.HasFlag(ApiKeyMask.Standings);
                }
            }
        }

        public string Background
        {
            get { return string.Format("{0} - {1} - {2}", Race, Bloodline, Ancestry); }
        }

        long m_ID = 0;
        public long ID
        {
            get { return m_ID; }
            set { SetProperty<long>("ID", ref m_ID, value); }
        }

        string m_Name = string.Empty;
        public string Name
        {
            get { return m_Name; }
            set { SetProperty<string>("Name", ref m_Name, value); }
        }

        string m_Race = string.Empty;
        public string Race
        {
            get { return m_Race; }
            set { SetProperty<string>("Race", ref m_Race, value); }
        }

        string m_Bloodline = string.Empty;
        public string Bloodline
        {
            get { return m_Race; }
            set { SetProperty<string>("Bloodline", ref m_Bloodline, value); }
        }

        double m_AccountBalanace = 0;
        public double AccountBalance
        {
            get { return m_AccountBalanace; }
            set { SetProperty<double>("AccountBalance", ref m_AccountBalanace, value); }
        }

        int m_SkillPoints = 0;
        public int SkillPoints
        {
            get { return m_SkillPoints; }
            set { SetProperty<int>("SkillPoints", ref m_SkillPoints, value); }
        }

        string m_ShipName = string.Empty;
        public string ShipName
        {
            get { return m_ShipName; }
            set { SetProperty<string>("ShipName", ref m_ShipName, value); }
        }

        long m_ShipTypeID = 0;
        public long ShipTypeID
        {
            get { return m_ShipTypeID; }
            set { SetProperty<long>("ShipTypeID", ref m_ShipTypeID, value); }
        }

        string m_ShipTypeName = string.Empty;
        public string ShipTypeName
        {
            get { return m_ShipTypeName; }
            set { SetProperty<string>("ShipTypeName", ref m_ShipTypeName, value); }
        }

        long m_CorporationID = 0;
        public long CorporationID
        {
            get { return m_CorporationID; }
            set { SetProperty<long>("CorporationID", ref m_CorporationID, value); }
        }

        string m_Corporation = string.Empty;
        public string Corporation
        {
            get { return m_Corporation; }
            set { SetProperty<string>("Corporation", ref m_Corporation, value); }
        }

        DateTime m_CorporationDate = DateTime.MinValue;
        public DateTime CorporationDate
        {
            get { return m_CorporationDate; }
            set { SetProperty<DateTime>("CorporationDate", ref m_CorporationDate, value); }
        }

        long m_AllianceID = 0;
        public long AllianceID
        {
            get { return m_AllianceID; }
            set { SetProperty<long>("AllianceID", ref m_AllianceID, value); }
        }

        string m_Alliance = string.Empty;
        public string Alliance
        {
            get { return m_Alliance; }
            set { SetProperty<string>("Alliance", ref m_Alliance, value); }
        }

        DateTime m_AllianceDate = DateTime.MinValue;
        public DateTime AllianceDate
        {
            get { return m_AllianceDate; }
            set { SetProperty<DateTime>("AllianceDate", ref m_AllianceDate, value); }
        }

        string m_LastKnownLocation = string.Empty;
        public string LastKnownLocation
        {
            get { return m_LastKnownLocation; }
            set { SetProperty<string>("LastKnownLocation", ref m_LastKnownLocation, value); }
        }

        double m_SecurityStatus = 0;
        public double SecurityStatus
        {
            get { return m_SecurityStatus; }
            set { SetProperty<double>("SecurityStatus", ref m_SecurityStatus, value); }
        }

        DateTime m_Birthday = DateTime.MinValue;
        public DateTime Birthday
        {
            get { return m_Birthday; }
            set { SetProperty<DateTime>("Birthday", ref m_Birthday, value); }
        }

        string m_Ancestry = string.Empty;
        public string Ancestry
        {
            get { return m_Ancestry; }
            set { SetProperty<string>("Ancestry", ref m_Ancestry, value); }
        }

        string m_Gender = string.Empty;
        public string Gender
        {
            get { return m_Gender; }
            set { SetProperty<string>("Gender", ref m_Gender, value); }
        }

        string m_CloneName = string.Empty;
        public string CloneName
        {
            get { return m_CloneName; }
            set { SetProperty<string>("CloneName", ref m_CloneName, value); }
        }

        long m_CloneSkillPoints = 0;
        public long CloneSkillPoints
        {
            get { return m_CloneSkillPoints; }
            set { SetProperty<long>("CloneSkillPoints", ref m_CloneSkillPoints, value); }
        }

        ImplantSet m_Implants = new ImplantSet();
        public ImplantSet Implants
        {
            get { return m_Implants; }
            set { SetProperty<ImplantSet>("Implants", ref m_Implants, value); }
        }

        CharacterAttributes m_Attributes = new CharacterAttributes();
        public CharacterAttributes Attributes
        {
            get { return m_Attributes; }
            set { SetProperty<CharacterAttributes>("Attributes", ref m_Attributes, value); }
        }

        Dictionary<long, CharacterSkills> m_Skills = new Dictionary<long, CharacterSkills>();
        public Dictionary<long, CharacterSkills> Skills
        {
            get { return m_Skills; }
            set { SetProperty<Dictionary<long, CharacterSkills>>("Skills", ref m_Skills, value); }
        }

        SkillQueue m_SkillQueue = new SkillQueue();
        public SkillQueue SkillQueue
        {
            get { return m_SkillQueue; }
            set { SetProperty<SkillQueue>("SkillQueue", ref m_SkillQueue, value); }
        }

        Dictionary<long, List<AssetListInfo>> m_Assets = new Dictionary<long, List<AssetListInfo>>();
        public Dictionary<long, List<AssetListInfo>> Assets
        {
            get { return m_Assets; }
            set { SetProperty<Dictionary<long, List<AssetListInfo>>>("Assets", ref m_Assets, value); }
        }

        List<CharacterCertificates> m_Certificates = new List<CharacterCertificates>();
        public List<CharacterCertificates> Certificates
        {
            get { return m_Certificates; }
            set { SetProperty<List<CharacterCertificates>>("Certificates", ref m_Certificates, value); }
        }

        public bool IsUpdating
        {
            get
            {
                return m_charInfoRequest.IsUpdating || m_charSheetRequest.IsUpdating;
            }
        }

        MemoryStream m_Portrait = null;
        public MemoryStream Portrait
        {
            get { return m_Portrait; }
            set { SetProperty<MemoryStream>("Portrait", ref m_Portrait, value); }
        }
        #endregion

        public Character (Account account, long characterID, string name)
        {
            Account = account;
            ID = characterID;
            Name = name;

            m_charSheetRequest = new CharacterApiRequest<CharacterSheet>(characterID, Account.KeyID, Account.VCode);
            m_charSheetRequest.OnRequestUpdate += ApiRequestUpdate;

            m_charInfoRequest = new CharacterApiRequest<CharacterInfo>(characterID, Account.KeyID, Account.VCode);
            m_charInfoRequest.OnRequestUpdate += ApiRequestUpdate;

            m_skillQueueRequest = new CharacterApiRequest<SkillQueue>(characterID, Account.KeyID, Account.VCode);
            m_skillQueueRequest.OnRequestUpdate += ApiRequestUpdate;

            m_AssetListRequest = new CharacterApiRequest<AssetList>(characterID, Account.KeyID, Account.VCode);
            m_AssetListRequest.OnRequestUpdate += ApiRequestUpdate;

            m_StandingsRequest = new CharacterApiRequest<CharacterStandings>(characterID, Account.KeyID, Account.VCode);
            m_StandingsRequest.OnRequestUpdate += ApiRequestUpdate;
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
                else if (result is ApiResult<SkillQueue>)
                {
                    ApiResult<SkillQueue> queue = result as ApiResult<SkillQueue>;
                    SkillQueue = queue.Result;
                }
                else if (result is ApiResult<AssetList>)
                {
                    ApiResult<AssetList> assets = result as ApiResult<AssetList>;

                    // Create asset dictionary keyed on locationID
                    Assets.Clear();

                    foreach (EveApi.AssetListInfo info in assets.Result.Assets)
                    {
                        if (!Assets.ContainsKey(info.LocationID))
                            Assets.Add(info.LocationID, new List<AssetListInfo>());

                        Assets[info.LocationID].Add(info);
                    }
                }
                else if (result is ApiResult<CharacterStandings>)
                {
                    Console.WriteLine("Hello");
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
            Skills = characterSheet.Skills.ToDictionary(p => p.ID);

            // we want to store implants with their TypeIDs
            Implants = characterSheet.Implants;
        }

        public void UpdateOnHeartbeat()
        {
            m_charSheetRequest.UpdateOnSecTick();
            m_charInfoRequest.UpdateOnSecTick();
            m_skillQueueRequest.UpdateOnSecTick();
        }

        public double SkillpointsPerMinute(SkillAttributes primary, SkillAttributes secondary)
        {
            double pri = 0, sec = 0;

            switch (primary)
            {
                case SkillAttributes.Charisma:
                    pri = Attributes.Charisma + Implants.Charisma.Amount;
                    break;
                case SkillAttributes.Intelligence:
                    pri = Attributes.Intelligence + Implants.Intelligence.Amount;
                    break;
                case SkillAttributes.Memory:
                    pri = Attributes.Memory + Implants.Memory.Amount;
                    break;
                case SkillAttributes.Perception:
                    pri = Attributes.Perception + Implants.Perception.Amount;
                    break;
                case SkillAttributes.Willpower:
                    pri = Attributes.Willpower + Implants.Willpower.Amount;
                    break;
            }

            switch (secondary)
            {
                case SkillAttributes.Charisma:
                    sec = Attributes.Charisma + Implants.Charisma.Amount;
                    break;
                case SkillAttributes.Intelligence:
                    sec = Attributes.Intelligence + Implants.Intelligence.Amount;
                    break;
                case SkillAttributes.Memory:
                    sec = Attributes.Memory + Implants.Memory.Amount;
                    break;
                case SkillAttributes.Perception:
                    sec = Attributes.Perception + Implants.Perception.Amount;
                    break;
                case SkillAttributes.Willpower:
                    sec = Attributes.Willpower + Implants.Willpower.Amount;
                    break;
            }

            return pri + (sec / 2);
        }

        #region implemented abstract members of ECM.DatabaseBase
        protected override void WriteToDatabase ()
        {
            Console.WriteLine(string.Format("Saving {0} to the database.", Name));
            AccountDatabase.AddCharacter(this);
        }
        #endregion

        public EveItemUseability GetItemUseability(ECM.EveItem item)
        {
            EveItemUseability useability = EveItemUseability.Useable;

            for (int i = 0; i < 6; i++)
            {
                RequiredSkill req = item.RequiredSkills[i];

                if (req.IsValid)
                {
                    if (!Skills.ContainsKey(req.SkillID))
                    {
                        return EveItemUseability.Untrainable;
                    }
                    else if(Skills[req.SkillID].Level < req.SkillLevel)
                    {
                        useability = EveItemUseability.Trainable;
                    }
                }
            }

            return useability;
        }
    }
}

