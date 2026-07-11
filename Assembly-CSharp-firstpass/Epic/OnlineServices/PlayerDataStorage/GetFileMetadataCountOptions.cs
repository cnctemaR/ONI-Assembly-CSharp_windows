using System;

namespace Epic.OnlineServices.PlayerDataStorage
{
	public class GetFileMetadataCountOptions
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
