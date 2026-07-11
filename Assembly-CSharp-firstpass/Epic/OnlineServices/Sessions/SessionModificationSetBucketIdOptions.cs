using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetBucketIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string BucketId { get; set; }
	}
}
