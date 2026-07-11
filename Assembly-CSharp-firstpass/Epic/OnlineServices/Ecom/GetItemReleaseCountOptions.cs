using System;

namespace Epic.OnlineServices.Ecom
{
	public class GetItemReleaseCountOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public string ItemId { get; set; }
	}
}
