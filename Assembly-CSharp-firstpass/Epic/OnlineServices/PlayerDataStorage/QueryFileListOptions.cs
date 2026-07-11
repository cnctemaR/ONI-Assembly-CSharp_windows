using System;

namespace Epic.OnlineServices.PlayerDataStorage
{
	public class QueryFileListOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId LocalUserId { get; set; }
	}
}
