using System;

namespace Epic.OnlineServices.Stats
{
	public class CopyStatByNameOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId TargetUserId { get; set; }

		public string Name { get; set; }
	}
}
