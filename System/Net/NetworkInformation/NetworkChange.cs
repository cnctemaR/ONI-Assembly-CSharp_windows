using System;

namespace System.Net.NetworkInformation
{
	public sealed class NetworkChange
	{
		private NetworkChange()
		{
		}

		public static event NetworkAddressChangedEventHandler NetworkAddressChanged;

		public static event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged;
	}
}
