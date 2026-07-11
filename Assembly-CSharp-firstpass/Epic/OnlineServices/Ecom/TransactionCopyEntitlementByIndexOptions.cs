using System;

namespace Epic.OnlineServices.Ecom
{
	public class TransactionCopyEntitlementByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint EntitlementIndex { get; set; }
	}
}
