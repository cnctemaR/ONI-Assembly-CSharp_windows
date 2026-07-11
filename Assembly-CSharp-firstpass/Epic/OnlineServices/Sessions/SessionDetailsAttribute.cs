using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionDetailsAttribute
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public AttributeData Data { get; set; }

		public SessionAttributeAdvertisementType AdvertisementType { get; set; }
	}
}
