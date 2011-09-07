
using System.Xml.Serialization;
using System;
namespace EveApi
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
        /// <summary>
        /// None
        /// </summary>
        [XmlEnum("0")]
        None = 0,
        /// <summary>
        /// Wallet
        /// </summary>
        [XmlEnum("1")]
        Wallet = 1,
        /// <summary>
        /// Factory
        /// </summary>
        [XmlEnum("2")]
        Factory = 2,
        /// <summary>
        /// Hangar
        /// </summary>
        [XmlEnum("4")]
        Hangar = 4,
        /// <summary>
        /// Cargo
        /// </summary>
        [XmlEnum("5")]
        Cargo = 5,
        /// <summary>
        /// Briefcase
        /// </summary>
        [XmlEnum("6")]
        Briefcase = 6,
        /// <summary>
        /// Skill
        /// </summary>
        [XmlEnum("7")]
        Skill = 7,
        /// <summary>
        /// Reward
        /// </summary>
        [XmlEnum("8")]
        Reward = 8,
        /// <summary>
        /// Character in station connected
        /// </summary>
        [XmlEnum("9")]
        Connected = 9,
        /// <summary>
        /// Character in station offline
        /// </summary>
        [XmlEnum("10")]
        Disconnected = 10,
        /// <summary>
        /// Low power slot 1
        /// </summary>
        [XmlEnum("11")]
        LoSlot0 = 11,
        /// <summary>
        /// Low power slot 2
        /// </summary>
        [XmlEnum("12")]
        LoSlot1 = 12,
        /// <summary>
        /// Low power slot 3
        /// </summary>
        [XmlEnum("13")]
        LoSlot2 = 13,
        /// <summary>
        /// Low power slot 4
        /// </summary>
        [XmlEnum("14")]
        LoSlot3 = 14,
        /// <summary>
        /// Low power slot 5
        /// </summary>
        [XmlEnum("15")]
        LoSlot4 = 15,
        /// <summary>
        /// Low power slot 6
        /// </summary>
        [XmlEnum("16")]
        LoSlot5 = 16,
        /// <summary>
        /// Low power slot 7
        /// </summary>
        [XmlEnum("17")]
        LoSlot6 = 17,
        /// <summary>
        /// Low power slot 8
        /// </summary>
        [XmlEnum("18")]
        LoSlot7 = 18,
        /// <summary>
        /// Medium power slot 1
        /// </summary>
        [XmlEnum("19")]
        MedSlot0 = 19,
        /// <summary>
        /// Medium power slot 2
        /// </summary>
        [XmlEnum("20")]
        MedSlot1 = 20,
        /// <summary>
        /// Medium power slot 3
        /// </summary>
        [XmlEnum("21")]
        MedSlot2 = 21,
        /// <summary>
        /// Medium power slot 4
        /// </summary>
        [XmlEnum("22")]
        MedSlot3 = 22,
        /// <summary>
        /// Medium power slot 5
        /// </summary>
        [XmlEnum("23")]
        MedSlot4 = 23,
        /// <summary>
        /// Medium power slot 6
        /// </summary>
        [XmlEnum("24")]
        MedSlot5 = 24,
        /// <summary>
        /// Medium power slot 7
        /// </summary>
        [XmlEnum("25")]
        MedSlot6 = 25,
        /// <summary>
        /// Medium power slot 8
        /// </summary>
        [XmlEnum("26")]
        MedSlot7 = 26,
        /// <summary>
        /// High power slot 1
        /// </summary>
        [XmlEnum("27")]
        HiSlot0 = 27,
        /// <summary>
        /// High power slot 2
        /// </summary>
        [XmlEnum("28")]
        HiSlot1 = 28,
        /// <summary>
        /// High power slot 3
        /// </summary>
        [XmlEnum("29")]
        HiSlot2 = 29,
        /// <summary>
        /// High power slot 4
        /// </summary>
        [XmlEnum("30")]
        HiSlot3 = 30,
        /// <summary>
        /// High power slot 5
        /// </summary>
        [XmlEnum("31")]
        HiSlot4 = 31,
        /// <summary>
        /// High power slot 6
        /// </summary>
        [XmlEnum("32")]
        HiSlot5 = 32,
        /// <summary>
        /// High power slot 7
        /// </summary>
        [XmlEnum("33")]
        HiSlot6 = 33,
        /// <summary>
        /// High power slot 8
        /// </summary>
        [XmlEnum("34")]
        HiSlot7 = 34,
        /// <summary>
        /// Fixed slot
        /// </summary>
        [XmlEnum("35")]
        FixedSlot = 35,
        /// <summary>
        /// Promenade slot 1
        /// </summary>
        [XmlEnum("40")]
        PromenadeSlot1 = 40,
        /// <summary>
        /// Promenade slot 2
        /// </summary>
        [XmlEnum("41")]
        PromenadeSlot2 = 41,
        /// <summary>
        /// Promenade slot 3
        /// </summary>
        [XmlEnum("42")]
        PromenadeSlot3 = 42,
        /// <summary>
        /// Promenade slot 4
        /// </summary>
        [XmlEnum("43")]
        PromenadeSlot4 = 43,
        /// <summary>
        /// Promenade slot 5
        /// </summary>
        [XmlEnum("44")]
        PromenadeSlot5 = 44,
        /// <summary>
        /// Promenade slot 6
        /// </summary>
        [XmlEnum("45")]
        PromenadeSlot6 = 45,
        /// <summary>
        /// Promenade slot 7
        /// </summary>
        [XmlEnum("46")]
        PromenadeSlot7 = 46,
        /// <summary>
        /// Promenade slot 8
        /// </summary>
        [XmlEnum("47")]
        PromenadeSlot8 = 47,
        /// <summary>
        /// Promenade slot 9
        /// </summary>
        [XmlEnum("48")]
        PromenadeSlot9 = 48,
        /// <summary>
        /// Promenade slot 10
        /// </summary>
        [XmlEnum("49")]
        PromenadeSlot10 = 49,
        /// <summary>
        /// Promenade slot 11
        /// </summary>
        [XmlEnum("50")]
        PromenadeSlot11 = 50,
        /// <summary>
        /// Promenade slot 12
        /// </summary>
        [XmlEnum("51")]
        PromenadeSlot12 = 51,
        /// <summary>
        /// Promenade slot 13
        /// </summary>
        [XmlEnum("52")]
        PromenadeSlot13 = 52,
        /// <summary>
        /// Promenade slot 14
        /// </summary>
        [XmlEnum("53")]
        PromenadeSlot14 = 53,
        /// <summary>
        /// Promenade slot 15
        /// </summary>
        [XmlEnum("54")]
        PromenadeSlot15 = 54,
        /// <summary>
        /// Promenade slot 16
        /// </summary>
        [XmlEnum("55")]
        PromenadeSlot16 = 55,
        /// <summary>
        /// Capsule
        /// </summary>
        [XmlEnum("56")]
        Capsule = 56,
        /// <summary>
        /// Pilot
        /// </summary>
        [XmlEnum("57")]
        Pilot = 57,
        /// <summary>
        /// Passenger
        /// </summary>
        [XmlEnum("58")]
        Passenger = 58,
        /// <summary>
        /// Boarding Gate
        /// </summary>
        [XmlEnum("59")]
        BoardingGate = 59,
        /// <summary>
        /// Crew
        /// </summary>
        [XmlEnum("60")]
        Crew = 60,
        /// <summary>
        /// Skill In Training
        /// </summary>
        [XmlEnum("61")]
        SkillInTraining = 61,
        /// <summary>
        /// Corporation Market Deliveries / Returns
        /// </summary>
        [XmlEnum("62")]
        CorpMarket = 62,
        /// <summary>
        /// Locked item
        /// </summary>
        [XmlEnum("63")]
        Locked = 63,
        /// <summary>
        /// Unlocked item
        /// </summary>
        [XmlEnum("64")]
        Unlocked = 64,
        /// <summary>
        /// Office slot 1
        /// </summary>
        [XmlEnum("70")]
        OfficeSlot1 = 70,
        /// <summary>
        /// Office slot 2
        /// </summary>
        [XmlEnum("71")]
        OfficeSlot2 = 71,
        /// <summary>
        /// Office slot 3
        /// </summary>
        [XmlEnum("72")]
        OfficeSlot3 = 72,
        /// <summary>
        /// Office slot 4
        /// </summary>
        [XmlEnum("73")]
        OfficeSlot4 = 73,
        /// <summary>
        /// Office slot 5
        /// </summary>
        [XmlEnum("74")]
        OfficeSlot5 = 74,
        /// <summary>
        /// Office slot 6
        /// </summary>
        [XmlEnum("75")]
        OfficeSlot6 = 75,
        /// <summary>
        /// Office slot 7
        /// </summary>
        [XmlEnum("76")]
        OfficeSlot7 = 76,
        /// <summary>
        /// Office slot 8
        /// </summary>
        [XmlEnum("77")]
        OfficeSlot8 = 77,
        /// <summary>
        /// Office slot 9
        /// </summary>
        [XmlEnum("78")]
        OfficeSlot9 = 78,
        /// <summary>
        /// Office slot 10
        /// </summary>
        [XmlEnum("79")]
        OfficeSlot10 = 79,
        /// <summary>
        /// Office slot 11
        /// </summary>
        [XmlEnum("80")]
        OfficeSlot11 = 80,
        /// <summary>
        /// Office slot 12
        /// </summary>
        [XmlEnum("81")]
        OfficeSlot12 = 81,
        /// <summary>
        /// Office slot 13
        /// </summary>
        [XmlEnum("82")]
        OfficeSlot13 = 82,
        /// <summary>
        /// Office slot 14
        /// </summary>
        [XmlEnum("83")]
        OfficeSlot14 = 83,
        /// <summary>
        /// Office slot 15
        /// </summary>
        [XmlEnum("84")]
        OfficeSlot15 = 84,
        /// <summary>
        /// Office slot 16
        /// </summary>
        [XmlEnum("85")]
        OfficeSlot16 = 85,
        /// <summary>
        /// Bonus
        /// </summary>
        [XmlEnum("86")]
        Bonus = 86,
        /// <summary>
        /// Drone Bay
        /// </summary>
        [XmlEnum("87")]
        DroneBay = 87,
        /// <summary>
        /// Booster
        /// </summary>
        [XmlEnum("88")]
        Booster = 88,
        /// <summary>
        /// Implant
        /// </summary>
        [XmlEnum("89")]
        Implant = 89,
        /// <summary>
        /// Ship Hangar
        /// </summary>
        [XmlEnum("90")]
        ShipHangar = 90,
        /// <summary>
        /// Low power slot 4
        /// </summary>
        [XmlEnum("91")]
        ShipOffline = 91,
        /// <summary>
        /// Rig slot 1
        /// </summary>
        [XmlEnum("92")]
        RigSlot0 = 92,
        /// <summary>
        /// Rig slot 2
        /// </summary>
        [XmlEnum("93")]
        RigSlot1 = 93,
        /// <summary>
        /// Rig slot 3
        /// </summary>
        [XmlEnum("94")]
        RigSlot2 = 94,
        /// <summary>
        /// Rig slot 4
        /// </summary>
        [XmlEnum("95")]
        RigSlot3 = 95,
        /// <summary>
        /// Rig slot 5
        /// </summary>
        [XmlEnum("96")]
        RigSlot4 = 96,
        /// <summary>
        /// Rig slot 6
        /// </summary>
        [XmlEnum("97")]
        RigSlot5 = 97,
        /// <summary>
        /// Rig slot 7
        /// </summary>
        [XmlEnum("98")]
        RigSlot6 = 98,
        /// <summary>
        /// Rig slot 8
        /// </summary>
        [XmlEnum("99")]
        RigSlot7 = 99,
        /// <summary>
        /// Factory Background Operation
        /// </summary>
        [XmlEnum("100")]
        FactoryOperation = 100,
        /// <summary>
        /// Corp Security Access Group 2
        /// </summary>
        [XmlEnum("116")]
        CorpSAG2 = 116,
        /// <summary>
        /// Corp Security Access Group 3
        /// </summary>
        [XmlEnum("117")]
        CorpSAG3 = 117,
        /// <summary>
        /// Corp Security Access Group 4
        /// </summary>
        [XmlEnum("118")]
        CorpSAG4 = 118,
        /// <summary>
        /// Corp Security Access Group 5
        /// </summary>
        [XmlEnum("119")]
        CorpSAG5 = 119,
        /// <summary>
        /// Corp Security Access Group 6
        /// </summary>
        [XmlEnum("120")]
        CorpSAG6 = 120,
        /// <summary>
        /// Corp Security Access Group 7
        /// </summary>
        [XmlEnum("121")]
        CorpSAG7 = 121,
        /// <summary>
        /// Secondary Storage
        /// </summary>
        [XmlEnum("122")]
        SecondaryStorage = 122,
        /// <summary>
        /// Captains Quarters
        /// </summary>
        [XmlEnum("123")]
        CaptainsQuarters = 123,
        /// <summary>
        /// Wis Promenade
        /// </summary>
        [XmlEnum("124")]
        WisPromenade = 124,
        /// <summary>
        /// Sub System slot 1
        /// </summary>
        [XmlEnum("125")]
        SubSystem0 = 125,
        /// <summary>
        /// Sub System slot 2
        /// </summary>
        [XmlEnum("126")]
        SubSystem1 = 126,
        /// <summary>
        /// Sub System slot 3
        /// </summary>
        [XmlEnum("127")]
        SubSystem2 = 127,
        /// <summary>
        /// Sub System slot 4
        /// </summary>
        [XmlEnum("128")]
        SubSystem3 = 128,
        /// <summary>
        /// Sub System slot 5
        /// </summary>
        [XmlEnum("129")]
        SubSystem4 = 129,
        /// <summary>
        /// Sub System slot 6
        /// </summary>
        [XmlEnum("130")]
        SubSystem5 = 130,
        /// <summary>
        /// Sub System slot 7
        /// </summary>
        [XmlEnum("131")]
        SubSystem6 = 131,
        /// <summary>
        /// Sub System slot 8
        /// </summary>
        [XmlEnum("132")]
        SubSystem7 = 132,
        /// <summary>
        /// Specialized Fuel Bay
        /// </summary>
        [XmlEnum("133")]
        SpecializedFuelBay = 133,
        /// <summary>
        /// Specialized Ore Hold
        /// </summary>
        [XmlEnum("134")]
        SpecializedOreHold = 134,
        /// <summary>
        /// Specialized Gas Hold
        /// </summary>
        [XmlEnum("135")]
        SpecializedGasHold = 135,
        /// <summary>
        /// Specialized Mineral Hold
        /// </summary>
        [XmlEnum("136")]
        SpecializedMineralHold = 136,
        /// <summary>
        /// SpecializedSalvageHold
        /// </summary>
        [XmlEnum("137")]
        SpecializedSalvageHold = 137,
        /// <summary>
        /// Specialized Ship Hold
        /// </summary>
        [XmlEnum("138")]
        SpecializedShipHold = 138,
        /// <summary>
        /// Specialized Small Ship Hold
        /// </summary>
        [XmlEnum("139")]
        SpecializedSmallShipHold = 139,
        /// <summary>
        /// Specialized Medium Ship Hold
        /// </summary>
        [XmlEnum("140")]
        SpecializedMediumShipHold = 140,
        /// <summary>
        /// Specialized Large Ship Hold
        /// </summary>
        [XmlEnum("141")]
        SpecializedLargeShipHold = 141,
        /// <summary>
        /// Specialized Industrial Ship Hold
        /// </summary>
        [XmlEnum("142")]
        SpecializedIndustrialShipHold = 142,
        /// <summary>
        /// Specialized Ammo Hold
        /// </summary>
        [XmlEnum("143")]
        SpecializedAmmoHold = 143,
        /// <summary>
        /// Structure Active
        /// </summary>
        [XmlEnum("144")]
        StructureActive = 144,
        /// <summary>
        /// Structure Inactive
        /// </summary>
        [XmlEnum("145")]
        StructureInactive = 145,
        /// <summary>
        /// This item was put into a junkyard through reprocession.
        /// </summary>
        [XmlEnum("146")]
        JunkyardReprocessed = 146,
        /// <summary>
        /// This item was put into a junkyard through being trashed by its owner.
        /// </summary>
        [XmlEnum("147")]
        JunkyardTrashed = 147,
    }
    
    /// <summary>
    /// Defines the different types of agents. This is to help cut out redundant information from the databases
    /// </summary>
    public enum AgentTypes
    {
        NonAgent = 1,
        BasicAgent = 2,
        TutorialAgent = 3,
        ResearchAgent = 4,
        CONCORDAgent = 5,
        GenericStorylineMissionAgent = 6,
        StorylineMissionAgent = 7,
        EventMissionAgent = 8,
        FactionalWarfareAgent = 9,
        EpicArcAgent = 10,
        AuraAgent = 11
    }
    
    public enum AgentDivisions
    {
        Accounting = 1,
        Administration,
        Advisory,
        Archives,
        Astrosurveying,
        Command,
        Distribution,
        Financial,
        Intelligence,
        Internal_Security,
        Legal,
        Manufacturing,
        Marketing,
        Mining,
        Personnel,
        Production,
        Public_Relations,
        RnD,
        Security,
        Storage,
        Surveillance,
        NewDistribution,
        NewMining,
        NewSecurity
    }
    
    public enum CorpActivities
    {
        Agriculture = 1,
        Construction = 2,
        Mining = 3,
        Chemical = 4,
        Military = 5,
        Biotech = 6,
        HiTech = 7,
        Entertainment = 8,
        Shipyard = 9,
        Warehouse = 10,
        Retail = 11,
        Trading = 12,
        Bureaucratic = 13,
        Political = 14,
        Legal = 15,
        Security = 16,
        Financial = 17,
        Education = 18,
        Manufacture = 19,
        Disputed = 20
    }
    
    public enum ItemMetaGroups
    {
        Tech1 = 1,
        Tech2 = 2,
        Storyline = 3,
        Faction = 4,
        Officer = 5,
        Deadspace = 6,
        Frigates = 7,
        EliteFrigates = 8,
        CommanderFrigates = 9,
        Destroyer = 10,
        Cruiser = 11,
        EliteCruiser = 12,
        CommanderCruiser = 13,
        Tech3 = 14
    }

    public enum ItemAttributeCategories
    {
        /// <summary>
        /// Fitting capabilities of a ship
        /// </summary>
        Fitting = 1,
        /// <summary>
        /// Shield attributes of ships
        /// </summary>
        Shield = 2,
        /// <summary>
        /// Armor attributes of ships
        /// </summary>
        Armor = 3,
        /// <summary>
        /// Structure attributes of ships
        /// </summary>
        Structure = 4,
        /// <summary>
        /// Capacitor attributes for ships
        /// </summary>
        Capacitor = 5,
        /// <summary>
        /// Targeting Attributes for ships
        /// </summary>
        Targeting = 6,
        /// <summary>
        /// Misc. attributes
        /// </summary>
        Miscellaneous = 7,
        /// <summary>
        /// Skill requirements
        /// </summary>
        RequiredSkills = 8,
        /// <summary>
        /// Attributes already checked and not going into a category
        /// </summary>
        Other = 9,
        /// <summary>
        /// All you need to know about drones
        /// </summary>
        Drones = 10,
        /// <summary>
        /// Attribs for the AI configuration
        /// </summary>
        AI = 12
    }

    [Flags]
    public enum StationServices
    {
        BountyMissions = 1 << 0,
        AssassinationMissions = 1 << 1,
        CourierMissions = 1 << 2,
        Interbus = 1 << 3,
        ReprocessingPlant = 1 << 4,
        Refinery = 1 << 5,
        Market = 1 << 6,
        BlackMarket = 1 << 7,
        StockExchange = 1 << 8,
        Cloning = 1 << 9,
        Surgery = 1 << 10,
        DNATherapy = 1 << 11,
        RepairFacilities = 1 << 12,
        Factory = 1 << 13,
        Laboratory = 1 << 14,
        Gambling = 1 << 15,
        Fitting = 1 << 16,
        Paintshop = 1 << 17,
        News = 1 << 18,
        Storage = 1 << 19,
        Insurance = 1 << 20,
        Docking = 1 << 21,
        OfficeRental = 1 << 22,
        JumpCloneFacility = 1 << 23,
        LoyaltyPointStore = 1 << 24,
        NavyOffices = 1 << 25,
    }
    

    [Flags]
    public enum ApiKeyMask
    {
        AccountBalance          = 1 << 0,
        AssetList               = 1 << 1,
        CalendarEventAttendees  = 1 << 2,
        CharacterSheet          = 1 << 3,
        ContactList             = 1 << 4,
        ContactNotifications    = 1 << 5,
        FacWarStats             = 1 << 6,
        IndustryJobs            = 1 << 7,
        KillLog                 = 1 << 8,
        MailBodies              = 1 << 9,
        MailingLists            = 1 << 10,
        MailMessages            = 1 << 11,
        MarketOrders            = 1 << 12,
        Medals                  = 1 << 13,
        Notifications           = 1 << 14,
        NotificationTexts       = 1 << 15,
        Research                = 1 << 16,
        SkillInTraining         = 1 << 17,
        SkillQueue              = 1 << 18,
        Standings               = 1 << 19,
        UpcomingCalendarEvents  = 1 << 20,
        WalletJournal           = 1 << 21,
        WalletTransactions      = 1 << 22,
        CharacterInfoPublic     = 1 << 23,
        CharacterInfoPrivate    = 1 << 24,
        AccountStatus           = 1 << 25,
        Contracts               = 1 << 26,
        Everything              = AccountBalance | AssetList | CalendarEventAttendees | CharacterSheet | ContactList | ContactNotifications |
                                FacWarStats | IndustryJobs | KillLog | MailBodies | MailMessages | MailingLists | MarketOrders | Medals | 
                                Notifications | NotificationTexts | Research | SkillInTraining | SkillQueue | Standings | UpcomingCalendarEvents | 
                                WalletJournal | WalletTransactions | CharacterInfoPublic | CharacterInfoPrivate | AccountStatus | Contracts,
    }

    public enum ApiKeyType
    {
        Account,
        Character,
        Corporation
    }
}
