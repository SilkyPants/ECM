using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EveApi
{
    [XmlRoot("eveapi")]
    public class ApiResult<T> : IApiResult
    {
        public ApiResult()
        {
            CachedUntil = DateTime.MinValue;
        }

        [XmlAttribute("version")]
        public int APIVersion
        {
            get;
            set;
        }

        [XmlElement("currentTime")]
        public string CurrentTime
        {
            get;
            set;
        }

        [XmlElement("cachedUntil")]
        public string CachedUntilXML
        {
            get { return CachedUntil.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                CachedUntil = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public DateTime CachedUntil
        {
            get;
            set;
        }

        [XmlElement("result")]
        public T Result
        {
            get;
            set;
        }

        [XmlElement("error")]
        public CCPError Error
        {
            get;
            set;
        }

        [XmlIgnore]
        public XmlDocument XmlDocument
        {
            get;
            set;
        }
    }
}
