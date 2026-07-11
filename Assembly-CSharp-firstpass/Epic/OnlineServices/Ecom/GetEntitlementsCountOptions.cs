using System;

namespace Epic.OnlineServices.Ecom
{
	public class GetEntitlementsCountOptions
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
