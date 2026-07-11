using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net.NetworkInformation
{
	internal sealed class LinuxNetworkChange : INetworkChange, IDisposable
	{
		public event NetworkAddressChangedEventHandler NetworkAddressChanged
		{
			add
			{
				this.Register(value);
			}
			remove
			{
				this.Unregister(value);
			}
		}

		public event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged
		{
			add
			{
				this.Register(value);
			}
			remove
			{
				this.Unregister(value);
			}
		}

		public bool HasRegisteredEvents
		{
			get
			{
				return this.AddressChanged != null || this.AvailabilityChanged != null;
			}
		}

		public void Dispose()
		{
		}

		private bool EnsureSocket()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				if (this.nl_sock != null)
				{
					return true;
				}
				IntPtr intPtr = LinuxNetworkChange.CreateNLSocket();
				if (intPtr.ToInt64() == -1L)
				{
					return false;
				}
				SafeSocketHandle safeSocketHandle = new SafeSocketHandle(intPtr, true);
				this.nl_sock = new Socket(AddressFamily.Unspecified, SocketType.Raw, ProtocolType.Udp, safeSocketHandle);
				this.nl_args = new SocketAsyncEventArgs();
				this.nl_args.SetBuffer(new byte[8192], 0, 8192);
				this.nl_args.Completed += this.OnDataAvailable;
				this.nl_sock.ReceiveAsync(this.nl_args);
			}
			return true;
		}

		private void MaybeCloseSocket()
		{
			if (this.nl_sock == null || this.AvailabilityChanged != null || this.AddressChanged != null)
			{
				return;
			}
			LinuxNetworkChange.CloseNLSocket(this.nl_sock.Handle);
			GC.SuppressFinalize(this.nl_sock);
			this.nl_sock = null;
			this.nl_args = null;
		}

		private bool GetAvailability()
		{
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					return true;
				}
			}
			return false;
		}

		private void OnAvailabilityChanged(object unused)
		{
			NetworkAvailabilityChangedEventHandler availabilityChanged = this.AvailabilityChanged;
			if (availabilityChanged != null)
			{
				availabilityChanged(null, new NetworkAvailabilityEventArgs(this.GetAvailability()));
			}
		}

		private void OnAddressChanged(object unused)
		{
			NetworkAddressChangedEventHandler addressChanged = this.AddressChanged;
			if (addressChanged != null)
			{
				addressChanged(null, EventArgs.Empty);
			}
		}

		private void OnEventDue(object unused)
		{
			object @lock = this._lock;
			LinuxNetworkChange.EventType eventType;
			lock (@lock)
			{
				eventType = this.pending_events;
				this.pending_events = (LinuxNetworkChange.EventType)0;
				this.timer.Change(-1, -1);
			}
			if ((eventType & LinuxNetworkChange.EventType.Availability) != (LinuxNetworkChange.EventType)0)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnAvailabilityChanged));
			}
			if ((eventType & LinuxNetworkChange.EventType.Address) != (LinuxNetworkChange.EventType)0)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnAddressChanged));
			}
		}

		private void QueueEvent(LinuxNetworkChange.EventType type)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				if (this.timer == null)
				{
					this.timer = new Timer(new TimerCallback(this.OnEventDue));
				}
				if (this.pending_events == (LinuxNetworkChange.EventType)0)
				{
					this.timer.Change(150, -1);
				}
				this.pending_events |= type;
			}
		}

		private unsafe void OnDataAvailable(object sender, SocketAsyncEventArgs args)
		{
			if (this.nl_sock == null)
			{
				return;
			}
			byte[] array;
			byte* ptr;
			if ((array = args.Buffer) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			LinuxNetworkChange.EventType eventType = LinuxNetworkChange.ReadEvents(this.nl_sock.Handle, new IntPtr((void*)ptr), args.BytesTransferred, 8192);
			array = null;
			this.nl_sock.ReceiveAsync(this.nl_args);
			if (eventType != (LinuxNetworkChange.EventType)0)
			{
				this.QueueEvent(eventType);
			}
		}

		private void Register(NetworkAddressChangedEventHandler d)
		{
			this.EnsureSocket();
			this.AddressChanged = (NetworkAddressChangedEventHandler)Delegate.Combine(this.AddressChanged, d);
		}

		private void Register(NetworkAvailabilityChangedEventHandler d)
		{
			this.EnsureSocket();
			this.AvailabilityChanged = (NetworkAvailabilityChangedEventHandler)Delegate.Combine(this.AvailabilityChanged, d);
		}

		private void Unregister(NetworkAddressChangedEventHandler d)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this.AddressChanged = (NetworkAddressChangedEventHandler)Delegate.Remove(this.AddressChanged, d);
				this.MaybeCloseSocket();
			}
		}

		private void Unregister(NetworkAvailabilityChangedEventHandler d)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this.AvailabilityChanged = (NetworkAvailabilityChangedEventHandler)Delegate.Remove(this.AvailabilityChanged, d);
				this.MaybeCloseSocket();
			}
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr CreateNLSocket();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern LinuxNetworkChange.EventType ReadEvents(IntPtr sock, IntPtr buffer, int count, int size);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr CloseNLSocket(IntPtr sock);

		private object _lock = new object();

		private Socket nl_sock;

		private SocketAsyncEventArgs nl_args;

		private LinuxNetworkChange.EventType pending_events;

		private Timer timer;

		private NetworkAddressChangedEventHandler AddressChanged;

		private NetworkAvailabilityChangedEventHandler AvailabilityChanged;

		private const string LIBNAME = "MonoPosixHelper";

		[Flags]
		private enum EventType
		{
			Availability = 1,
			Address = 2
		}
	}
}
