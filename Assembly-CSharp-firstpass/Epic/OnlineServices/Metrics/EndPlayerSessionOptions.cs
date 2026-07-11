using System;

namespace Epic.OnlineServices.Metrics
{
	public class EndPlayerSessionOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EndPlayerSessionOptionsAccountId AccountId { get; set; }
	}
}
