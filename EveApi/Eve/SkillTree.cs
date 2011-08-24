using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EveApi.Eve
{
    public class SkillTree : ApiBase
    {
        public struct SkillInfo
        {
            public int TypeID;
            public string Name;
            public bool Published;
            public string Description;
            public int Rank;
            
        }

        public class SkillGroup
        {
            public int GroupID;
            public string Name;
            public List<SkillInfo> Skills = new List<SkillInfo>();

            public bool ParseSkillGroup(XmlReader groupReader)
            {
                while (groupReader.Read())
                {
                    if(groupReader.NodeType == XmlNodeType.Element)
                    {
                        string setName = groupReader.GetAttribute("name");
                        if (groupReader.Name.Equals("rowset") && setName == "skills")
                        {
                            // Grab the subtree
                            using (XmlReader skillsReader = groupReader.ReadSubtree())
                            {
                                while (skillsReader.Read())
                                {
                                    if (skillsReader.Name.Equals("row"))
                                    {
                                        SkillInfo newSkill = new SkillInfo();

                                        newSkill.Published = skillsReader.GetAttribute("published").Equals("1") ? true : false;
                                        newSkill.TypeID = int.Parse(skillsReader.GetAttribute("typeID"));
                                        newSkill.Name = skillsReader.GetAttribute("typeName");

                                        using (XmlReader skillReader = skillsReader.ReadSubtree())
                                        {
                                            while (skillReader.Read())
                                            {
                                                if (skillReader.NodeType == XmlNodeType.Element)
                                                {
                                                    if (skillReader.Name.Equals("description"))
                                                    {
                                                        newSkill.Description = skillReader.ReadElementString();
                                                    }
                                                    if (skillReader.Name.Equals("rank"))
                                                    {
                                                        newSkill.Rank = int.Parse(skillReader.ReadElementString());
                                                    }
                                                }
                                            }
                                        }

                                        Skills.Add(newSkill);
                                    }
                                }
                            }
                        }
                        else if (groupReader.Name.Equals("row"))
                        {
                            GroupID = int.Parse(groupReader.GetAttribute("groupID"));
                            Name = groupReader.GetAttribute("groupName");
                        }
                    }
                }

                return true;
            }
        }

        public List<SkillGroup> SkillGroups
        {
            get;
            private set;
        }

        public override string ApiUri
        {
            get { return "/eve/SkillTree.xml.aspx"; }
        }

        public override bool ParseData()
        {
            XmlReader reader = ApiRawDocument.CreateNavigator().ReadSubtree();
            while (reader.Read())
            {
                XmlNodeType nodeType = reader.NodeType;
                switch (nodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.Name.Equals("rowset") && reader.GetAttribute("name") == "skillGroups")
                            {
                                SkillGroup newGroup = new SkillGroup();

                                newGroup.ParseSkillGroup(reader.ReadSubtree());

                                if (SkillGroups == null)
                                    SkillGroups = new List<SkillGroup>();

                                SkillGroups.Add(newGroup);
                            }
                        }
                        break;
                }
            }

            return true;
        }
    }
}