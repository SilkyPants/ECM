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
    }
}
