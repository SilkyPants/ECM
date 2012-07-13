using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECM.API.EVE
{
    [ApiUri("/server/ServerStatus.xml.aspx")]
    public class ServerStatus
    {
        [XmlElement("serverOpen")]
        public string ServerOnlineXML
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool ServerOnline
        {
            get { return bool.Parse(ServerOnlineXML); }
        }

        [XmlElement("onlinePlayers")]
        public long NumberOfPlayers
        {
            get;
            set;
        }
    }
}
