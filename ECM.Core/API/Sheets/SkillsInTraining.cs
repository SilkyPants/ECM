using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ECM.API.EVE
{
    [NeedsCharacterID]
    [KeyNeedsMask(ApiKeyMask.SkillInTraining)]
	public class SkillInTraining
	{
        [XmlIgnore]
        public static string ApiUri { get { return "/char/SkillInTraining.xml.aspx"; } }
		
		[XmlElement("skillInTraining")]
		public int IsTrainingXML
		{
			get
			{
				return IsTraining ? 1 : 0;
			}
			set
			{
				IsTraining = value != 0;
			}
		}
		
		public bool IsTraining
		{
			get;
			set;
		}
		
		[XmlElement("trainingToLevel")]
		public int ToLevel
		{
			get;
			set;
		}
		
		[XmlElement("trainingTypeID")]
		public long TypeID
		{
			get;
			set;
		}
		
		[XmlElement("trainingStartSP")]
		public long StartSkillPoints
		{
			get;
			set;
		}
		
		[XmlElement("trainingDestinationSP")]
		public long EndSkillPoints
		{
			get;
			set;
		}
		
		[XmlElement("trainingEndTime")]
		public string EndTimeXML
		{
            get { return EndTime.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                EndTime = value.TimeStringToDateTime();
            }
		}
		
		[XmlIgnore]
		public DateTime EndTime
		{
			get;
			set;
		}
		
		[XmlElement("trainingStartTime")]
		public string StartTimeXML
		{
            get { return StartTime.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                StartTime = value.TimeStringToDateTime();
            }
		}
		
		[XmlIgnore]
		public DateTime StartTime
		{
			get;
			set;
		}
	}
	
	[NeedsCharacterID]
	public class SkillQueue
	{
        [XmlIgnore]
        public static string ApiUri { get { return "/char/SkillQueue.xml.aspx"; } }
		
		
		[XmlArray("queue")]
		[XmlArrayItem("skill")]
		public List<SkillQueueInfo> Queue
		{
			get;
			set;
		}
	}
	
	public class SkillQueueInfo
	{
		[XmlAttribute("queuePosition")]
		public int QueuePosition
		{
			get;
			set;
		}
		
		[XmlAttribute("typeID")]
		public int TypeID
		{
			get;
			set;
		}
		
		[XmlAttribute("level")]
		public int Level
		{
			get;
			set;
		}
		
		[XmlAttribute("startSP")]
		public int StartSkillPoints
		{
			get;
			set;
		}
		
		[XmlAttribute("endSP")]
		public int EndSkillPoints
		{
			get;
			set;
		}
		
		[XmlAttribute("startTime")]
		public string StartTimeXML
		{
            get { return StartTime.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                StartTime = value.TimeStringToDateTime();
            }
		}
		
		[XmlAttribute("endTime")]
		public string EndTimeXML
		{
            get { return EndTime.DateTimeToTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                EndTime = value.TimeStringToDateTime();
            }
		}
		
		[XmlIgnore]
		public DateTime StartTime
		{
			get;
			set;
		}
		
		[XmlIgnore]
		public DateTime EndTime
		{
			get;
			set;
		}
	}
}

