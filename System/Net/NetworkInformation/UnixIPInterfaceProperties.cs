using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace System.Net.NetworkInformation
{
	internal abstract class UnixIPInterfaceProperties : IPInterfaceProperties
	{
		public UnixIPInterfaceProperties(UnixNetworkInterface iface, List<IPAddress> addresses)
		{
			this.iface = iface;
			this.addresses = addresses;
		}

		public override IPv6InterfaceProperties GetIPv6Properties()
		{
			throw new NotImplementedException();
		}

		private void ParseResolvConf()
		{
			try
			{
				DateTime lastWriteTime = File.GetLastWriteTime("/etc/resolv.conf");
				if (!(lastWriteTime <= this.last_parse))
				{
					this.last_parse = lastWriteTime;
					this.dns_suffix = "";
					this.dns_servers = new IPAddressCollection();
					using (StreamReader streamReader = new StreamReader("/etc/resolv.conf"))
					{
						string text;
						while ((text = streamReader.ReadLine()) != null)
						{
							text = text.Trim();
							if (text.Length != 0 && text[0] != '#')
							{
								Match match = UnixIPInterfaceProperties.ns.Match(text);
								if (match.Success)
								{
									try
									{
										string text2 = match.Groups["address"].Value;
										text2 = text2.Trim();
										this.dns_servers.InternalAdd(IPAddress.Parse(text2));
										continue;
									}
									catch
									{
										continue;
									}
								}
								match = UnixIPInterfaceProperties.search.Match(text);
								if (match.Success)
								{
									string text2 = match.Groups["domain"].Value;
									string[] array = text2.Split(new char[] { ',' });
									this.dns_suffix = array[0].Trim();
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		public override IPAddressInformationCollection AnycastAddresses
		{
			get
			{
				IPAddressInformationCollection ipaddressInformationCollection = new IPAddressInformationCollection();
				foreach (IPAddress ipaddress in this.addresses)
				{
					ipaddressInformationCollection.InternalAdd(new SystemIPAddressInformation(ipaddress, false, false));
				}
				return ipaddressInformationCollection;
			}
		}

		[MonoTODO("Always returns an empty collection.")]
		public override IPAddressCollection DhcpServerAddresses
		{
			get
			{
				return new IPAddressCollection();
			}
		}

		public override IPAddressCollection DnsAddresses
		{
			get
			{
				this.ParseResolvConf();
				return this.dns_servers;
			}
		}

		public override string DnsSuffix
		{
			get
			{
				this.ParseResolvConf();
				return this.dns_suffix;
			}
		}

		[MonoTODO("Always returns true")]
		public override bool IsDnsEnabled
		{
			get
			{
				return true;
			}
		}

		[MonoTODO("Always returns false")]
		public override bool IsDynamicDnsEnabled
		{
			get
			{
				return false;
			}
		}

		public override MulticastIPAddressInformationCollection MulticastAddresses
		{
			get
			{
				MulticastIPAddressInformationCollection multicastIPAddressInformationCollection = new MulticastIPAddressInformationCollection();
				foreach (IPAddress ipaddress in this.addresses)
				{
					byte[] addressBytes = ipaddress.GetAddressBytes();
					if (addressBytes[0] >= 224 && addressBytes[0] <= 239)
					{
						multicastIPAddressInformationCollection.InternalAdd(new SystemMulticastIPAddressInformation(new SystemIPAddressInformation(ipaddress, true, false)));
					}
				}
				return multicastIPAddressInformationCollection;
			}
		}

		public override UnicastIPAddressInformationCollection UnicastAddresses
		{
			get
			{
				UnicastIPAddressInformationCollection unicastIPAddressInformationCollection = new UnicastIPAddressInformationCollection();
				foreach (IPAddress ipaddress in this.addresses)
				{
					AddressFamily addressFamily = ipaddress.AddressFamily;
					if (addressFamily != AddressFamily.InterNetwork)
					{
						if (addressFamily == AddressFamily.InterNetworkV6)
						{
							if (!ipaddress.IsIPv6Multicast)
							{
								unicastIPAddressInformationCollection.InternalAdd(new LinuxUnicastIPAddressInformation(ipaddress));
							}
						}
					}
					else
					{
						byte b = ipaddress.GetAddressBytes()[0];
						if (b < 224 || b > 239)
						{
							unicastIPAddressInformationCollection.InternalAdd(new LinuxUnicastIPAddressInformation(ipaddress));
						}
					}
				}
				return unicastIPAddressInformationCollection;
			}
		}

		[MonoTODO("Always returns an empty collection.")]
		public override IPAddressCollection WinsServersAddresses
		{
			get
			{
				return new IPAddressCollection();
			}
		}

		protected IPv4InterfaceProperties ipv4iface_properties;

		protected UnixNetworkInterface iface;

		private List<IPAddress> addresses;

		private IPAddressCollection dns_servers;

		private static Regex ns = new Regex("\\s*nameserver\\s+(?<address>.*)");

		private static Regex search = new Regex("\\s*search\\s+(?<domain>.*)");

		private string dns_suffix;

		private DateTime last_parse;
	}
}
