using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionDetailsCopySessionAttributeByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint AttrIndex { get; set; }
	}
}
