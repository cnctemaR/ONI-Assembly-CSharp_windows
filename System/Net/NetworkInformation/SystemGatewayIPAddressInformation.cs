using System;

namespace System.Net.NetworkInformation
{
	internal class SystemGatewayIPAddressInformation : GatewayIPAddressInformation
	{
		internal SystemGatewayIPAddressInformation(IPAddress address)
		{
			this.address = address;
		}

		public override IPAddress Address
		{
			get
			{
				return this.address;
			}
		}

		internal static GatewayIPAddressInformationCollection ToGatewayIpAddressInformationCollection(IPAddressCollection addresses)
		{
			GatewayIPAddressInformationCollection gatewayIPAddressInformationCollection = new GatewayIPAddressInformationCollection();
			foreach (IPAddress ipaddress in addresses)
			{
				gatewayIPAddressInformationCollection.InternalAdd(new SystemGatewayIPAddressInformation(ipaddress));
			}
			return gatewayIPAddressInformationCollection;
		}

		private IPAddress address;
	}
}
