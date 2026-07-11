using System;

namespace Epic.OnlineServices.Stats
{
	public class CopyStatByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId TargetUserId { get; set; }

		public uint StatIndex { get; set; }
	}
}
