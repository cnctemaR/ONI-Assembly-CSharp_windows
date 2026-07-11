using System;

namespace Epic.OnlineServices.Connect
{
	public class CopyProductUserExternalAccountByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId TargetUserId { get; set; }

		public uint ExternalAccountInfoIndex { get; set; }
	}
}
