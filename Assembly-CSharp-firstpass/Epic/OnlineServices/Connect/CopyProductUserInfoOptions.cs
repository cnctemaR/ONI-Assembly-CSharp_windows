using System;

namespace Epic.OnlineServices.Connect
{
	public class CopyProductUserInfoOptions
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
