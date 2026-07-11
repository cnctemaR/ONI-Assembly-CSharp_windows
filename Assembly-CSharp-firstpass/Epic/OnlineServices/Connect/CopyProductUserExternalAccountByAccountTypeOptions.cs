using System;

namespace Epic.OnlineServices.Connect
{
	public class CopyProductUserExternalAccountByAccountTypeOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId TargetUserId { get; set; }

		public ExternalAccountType AccountIdType { get; set; }
	}
}
