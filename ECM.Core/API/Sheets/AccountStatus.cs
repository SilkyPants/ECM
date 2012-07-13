using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ECM.API.EVE
{
    [NeedsApiKey]
    [KeyNeedsMask(ApiKeyMask.AccountStatus)]
    [ApiUri("/account/AccountStatus.xml.aspx")]
    public class AccountStatus : IAccountStatus
    {
        /// <summary>
        /// User ID of account that you queried
        /// </summary>
        [XmlElement("userID")]
        public int UserID
        {
            get;
            set;
        }

        /// <summary>
        /// The number of times you logged into CCP's services (this includes not only the game logons but also forum logons, likely also EVEGate logons)
        /// </summary>
        [XmlElement("logonCount")]
        public int LogonCount
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of time you actually spent logged on in the game
        /// </summary>
        [XmlElement("logonMinutes")]
        public int LogonMinutes
        {
            get;
            set;
        }

        /// <summary>
        /// The date until which the account is currently subscribed in Eve text form
        /// </summary>
        [XmlElement("paidUntil")]
        public string PaidUntilXML
        {
            get { return PaidUntil.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                PaidUntil = value.TimeStringToDateTime();
            }
        }

        /// <summary>
        /// The date the account was created in Eve text form
        /// </summary>
        [XmlElement("createDate")]
        public string CreateDateXML
        {
            get { return CreateDate.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                CreateDate = value.TimeStringToDateTime();
            }
        }

        /// <summary>
        /// The date until which the account is currently subscribed
        /// </summary>
        [XmlIgnore]
        public DateTime PaidUntil
        {
            get;
            set;
        }

        /// <summary>
        /// The date the account was created
        /// </summary>
        [XmlIgnore]
        public DateTime CreateDate
        {
            get;
            set;
        }
    }
}
