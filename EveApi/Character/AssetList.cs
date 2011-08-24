using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EveApi.Character
{
    public class AssetList : CharacterApiBase
    {
        /// <summary>
        /// EVE stores items in a location with a value we call the "flag." 
        /// This value is used to indicate more data about the item's location. 
        /// For example, instead of having different locations for each of the modules, we instead have the single location (the ship) with many flags. 
        /// The flags indicate if the item is in cargo or fitted, and if fitted, to which slot.
        /// 
        /// http://wiki.eve-id.net/API_Inventory_Flags
        /// </summary>
        public enum InventoryFlags
        {
            None=0,                             //None
            Wallet=1,                           //Wallet
            Factory=2,                          //Factory
            Hangar=4,                           //Hangar
            Cargo=5,                            //Cargo
            Briefcase=6,                        //Briefcase
            Skill=7,                            //Skill
            Reward=8,                           //Reward
            Connected=9,                        //Character in station connected
            Disconnected=10,                    //Character in station offline
            LoSlot0=11,                         //Low power slot 1
            LoSlot1=12,                         //Low power slot 2
            LoSlot2=13,                         //Low power slot 3
            LoSlot3=14,                         //Low power slot 4
            LoSlot4=15,                         //Low power slot 5
            LoSlot5=16,                         //Low power slot 6
            LoSlot6=17,                         //Low power slot 7
            LoSlot7=18,                         //Low power slot 8
            MedSlot0=19,                        //Medium power slot 1
            MedSlot1=20,                        //Medium power slot 2
            MedSlot2=21,                        //Medium power slot 3
            MedSlot3=22,                        //Medium power slot 4
            MedSlot4=23,                        //Medium power slot 5
            MedSlot5=24,                        //Medium power slot 6
            MedSlot6=25,                        //Medium power slot 7
            MedSlot7=26,                        //Medium power slot 8
            HiSlot0=27,                         //High power slot 1
            HiSlot1=28,                         //High power slot 2
            HiSlot2=29,                         //High power slot 3
            HiSlot3=30,                         //High power slot 4
            HiSlot4=31,                         //High power slot 5
            HiSlot5=32,                         //High power slot 6
            HiSlot6=33,                         //High power slot 7
            HiSlot7=34,                         //High power slot 8
            FixedSlot=35,                       //Fixed Slot
            PromenadeSlot1=40,                  //Promenade Slot 1
            PromenadeSlot2=41,                  //Promenade Slot 2
            PromenadeSlot3=42,                  //Promenade Slot 3
            PromenadeSlot4=43,                  //Promenade Slot 4
            PromenadeSlot5=44,                  //Promenade Slot 5
            PromenadeSlot6=45,                  //Promenade Slot 6
            PromenadeSlot7=46,                  //Promenade Slot 7
            PromenadeSlot8=47,                  //Promenade Slot 8
            PromenadeSlot9=48,                  //Promenade Slot 9
            PromenadeSlot10=49,                 //Promenade Slot 10
            PromenadeSlot11=50,                 //Promenade Slot 11
            PromenadeSlot12=51,                 //Promenade Slot 12
            PromenadeSlot13=52,                 //Promenade Slot 13
            PromenadeSlot14=53,                 //Promenade Slot 14
            PromenadeSlot15=54,                 //Promenade Slot 15
            PromenadeSlot16=55,                 //Promenade Slot 16
            Capsule=56,                         //Capsule
            Pilot=57,                           //Pilot
            Passenger=58,                       //Passenger
            BoardingGate=59,                    //Boarding gate
            Crew=60,                            //Crew
            SkillInTraining=61,                 //Skill in training
            CorpMarket=62,                      //Corporation Market Deliveries / Returns
            Locked=63,                          //Locked item
            Unlocked=64,                        //Unlocked item
            OfficeSlot1=70,                     //Office slot 1
            OfficeSlot2=71,                     //Office slot 2
            OfficeSlot3=72,                     //Office slot 3
            OfficeSlot4=73,                     //Office slot 4
            OfficeSlot5=74,                     //Office slot 5
            OfficeSlot6=75,                     //Office slot 6
            OfficeSlot7=76,                     //Office slot 7
            OfficeSlot8=77,                     //Office slot 8
            OfficeSlot9=78,                     //Office slot 9
            OfficeSlot10=79,                    //Office slot 10
            OfficeSlot11=80,                    //Office slot 11
            OfficeSlot12=81,                    //Office slot 12
            OfficeSlot13=82,                    //Office slot 13
            OfficeSlot14=83,                    //Office slot 14
            OfficeSlot15=84,                    //Office slot 15
            OfficeSlot16=85,                    //Office slot 16
            Bonus=86,                           //Bonus
            DroneBay=87,                        //Drone Bay
            Booster=88,                         //Booster
            Implant=89,                         //Implant
            ShipHangar=90,                      //Ship Hangar
            ShipOffline=91,                     //Ship Offline
            RigSlot0=92,                        //Rig power slot 1
            RigSlot1=93,                        //Rig power slot 2
            RigSlot2=94,                        //Rig power slot 3
            RigSlot3=95,                        //Rig power slot 4
            RigSlot4=96,                        //Rig power slot 5
            RigSlot5=97,                        //Rig power slot 6
            RigSlot6=98,                        //Rig power slot 7
            RigSlot7=99,                        //Rig power slot 8
            FactoryOperation=100,               //Factory Background Operation
            CorpSAG2=116,                       //Corp Security Access Group 2
            CorpSAG3=117,                       //Corp Security Access Group 3
            CorpSAG4=118,                       //Corp Security Access Group 4
            CorpSAG5=119,                       //Corp Security Access Group 5
            CorpSAG6=120,                       //Corp Security Access Group 6
            CorpSAG7=121,                       //Corp Security Access Group 7
            SecondaryStorage=122,               //Secondary Storage
            CaptainsQuarters=123,               //Captains Quarters
            WisPromenade=124,                   //Wis Promenade
            SubSystem0=125,                     //Sub system slot 0
            SubSystem1=126,                     //Sub system slot 1
            SubSystem2=127,                     //Sub system slot 2
            SubSystem3=128,                     //Sub system slot 3
            SubSystem4=129,                     //Sub system slot 4
            SubSystem5=130,                     //Sub system slot 5
            SubSystem6=131,                     //Sub system slot 6
            SubSystem7=132,                     //Sub system slot 7
            SpecializedFuelBay=133,             //Specialized Fuel Bay
            SpecializedOreHold=134,             //Specialized Ore Hold
            SpecializedGasHold=135,             //Specialized Gas Hold
            SpecializedMineralHold=136,         //Specialized Mineral Hold
            SpecializedSalvageHold=137,         //Specialized Salvage Hold
            SpecializedShipHold=138,            //Specialized Ship Hold
            SpecializedSmallShipHold=139,       //Specialized Small Ship Hold
            SpecializedMediumShipHold=140,      //Specialized Medium Ship Hold
            SpecializedLargeShipHold=141,       //Specialized Large Ship Hold
            SpecializedIndustrialShipHold=142,  //Specialized Industrial Ship Hold
            SpecializedAmmoHold=143,            //Specialized Ammo Hold
            StructureActive=144,                //StructureActive
            StructureInactive=145,              //StructureInactive
            JunkyardReprocessed=146,            //This item was put into a junkyard through reprocession.
            JunkyardTrashed=147,                //This item was put into a junkyard through being trashed by its owner.
        }

        public class AssetInfo
        {
            public List<AssetInfo> Contents = new List<AssetInfo>();
            public int TypeID;
            public Int64 LocationID;
            public int Quantity;
            public InventoryFlags Flags;
            public bool IsPackaged;
        }

        public List<AssetInfo> m_Assets = new List<AssetInfo>();
        public IList<AssetInfo> Assets
        {
            get { return m_Assets.AsReadOnly(); }
        }

        public override string ApiUri
        {
            get { return "/char/AssetList.xml.aspx"; }
        }

        public override bool ParseData()
        {
            ApiRawDocument.Save("AssetList.xml");
            using (XmlReader reader = ApiRawDocument.CreateNavigator().ReadSubtree())
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("rowset") && reader.GetAttribute("name").Equals("assets"))
                        {
                            using(XmlReader assetsReader = reader.ReadSubtree())
                            {
                                ParseItemContents(assetsReader, ref m_Assets);
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void ParseItemContents(XmlReader reader, ref List<AssetInfo> assetList)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.Equals("row"))
                    {
                        AssetInfo newAsset = new AssetInfo();

                        newAsset.TypeID = int.Parse(reader.GetAttribute("typeID"));
                        newAsset.Flags = (InventoryFlags)Enum.Parse(typeof(InventoryFlags), reader.GetAttribute("flag"));
                        newAsset.IsPackaged = reader.GetAttribute("singleton").Equals("1") ? false : true;

                        string locationID = reader.GetAttribute("locationID");
                        
                        if(locationID != null)
                            newAsset.LocationID = Int64.Parse(locationID);

                        if(reader.IsEmptyElement == false)
                        {
                            using (XmlReader assetsReader = reader.ReadSubtree())
                            {
                                // Need to read passed the current
                                assetsReader.Read();

                                // Read item contents
                                ParseItemContents(assetsReader, ref newAsset.Contents);
                            }
                        }

                        assetList.Add(newAsset);
                    }
                }
            }
        }
    }
}
