using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EveApi
{
    public class ServerStatus : ApiBase
    {
        public bool ServerOnline
        {
            get;
            private set;
        }

        public int NumberOfPlayers
        {
            get;
            private set;
        }

        public override string ApiUri
        {
            get { return "/server/ServerStatus.xml.aspx"; }
        }

        public override bool ParseData()
        {
            using (XmlReader reader = ApiRawDocument.CreateNavigator().ReadSubtree())
            {
                while (reader.Read())
                {
                    if(reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("serverOpen"))
                        {
                            ServerOnline = bool.Parse(reader.ReadElementString());
                        }
                        if (reader.Name.Equals("onlinePlayers"))
                        {
                            NumberOfPlayers = int.Parse(reader.ReadElementString());
                        }
                    }
                }
            }

            return true;
        }
    }
}
