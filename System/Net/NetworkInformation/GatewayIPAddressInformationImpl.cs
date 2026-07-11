using System;

namespace System.Net.NetworkInformation
{
	internal class GatewayIPAddressInformationImpl : GatewayIPAddressInformation
	{
		public GatewayIPAddressInformationImpl(IPAddress address)
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

		private IPAddress address;
	}
}
