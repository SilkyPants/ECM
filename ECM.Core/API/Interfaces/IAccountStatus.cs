using System;

namespace ECM.API.EVE
{
    public interface IAccountStatus
    {
        /// <summary>
        /// The number of times you logged into CCP's services (this includes not only the game logons but also forum logons, likely also EVEGate logons)
        /// </summary>
        int LogonCount
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of time you actually spent logged on in the game
        /// </summary>
         int LogonMinutes
        {
            get;
            set;
        }

        /// <summary>
        /// The date until which the account is currently subscribed
        /// </summary>
        DateTime PaidUntil
        {
            get;
            set;
        }

        /// <summary>
        /// The date the account was created
        /// </summary>
        DateTime CreateDate
        {
            get;
            set;
        }
    }
}

