using System;

namespace Epic.OnlineServices.Ecom
{
	public class GetEntitlementsByNameCountOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public string EntitlementName { get; set; }
	}
}
