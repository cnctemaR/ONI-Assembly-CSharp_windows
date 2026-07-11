using System;

namespace Epic.OnlineServices.Ecom
{
	public class CatalogRelease
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string[] CompatibleAppIds { get; set; }

		public string[] CompatiblePlatforms { get; set; }

		public string ReleaseNote { get; set; }
	}
}
