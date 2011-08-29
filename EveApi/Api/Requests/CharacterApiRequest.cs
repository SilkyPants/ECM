using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveApi
{
    public class CharacterApiRequest<T> : AuthorisedApiRequest<T>
    {
        public CharacterApiRequest(int characterID, string userID, string apiKey) : base (userID, apiKey)
        {
            postItems.Add("userID", userID);
            postItems.Add("apiKey", apiKey);
            postItems.Add("characterID", characterID.ToString());
        }
    }
}
