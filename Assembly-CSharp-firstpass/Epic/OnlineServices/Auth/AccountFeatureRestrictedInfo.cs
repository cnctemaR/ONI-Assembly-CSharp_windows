using System;

namespace Epic.OnlineServices.Auth
{
	public class AccountFeatureRestrictedInfo
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string VerificationURI { get; set; }
	}
}
