using System;

namespace Epic.OnlineServices.PlayerDataStorage
{
	public class FileMetadata
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint FileSizeBytes { get; set; }

		public string MD5Hash { get; set; }

		public string Filename { get; set; }
	}
}
