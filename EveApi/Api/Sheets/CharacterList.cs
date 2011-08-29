using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EveApi.Attributes;

namespace EveApi
{
    [NeedsCharacterID]
    public class CharacterList
    {
        [XmlIgnore]
        public static string ApiUri { get { return "/account/Characters.xml.aspx"; } }

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
