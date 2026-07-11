using System;

namespace Epic.OnlineServices.Ecom
{
	public class GetTransactionCountOptions
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
