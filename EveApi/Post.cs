using System;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using System.Collections.Specialized;

namespace EveApi
{
    /// <summary>
    /// Submits post data to a url.
    /// </summary>
    #region PostSubmitter class
    public class PostSubmitter
    {
        /// <summary>
        /// determines what type of post to perform.
        /// </summary>
        public enum PostTypeEnum
        {
            /// <summary>
            /// Does a get against the source.
            /// </summary>
            Get,
            /// <summary>
            /// Does a post against the source.
            /// </summary>
            Post
        }

        private string m_url=string.Empty;
        private NameValueCollection m_values=new NameValueCollection();

        private ProxyInfo proxyInfo = new ProxyInfo();

        private PostTypeEnum m_type=PostTypeEnum.Get;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PostSubmitter(ProxyInfo newProxyInfo)
        {
            proxyInfo = newProxyInfo;
        }

        /// <summary>
        /// Constructor that accepts a url as a parameter
				/// </summary>
				/// <param name="newProxyInfo">The proxy info needed to make the post.</param>
				/// <param name="url">The url where the post will be submitted to.</param>
        public PostSubmitter(ProxyInfo newProxyInfo, string url)
            : this(newProxyInfo)
        {
            m_url=url;
        }

        /// <summary>
        /// Constructor allowing the setting of the url and items to post.
				/// </summary>
				/// <param name="newProxyInfo">the proxy info needed to make the post.</param>
				/// <param name="url">the url for the post.</param>
        /// <param name="values">The values for the post.</param>
        public PostSubmitter(ProxyInfo newProxyInfo, string url, NameValueCollection values)
            : this(newProxyInfo, url)
        {
            m_values=values;
        }

