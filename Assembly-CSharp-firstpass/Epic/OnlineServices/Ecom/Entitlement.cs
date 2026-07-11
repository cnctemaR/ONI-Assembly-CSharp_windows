using System;

namespace Epic.OnlineServices.Ecom
{
	public class Entitlement
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public string EntitlementName { get; set; }

		public string EntitlementId { get; set; }

		public string CatalogItemId { get; set; }

		public int ServerIndex { get; set; }

		public bool Redeemed { get; set; }

		public long EndTimestamp { get; set; }
	}
}
