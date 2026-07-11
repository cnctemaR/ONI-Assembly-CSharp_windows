using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetJoinInProgressAllowedOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public bool AllowJoinInProgress { get; set; }
	}
}
