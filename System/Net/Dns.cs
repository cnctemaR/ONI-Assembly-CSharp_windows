using System;
using System.Collections;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

namespace System.Net
{
	public static class Dns
	{
		static Dns()
		{
			global::System.Net.Sockets.Socket.CheckProtocolSupport();
		}

		[Obsolete("Use BeginGetHostEntry instead")]
		public static IAsyncResult BeginGetHostByName(string hostName, AsyncCallback requestCallback, object stateObject)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			Dns.GetHostByNameCallback getHostByNameCallback = new Dns.GetHostByNameCallback(Dns.GetHostByName);
			return getHostByNameCallback.BeginInvoke(hostName, requestCallback, stateObject);
		}

		[Obsolete("Use BeginGetHostEntry instead")]
		public static IAsyncResult BeginResolve(string hostName, AsyncCallback requestCallback, object stateObject)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			Dns.ResolveCallback resolveCallback = new Dns.ResolveCallback(Dns.Resolve);
			return resolveCallback.BeginInvoke(hostName, requestCallback, stateObject);
		}

		public static IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback requestCallback, object stateObject)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostName");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			Dns.GetHostAddressesCallback getHostAddressesCallback = new Dns.GetHostAddressesCallback(Dns.GetHostAddresses);
			return getHostAddressesCallback.BeginInvoke(hostNameOrAddress, requestCallback, stateObject);
		}

		public static IAsyncResult BeginGetHostEntry(string hostNameOrAddress, AsyncCallback requestCallback, object stateObject)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostName");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			Dns.GetHostEntryNameCallback getHostEntryNameCallback = new Dns.GetHostEntryNameCallback(Dns.GetHostEntry);
			return getHostEntryNameCallback.BeginInvoke(hostNameOrAddress, requestCallback, stateObject);
		}

		public static IAsyncResult BeginGetHostEntry(IPAddress address, AsyncCallback requestCallback, object stateObject)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			Dns.GetHostEntryIPCallback getHostEntryIPCallback = new Dns.GetHostEntryIPCallback(Dns.GetHostEntry);
			return getHostEntryIPCallback.BeginInvoke(address, requestCallback, stateObject);
		}

		[Obsolete("Use EndGetHostEntry instead")]
		public static IPHostEntry EndGetHostByName(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			Dns.GetHostByNameCallback getHostByNameCallback = (Dns.GetHostByNameCallback)asyncResult2.AsyncDelegate;
			return getHostByNameCallback.EndInvoke(asyncResult);
		}

		[Obsolete("Use EndGetHostEntry instead")]
		public static IPHostEntry EndResolve(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			Dns.ResolveCallback resolveCallback = (Dns.ResolveCallback)asyncResult2.AsyncDelegate;
			return resolveCallback.EndInvoke(asyncResult);
		}

		public static IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			Dns.GetHostAddressesCallback getHostAddressesCallback = (Dns.GetHostAddressesCallback)asyncResult2.AsyncDelegate;
			return getHostAddressesCallback.EndInvoke(asyncResult);
		}

		public static IPHostEntry EndGetHostEntry(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			if (asyncResult2.AsyncDelegate is Dns.GetHostEntryIPCallback)
			{
				return ((Dns.GetHostEntryIPCallback)asyncResult2.AsyncDelegate).EndInvoke(asyncResult);
			}
			Dns.GetHostEntryNameCallback getHostEntryNameCallback = (Dns.GetHostEntryNameCallback)asyncResult2.AsyncDelegate;
			return getHostEntryNameCallback.EndInvoke(asyncResult);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHostByName_internal(string host, out string h_name, out string[] h_aliases, out string[] h_addr_list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHostByAddr_internal(string addr, out string h_name, out string[] h_aliases, out string[] h_addr_list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHostName_internal(out string h_name);

		private static IPHostEntry hostent_to_IPHostEntry(string h_name, string[] h_aliases, string[] h_addrlist)
		{
			IPHostEntry iphostEntry = new IPHostEntry();
			ArrayList arrayList = new ArrayList();
			iphostEntry.HostName = h_name;
			iphostEntry.Aliases = h_aliases;
			for (int i = 0; i < h_addrlist.Length; i++)
			{
				try
				{
					IPAddress ipaddress = IPAddress.Parse(h_addrlist[i]);
					if ((global::System.Net.Sockets.Socket.SupportsIPv6 && ipaddress.AddressFamily == global::System.Net.Sockets.AddressFamily.InterNetworkV6) || (global::System.Net.Sockets.Socket.SupportsIPv4 && ipaddress.AddressFamily == global::System.Net.Sockets.AddressFamily.InterNetwork))
					{
						arrayList.Add(ipaddress);
					}
				}
				catch (ArgumentNullException)
				{
				}
			}
			if (arrayList.Count == 0)
			{
				throw new global::System.Net.Sockets.SocketException(11001);
			}
			iphostEntry.AddressList = arrayList.ToArray(typeof(IPAddress)) as IPAddress[];
			return iphostEntry;
		}

		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry GetHostByAddress(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return Dns.GetHostByAddressFromString(address.ToString(), false);
		}

		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry GetHostByAddress(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return Dns.GetHostByAddressFromString(address, true);
		}

		private static IPHostEntry GetHostByAddressFromString(string address, bool parse)
		{
			if (address.Equals("0.0.0.0"))
			{
				address = "127.0.0.1";
				parse = false;
			}
			if (parse)
			{
				IPAddress.Parse(address);
			}
			string text;
			string[] array;
			string[] array2;
			if (!Dns.GetHostByAddr_internal(address, out text, out array, out array2))
			{
				throw new global::System.Net.Sockets.SocketException(11001);
			}
			return Dns.hostent_to_IPHostEntry(text, array, array2);
		}

		public static IPHostEntry GetHostEntry(string hostNameOrAddress)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostNameOrAddress");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			IPAddress ipaddress;
			if (hostNameOrAddress.Length > 0 && IPAddress.TryParse(hostNameOrAddress, out ipaddress))
			{
				return Dns.GetHostEntry(ipaddress);
			}
			return Dns.GetHostByName(hostNameOrAddress);
		}

		public static IPHostEntry GetHostEntry(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return Dns.GetHostByAddressFromString(address.ToString(), false);
		}

		public static IPAddress[] GetHostAddresses(string hostNameOrAddress)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostNameOrAddress");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			IPAddress ipaddress;
			if (hostNameOrAddress.Length > 0 && IPAddress.TryParse(hostNameOrAddress, out ipaddress))
			{
				return new IPAddress[] { ipaddress };
			}
			return Dns.GetHostEntry(hostNameOrAddress).AddressList;
		}

		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry GetHostByName(string hostName)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			string text;
			string[] array;
			string[] array2;
			if (!Dns.GetHostByName_internal(hostName, out text, out array, out array2))
			{
				throw new global::System.Net.Sockets.SocketException(11001);
			}
			return Dns.hostent_to_IPHostEntry(text, array, array2);
		}

		public static string GetHostName()
		{
			string text;
			if (!Dns.GetHostName_internal(out text))
			{
				throw new global::System.Net.Sockets.SocketException(11001);
			}
			return text;
		}

		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry Resolve(string hostName)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			IPHostEntry iphostEntry = null;
			try
			{
				iphostEntry = Dns.GetHostByAddress(hostName);
			}
			catch
			{
			}
			if (iphostEntry == null)
			{
				iphostEntry = Dns.GetHostByName(hostName);
			}
			return iphostEntry;
		}

		private delegate IPHostEntry GetHostByNameCallback(string hostName);

		private delegate IPHostEntry ResolveCallback(string hostName);

		private delegate IPHostEntry GetHostEntryNameCallback(string hostName);

		private delegate IPHostEntry GetHostEntryIPCallback(IPAddress hostAddress);

		private delegate IPAddress[] GetHostAddressesCallback(string hostName);
	}
}
