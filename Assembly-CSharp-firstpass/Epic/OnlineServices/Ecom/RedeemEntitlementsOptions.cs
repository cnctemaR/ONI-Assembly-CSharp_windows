using System;

namespace Epic.OnlineServices.Ecom
{
	public class RedeemEntitlementsOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public string[] EntitlementIds { get; set; }
	}
}
