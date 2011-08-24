using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EveApi.Account
{
    public class AccountStatus : ApiBase
    {
        /// <summary>
        /// The date the account is paid until
        /// </summary>
        public DateTime PaidUntil
        {
            get;
            private set;
        }

        /// <summary>
        /// The date the account was created
        /// </summary>
        public DateTime CreationDate
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of times the account has logged into CCP services
        /// </summary>
        public int NumberOfLogons
        {
            get;
            private set;
        }

        /// <summary>
        /// THe number of minutes the account has played EVE for
        /// </summary>
        public int PlayTimeMinutes
        {
            get;
            private set;
        }

        public override string ApiUri
        {
            get { return "/account/AccountStatus.xml.aspx"; }
        }

        public override bool ParseData()
        {
            using (XmlReader reader = ApiRawDocument.CreateNavigator().ReadSubtree())
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("paidUntil"))
                        {
                            PaidUntil = DateTime.Parse(reader.ReadElementString());
                        }
                        if (reader.Name.Equals("createDate"))
                        {
                            CreationDate = DateTime.Parse(reader.ReadElementString());
                        }
                        if (reader.Name.Equals("logonCount"))
                        {
                            NumberOfLogons = int.Parse(reader.ReadElementString());
                        }
                        if (reader.Name.Equals("logonMinutes"))
                        {
                            PlayTimeMinutes = int.Parse(reader.ReadElementString());
                        }
                    }
                }
            }

            return true;
        }
    }
}
