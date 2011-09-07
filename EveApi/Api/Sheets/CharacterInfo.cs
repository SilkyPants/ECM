using System;
using EveApi.Attributes;
using System.Xml.Serialization;

namespace EveApi
{
	[KeyNeedsMask(ApiKeyMask.CharacterInfoPublic|ApiKeyMask.CharacterInfoPrivate)]
	[NeedsCharacterID]
	public class CharacterInfo
	{
		/*
		<?xml version="1.0" encoding="UTF-8"?>
		<eveapi version="2">
		  <currentTime>2011-09-07 06:33:51</currentTime>
		  <result>
		    <characterID>91145028</characterID>
		    <characterName>Nyai Maricadie</characterName>
		    <race>Gallente</race>
		    <bloodline>Gallente</bloodline>
		    <accountBalance>636101.10</accountBalance>
		    <skillPoints>787152</skillPoints>
		    <nextTrainingEnds>2011-09-06 10:58:04</nextTrainingEnds>
		    <shipName />
		    <shipTypeID>606</shipTypeID>
		    <shipTypeName>Velator</shipTypeName>
		    <corporationID>1000168</corporationID>
		    <corporation>Federal Navy Academy</corporation>
		    <corporationDate>2011-08-20 07:24:00</corporationDate>
		    <lastKnownLocation>Couster</lastKnownLocation>
		    <securityStatus>0.000999975000000042</securityStatus>
		    <rowset name="employmentHistory" key="recordID" columns="recordID,corporationID,startDate">
		      <row recordID="17681093" corporationID="1000168" startDate="2011-08-20 07:24:00" />
		    </rowset>
		  </result>
		  <cachedUntil>2011-09-07 07:30:51</cachedUntil>
		</eveapi> 
		 */
        [XmlIgnore]
        public static string ApiUri { get { return "/eve/CharacterInfo.xml.aspx"; } }
		
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
		public decimal	AccountBalance
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

