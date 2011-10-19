using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using EveApi.Attributes;
using EveApi.Api.Interfaces;

namespace EveApi
{
    [NeedsApiKey]
    [KeyNeedsMask(ApiKeyMask.CharacterSheet)]
	public class CharacterSheet : ICharacterSheet
	{
        [XmlIgnore]
        public static string ApiUri { get { return "/char/CharacterSheet.xml.aspx"; } }
		
	    /*<rowset name="corporationRoles" key="roleID" columns="roleID,roleName">
	      <row roleID="1" roleName="roleDirector" />
	    </rowset>
	    <rowset name="corporationRolesAtHQ" key="roleID" columns="roleID,roleName">
	      <row roleID="1" roleName="roleDirector" />
	    </rowset>
	    <rowset name="corporationRolesAtBase" key="roleID" columns="roleID,roleName">
	      <row roleID="1" roleName="roleDirector" />
	    </rowset>
	    <rowset name="corporationRolesAtOther" key="roleID" columns="roleID,roleName">
	      <row roleID="1" roleName="roleDirector" />
	    </rowset>
	    <rowset name="corporationTitles" key="titleID" columns="titleID,titleName">
	      <row titleID="1" titleName="Member" />
		</rowset>	
		*/
		
        [XmlElement("characterID")]
		public long ID
		{
			get;
			set;
		}
		
        [XmlElement("name")]
		public string Name
		{
			get;
			set;
		}
		
        [XmlElement("DoB")]
		public string BirthdayXML
		{
            get { return Birthday.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                Birthday = value.TimeStringToDateTime();
            }
		}
		
        [XmlIgnore]
		public DateTime Birthday
		{
			get;
			set;
		}
		
        [XmlElement("race")]
		public string Race
		{
			get;
			set;
		}
		
        [XmlElement("bloodLine")]
		public string Bloodline
		{
			get;
			set;
		}
		
        [XmlElement("ancestry")]
		public string Ancestry
		{
			get;
			set;
		}
		
        [XmlElement("gender")]
		public string Gender
		{
			get;
			set;
		}
		
        [XmlElement("corporationName")]
		public string Corporation
		{
			get;
			set;
		}
		
        [XmlElement("corporationID")]
        public long CorporationID
		{
			get;
			set;
		}
		
        [XmlElement("allianceName")]
		public string Alliance
		{
			get;
			set;
		}
		
        [XmlElement("allianceID")]
        public long AllianceID
		{
			get;
			set;
		}
		
        [XmlElement("cloneName")]
		public string CloneName
		{
			get;
			set;
		}
		
        [XmlElement("cloneSkillPoints")]
		public long CloneSkillPoints
		{
			get;
			set;
		}
		
        [XmlElement("balance")]
		public double AccountBalance
		{
			get;
			set;
		}
		
        [XmlElement("attributeEnhancers")]
		public ImplantSet Implants
		{
			get;
			set;
		}
		
        [XmlElement("attributes")]
		public CharacterAttributes Attributes
		{
			get;
			set;
		}
		
        [XmlArray("skills")]
        [XmlArrayItem("skill")]
		public List<CharacterSkills> Skills
		{
			get;
			set;
		}
		
        [XmlArray("certificates")]
        [XmlArrayItem("certificate")]
		public List<CharacterCertificates> Certificates
		{
			get;
			set;
		}
	}
	
	public class ImplantSet
	{
        public ImplantSet()
        {
            Intelligence = new Implant();
            Memory = new Implant();
            Willpower = new Implant();
            Perception = new Implant();
            Charisma = new Implant();
        }

        [XmlElement("intelligenceBonus")]
        public Implant Intelligence
        {
            get;
            set;
        }

        [XmlElement("memoryBonus")]
        public Implant Memory
        {
            get;
            set;
        }

        [XmlElement("willpowerBonus")]
        public Implant Willpower
        {
            get;
            set;
        }

        [XmlElement("perceptionBonus")]
        public Implant Perception
        {
            get;
            set;
        }

        [XmlElement("charismaBonus")]
        public Implant Charisma
        {
            get;
            set;
        }
	}
	
	public class Implant
	{
        public Implant()
        {
            Name = string.Empty;
            Amount = 0;
        }

        [XmlElement("augmentatorName")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("augmentatorValue")]
        public int Amount
        {
            get;
            set;
        }
	}
	
	public class CharacterAttributes
	{
        public CharacterAttributes()
        {
            Intelligence = Memory = Perception = Charisma = Willpower = 1;
        }

        [XmlElement("intelligence")]
        public int Intelligence
        {
            get;
            set;
        }

        [XmlElement("memory")]
        public int Memory
        {
            get;
            set;
        }

        [XmlElement("perception")]
        public int Perception
        {
            get;
            set;
        }

        [XmlElement("willpower")]
        public int Willpower
        {
            get;
            set;
        }

        [XmlElement("charisma")]
        public int Charisma
        {
            get;
            set;
        }
	}
	
	public class CharacterSkills
	{
        [XmlAttribute("typeID")]
        public long ID
        {
            get;
            set;
        }

        [XmlAttribute("level")]
        public int Level
        {
            get;
            set;
        }

        [XmlAttribute("skillpoints")]
        public int Skillpoints
        {
            get;
            set;
        }
	}
	
	public class CharacterCertificates
	{
        [XmlAttribute("certificateID")]
        public long ID
        {
            get;
            set;
        }
	}
}

