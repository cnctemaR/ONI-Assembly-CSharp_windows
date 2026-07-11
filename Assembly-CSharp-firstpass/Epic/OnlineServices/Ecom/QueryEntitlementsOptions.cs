using System;

namespace Epic.OnlineServices.Ecom
{
	public class QueryEntitlementsOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public string[] EntitlementNames { get; set; }

		public bool IncludeRedeemed { get; set; }
	}
}
