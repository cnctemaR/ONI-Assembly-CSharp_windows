using System;

namespace Epic.OnlineServices.Connect
{
	public class Credentials
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string Token { get; set; }

		public ExternalCredentialType Type { get; set; }
	}
}
