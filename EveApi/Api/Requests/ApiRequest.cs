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
using EveApi.Attributes;
using System.Diagnostics;
using System.Collections.Specialized;

namespace EveApi
{
    public class ApiRequest<T> : IApiRequest
        where T : class
    {
        private static XslCompiledTransform m_RowsetTransform;
        private DateTime m_LastUpdate = DateTime.MinValue;
        private ApiResult<T> m_LastResult = null;
        private Thread m_UpdatingThread = null;
        private bool m_Enabled = false;

        public event RequestUpdated OnRequestUpdate;
        public delegate void RequestUpdated(ApiResult<T> result);

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
            get { return m_UpdatingThread != null && m_UpdatingThread.IsAlive; }
        }

        public IApiResult LastResult
        {
            get { return m_LastResult; }
        }

        public static XslCompiledTransform RowsetTransform
        {
            // Neat way to check for null and assign if needed \o/
            get { return m_RowsetTransform ?? (m_RowsetTransform = LoadXSLT(Properties.Resources.RowsetXSLT)); }
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
        }

        public void UpdateOnSecTick()
        {
            if (Enabled && NextUpdate.CompareTo(DateTime.UtcNow) <= 0 && !IsUpdating)
            {
                //if (m_UpdatingThread == null || !m_UpdatingThread.IsAlive)
                {
                    m_UpdatingThread = new Thread(new ThreadStart(QueryApi));
                    m_UpdatingThread.IsBackground = true;
                }

                m_UpdatingThread.Start();
            }
        }

        private void QueryApi()
        {
            Console.WriteLine("Updating from API for {0}", typeof(T).Name);

            PropertyInfo pi = typeof(T).GetProperty("ApiUri", typeof(string));
            string apiUri = pi.GetValue(null, null) as string;

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

            string url = "http://api.eveonline.com" + apiUri;
            HttpWebRequest request = null;

            Uri uri = new Uri(url);
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";

            string xmlResult = string.Empty;

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

            if(OnRequestUpdate != null)
                OnRequestUpdate(m_LastResult);
        }

        private static void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return;

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
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
