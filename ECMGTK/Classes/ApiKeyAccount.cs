using System;
using ECM.API.EVE;

namespace ECMGTK
{
	public class ApiKeyAccount
	{
		public ApiKeyData KeyInfo
		{
			get;
			set;
		}
		
		public string KeyID
		{
			get;
			set;
		}
		
		public string VerificationCode
		{
			get;
			set;
		}		
		
		public ApiKeyAccount ()
		{
		}
	}
}

