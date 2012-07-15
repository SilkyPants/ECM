using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECM.API.EVE;

namespace ECM
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KeyNeedsMaskAttribute : Attribute
    {
        public ApiKeyMask RequiredMask { get; set; }

        public KeyNeedsMaskAttribute(ApiKeyMask requiredMask)
        {
            RequiredMask = requiredMask;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NeedsApiKeyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NeedsCharacterIDAttribute : NeedsApiKeyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NeedsExtraInfoAttribute : NeedsApiKeyAttribute
    {
        //TODO: Something that requires this
        // Create new ApiRequestClass for it, have attribute store additional post items
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ApiUriAttribute : Attribute
    {
        public string ApiUri { get; private set; }

        public ApiUriAttribute(string uri)
        {
            ApiUri = uri;
        }
    }
}
