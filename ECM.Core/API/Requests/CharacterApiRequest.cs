using System;
using System.Collections.Generic;
using System.Text;

namespace ECM.API.EVE
{
    public class CharacterApiRequest<T> : AuthorisedApiRequest<T>
        where T : class
    {
        public CharacterApiRequest(long characterID, string userID, string apiKey) : base (userID, apiKey)
        {
            postItems.Add("characterID", characterID.ToString());
        }
    }
}
