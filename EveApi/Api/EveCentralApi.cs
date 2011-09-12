using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Xml.Serialization;

namespace EveApi
{

    #region Sample XML
    /*
    <!-- Automatically generated data from EVE-Central.com -->
    <!-- This is the new API :-) -->
    <evec_api version="2.0" method="marketstat_xml">
      <marketstat>
        <type id="1137">
          <all>
            <volume>978.00</volume>
            <avg>2198366.36</avg>
            <max>900000000.01</max>
            <min>100.00</min>
            <stddev>43498632.76</stddev>
            <median>90000.00</median>
            <percentile>0.00</percentile>
          </all>
          <buy>
            <volume>20.00</volume>
            <avg>100.00</avg>
            <max>100.00</max>
            <min>100.00</min>
            <stddev>0.00</stddev>
            <median>0.00</median>
            <percentile>100.00</percentile>
          </buy>
          <sell>
            <volume>958.00</volume>
            <avg>1033170.49</avg>
            <max>900000000.01</max>
            <min>90000.00</min>
            <stddev>43549527.03</stddev>
            <median>90000.00</median>
            <percentile>90000.00</percentile>
          </sell>
        </type>
        <type id="438">
          <all>
            <volume>27896.00</volume>
            <avg>10078541.99</avg>
            <max>992500000.00</max>
            <min>1000.00</min>
            <stddev>92417892.66</stddev>
            <median>949999.98</median>
            <percentile>0.00</percentile>
          </all>
          <buy>
            <volume>5527.00</volume>
            <avg>413197.07</avg>
            <max>680000.00</max>
            <min>1000.00</min>
            <stddev>154691.01</stddev>
            <median>407913.00</median>
            <percentile>647967.54</percentile>
          </buy>
          <sell>
            <volume>22369.00</volume>
            <avg>1324507.74</avg>
            <max>992500000.00</max>
            <min>624346.98</min>
            <stddev>103818228.54</stddev>
            <median>817987.88</median>
            <percentile>624350.68</percentile>
          </sell>
        </type>
      </marketstat>
    </evec_api>
    */
    #endregion
    public static class EveCentralApi
    {
        public static EveCentralMarketStats GetAveragePrice(List<long> typeIDs)
        {
            if (typeIDs.Count < 0) return null;

            string url = "http://api.eve-central.com/api/marketstat?";

            for (int i = 0; i < typeIDs.Count; i++)
            {
                if (i > 0)
                    url += "&";

                url += "typeid=" + typeIDs[i];
            }

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

            XmlNodeReader reader = new XmlNodeReader(doc);

            EveCentralMarketStats stats = null;
            
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(EveCentralMarketStats));
                stats = (EveCentralMarketStats)xs.Deserialize(reader); 
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return stats;
        }
    }

    [XmlRoot("evec_api")]
    public class EveCentralMarketStats
    {
        [XmlArray("marketstat")]
        [XmlArrayItem("type")]
        public List<EveCentralItem> Items { get; set; }
    }

    public class EveCentralItem
    {
        [XmlAttribute("id")]
        public long ID { get; set; }

        [XmlElement("all")]
        public EveCentralItemData All { get; set; }

        [XmlElement("buy")]
        public EveCentralItemData Buy { get; set; }

        [XmlElement("sell")]
        public EveCentralItemData Sell { get; set; }
    }

    public class EveCentralItemData
    {
        [XmlElement("volume")]
        public float Volume { get; set; }

        [XmlElement("avg")]
        public float Average { get; set; }

        [XmlElement("max")]
        public float Max { get; set; }

        [XmlElement("min")]
        public float Min { get; set; }

        [XmlElement("stddev")]
        public float StdDeviation { get; set; }

        [XmlElement("median")]
        public float Median { get; set; }

        [XmlElement("percentile")]
        public float Percentile { get; set; }
    }
}
