using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace System.Net.NetworkInformation
{
	internal class LinuxIPInterfaceProperties : UnixIPInterfaceProperties
	{
		public LinuxIPInterfaceProperties(LinuxNetworkInterface iface, List<IPAddress> addresses)
			: base(iface, addresses)
		{
		}

		public override IPv4InterfaceProperties GetIPv4Properties()
		{
			if (this.ipv4iface_properties == null)
			{
				this.ipv4iface_properties = new LinuxIPv4InterfaceProperties(this.iface as LinuxNetworkInterface);
			}
			return this.ipv4iface_properties;
		}

		private IPAddressCollection ParseRouteInfo(string iface)
		{
			IPAddressCollection ipaddressCollection = new IPAddressCollection();
			try
			{
				using (StreamReader streamReader = new StreamReader("/proc/net/route"))
				{
					streamReader.ReadLine();
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						text = text.Trim();
						if (text.Length != 0)
						{
							string[] array = text.Split(new char[] { '\t' });
							if (array.Length >= 3)
							{
								string text2 = array[2].Trim();
								byte[] array2 = new byte[4];
								if (text2.Length == 8 && iface.Equals(array[0], StringComparison.OrdinalIgnoreCase))
								{
									for (int i = 0; i < 4; i++)
									{
										byte.TryParse(text2.Substring(i * 2, 2), NumberStyles.HexNumber, null, out array2[3 - i]);
									}
									IPAddress ipaddress = new IPAddress(array2);
									if (!ipaddress.Equals(IPAddress.Any) && !ipaddressCollection.Contains(ipaddress))
									{
										ipaddressCollection.InternalAdd(ipaddress);
									}
								}
							}
						}
					}
				}
			}
			catch
			{
			}
			return ipaddressCollection;
		}

		public override GatewayIPAddressInformationCollection GatewayAddresses
		{
			get
			{
				return SystemGatewayIPAddressInformation.ToGatewayIpAddressInformationCollection(this.ParseRouteInfo(this.iface.Name.ToString()));
			}
		}
	}
}
