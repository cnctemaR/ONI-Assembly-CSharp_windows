using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionSearchRemoveParameterOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string Key { get; set; }

		public ComparisonOp ComparisonOp { get; set; }
	}
}
