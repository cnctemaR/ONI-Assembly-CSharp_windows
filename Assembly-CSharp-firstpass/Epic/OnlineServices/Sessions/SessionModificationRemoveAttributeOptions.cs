using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationRemoveAttributeOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string Key { get; set; }
	}
}
