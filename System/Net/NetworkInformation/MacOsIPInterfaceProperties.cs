using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Net.NetworkInformation
{
	internal class MacOsIPInterfaceProperties : UnixIPInterfaceProperties
	{
		public MacOsIPInterfaceProperties(MacOsNetworkInterface iface, List<IPAddress> addresses)
			: base(iface, addresses)
		{
		}

		public override IPv4InterfaceProperties GetIPv4Properties()
		{
			if (this.ipv4iface_properties == null)
			{
				this.ipv4iface_properties = new MacOsIPv4InterfaceProperties(this.iface as MacOsNetworkInterface);
			}
			return this.ipv4iface_properties;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ParseRouteInfo_internal(string iface, out string[] gw_addr_list);

		public override GatewayIPAddressInformationCollection GatewayAddresses
		{
			get
			{
				IPAddressCollection ipaddressCollection = new IPAddressCollection();
				string[] array;
				if (!MacOsIPInterfaceProperties.ParseRouteInfo_internal(this.iface.Name.ToString(), out array))
				{
					return new GatewayIPAddressInformationCollection();
				}
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						IPAddress ipaddress = IPAddress.Parse(array[i]);
						if (!ipaddress.Equals(IPAddress.Any) && !ipaddressCollection.Contains(ipaddress))
						{
							ipaddressCollection.InternalAdd(ipaddress);
						}
					}
					catch (ArgumentNullException)
					{
					}
				}
				return SystemGatewayIPAddressInformation.ToGatewayIpAddressInformationCollection(ipaddressCollection);
			}
		}
	}
}
