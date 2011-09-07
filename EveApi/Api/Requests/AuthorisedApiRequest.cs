using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveApi
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
