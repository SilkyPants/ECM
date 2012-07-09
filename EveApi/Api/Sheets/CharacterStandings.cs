using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EveApi.Attributes;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace EveApi
{
    [NeedsCharacterID]
    [KeyNeedsMask(ApiKeyMask.Standings)]
    public class CharacterStandings
    {
        [XmlIgnore]
        public static string ApiUri { get { return "/char/Standings.xml.aspx"; } }

        [XmlElement("characterNPCStandings")]
        public CharacterNPCStandings CharacterNPCStandings { get; set; }
    }
    /*
    public class CharacterNPCStandings
    {
        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public List<StandingInfo> Agents { get; set; }

        [XmlArray("NPCCorporations")]
        [XmlArrayItem("NPCCorporation")]
        public List<StandingInfo> Corporations { get; set; }

        [XmlArray("factions")]
        [XmlArrayItem("faction")]
        public List<StandingInfo> Factions { get; set; }
    }*/

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

    public sealed class CharacterNPCStandings
    {
        private readonly List<StandingInfo> m_agentStandings;
        private readonly List<StandingInfo> m_npcCorporationStandings;
        private readonly List<StandingInfo> m_factionStandings;

        public CharacterNPCStandings()
        {
            m_agentStandings = new List<StandingInfo>();
            m_npcCorporationStandings = new List<StandingInfo>();
            m_factionStandings = new List<StandingInfo>();
        }

        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public List<StandingInfo> AgentStandings
        {
            get
            {
                return m_agentStandings;
            }
        }

        [XmlArray("NPCCorporations")]
        [XmlArrayItem("NPCCorporation")]
        public List<StandingInfo> NPCCorporationStandings
        {
            get
            {
                return m_npcCorporationStandings;
            }
        }

        [XmlArray("factions")]
        [XmlArrayItem("faction")]
        public List<StandingInfo> FactionStandings
        {
            get
            {
                return m_factionStandings;
            }
        }

//        [XmlIgnore]
//        public IEnumerable<StandingInfo> All
//        {
//            get
//            {
//                List<StandingInfo> standings = new List<StandingInfo>();
//                standings.AddRange(AgentStandings);
//                standings.AddRange(NPCCorporationStandings);
//                standings.AddRange(FactionStandings);
//                return standings;
//            }
//        }
    }
}
