using System;

namespace Epic.OnlineServices.P2P
{
	public class SetPortRangeOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ushort Port { get; set; }

		public ushort MaxAdditionalPortsToTry { get; set; }
	}
}
