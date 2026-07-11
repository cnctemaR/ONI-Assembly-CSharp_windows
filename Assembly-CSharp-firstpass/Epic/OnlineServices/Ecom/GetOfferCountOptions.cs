using System;

namespace Epic.OnlineServices.Ecom
{
	public class GetOfferCountOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }
	}
}
