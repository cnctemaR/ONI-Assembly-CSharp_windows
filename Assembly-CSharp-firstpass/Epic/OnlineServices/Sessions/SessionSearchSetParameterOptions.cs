using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionSearchSetParameterOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public AttributeData Parameter { get; set; }

		public ComparisonOp ComparisonOp { get; set; }
	}
}
