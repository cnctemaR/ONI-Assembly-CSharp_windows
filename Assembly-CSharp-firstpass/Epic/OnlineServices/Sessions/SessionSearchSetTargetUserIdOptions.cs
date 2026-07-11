using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionSearchSetTargetUserIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId TargetUserId { get; set; }
	}
}
