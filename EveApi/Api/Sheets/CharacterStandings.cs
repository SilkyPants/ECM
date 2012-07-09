using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EveApi.Attributes;
using System.Xml.Serialization;

namespace EveApi
{
    [NeedsCharacterID]
    [KeyNeedsMask(ApiKeyMask.Standings)]
    public class CharacterStandings
    {
        [XmlIgnore]
        public static string ApiUri { get { return "/char/Standings.xml.aspx"; } }

        [XmlArray("factions")]
        [XmlArrayItem("faction")]
        public List<StandingInfo> Factions { get; set; }

        [XmlArray("NPCCorporations")]
        [XmlArrayItem("NPCCorporation")]
        public List<StandingInfo> Corporations { get; set; }

        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public List<StandingInfo> Agents { get; set; }
    }

    public class StandingInfo
    {
        /// <summary>
        /// ID number of agent, NPC corporation, or faction you have standing with
        /// </summary>
        [XmlAttribute("fromID")]
        public long FromID { get; set; }

        /// <summary>
        /// Name of agent, NPC corporation, or faction you have standing with.
        /// </summary>
        [XmlAttribute("fromName")]
        public string FromName { get; set; }

        /// <summary>
        /// Current standing with this entity. This is the base standing, not taking into account Connections or Diplomacy skill and is in the range [-10,10].
        /// </summary>
        [XmlAttribute("standing")]
        public float Standing { get; set; }
    }
}
