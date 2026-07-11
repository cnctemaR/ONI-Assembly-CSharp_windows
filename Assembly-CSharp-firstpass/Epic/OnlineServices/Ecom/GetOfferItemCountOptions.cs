using System;

namespace Epic.OnlineServices.Ecom
{
	public class GetOfferItemCountOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public string OfferId { get; set; }
	}
}
