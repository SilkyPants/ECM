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
    }

    public class EveSkill : EveItem
    {
        public int Rank { get; set; }
        public List<long> RequiredSkills { get; set; }

        public SkillAttributes PrimaryAttribute { get; set; }
        public SkillAttributes SecondaryAttribute { get; set; }

        public int PointsAtLevel(int level)
        {
            if (level > 5)
                level = 5;

            return (int)Math.Round(Math.Pow(2, (2.5f * level) - 2.5f) * 250 * Rank, MidpointRounding.AwayFromZero);
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
    }

    public class EveShip : EveItem
    {

    }
}

