using System;

namespace ECM
{
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
}

