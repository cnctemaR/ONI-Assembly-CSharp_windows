using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class MulticastIPAddressInformationImplCollection : MulticastIPAddressInformationCollection
	{
		private MulticastIPAddressInformationImplCollection(bool isReadOnly)
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

		public static MulticastIPAddressInformationCollection Win32FromMulticast(IntPtr ptr)
		{
			MulticastIPAddressInformationImplCollection multicastIPAddressInformationImplCollection = new MulticastIPAddressInformationImplCollection(false);
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				Win32_IP_ADAPTER_MULTICAST_ADDRESS win32_IP_ADAPTER_MULTICAST_ADDRESS = (Win32_IP_ADAPTER_MULTICAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADAPTER_MULTICAST_ADDRESS));
				multicastIPAddressInformationImplCollection.Add(new MulticastIPAddressInformationImpl(win32_IP_ADAPTER_MULTICAST_ADDRESS.Address.GetIPAddress(), win32_IP_ADAPTER_MULTICAST_ADDRESS.LengthFlags.IsDnsEligible, win32_IP_ADAPTER_MULTICAST_ADDRESS.LengthFlags.IsTransient));
				intPtr = win32_IP_ADAPTER_MULTICAST_ADDRESS.Next;
			}
			multicastIPAddressInformationImplCollection.is_readonly = true;
			return multicastIPAddressInformationImplCollection;
		}

		public static MulticastIPAddressInformationImplCollection LinuxFromList(List<IPAddress> addresses)
		{
			MulticastIPAddressInformationImplCollection multicastIPAddressInformationImplCollection = new MulticastIPAddressInformationImplCollection(false);
			foreach (IPAddress ipaddress in addresses)
			{
				multicastIPAddressInformationImplCollection.Add(new MulticastIPAddressInformationImpl(ipaddress, true, false));
			}
			multicastIPAddressInformationImplCollection.is_readonly = true;
			return multicastIPAddressInformationImplCollection;
		}

		public static readonly MulticastIPAddressInformationImplCollection Empty = new MulticastIPAddressInformationImplCollection(true);

		private bool is_readonly;
	}
}