        /// <summary>
        /// Gets or sets the url to submit the post to.
        /// </summary>
        public string Url
        {
            get
            {
                return m_url;
            }
            set
            {
                m_url=value;
            }
        }
        /// <summary>
        /// Gets or sets the name value collection of items to post.
        /// </summary>
        public NameValueCollection PostItems
        {
            get
            {
                return m_values;
            }
            set
            {
                m_values=value;
            }
        }
        /// <summary>
        /// Gets or sets the type of action to perform against the url.
        /// </summary>
        public PostTypeEnum Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type=value;
            }
        }

        /// <summary>
        /// Gets or sets the proxy info to use in sending the POST.
        /// </summary>
        public ProxyInfo ProxyInfo
        {
            get
            {
                return proxyInfo;
            }
            set
            {
                proxyInfo = value;
            }
        }
        /// <summary>
        /// Posts the supplied data to specified url.
        /// </summary>
        /// <returns>a string containing the result of the post.</returns>
        public bool Post(ref string result)
        {
            StringBuilder parameters=new StringBuilder();

            for (int i=0;i < m_values.Count;i++)
            {
                EncodeAndAddItem(ref parameters,m_values.GetKey(i),m_values[i]);
            }

            result=PostData(m_url,parameters.ToString());

            return (result!=null);
        }
        ///// <summary>
        ///// Posts the supplied data to specified url.
        ///// </summary>
        ///// <param name="url">The url to post to.</param>
        ///// <returns>a string containing the result of the post.</returns>
        //public string Post(string url)
        //{
        //    m_url=url;
        //    return this.Post();
        //}
				///// <summary>
				///// Posts the supplied data to specified url.
				///// </summary>
				///// <param name="url">The url to post to.</param>
				///// <param name="values">The values to post.</param>
				///// <returns>a string containing the result of the post.</returns>
        //public string Post(string url, NameValueCollection values)
        //{
        //    m_values=values;
        //    return this.Post(url);
        //}
				///// <summary>
				///// Posts data to a specified url. Note that this assumes that you have already url encoded the post data.
				///// </summary>
				///// <param name="postData">The data to post.</param>
				///// <param name="url">the url to post to.</param>
				///// <returns>Returns the result of the post.</returns>
        private string PostData(string url, string postData)
        {
            HttpWebRequest request=null;

            if (m_type==PostTypeEnum.Post)
            {
                Uri uri = new Uri(url);
                request = (HttpWebRequest) WebRequest.Create(uri);
                
                // Proxy Stuff
                if(proxyInfo.UseProxy)
                {
                    WebProxy myProxy = new WebProxy();

                    // Associate the newUri object to 'myProxy' object so that new myProxy settings can be set.
                    myProxy.Address = new Uri("http://" + proxyInfo.ProxyIP + ":" + proxyInfo.ProxyPort);
                    // Create a NetworkCredential object and associate it with the Proxy property of request object.
                    if (proxyInfo.ProxyDomain.Length > 0)
                        myProxy.Credentials = new NetworkCredential(proxyInfo.ProxyUser, proxyInfo.ProxyPass, proxyInfo.ProxyDomain);
                    else
                        myProxy.Credentials = new NetworkCredential(proxyInfo.ProxyUser, proxyInfo.ProxyPass);
                    request.Proxy = myProxy;
                }
		        //End Proxy Stuff
                
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
								request.Timeout = 5000;

                try
                {
                    using (Stream writeStream = request.GetRequestStream())
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        byte[] bytes = encoding.GetBytes(postData);
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }
								catch (WebException webEx)
                {
										//System.Windows.Forms.MessageBox.Show(
                                        Console.WriteLine(
                                        webEx.Message + " (" + url + ")"
										//,"Error: " + webEx.Status.ToString(),
										//System.Windows.Forms.MessageBoxButtons.OK,
										//System.Windows.Forms.MessageBoxIcon.Error
                                        );
                    return null;
                }
            }
            else
            {
                Uri uri = new Uri(url + "?" + postData);
                request = (HttpWebRequest) WebRequest.Create(uri);
                request.Method = "GET";
            }

            string result=string.Empty;

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader (responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Encodes an item and ads it to the string.
        /// </summary>
				/// <param name="baseRequest">The previously encoded data.</param>
				/// <param name="key">The key to encode.</param>
        /// <param name="dataItem">The data to encode.</param>
        /// <returns>A string containing the old data and the previously encoded data.</returns>
        private void EncodeAndAddItem(ref StringBuilder baseRequest, string key, string dataItem)
        {
            if (baseRequest==null)
            {
                baseRequest=new StringBuilder();
            }

            if (baseRequest.Length!=0)
            {
                baseRequest.Append("&");
            }

            baseRequest.Append(key);
            baseRequest.Append("=");
            baseRequest.Append(HttpUtility.UrlEncode(dataItem));
        }
    }
    #endregion

    #region ProxyInfo Class
		/// <summary>
		/// Class that encapsulates all info needed to use a proxy
		/// </summary>
		public class ProxyInfo
    {
        // Proxy Stuff
        string proxyUser = "";
        string proxyPass = "";
        string proxyDomain = "";
        string proxyIP = "";
        string proxyPort = "";
        bool useProxy = false;

				/// <summary>
				/// Gets or sets the user name
				/// </summary>
				public string ProxyUser
        {
            get
            {
                return proxyUser;
            }
            set
            {
                proxyUser = value;
            }
        }

				/// <summary>
				/// Gets or sets the password
				/// </summary>
				public string ProxyPass
        {
            get
            {
                return proxyPass;
            }
            set
            {
                proxyPass = value;
            }
        }

				/// <summary>
				/// Gets or sets the Domain
				/// </summary>
				public string ProxyDomain
        {
            get
            {
                return proxyDomain;
            }
            set
            {
                proxyDomain = value;
            }
        }

				/// <summary>
				/// Gets or sets the proxy IP address
				/// </summary>
				public string ProxyIP
        {
            get
            {
                return proxyIP;
            }
            set
            {
                proxyIP = value;
            }
        }

				/// <summary>
				/// Gets or sets the proxy port
				/// </summary>
				public string ProxyPort
        {
            get
            {
                return proxyPort;
            }
            set
            {
                proxyPort = value;
            }
        }

				/// <summary>
				/// Gets or sets the boolean to use the proxy or not
				/// </summary>
				public bool UseProxy
        {
            get
            {
                return useProxy;
            }
            set
            {
                useProxy = value;
            }
        }
    }
    #endregion
}
