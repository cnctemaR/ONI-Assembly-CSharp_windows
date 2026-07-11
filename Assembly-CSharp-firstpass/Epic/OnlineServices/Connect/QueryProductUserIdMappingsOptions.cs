using System;

namespace Epic.OnlineServices.Connect
{
	public class QueryProductUserIdMappingsOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public ProductUserId LocalUserId { get; set; }

		public ExternalAccountType AccountIdType_DEPRECATED { get; set; }

		public ProductUserId[] ProductUserIds { get; set; }
	}
}
