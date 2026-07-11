using System;

namespace System.Net.NetworkInformation
{
	internal interface INetworkChange : IDisposable
	{
		event NetworkAddressChangedEventHandler NetworkAddressChanged;

		event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged;

		bool HasRegisteredEvents { get; }
	}
}
