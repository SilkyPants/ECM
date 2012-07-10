using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ECM.API.EVE
{
	public class MapKills
	{
        [XmlIgnore]
        public static string ApiUri { get { return "/map/Kills.xml.aspx"; } }

        [XmlArray("solarSystems")]
        [XmlArrayItem("solarSystem")]
        public List<SystemKills> SystemKills { get; set; }
	}
	
	public class SystemKills
	{
		[XmlAttribute("solarSystemID")]
		public long SolarSystemID
		{
			get;
			set;
		}
		
		[XmlAttribute("shipKills")]
		public int ShipKills
		{
			get;
			set;
		}
		
		[XmlAttribute("factionKills")]
		public int FactionKills
		{
			get;
			set;
		}
		
		[XmlAttribute("podKills")]
		public int PodKills
		{
			get;
			set;
		}
	}
}

