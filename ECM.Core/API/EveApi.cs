using System;
using ECM.API.EVE;
using System.Collections.ObjectModel;

namespace ECM.API
{
    public static class EveApi
    {
        static Collection<IApiRequest> m_ApiRequests = new Collection<IApiRequest>();

        public static bool IsRetrieving
        {
            get 
            { 
                foreach (IApiRequest request in m_ApiRequests)
                {
                    if (request.IsUpdating)
                        return true;
                }

                return false;
            }
        }

        public static void AddRequest(IApiRequest request)
        {
            request.OnRequestUpdate += new RequestUpdated(OnRequestUpdated);

            m_ApiRequests.Add(request);
        }

        static void OnRequestUpdated(IApiRequest request)
        {
            //Remove requests that are flagged as one shot
            if (request.RemoveAfterUpdate)
                m_ApiRequests.Remove(request);
        }

        public static void UpdateOnHeartbeat()
        {
            if (!ECM.Core.IsLoaded) return;

            foreach (IApiRequest request in m_ApiRequests)
            {
                request.UpdateOnSecTick();
            }
        }
    }
}

