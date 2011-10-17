using System;

namespace ECM
{
    public static class Helper
    {
        public static string GetDurationInWords( TimeSpan aTimeSpan )
        {
            string timeTaken = string.Empty;
        
            if( aTimeSpan.Days > 0 )
                timeTaken += aTimeSpan.Days + " day" + ( aTimeSpan.Days > 1 ? "s" : "" );
        
            if( aTimeSpan.Hours > 0 )
            {
                if( !string.IsNullOrEmpty( timeTaken ) )
                   timeTaken += " ";
                timeTaken += aTimeSpan.Hours + " hour" + ( aTimeSpan.Hours > 1 ? "s" : "" );
            }
        
            if( aTimeSpan.Minutes > 0 )
            {
               if( !string.IsNullOrEmpty( timeTaken ) )
                   timeTaken += " ";
               timeTaken += aTimeSpan.Minutes + " minute" + ( aTimeSpan.Minutes > 1 ? "s" : "" );
            }
        
            if( aTimeSpan.Seconds > 0 )
            {
               if( !string.IsNullOrEmpty( timeTaken ) )
                   timeTaken += " ";
               timeTaken += aTimeSpan.Seconds + " second" + ( aTimeSpan.Seconds > 1 ? "s" : "" );
            }

            if (string.IsNullOrEmpty(timeTaken))
                timeTaken = string.Empty;
        
             return timeTaken;
        }

        public static string GetDurationInWordsShort(TimeSpan aTimeSpan)
        {
            string timeTaken = string.Empty;

            if (aTimeSpan.Days > 0)
                timeTaken += aTimeSpan.Days + "D";

            if (aTimeSpan.Hours > 0)
            {
                if (!string.IsNullOrEmpty(timeTaken))
                    timeTaken += " ";
                timeTaken += aTimeSpan.Hours + "H";
            }

            if (aTimeSpan.Minutes > 0)
            {
                if (!string.IsNullOrEmpty(timeTaken))
                    timeTaken += " ";
                timeTaken += aTimeSpan.Minutes + "M";
            }

            if (aTimeSpan.Seconds > 0)
            {
                if (!string.IsNullOrEmpty(timeTaken))
                    timeTaken += " ";
                timeTaken += aTimeSpan.Seconds + "S";
            }

            if (string.IsNullOrEmpty(timeTaken))
                timeTaken = string.Empty;

            return timeTaken;
        }
    }
}

