using System;

namespace Epic.OnlineServices.Ecom
{
	public class CopyItemImageInfoByIndexOptions
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

		public uint ImageInfoIndex { get; set; }
	}
}
