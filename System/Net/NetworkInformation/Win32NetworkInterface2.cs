using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal sealed class Win32NetworkInterface2 : NetworkInterface
	{
		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern int GetAdaptersInfo(IntPtr info, ref int size);

		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern int GetIfEntry(ref Win32_MIB_IFROW row);

		private static Win32_IP_ADAPTER_INFO[] GetAdaptersInfo()
		{
			int num = 0;
			Win32NetworkInterface2.GetAdaptersInfo(IntPtr.Zero, ref num);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			int adaptersInfo = Win32NetworkInterface2.GetAdaptersInfo(intPtr, ref num);
			if (adaptersInfo != 0)
			{
				throw new NetworkInformationException(adaptersInfo);
			}
			List<Win32_IP_ADAPTER_INFO> list = new List<Win32_IP_ADAPTER_INFO>();
			IntPtr intPtr2 = intPtr;
			while (intPtr2 != IntPtr.Zero)
			{
				Win32_IP_ADAPTER_INFO win32_IP_ADAPTER_INFO = Marshal.PtrToStructure<Win32_IP_ADAPTER_INFO>(intPtr2);
				list.Add(win32_IP_ADAPTER_INFO);
				intPtr2 = win32_IP_ADAPTER_INFO.Next;
			}
			return list.ToArray();
		}

		internal Win32NetworkInterface2(Win32_IP_ADAPTER_ADDRESSES addr)
		{
			this.addr = addr;
			this.mib4 = default(Win32_MIB_IFROW);
			this.mib4.Index = addr.Alignment.IfIndex;
			if (Win32NetworkInterface2.GetIfEntry(ref this.mib4) != 0)
			{
				this.mib4.Index = -1;
			}
			this.mib6 = default(Win32_MIB_IFROW);
			this.mib6.Index = addr.Ipv6IfIndex;
			if (Win32NetworkInterface2.GetIfEntry(ref this.mib6) != 0)
			{
				this.mib6.Index = -1;
			}
			this.ip4stats = new Win32IPv4InterfaceStatistics(this.mib4);
			this.ip_if_props = new Win32IPInterfaceProperties2(addr, this.mib4, this.mib6);
		}

		public override IPInterfaceProperties GetIPProperties()
		{
			return this.ip_if_props;
		}

		public override IPv4InterfaceStatistics GetIPv4Statistics()
		{
			return this.ip4stats;
		}

		public override PhysicalAddress GetPhysicalAddress()
		{
			byte[] array = new byte[this.addr.PhysicalAddressLength];
			Array.Copy(this.addr.PhysicalAddress, 0, array, 0, array.Length);
			return new PhysicalAddress(array);
		}

		public override bool Supports(NetworkInterfaceComponent networkInterfaceComponent)
		{
			if (networkInterfaceComponent != NetworkInterfaceComponent.IPv4)
			{
				return networkInterfaceComponent == NetworkInterfaceComponent.IPv6 && this.mib6.Index >= 0;
			}
			return this.mib4.Index >= 0;
		}

		public override string Description
		{
			get
			{
				return this.addr.Description;
			}
		}

		public override string Id
		{
			get
			{
				return this.addr.AdapterName;
			}
		}

		public override bool IsReceiveOnly
		{
			get
			{
				return this.addr.IsReceiveOnly;
			}
		}

		public override string Name
		{
			get
			{
				return this.addr.FriendlyName;
			}
		}

		public override NetworkInterfaceType NetworkInterfaceType
		{
			get
			{
				return this.addr.IfType;
			}
		}

		public override OperationalStatus OperationalStatus
		{
			get
			{
				return this.addr.OperStatus;
			}
		}

		public override long Speed
		{
			get
			{
				return (long)((ulong)((this.mib6.Index >= 0) ? this.mib6.Speed : this.mib4.Speed));
			}
		}

		public override bool SupportsMulticast
		{
			get
			{
				return !this.addr.NoMulticast;
			}
		}

		private Win32_IP_ADAPTER_ADDRESSES addr;

		private Win32_MIB_IFROW mib4;

		private Win32_MIB_IFROW mib6;

		private Win32IPv4InterfaceStatistics ip4stats;

		private IPInterfaceProperties ip_if_props;
	}
}
