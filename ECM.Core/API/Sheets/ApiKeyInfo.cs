using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECM.API.EVE
{
	[NeedsApiKey]
    [ApiUri("/account/APIKeyInfo.xml.aspx")]
	public class ApiKeyInfo
	{
        /*
         * <?xml version="1.0" encoding="UTF-8"?>
<eveapi version="2">
  <currentTime>2011-09-06 23:32:48</currentTime>
  <result>
    <key accessMask="134217727" type="Account" expires="">
      <rowset name="characters" key="characterID" columns="characterID,characterName,corporationID,corporationName">
        <row characterID="91145028" characterName="Nyai Maricadie" corporationID="1000168" corporationName="Federal Navy Academy" />
        <row characterID="417386255" characterName="Jittta" corporationID="1000172" corporationName="Pator Tech School" />
        <row characterID="1350842947" characterName="Oldin Kinrod" corporationID="1000169" corporationName="Center for Advanced Studies" />
      </rowset>
    </key>
  </result>
  <cachedUntil>2011-09-06 23:37:48</cachedUntil>
</eveapi>
         */
        [XmlIgnore]
		public static string CreateKeyUrl { get { return "https://support.eveonline.com/api/Key/Create"; } }

        [XmlElement("key")]
        public ApiKeyData Key
        {
            get;
            set;
        }
    }

    public class ApiKeyData
    {
        [XmlAttribute("accessMask")]
        public int AccessMaskXML
        {
            get { return (int)AccessMask; }
            set { AccessMask = (ApiKeyMask)value; }
        }

        [XmlIgnore]
        public ApiKeyMask AccessMask
        {
            get;
            set;
        }
		
		[XmlAttribute("type")]
		public ApiKeyType Type
		{
			get;
			set;
		}

        [XmlAttribute("expires")]
		public string ExpiresXML
		{
            get { return Expires.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                Expires = value.TimeStringToDateTime();
            }
		}
		
        [XmlIgnore]
		public DateTime Expires
		{
			get;
			set;
		}
		
		[XmlArray("characters")]
        [XmlArrayItem("character")]
        public List<CharacterListItem> Characters { get; set; }
	}

    public class CharacterListItem
    {
        [XmlAttribute("characterName")]
        public string Name { get; set; }

        [XmlAttribute("characterID")]
        public long CharacterID { get; set; }

        [XmlAttribute("corporationName")]
        public string CorporationName { get; set; }

        [XmlAttribute("corporationID")]
        public long CorporationID { get; set; }

    }
}

