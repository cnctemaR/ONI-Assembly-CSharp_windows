using System;

namespace Epic.OnlineServices.Ecom
{
	public class CopyEntitlementByNameAndIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public string EntitlementName { get; set; }

		public uint Index { get; set; }
	}
}
