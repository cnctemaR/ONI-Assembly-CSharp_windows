using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class UnicastIPAddressInformationImplCollection : UnicastIPAddressInformationCollection
	{
		private UnicastIPAddressInformationImplCollection(bool isReadOnly)
		{
			this.is_readonly = isReadOnly;
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.is_readonly;
			}
		}

		public static UnicastIPAddressInformationCollection Win32FromUnicast(int ifIndex, IntPtr ptr)
		{
			UnicastIPAddressInformationImplCollection unicastIPAddressInformationImplCollection = new UnicastIPAddressInformationImplCollection(false);
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				Win32_IP_ADAPTER_UNICAST_ADDRESS win32_IP_ADAPTER_UNICAST_ADDRESS = (Win32_IP_ADAPTER_UNICAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADAPTER_UNICAST_ADDRESS));
				unicastIPAddressInformationImplCollection.Add(new Win32UnicastIPAddressInformation(ifIndex, win32_IP_ADAPTER_UNICAST_ADDRESS));
				intPtr = win32_IP_ADAPTER_UNICAST_ADDRESS.Next;
			}
			unicastIPAddressInformationImplCollection.is_readonly = true;
			return unicastIPAddressInformationImplCollection;
		}

		public static UnicastIPAddressInformationCollection LinuxFromList(List<IPAddress> addresses)
		{
			UnicastIPAddressInformationImplCollection unicastIPAddressInformationImplCollection = new UnicastIPAddressInformationImplCollection(false);
			foreach (IPAddress ipaddress in addresses)
			{
				unicastIPAddressInformationImplCollection.Add(new LinuxUnicastIPAddressInformation(ipaddress));
			}
			unicastIPAddressInformationImplCollection.is_readonly = true;
			return unicastIPAddressInformationImplCollection;
		}

		public static readonly UnicastIPAddressInformationImplCollection Empty = new UnicastIPAddressInformationImplCollection(true);

		private bool is_readonly;
	}
}
