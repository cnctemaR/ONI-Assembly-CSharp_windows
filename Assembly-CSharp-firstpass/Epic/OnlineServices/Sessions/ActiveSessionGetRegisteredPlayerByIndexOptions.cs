using System;

namespace Epic.OnlineServices.Sessions
{
	public class ActiveSessionGetRegisteredPlayerByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint PlayerIndex { get; set; }
	}
}
