using System;

namespace Epic.OnlineServices.Connect
{
	public class GetProductUserExternalAccountCountOptions
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
