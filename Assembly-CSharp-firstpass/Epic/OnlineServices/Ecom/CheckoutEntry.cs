using System;

namespace Epic.OnlineServices.Ecom
{
	public class CheckoutEntry
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string OfferId { get; set; }
	}
}
