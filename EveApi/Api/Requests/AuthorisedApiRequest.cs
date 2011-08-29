using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveApi
{
    public class AuthorisedApiRequest<T> : ApiRequest<T>
        where T : class
    {
        public AuthorisedApiRequest(string userID, string apiKey)
        {
            postItems.Add("userID", userID);
            postItems.Add("apiKey", apiKey);
        }
    }
}
