using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EveApi.Attributes;
using System.Xml.Serialization;

namespace EveApi
{
	[NeedsApiKey]
	public class ApiKeyInfo
	{
        [XmlIgnore]
		public static string CreateKeyUrl { get { return "https://support.eveonline.com/api/Key/Create"; } }
		
        [XmlIgnore]
        public static string ApiUri { get { return "/account/AccountStatus.xml.aspx"; } }
		
		[XmlAttribute("accessMask")]
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
		
        [XmlElement("expires")]
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
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("characterID")]
        public long CharacterID { get; set; }

        [XmlAttribute("corporationName")]
        public string CorporationName { get; set; }

        [XmlAttribute("corporationID")]
        public long CorporationID { get; set; }

    }
}

