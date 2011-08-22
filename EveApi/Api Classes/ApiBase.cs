using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EveApi
{
    public abstract class ApiBase
    {
        #region Properties
        public abstract string ApiUrl
        {
            get;
        }

        public string ApiKey
        {
            get;
            set;
        }

        public string ApiUserId
        {
            get;
            set;
        }

        XmlDocument m_ApiRawDocument = null;
        public XmlDocument ApiRawDocument
        {
            get { return m_ApiRawDocument; }
            set
            {
                m_ApiRawDocument = value;
                if(value != null)
                {
                    XmlReader reader = m_ApiRawDocument.CreateNavigator().ReadSubtree();
                    
                    if(reader.ReadToFollowing("cachedUntil"))
                    {
                        CacheTime = DateTime.Parse(reader.ReadElementString());
                    }

                }
            }
        }

        public DateTime CacheTime
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        public abstract bool ParseData();

        public bool ParseForErrors(XmlDocument docToCheck)
        {
            XmlReader reader = docToCheck.CreateNavigator().ReadSubtree();
            while (reader.Read())
            {
                XmlNodeType nodeType = reader.NodeType;
                switch (nodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.Equals("error"))
                        {
                            string errorCode = reader.GetAttribute("code");
                            string errorString = reader.ReadElementString();

                            Console.WriteLine(errorString + "\n\nError Code: " + errorCode);

                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        public virtual void AddDataToPost() { /* nothing */ }

        public bool GrabDataFromApi(ProxyInfo proxyInfo)
        {
            XmlDocument returnDoc = new XmlDocument();

            // TODO: Add proxy info in properly.
            PostSubmitter post = new PostSubmitter(proxyInfo);

            post.PostItems.Add("userID", ApiUserId);
            post.PostItems.Add("apiKey", ApiKey);
            post.PostItems.Add("version", "2");

            post.Url = "http://api.eve-online.com/" + ApiUrl;

            post.Type = PostSubmitter.PostTypeEnum.Post;

            // Post it and get the results
            string result = "";

            if (!post.Post(ref result))
            {
                return false;
            }

            // we create a XML document from the returned string
            returnDoc.InnerXml = result;

            // Quickly go through the file looking for errors
            if (ParseForErrors(returnDoc))
                return false;

            ApiRawDocument = returnDoc;

            return true;
        }
        #endregion
    }
}
