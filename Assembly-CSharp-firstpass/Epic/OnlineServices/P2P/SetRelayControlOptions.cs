using System;

namespace Epic.OnlineServices.P2P
{
	public class SetRelayControlOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public RelayControl RelayControl { get; set; }
	}
}
