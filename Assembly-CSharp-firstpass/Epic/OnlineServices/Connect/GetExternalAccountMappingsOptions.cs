using System;

namespace Epic.OnlineServices.Connect
{
	public class GetExternalAccountMappingsOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId LocalUserId { get; set; }

		public ExternalAccountType AccountIdType { get; set; }

		public string TargetExternalUserId { get; set; }
	}
}
