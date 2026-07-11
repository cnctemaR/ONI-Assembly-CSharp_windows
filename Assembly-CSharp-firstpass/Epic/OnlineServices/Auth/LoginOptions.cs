using System;

namespace Epic.OnlineServices.Auth
{
	public class LoginOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public Credentials Credentials { get; set; }

		public AuthScopeFlags ScopeFlags { get; set; }
	}
}
