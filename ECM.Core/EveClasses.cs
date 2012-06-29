using System;
using System.Collections.Generic;

namespace ECM
{
    public enum SkillAttributes
    {
        Charisma = 164,
        Intelligence = 165,
        Memory = 166,
        Perception = 167,
        Willpower = 168
    }

	public class EveBase
	{
		public long ID { get; set; }
		public string Name { get; set; }
		public string IconString { get; set; }
		public object Tag { get; set; }

        public override string ToString()
        {
            return Name;
        }
	}

    public class EveMarketGroup : EveBase
    {
        public long ParentID { get; set; }
        public bool HasItems { get; set; }
    }

    public class EveItem : EveBase
    {
        public long MarketGroupID { get; set; }
        public string Description { get; set; }

        RequiredSkill[] m_RequiredSkills = new RequiredSkill[6];
        public RequiredSkill[] RequiredSkills 
        { 
            get
            {
                return m_RequiredSkills;
            }
        }
    }

    public class EveSkill : EveItem
    {
        public int Rank { get; set; }

        public SkillAttributes PrimaryAttribute { get; set; }
        public SkillAttributes SecondaryAttribute { get; set; }

        public int PointsAtLevel(int level)
        {
            if (level > 5)
                level = 5;

            return (int)Math.Ceiling(Math.Pow(2, (2.5f * level) - 2.5f) * 250 * Rank);
        }
    }

    public class EveCertGroup : EveBase
    {
        public long ParentID { get; set; }
        public string Description { get; set; }
    }

    public class EveCertificate : EveBase
    {
        public long GroupID { get; set; }
        public int Grade { get; set; }
        public string Description { get; set; }
    }

    public class EveShip : EveItem
    {

    }

    public struct RequiredSkill
    {
        public long SkillID { get; set; }
        public int SkillLevel { get; set; }
        public bool IsValid { get { return SkillID != 0 && SkillLevel != 0; } }
    }
}

