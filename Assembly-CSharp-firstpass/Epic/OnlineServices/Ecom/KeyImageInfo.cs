using System;

namespace Epic.OnlineServices.Ecom
{
	public class KeyImageInfo
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string Type { get; set; }

		public string Url { get; set; }

		public uint Width { get; set; }

		public uint Height { get; set; }
	}
}
