using System;

namespace Epic.OnlineServices.Ecom
{
	public class CheckoutOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public string OverrideCatalogNamespace { get; set; }

		public CheckoutEntry[] Entries { get; set; }
	}
}
