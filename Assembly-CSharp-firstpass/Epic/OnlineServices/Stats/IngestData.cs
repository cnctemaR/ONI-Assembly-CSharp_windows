using System;

namespace Epic.OnlineServices.Stats
{
	public class IngestData
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string StatName { get; set; }

		public int IngestAmount { get; set; }
	}
}
