using System;

namespace Epic.OnlineServices.Ecom
{
	public class CopyTransactionByIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public string TransactionId { get; set; }
	}
}
