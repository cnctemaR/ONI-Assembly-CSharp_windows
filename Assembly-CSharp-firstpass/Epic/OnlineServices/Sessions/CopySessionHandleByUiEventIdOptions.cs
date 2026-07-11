using System;

namespace Epic.OnlineServices.Sessions
{
	public class CopySessionHandleByUiEventIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ulong UiEventId { get; set; }
	}
}
