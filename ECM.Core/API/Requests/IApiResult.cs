using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ECM.API.EVE
{
    public interface IApiResult
    {
        int APIVersion
        {
            get;
            set;
        }

        string CurrentTime
        {
            get;
            set;
        }

        string CachedUntilXML
        {
            get;
            set;
        }

        DateTime CachedUntil
        {
            get;
            set;
        }

        CCPError Error
        {
            get;
            set;
        }

        XmlDocument XmlDocument
        {
            get;
            set;
        }
    }
}