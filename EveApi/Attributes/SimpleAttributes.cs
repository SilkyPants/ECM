using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NeedsFullKeyAttribute : Attribute
    {
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
