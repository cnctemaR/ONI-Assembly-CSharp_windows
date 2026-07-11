using System;

namespace Epic.OnlineServices.Auth
{
	public class VerifyUserAuthOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public Token AuthToken { get; set; }
	}
}
