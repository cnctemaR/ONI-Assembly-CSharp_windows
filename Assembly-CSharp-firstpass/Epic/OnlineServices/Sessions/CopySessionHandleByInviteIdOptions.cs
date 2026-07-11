using System;

namespace Epic.OnlineServices.Sessions
{
	public class CopySessionHandleByInviteIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string InviteId { get; set; }
	}
}
