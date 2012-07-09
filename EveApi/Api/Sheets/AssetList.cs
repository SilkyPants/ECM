using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EveApi.Attributes;
using System.Runtime.Serialization.Formatters.Binary;

namespace EveApi
{
    [NeedsCharacterID]
    [KeyNeedsMask(ApiKeyMask.AssetList)]
    public class AssetList
    {
        [XmlIgnore]
        public static string ApiUri { get { return "/char/AssetList.xml.aspx"; } }

        [XmlArray("assets")]
        [XmlArrayItem("asset")]
        public List<AssetListInfo> Assets { get; set; }
    }

    public class AssetListInfo : ContentInfo
    {
        [XmlAttribute("locationID")]
        public long LocationID { get; set; }

        [XmlArray("contents")]
        [XmlArrayItem("content")]
        public List<ContentInfo> Contents { get; set; }
    }

    public class ContentInfo
    {
        [XmlAttribute("itemID")]
        public long ItemID { get; set; }

        [XmlAttribute("typeID")]
        public long TypeID { get; set; }

        [XmlAttribute("quantity")]
        public int Quantity { get; set; }

        [XmlAttribute("flag")]
        public InventoryFlags Flag { get; set; }

        [XmlAttribute("singleton")]
        public int Singleton { get; set; }
		
		///
		///Items in the AssetList and ContractItems will now include a rawQuantity attribute if the quantity in the database is negative. 
		///Negative quantities are in fact codes: -1 is a singleton item and -2 is a blueprint copy.
		///
        [XmlAttribute("rawQuantity")]
        public int rawQuantity { get; set; }
    }
}
