using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation.MacOsStructs;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal abstract class NetworkInterfaceFactory
	{
		public abstract NetworkInterface[] GetAllNetworkInterfaces();

		public abstract int GetLoopbackInterfaceIndex();

		public abstract IPAddress GetNetMask(IPAddress address);

		public static NetworkInterfaceFactory Create()
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				if (Platform.IsMacOS || Platform.IsFreeBSD)
				{
					return new NetworkInterfaceFactory.MacOsNetworkInterfaceAPI();
				}
				return new NetworkInterfaceFactory.LinuxNetworkInterfaceAPI();
			}
			else
			{
				Version version = new Version(5, 1);
				if (Environment.OSVersion.Version >= version)
				{
					return new NetworkInterfaceFactory.Win32NetworkInterfaceAPI();
				}
				throw new NotImplementedException();
			}
		}

		internal abstract class UnixNetworkInterfaceAPI : NetworkInterfaceFactory
		{
			[DllImport("libc")]
			public static extern int if_nametoindex(string ifname);

			[DllImport("libc")]
			protected static extern int getifaddrs(out IntPtr ifap);

			[DllImport("libc")]
			protected static extern void freeifaddrs(IntPtr ifap);
		}

		private class MacOsNetworkInterfaceAPI : NetworkInterfaceFactory.UnixNetworkInterfaceAPI
		{
			public override NetworkInterface[] GetAllNetworkInterfaces()
			{
				Dictionary<string, MacOsNetworkInterface> dictionary = new Dictionary<string, MacOsNetworkInterface>();
				IntPtr intPtr;
				if (NetworkInterfaceFactory.UnixNetworkInterfaceAPI.getifaddrs(out intPtr) != 0)
				{
					throw new SystemException("getifaddrs() failed");
				}
				try
				{
					IntPtr intPtr2 = intPtr;
					while (intPtr2 != IntPtr.Zero)
					{
						ifaddrs ifaddrs = (ifaddrs)Marshal.PtrToStructure(intPtr2, typeof(ifaddrs));
						IPAddress ipaddress = IPAddress.None;
						string ifa_name = ifaddrs.ifa_name;
						int num = -1;
						byte[] array = null;
						NetworkInterfaceType networkInterfaceType = NetworkInterfaceType.Unknown;
						if (ifaddrs.ifa_addr != IntPtr.Zero)
						{
							sockaddr sockaddr = (sockaddr)Marshal.PtrToStructure(ifaddrs.ifa_addr, typeof(sockaddr));
							if (sockaddr.sa_family == 30)
							{
								sockaddr_in6 sockaddr_in = (sockaddr_in6)Marshal.PtrToStructure(ifaddrs.ifa_addr, typeof(sockaddr_in6));
								ipaddress = new IPAddress(sockaddr_in.sin6_addr.u6_addr8, (long)((ulong)sockaddr_in.sin6_scope_id));
							}
							else if (sockaddr.sa_family == 2)
							{
								ipaddress = new IPAddress((long)((ulong)((sockaddr_in)Marshal.PtrToStructure(ifaddrs.ifa_addr, typeof(sockaddr_in))).sin_addr));
							}
							else if (sockaddr.sa_family == 18)
							{
								sockaddr_dl sockaddr_dl = default(sockaddr_dl);
								sockaddr_dl.Read(ifaddrs.ifa_addr);
								array = new byte[(int)sockaddr_dl.sdl_alen];
								Array.Copy(sockaddr_dl.sdl_data, (int)sockaddr_dl.sdl_nlen, array, 0, Math.Min(array.Length, sockaddr_dl.sdl_data.Length - (int)sockaddr_dl.sdl_nlen));
								num = (int)sockaddr_dl.sdl_index;
								int sdl_type = (int)sockaddr_dl.sdl_type;
								if (Enum.IsDefined(typeof(MacOsArpHardware), sdl_type))
								{
									MacOsArpHardware macOsArpHardware = (MacOsArpHardware)sdl_type;
									if (macOsArpHardware <= MacOsArpHardware.PPP)
									{
										if (macOsArpHardware != MacOsArpHardware.ETHER)
										{
											if (macOsArpHardware != MacOsArpHardware.FDDI)
											{
												if (macOsArpHardware == MacOsArpHardware.PPP)
												{
													networkInterfaceType = NetworkInterfaceType.Ppp;
												}
											}
											else
											{
												networkInterfaceType = NetworkInterfaceType.Fddi;
											}
										}
										else
										{
											networkInterfaceType = NetworkInterfaceType.Ethernet;
										}
									}
									else if (macOsArpHardware != MacOsArpHardware.LOOPBACK)
									{
										if (macOsArpHardware != MacOsArpHardware.SLIP)
										{
											if (macOsArpHardware == MacOsArpHardware.ATM)
											{
												networkInterfaceType = NetworkInterfaceType.Atm;
											}
										}
										else
										{
											networkInterfaceType = NetworkInterfaceType.Slip;
										}
									}
									else
									{
										networkInterfaceType = NetworkInterfaceType.Loopback;
										array = null;
									}
								}
							}
						}
						MacOsNetworkInterface macOsNetworkInterface = null;
						if (!dictionary.TryGetValue(ifa_name, out macOsNetworkInterface))
						{
							macOsNetworkInterface = new MacOsNetworkInterface(ifa_name, ifaddrs.ifa_flags);
							dictionary.Add(ifa_name, macOsNetworkInterface);
						}
						if (!ipaddress.Equals(IPAddress.None))
						{
							macOsNetworkInterface.AddAddress(ipaddress);
						}
						if (array != null || networkInterfaceType == NetworkInterfaceType.Loopback)
						{
							macOsNetworkInterface.SetLinkLayerInfo(num, array, networkInterfaceType);
						}
						intPtr2 = ifaddrs.ifa_next;
					}
				}
				finally
				{
					NetworkInterfaceFactory.UnixNetworkInterfaceAPI.freeifaddrs(intPtr);
				}
				NetworkInterface[] array2 = new NetworkInterface[dictionary.Count];
				int num2 = 0;
				foreach (NetworkInterface networkInterface in dictionary.Values)
				{
					array2[num2] = networkInterface;
					num2++;
				}
				return array2;
			}

			public override int GetLoopbackInterfaceIndex()
			{
				return NetworkInterfaceFactory.UnixNetworkInterfaceAPI.if_nametoindex("lo0");
			}

			public override IPAddress GetNetMask(IPAddress address)
			{
				IntPtr intPtr;
				if (NetworkInterfaceFactory.UnixNetworkInterfaceAPI.getifaddrs(out intPtr) != 0)
				{
					throw new SystemException("getifaddrs() failed");
				}
				try
				{
					IntPtr intPtr2 = intPtr;
					while (intPtr2 != IntPtr.Zero)
					{
						ifaddrs ifaddrs = (ifaddrs)Marshal.PtrToStructure(intPtr2, typeof(ifaddrs));
						if (ifaddrs.ifa_addr != IntPtr.Zero && ((sockaddr)Marshal.PtrToStructure(ifaddrs.ifa_addr, typeof(sockaddr))).sa_family == 2)
						{
							IPAddress ipaddress = new IPAddress((long)((ulong)((sockaddr_in)Marshal.PtrToStructure(ifaddrs.ifa_addr, typeof(sockaddr_in))).sin_addr));
							if (address.Equals(ipaddress))
							{
								return new IPAddress((long)((ulong)((sockaddr_in)Marshal.PtrToStructure(ifaddrs.ifa_netmask, typeof(sockaddr_in))).sin_addr));
							}
						}
						intPtr2 = ifaddrs.ifa_next;
					}
				}
				finally
				{
					NetworkInterfaceFactory.UnixNetworkInterfaceAPI.freeifaddrs(intPtr);
				}
				return null;
			}

			private const int AF_INET = 2;

			private const int AF_INET6 = 30;

			private const int AF_LINK = 18;
		}

		private class LinuxNetworkInterfaceAPI : NetworkInterfaceFactory.UnixNetworkInterfaceAPI
		{
			private static void FreeInterfaceAddresses(IntPtr ifap)
			{
				NetworkInterfaceFactory.UnixNetworkInterfaceAPI.freeifaddrs(ifap);
			}

			private static int GetInterfaceAddresses(out IntPtr ifap)
			{
				return NetworkInterfaceFactory.UnixNetworkInterfaceAPI.getifaddrs(out ifap);
			}

			public override NetworkInterface[] GetAllNetworkInterfaces()
			{
				Dictionary<string, LinuxNetworkInterface> dictionary = new Dictionary<string, LinuxNetworkInterface>();
				IntPtr intPtr;
				if (NetworkInterfaceFactory.LinuxNetworkInterfaceAPI.GetInterfaceAddresses(out intPtr) != 0)
				{
					throw new SystemException("getifaddrs() failed");
				}
				try
				{
					IntPtr intPtr2 = intPtr;
					while (intPtr2 != IntPtr.Zero)
					{
						ifaddrs ifaddrs = (ifaddrs)Marshal.PtrToStructure(intPtr2, typeof(ifaddrs));
						IPAddress ipaddress = IPAddress.None;
						string text = ifaddrs.ifa_name;
						int num = -1;
						byte[] array = null;
						NetworkInterfaceType networkInterfaceType = NetworkInterfaceType.Unknown;
						int num2 = 0;
						if (ifaddrs.ifa_addr != IntPtr.Zero)
						{
							sockaddr_in sockaddr_in = (sockaddr_in)Marshal.PtrToStructure(ifaddrs.ifa_addr, typeof(sockaddr_in));
							if (sockaddr_in.sin_family == 10)
							{
								sockaddr_in6 sockaddr_in2 = (sockaddr_in6)Marshal.PtrToStructure(ifaddrs.ifa_addr, typeof(sockaddr_in6));
								ipaddress = new IPAddress(sockaddr_in2.sin6_addr.u6_addr8, (long)((ulong)sockaddr_in2.sin6_scope_id));
							}
							else if (sockaddr_in.sin_family == 2)
							{
								ipaddress = new IPAddress((long)((ulong)sockaddr_in.sin_addr));
							}
							else if (sockaddr_in.sin_family == 17)
							{
								sockaddr_ll sockaddr_ll = (sockaddr_ll)Marshal.PtrToStructure(ifaddrs.ifa_addr, typeof(sockaddr_ll));
								if ((int)sockaddr_ll.sll_halen > sockaddr_ll.sll_addr.Length)
								{
									intPtr2 = ifaddrs.ifa_next;
									continue;
								}
								array = new byte[(int)sockaddr_ll.sll_halen];
								Array.Copy(sockaddr_ll.sll_addr, 0, array, 0, array.Length);
								num = sockaddr_ll.sll_ifindex;
								int sll_hatype = (int)sockaddr_ll.sll_hatype;
								if (Enum.IsDefined(typeof(LinuxArpHardware), sll_hatype))
								{
									LinuxArpHardware linuxArpHardware = (LinuxArpHardware)sll_hatype;
									if (linuxArpHardware <= LinuxArpHardware.CSLIP6)
									{
										switch (linuxArpHardware)
										{
										case LinuxArpHardware.ETHER:
										case LinuxArpHardware.EETHER:
											networkInterfaceType = NetworkInterfaceType.Ethernet;
											break;
										case (LinuxArpHardware)3:
											break;
										case LinuxArpHardware.PRONET:
											networkInterfaceType = NetworkInterfaceType.TokenRing;
											break;
										default:
											if (linuxArpHardware != LinuxArpHardware.ATM)
											{
												if (linuxArpHardware - LinuxArpHardware.SLIP <= 3)
												{
													networkInterfaceType = NetworkInterfaceType.Slip;
												}
											}
											else
											{
												networkInterfaceType = NetworkInterfaceType.Atm;
											}
											break;
										}
									}
									else if (linuxArpHardware != LinuxArpHardware.PPP)
									{
										switch (linuxArpHardware)
										{
										case LinuxArpHardware.TUNNEL:
										case LinuxArpHardware.TUNNEL6:
										case LinuxArpHardware.SIT:
										case LinuxArpHardware.IPDDP:
										case LinuxArpHardware.IPGRE:
											break;
										case (LinuxArpHardware)770:
										case (LinuxArpHardware)771:
										case (LinuxArpHardware)773:
										case (LinuxArpHardware)775:
											goto IL_0246;
										case LinuxArpHardware.LOOPBACK:
											networkInterfaceType = NetworkInterfaceType.Loopback;
											array = null;
											goto IL_0246;
										case LinuxArpHardware.FDDI:
											networkInterfaceType = NetworkInterfaceType.Fddi;
											goto IL_0246;
										default:
											if (linuxArpHardware != LinuxArpHardware.IP6GRE)
											{
												goto IL_0246;
											}
											break;
										}
										networkInterfaceType = NetworkInterfaceType.Tunnel;
									}
									else
									{
										networkInterfaceType = NetworkInterfaceType.Ppp;
									}
								}
							}
						}
						IL_0246:
						LinuxNetworkInterface linuxNetworkInterface = null;
						if (string.IsNullOrEmpty(text))
						{
							text = "\0" + (num2 + 1).ToString();
						}
						if (!dictionary.TryGetValue(text, out linuxNetworkInterface))
						{
							linuxNetworkInterface = new LinuxNetworkInterface(text);
							dictionary.Add(text, linuxNetworkInterface);
						}
						if (!ipaddress.Equals(IPAddress.None))
						{
							linuxNetworkInterface.AddAddress(ipaddress);
						}
						if (array != null || networkInterfaceType == NetworkInterfaceType.Loopback)
						{
							if (networkInterfaceType == NetworkInterfaceType.Ethernet && Directory.Exists(linuxNetworkInterface.IfacePath + "wireless"))
							{
								networkInterfaceType = NetworkInterfaceType.Wireless80211;
							}
							linuxNetworkInterface.SetLinkLayerInfo(num, array, networkInterfaceType);
						}
						intPtr2 = ifaddrs.ifa_next;
					}
				}
				finally
				{
					NetworkInterfaceFactory.LinuxNetworkInterfaceAPI.FreeInterfaceAddresses(intPtr);
				}
				NetworkInterface[] array2 = new NetworkInterface[dictionary.Count];
				int num3 = 0;
				foreach (NetworkInterface networkInterface in dictionary.Values)
				{
					array2[num3] = networkInterface;
					num3++;
				}
				return array2;
			}

			public override int GetLoopbackInterfaceIndex()
			{
				return NetworkInterfaceFactory.UnixNetworkInterfaceAPI.if_nametoindex("lo");
			}

			public override IPAddress GetNetMask(IPAddress address)
			{
				foreach (ifaddrs ifaddrs in NetworkInterfaceFactory.LinuxNetworkInterfaceAPI.GetNetworkInterfaces())
				{
					if (!(ifaddrs.ifa_addr == IntPtr.Zero))
					{
						sockaddr_in sockaddr_in = (sockaddr_in)Marshal.PtrToStructure(ifaddrs.ifa_addr, typeof(sockaddr_in));
						if (sockaddr_in.sin_family == 2 && address.Equals(new IPAddress((long)((ulong)sockaddr_in.sin_addr))))
						{
							return new IPAddress((long)((ulong)((sockaddr_in)Marshal.PtrToStructure(ifaddrs.ifa_netmask, typeof(sockaddr_in))).sin_addr));
						}
					}
				}
				return null;
			}

			private static IEnumerable<ifaddrs> GetNetworkInterfaces()
			{
				IntPtr ifap = IntPtr.Zero;
				try
				{
					if (NetworkInterfaceFactory.LinuxNetworkInterfaceAPI.GetInterfaceAddresses(out ifap) != 0)
					{
						yield break;
					}
					IntPtr intPtr = ifap;
					while (intPtr != IntPtr.Zero)
					{
						ifaddrs addr = (ifaddrs)Marshal.PtrToStructure(intPtr, typeof(ifaddrs));
						yield return addr;
						intPtr = addr.ifa_next;
						addr = default(ifaddrs);
					}
				}
				finally
				{
					if (ifap != IntPtr.Zero)
					{
						NetworkInterfaceFactory.LinuxNetworkInterfaceAPI.FreeInterfaceAddresses(ifap);
					}
				}
				yield break;
				yield break;
			}

			private const int AF_INET = 2;

			private const int AF_INET6 = 10;

			private const int AF_PACKET = 17;
		}

		private class Win32NetworkInterfaceAPI : NetworkInterfaceFactory
		{
			[DllImport("iphlpapi.dll", SetLastError = true)]
			private static extern int GetAdaptersAddresses(uint family, uint flags, IntPtr reserved, IntPtr info, ref int size);

			[DllImport("iphlpapi.dll")]
			private static extern uint GetBestInterfaceEx(byte[] ipAddress, out int index);

			private static Win32_IP_ADAPTER_ADDRESSES[] GetAdaptersAddresses()
			{
				IntPtr intPtr = IntPtr.Zero;
				int num = 0;
				NetworkInterfaceFactory.Win32NetworkInterfaceAPI.GetAdaptersAddresses(0U, 0U, IntPtr.Zero, intPtr, ref num);
				intPtr = Marshal.AllocHGlobal(num);
				int adaptersAddresses = NetworkInterfaceFactory.Win32NetworkInterfaceAPI.GetAdaptersAddresses(0U, 0U, IntPtr.Zero, intPtr, ref num);
				if (adaptersAddresses != 0)
				{
					throw new NetworkInformationException(adaptersAddresses);
				}
				List<Win32_IP_ADAPTER_ADDRESSES> list = new List<Win32_IP_ADAPTER_ADDRESSES>();
				IntPtr intPtr2 = intPtr;
				while (intPtr2 != IntPtr.Zero)
				{
					Win32_IP_ADAPTER_ADDRESSES win32_IP_ADAPTER_ADDRESSES = Marshal.PtrToStructure<Win32_IP_ADAPTER_ADDRESSES>(intPtr2);
					list.Add(win32_IP_ADAPTER_ADDRESSES);
					intPtr2 = win32_IP_ADAPTER_ADDRESSES.Next;
				}
				return list.ToArray();
			}

			public override NetworkInterface[] GetAllNetworkInterfaces()
			{
				Win32_IP_ADAPTER_ADDRESSES[] adaptersAddresses = NetworkInterfaceFactory.Win32NetworkInterfaceAPI.GetAdaptersAddresses();
				NetworkInterface[] array = new NetworkInterface[adaptersAddresses.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Win32NetworkInterface2(adaptersAddresses[i]);
				}
				return array;
			}

			private static int GetBestInterfaceForAddress(IPAddress addr)
			{
				int num;
				int bestInterfaceEx = (int)NetworkInterfaceFactory.Win32NetworkInterfaceAPI.GetBestInterfaceEx(new SocketAddress(addr).m_Buffer, out num);
				if (bestInterfaceEx != 0)
				{
					throw new NetworkInformationException(bestInterfaceEx);
				}
				return num;
			}

			public override int GetLoopbackInterfaceIndex()
			{
				return NetworkInterfaceFactory.Win32NetworkInterfaceAPI.GetBestInterfaceForAddress(IPAddress.Loopback);
			}

			public override IPAddress GetNetMask(IPAddress address)
			{
				throw new NotImplementedException();
			}

			private const string IPHLPAPI = "iphlpapi.dll";
		}
	}
}
