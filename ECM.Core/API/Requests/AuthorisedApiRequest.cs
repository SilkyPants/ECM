using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECM.API.EVE
{
    public class AuthorisedApiRequest<T> : ApiRequest<T>
        where T : class
    {
        public AuthorisedApiRequest(string keyID, string vCode)
        {
            postItems.Add("keyID", keyID);
            postItems.Add("vCode", vCode);
        }
    }
}
