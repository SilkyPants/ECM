using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EveApi.Character
{
    public class AccountBalance : CharacterApiBase
    {
        public struct AccountInfo
        {
            public int AccountID;
            public int AccountKey;
            public decimal AccountBalance;
        }

        public List<AccountInfo> Accounts
        {
            get;
            private set;
        }

        public override string ApiUri
        {
            get { return "/char/AccountBalance.xml.aspx"; }
        }

        public override bool ParseData()
        {
            Accounts = new List<AccountInfo>();

            using (XmlReader reader = ApiRawDocument.CreateNavigator().ReadSubtree())
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("row"))
                        {
                            AccountInfo newAccount = new AccountInfo();

                            newAccount.AccountID = int.Parse(reader.GetAttribute("accountID"));
                            newAccount.AccountKey = int.Parse(reader.GetAttribute("accountKey"));
                            newAccount.AccountBalance = decimal.Parse(reader.GetAttribute("balance"));

                            Accounts.Add(newAccount);
                        }
                    }
                }
            }

            return true;
        }
    }
}
