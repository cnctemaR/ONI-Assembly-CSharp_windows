using System;

namespace Epic.OnlineServices.Stats
{
	public class GetStatCountOptions
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
