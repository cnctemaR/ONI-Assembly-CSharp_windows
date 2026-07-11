using System;

namespace Epic.OnlineServices.Connect
{
	public class TransferDeviceIdAccountOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId PrimaryLocalUserId { get; set; }

		public ProductUserId LocalDeviceUserId { get; set; }

		public ProductUserId ProductUserIdToPreserve { get; set; }
	}
}
