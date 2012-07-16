using System;
using System.Collections.Generic;
using System.Text;

namespace ECM.API.EVE
{
    public interface IApiRequest
    {
        /// <summary>
        /// Is the request enabled for updating?
        /// </summary>
		bool Enabled {get;set;}

        /// <summary>
        /// Should this request get removed after updating?
        /// </summary>
        bool RemoveAfterUpdate { get; set; }

        /// <summary>
        /// Gets the last time this instance was updated (UTC).
        /// </summary>
        DateTime LastUpdate { get; }

        /// <summary>
        /// Gets the next time this instance should be updated (UTC), based on both the CCP cache time and the user preferences.
        /// </summary>
        DateTime NextUpdate { get; }

        /// <summary>
        /// Gets true whether the method is curently being requeried.
        /// </summary>
        bool IsUpdating { get; }

        /// <summary>
        /// Gets the last API result.
        /// </summary>
        IApiResult LastResult { get; }

        event RequestUpdated OnRequestUpdate;

        void UpdateOnSecTick();
    }
}
