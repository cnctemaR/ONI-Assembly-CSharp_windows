using System;

namespace Epic.OnlineServices.Sessions
{
	public class RejectInviteOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId LocalUserId { get; set; }

		public string InviteId { get; set; }
	}
}
