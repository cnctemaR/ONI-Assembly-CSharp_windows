using System;

namespace Epic.OnlineServices.TitleStorage
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
