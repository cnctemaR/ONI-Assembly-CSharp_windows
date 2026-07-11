using System;

namespace Epic.OnlineServices.Auth
{
	public class PinGrantInfo
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public string UserCode { get; set; }

		public string VerificationURI { get; set; }

		public int ExpiresIn { get; set; }

		public string VerificationURIComplete { get; set; }
	}
}
