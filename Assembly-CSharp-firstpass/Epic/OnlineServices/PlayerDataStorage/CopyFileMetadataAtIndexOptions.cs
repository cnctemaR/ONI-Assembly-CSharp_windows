using System;

namespace Epic.OnlineServices.PlayerDataStorage
{
	public class CopyFileMetadataAtIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId LocalUserId { get; set; }

		public uint Index { get; set; }
	}
}
