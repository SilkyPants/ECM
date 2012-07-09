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
        public SerializableStandings CharacterNPCStandings { get; set; }
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
    }*/

    public sealed class SerializableStandings
    {
        private readonly Collection<SerializableStandingsListItem> m_agentStandings;
        private readonly Collection<SerializableStandingsListItem> m_npcCorporationStandings;
        private readonly Collection<SerializableStandingsListItem> m_factionStandings;

        public SerializableStandings()
        {
            m_agentStandings = new Collection<SerializableStandingsListItem>();
            m_npcCorporationStandings = new Collection<SerializableStandingsListItem>();
            m_factionStandings = new Collection<SerializableStandingsListItem>();
        }

        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public Collection<SerializableStandingsListItem> AgentStandings
        {
            get
            {
                //foreach (SerializableStandingsListItem agentStanding in m_agentStandings)
                //{
                //    agentStanding.Group = StandingGroup.Agents;
                //}
                return m_agentStandings;
            }
        }

        [XmlArray("NPCCorporations")]
        [XmlArrayItem("NPCCorporation")]
        public Collection<SerializableStandingsListItem> NPCCorporationStandings
        {
            get
            {
                //foreach (SerializableStandingsListItem npcCorporationStanding in m_npcCorporationStandings)
                //{
                //    npcCorporationStanding.Group = StandingGroup.NPCCorporations;
                //}
                return m_npcCorporationStandings;
            }
        }

        [XmlArray("factions")]
        [XmlArrayItem("faction")]
        public Collection<SerializableStandingsListItem> FactionStandings
        {
            get
            {
                //foreach (SerializableStandingsListItem factionStanding in m_factionStandings)
                //{
                //    factionStanding.Group = StandingGroup.Factions;
                //}
                return m_factionStandings;
            }
        }

        [XmlIgnore]
        public IEnumerable<SerializableStandingsListItem> All
        {
            get
            {
                List<SerializableStandingsListItem> standings = new List<SerializableStandingsListItem>();
                standings.AddRange(AgentStandings);
                standings.AddRange(NPCCorporationStandings);
                standings.AddRange(FactionStandings);
                return standings;
            }
        }
    }

    public sealed class SerializableStandingsListItem
    {
        [XmlAttribute("fromID")]
        public int ID { get; set; }

        [XmlAttribute("fromName")]
        public string Name { get; set; }

        [XmlAttribute("standing")]
        public double StandingValue { get; set; }

    }
}
