using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECM.API.EVE
{
    [XmlRoot("error")]
    public class CCPError
    {
        [XmlAttribute("code")]
        public int ErrorCode
        {
            get;
            set;
        }

        [XmlText]
        public string ErrorString
        {
            get;
            set;
        }

    }
}
