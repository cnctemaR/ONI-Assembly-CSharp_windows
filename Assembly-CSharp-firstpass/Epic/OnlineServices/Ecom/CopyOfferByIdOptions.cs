using System;

namespace Epic.OnlineServices.Ecom
{
	public class CopyOfferByIdOptions
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
