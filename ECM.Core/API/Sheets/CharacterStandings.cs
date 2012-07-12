using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace ECM.API.EVE
{
    public enum CharacterStandingType
    {
        Faction,
        Corporation,
        Agent
    }

    [NeedsCharacterID]
    [KeyNeedsMask(ApiKeyMask.Standings)]
    public class CharacterStandings
    {
        [XmlIgnore]
        public static string ApiUri { get { return "/char/Standings.xml.aspx"; } }

        [XmlElement("characterNPCStandings")]
        public CharacterNPCStandings NPCStandings { get; set; }
    }
    
    public class CharacterNPCStandings
    {
        private Collection<StandingInfo> m_Agents = new Collection<StandingInfo>();
        private Collection<StandingInfo> m_NPCCorporations = new Collection<StandingInfo>();
        private Collection<StandingInfo> m_Factions = new Collection<StandingInfo>();

        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public Collection<StandingInfo> Agents 
        {
            get
            {
                foreach (StandingInfo agentStanding in m_Agents)
                {
                    agentStanding.Type = CharacterStandingType.Agent;
                }
                return m_Agents;
            }
        }

        [XmlArray("NPCCorporations")]
        [XmlArrayItem("NPCCorporation")]
        public Collection<StandingInfo> NPCCorporations
        {
            get
            {
                foreach (StandingInfo corpStanding in m_NPCCorporations)
                {
                    corpStanding.Type = CharacterStandingType.Corporation;
                }
                return m_NPCCorporations;
            }
        }

        [XmlArray("factions")]
        [XmlArrayItem("faction")]
        public Collection<StandingInfo> Factions
        {
            get
            {
                foreach (StandingInfo factionStanding in m_Factions)
                {
                    factionStanding.Type = CharacterStandingType.Faction;
                }
                return m_Factions;
            }
        }

        [XmlIgnore]
        public ReadOnlyCollection<StandingInfo> All
        {
            get
            {
                List<StandingInfo> allStandings = new List<StandingInfo>();
                allStandings.AddRange(m_Agents);
                allStandings.AddRange(m_NPCCorporations);
                allStandings.AddRange(m_Factions);

                return allStandings.AsReadOnly();
            }
        }
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

        [XmlIgnore]
        public string Status 
        {
            get
            {
                if (Standing <= -5.5)
                    return "Terrible";

                if (Standing <= -0.5)
                    return "Bad";

                if (Standing < 0.5)
                    return "Neutral";

                return Standing < 5.5 ? "Good" : "Excellent";
            }
        }

        [XmlIgnore]
        public CharacterStandingType Type 
        { 
            get; 
            set; 
        }
    }
}
