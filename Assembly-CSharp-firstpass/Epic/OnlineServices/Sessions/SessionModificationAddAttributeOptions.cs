using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationAddAttributeOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public AttributeData SessionAttribute { get; set; }

		public SessionAttributeAdvertisementType AdvertisementType { get; set; }
	}
}
