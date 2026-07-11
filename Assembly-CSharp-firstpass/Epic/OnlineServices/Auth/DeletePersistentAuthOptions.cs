using System;

namespace Epic.OnlineServices.Auth
{
	public class DeletePersistentAuthOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public string RefreshToken { get; set; }
	}
}
