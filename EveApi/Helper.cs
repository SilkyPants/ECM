using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveApi.Character
{
    /// <summary>
    /// This class contains helper functions to do things with the API that don't fit elsewhere
    /// </summary>
    public class Helper
    {
        public static bool IsKeyFull(string userID, string apiKey)
        {
            Account.AccountStatus status = new Account.AccountStatus();

            return status.GrabDataFromApi(new ProxyInfo());
        }
    }
}
