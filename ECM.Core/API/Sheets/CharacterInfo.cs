using System;
using System.Xml.Serialization;

namespace ECM.API.EVE
{
	//[KeyNeedsMask(ApiKeyMask.CharacterInfoPublic)]
	[NeedsCharacterID]
    [ApiUriAttribute("/eve/CharacterInfo.xml.aspx")]
	public class CharacterInfo : ICharacterInfo
	{	
		// The character's ID number.
		[XmlElement("characterID")]
		public long ID
		{
			get;
			set;
		}
		
		// The character's name.
		[XmlElement("characterName")] 
		public string Name
		{
			get;
			set;
		}
		
		// The character's race. 
		[XmlElement("race")]
		public string Race
		{
			get;
			set;
		}
		
		// The character's bloodline. 
		[XmlElement("bloodline")]
		public string Bloodline
		{
			get;
			set;
		}
		
		// Amount of ISK the character has. 
		[XmlElement("accountBalance")]
		public double	AccountBalance
		{
			get;
			set;
		}
		
		// How many skill points the characters has.
		[XmlElement("skillPoints")] 
		public int SkillPoints
		{
			get;
			set;
		}
		
		// The name of the ship the character is currently piloting.
		[XmlElement("shipName")] 
		public string ShipName
		{
			get;
			set;
		}
		
		// The type of ship the character is currently piloting. 
		[XmlElement("shipTypeID")]
		public long ShipTypeID
		{
			get;
			set;
		}
		
		// The name of the type of ship the character is currently piloting. 
		[XmlElement("shipTypeName")]
		public string ShipTypeName
		{
			get;
			set;
		}
		
		// The ID of the character's corporation. 
		[XmlElement("corporationID")]
		public long CorporationID
		{
			get;
			set;
		}
		
		// The name of the character's corporation 
		[XmlElement("corporation")]
		public string Corporation
		{
			get;
			set;
		}
		
		// The date the character joined the corporation. 
		[XmlElement("corporationDate")]
		public string CorporationDateXML
		{
            get { return CorporationDate.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                CorporationDate = value.TimeStringToDateTime();
            }
		}
		
		[XmlIgnore]
		public DateTime CorporationDate
		{
			get;
			set;
		}
		
		// The ID of the character's alliance. 
		[XmlElement("allianceID")]
		public long AllianceID
		{
			get;
			set;
		}
		
		// The name of the character's alliance. 
		[XmlElement("alliance")]
		public string Alliance
		{
			get;
			set;
		}
		
		// The date the character's corporation joined the alliance. 
		[XmlElement("allianceDate")]
		public string AllianceDateXML
		{
            get { return AllianceDate.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                AllianceDate = value.TimeStringToDateTime();
            }
		}
		
		[XmlIgnore]
		public DateTime AllianceDate
		{
			get;
			set;
		}
		
		// The name of the character's current location. 
		[XmlElement("lastKnownLocation")]
		public string LastKnownLocation
		{
			get;
			set;
		}
		
		// The standing the character has with CONCORD. 
		[XmlElement("securityStatus")]
		public double SecurityStatus
		{
			get;
			set;
		}
		
		public CharacterInfo ()
		{
			LastKnownLocation = "None";
			SkillPoints = -1;
			AccountBalance = -1;
			ShipTypeID = -1;
		}
	}
}

