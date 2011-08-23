using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EveApi.Account
{
    public class CharacterList : ApiBase
    {
        public struct CharacterInfo
        {
            public string Name;
            public string ID;
            public string CorporationName;
            public string CorporationID;
        }

        public List<CharacterInfo> Characters
        {
            get;
            private set;
        }

        public override string ApiUri
        {
            get { return "/account/Characters.xml.aspx"; }
        }

        public override bool ParseData()
        {
            XmlReader reader = ApiRawDocument.CreateNavigator().ReadSubtree();
            ApiRawDocument.Save("CharList.xml");
            if (reader.ReadToFollowing("rowset"))
            {
                Characters = new List<CharacterInfo>();

                using (XmlReader characterTree = reader.ReadSubtree())
                {
                    while (characterTree.Read())
                    {
                        if (characterTree.NodeType == XmlNodeType.Element && characterTree.Name == "row")
                        {
                            CharacterInfo character = new CharacterInfo();

                            character.Name = reader.GetAttribute("name");
                            character.ID = reader.GetAttribute("characterID");
                            character.CorporationName = reader.GetAttribute("corporationName");
                            character.CorporationID = reader.GetAttribute("corporationID");

                            Characters.Add(character);
                        }
                    }
                }
            }

            return true;
        }
    }
}
