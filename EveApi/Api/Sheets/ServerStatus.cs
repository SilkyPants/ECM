using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EveApi
{
    public class ServerStatus
    {
        [XmlIgnore]
        public static string ApiUri { get { return "/server/ServerStatus.xml.aspx"; } }

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
