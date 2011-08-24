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
        /// <summary>
        /// The URI to retrieve the information from the API.
        /// Must be appended to the API URL
        /// </summary>
        public abstract string ApiUri
        {
            get;
        }

        /// <summary>
        /// API key to get the infomation
        /// </summary>
        public string ApiKey
        {
            get;
            set;
        }

        /// <summary>
        /// API user ID to get the information
        /// </summary>
        public string ApiUserId
        {
            get;
            set;
        }

        XmlDocument m_ApiRawDocument = null;
        /// <summary>
        /// The raw XML document from the API
        /// </summary>
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

        /// <summary>
        /// Time the document is valid until
        /// </summary>
        public DateTime CacheTime
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parse the raw XML into a usable class
        /// </summary>
        /// <returns></returns>
        public abstract bool ParseData();

        /// <summary>
        /// Parse the XML document for errors
        /// </summary>
        /// <param name="docToCheck">The document to check for errors</param>
        /// <returns>True if any erorrs are found</returns>
        private bool ParseForErrors(XmlDocument docToCheck)
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

        /// <summary>
        /// Used to add addditional information to the POST string
        /// </summary>
        public virtual void AddDataToPost(PostSubmitter post) 
        {
            post.PostItems.Add("userID", ApiUserId);
            post.PostItems.Add("apiKey", ApiKey);
            post.PostItems.Add("version", "2");
        }

        /// <summary>
        /// Downloads the information from the API and stored is if it's valid
        /// </summary>
        /// <param name="proxyInfo">Proxy info to use</param>
        /// <returns>True if the information was retrieved successfully</returns>
        public bool GrabDataFromApi(ProxyInfo proxyInfo)
        {
            XmlDocument returnDoc = new XmlDocument();

            // TODO: Add proxy info in properly.
            PostSubmitter post = new PostSubmitter(proxyInfo);

            AddDataToPost(post);

            post.Url = "http://api.eve-online.com" + ApiUri;

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

            return ParseData();
        }
        #endregion
    }
}
