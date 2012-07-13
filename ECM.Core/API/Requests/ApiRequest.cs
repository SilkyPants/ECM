using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Xsl;
using System.Xml;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ECM.API.EVE
{
    public delegate void RequestUpdated(IApiResult result);

    public class ApiRequest<T> : IApiRequest
        where T : class
    {
        const string EVE_API_URL = "http://api.eveonline.com";
        private static XslCompiledTransform m_RowsetTransform;
        private DateTime m_LastUpdate = DateTime.MinValue;
        private ApiResult<T> m_LastResult = null;
        private BackgroundWorker m_Worker = new BackgroundWorker();
        private bool m_Enabled = false;

        public event RequestUpdated OnRequestUpdate;

        protected NameValueCollection postItems = new NameValueCollection();

        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        public DateTime LastUpdate
        {
            get { return m_LastUpdate; }
        }

        public DateTime NextUpdate
        {
            get
            {
                if (LastResult != null)
                {
                    return LastResult.CachedUntil;
                }

                return DateTime.UtcNow;
            }
        }

        public bool IsUpdating
        {
            get { return m_Worker.IsBusy; }
        }

        public IApiResult LastResult
        {
            get { return m_LastResult; }
        }

        public static XslCompiledTransform RowsetTransform
        {
            // Neat way to check for null and assign if needed \o/
            get { return m_RowsetTransform ?? (m_RowsetTransform = LoadXSLT(Core.RowsetXSLT)); }
        }

        public ApiRequest()
        {
            if(this.GetType() == typeof(ApiRequest<T>))
            {
                Debug.Assert(typeof(T).GetProperty("ApiUri", typeof(string)) != null, string.Format("The generic class '{0}' needs a property named 'ApiUri' that points to the Api page", typeof(T).Name));

                bool needsApiKey = typeof(T).GetCustomAttributes(typeof(NeedsApiKeyAttribute), true).Length != 0;

                if (needsApiKey)
                {
                    string typeNeeded = "AuthorisedApiRequest";

                    if (typeof(T).GetCustomAttributes(typeof(NeedsExtraInfoAttribute), true).Length > 0)
                    {
                    }
                    else if (typeof(T).GetCustomAttributes(typeof(NeedsCharacterIDAttribute), true).Length > 0)
                    {
                        typeNeeded = "CharacterApiRequest";
                    }

                    Debug.Assert(false, string.Format("The generic class '{0}' needs additional information, you need to use {1} for this type", typeof(T).Name, typeNeeded));
                }
            }

            m_Worker.DoWork += new DoWorkEventHandler(QueryAPI);
            m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(QueryAPIDone);
        }

        void QueryAPIDone(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Updated from API for {0}", typeof(T).Name);

            if (OnRequestUpdate != null)
                OnRequestUpdate(m_LastResult);
        }

        void QueryAPI (object sender, DoWorkEventArgs e)
        {
            Console.WriteLine ("Updating from API for {0}", typeof(T).Name);

            ApiUriAttribute[] apiUris = (ApiUriAttribute[])typeof(T).GetCustomAttributes (typeof(ApiUriAttribute), false);

            string apiUri = string.Empty;

            if (apiUris.Length == 0)
            {
                Console.WriteLine("{0} does not have an ApiUriAttribute!", typeof(T).Name);
                Enabled = false;
                return;
            }

            apiUri = apiUris[0].ApiUri;

            if (postItems.Count > 0)
            {
                apiUri += "?";

                for (int i = 0; i < postItems.Count; i++)
                {
                    if (i > 0)
                        apiUri += "&";

                    apiUri += postItems.GetKey(i) + "=" + postItems[i];
                }
            }

            string url = EVE_API_URL + apiUri;
            HttpWebRequest request = null;

            Uri uri = new Uri(url);
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";

            string xmlResult = string.Empty;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            xmlResult = readStream.ReadToEnd();
                        }
                    }
                }

                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.InnerXml = xmlResult;

                m_LastResult = DeserializeAPIResultCore<T>(RowsetTransform, doc);

                m_LastUpdate = DateTime.UtcNow;
            }
            //TODO: Something here with the exceptions
            catch (WebException we)
            {
                Console.WriteLine(we.Message);
                Enabled = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Enabled = false;
            }
        }

        public void UpdateOnSecTick()
        {
            if (Enabled && NextUpdate.CompareTo(DateTime.UtcNow) <= 0 && !IsUpdating)
            {
                m_Worker.RunWorkerAsync();
            }
        }

        private ApiResult<U> DeserializeAPIResultCore<U>(XslCompiledTransform transform, XmlDocument doc)
        {
            ApiResult<U> result;

            try
            {
                // Deserialization with a transform
                using (XmlNodeReader reader = new XmlNodeReader(doc))
                {
                    if (transform != null)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                            {
                                // Apply the XSL transform
                                writer.Formatting = Formatting.Indented;
                                transform.Transform(reader, writer);
                                writer.Flush();

                                // Deserialize from the given stream
                                stream.Seek(0, SeekOrigin.Begin);

                                doc.Load(stream);
                                stream.Seek(0, SeekOrigin.Begin);

                                XmlSerializer xs = new XmlSerializer(typeof(ApiResult<U>));
                                result = (ApiResult<U>)xs.Deserialize(stream);

                            }
                        }
                    }
                    // Deserialization without transform
                    else
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(ApiResult<U>));
                        result = (ApiResult<U>)xs.Deserialize(reader);
                    }
                }
            }
            // An error occurred during the XSL transform
            catch (Exception exc)
            {
				Console.WriteLine(exc.Message);
                return null;
            }
            // An error occurred during the deserialization
            //catch (InvalidOperationException exc)
            //{
            //    ExceptionHandler.LogException(exc, true);
            //    result = new APIResult<T>(exc);
            //}
            //catch (XmlException exc)
            //{
            //    ExceptionHandler.LogException(exc, true);
            //    result = new APIResult<T>(exc);
            //}

            // Stores XMLDocument
            result.XmlDocument = doc;
            return result;
        }

        private static XslCompiledTransform LoadXSLT(string content)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            using (StringReader stringReader = new StringReader(content))
            {
                using (XmlTextReader reader = new XmlTextReader(stringReader))
                {
                    xslt.Load(reader);
                }
            }

            return xslt;
        }
    }
}
