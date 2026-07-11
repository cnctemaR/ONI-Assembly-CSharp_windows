using System;

namespace Epic.OnlineServices.Presence
{
	public class DataRecord
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string Key { get; set; }

		public string Value { get; set; }
	}
}
