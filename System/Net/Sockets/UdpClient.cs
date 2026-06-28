using System;

namespace System.Net.Sockets
{
	public class UdpClient : IDisposable
	{
		public UdpClient()
			: this(AddressFamily.InterNetwork)
		{
		}

		public UdpClient(AddressFamily family)
		{
			this.family = AddressFamily.InterNetwork;
			base..ctor();
			if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
			}
			this.family = family;
			this.InitSocket(null);
		}

		public UdpClient(int port)
		{
			this.family = AddressFamily.InterNetwork;
			base..ctor();
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.family = AddressFamily.InterNetwork;
			IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Any, port);
			this.InitSocket(ipendPoint);
		}

		public UdpClient(IPEndPoint localEP)
		{
			this.family = AddressFamily.InterNetwork;
			base..ctor();
			if (localEP == null)
			{
				throw new ArgumentNullException("localEP");
			}
			this.family = localEP.AddressFamily;
			this.InitSocket(localEP);
		}

		public UdpClient(int port, AddressFamily family)
		{
			this.family = AddressFamily.InterNetwork;
			base..ctor();
			if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.family = family;
			IPEndPoint ipendPoint;
			if (family == AddressFamily.InterNetwork)
			{
				ipendPoint = new IPEndPoint(IPAddress.Any, port);
			}
			else
			{
				ipendPoint = new IPEndPoint(IPAddress.IPv6Any, port);
			}
			this.InitSocket(ipendPoint);
		}

		public UdpClient(string hostname, int port)
		{
			this.family = AddressFamily.InterNetwork;
			base..ctor();
			if (hostname == null)
			{
				throw new ArgumentNullException("hostname");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.InitSocket(null);
			this.Connect(hostname, port);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void InitSocket(EndPoint localEP)
		{
			if (this.socket != null)
			{
				this.socket.Close();
				this.socket = null;
			}
			this.socket = new Socket(this.family, SocketType.Dgram, ProtocolType.Udp);
			if (localEP != null)
			{
				this.socket.Bind(localEP);
			}
		}

		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		private void DoConnect(IPEndPoint endPoint)
		{
			try
			{
				this.socket.Connect(endPoint);
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode != 10013)
				{
					throw;
				}
				this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
				this.socket.Connect(endPoint);
			}
		}

		public void Connect(IPEndPoint endPoint)
		{
			this.CheckDisposed();
			if (endPoint == null)
			{
				throw new ArgumentNullException("endPoint");
			}
			this.DoConnect(endPoint);
			this.active = true;
		}

		public void Connect(IPAddress addr, int port)
		{
			if (addr == null)
			{
				throw new ArgumentNullException("addr");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.Connect(new IPEndPoint(addr, port));
		}

		public void Connect(string hostname, int port)
		{
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
			for (int i = 0; i < hostAddresses.Length; i++)
			{
				try
				{
					this.family = hostAddresses[i].AddressFamily;
					this.Connect(new IPEndPoint(hostAddresses[i], port));
					break;
				}
				catch (Exception ex)
				{
					if (i == hostAddresses.Length - 1)
					{
						if (this.socket != null)
						{
							this.socket.Close();
							this.socket = null;
						}
						throw ex;
					}
				}
			}
		}

		public void DropMulticastGroup(IPAddress multicastAddr)
		{
			this.CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (this.family == AddressFamily.InterNetwork)
			{
				this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(multicastAddr));
			}
			else
			{
				this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, new IPv6MulticastOption(multicastAddr));
			}
		}

		public void DropMulticastGroup(IPAddress multicastAddr, int ifindex)
		{
			this.CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (this.family == AddressFamily.InterNetworkV6)
			{
				this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, new IPv6MulticastOption(multicastAddr, (long)ifindex));
			}
		}

		public void JoinMulticastGroup(IPAddress multicastAddr)
		{
			this.CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (this.family == AddressFamily.InterNetwork)
			{
				this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr));
			}
			else
			{
				this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(multicastAddr));
			}
		}

		public void JoinMulticastGroup(int ifindex, IPAddress multicastAddr)
		{
			this.CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (this.family == AddressFamily.InterNetworkV6)
			{
				this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(multicastAddr, (long)ifindex));
				return;
			}
			throw new SocketException(10045);
		}

		public void JoinMulticastGroup(IPAddress multicastAddr, int timeToLive)
		{
			this.CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (timeToLive < 0 || timeToLive > 255)
			{
				throw new ArgumentOutOfRangeException("timeToLive");
			}
			this.JoinMulticastGroup(multicastAddr);
			if (this.family == AddressFamily.InterNetwork)
			{
				this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, timeToLive);
			}
			else
			{
				this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, timeToLive);
			}
		}

		public void JoinMulticastGroup(IPAddress multicastAddr, IPAddress localAddress)
		{
			this.CheckDisposed();
			if (this.family == AddressFamily.InterNetwork)
			{
				this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr, localAddress));
				return;
			}
			throw new SocketException(10045);
		}

		public byte[] Receive(ref IPEndPoint remoteEP)
		{
			this.CheckDisposed();
			byte[] array = new byte[65536];
			EndPoint endPoint;
			if (this.family == AddressFamily.InterNetwork)
			{
				endPoint = new IPEndPoint(IPAddress.Any, 0);
			}
			else
			{
				endPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
			}
			int num = this.socket.ReceiveFrom(array, ref endPoint);
			if (num < array.Length)
			{
				array = this.CutArray(array, num);
			}
			remoteEP = (IPEndPoint)endPoint;
			return array;
		}

		private int DoSend(byte[] dgram, int bytes, IPEndPoint endPoint)
		{
			int num;
			try
			{
				if (endPoint == null)
				{
					num = this.socket.Send(dgram, 0, bytes, SocketFlags.None);
				}
				else
				{
					num = this.socket.SendTo(dgram, 0, bytes, SocketFlags.None, endPoint);
				}
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode != 10013)
				{
					throw;
				}
				this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
				if (endPoint == null)
				{
					num = this.socket.Send(dgram, 0, bytes, SocketFlags.None);
				}
				else
				{
					num = this.socket.SendTo(dgram, 0, bytes, SocketFlags.None, endPoint);
				}
			}
			return num;
		}

		public int Send(byte[] dgram, int bytes)
		{
			this.CheckDisposed();
			if (dgram == null)
			{
				throw new ArgumentNullException("dgram");
			}
			if (!this.active)
			{
				throw new InvalidOperationException("Operation not allowed on non-connected sockets.");
			}
			return this.DoSend(dgram, bytes, null);
		}

		public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
		{
			this.CheckDisposed();
			if (dgram == null)
			{
				throw new ArgumentNullException("dgram is null");
			}
			if (!this.active)
			{
				return this.DoSend(dgram, bytes, endPoint);
			}
			if (endPoint != null)
			{
				throw new InvalidOperationException("Cannot send packets to an arbitrary host while connected.");
			}
			return this.DoSend(dgram, bytes, null);
		}

		public int Send(byte[] dgram, int bytes, string hostname, int port)
		{
			return this.Send(dgram, bytes, new IPEndPoint(Dns.GetHostAddresses(hostname)[0], port));
		}

		private byte[] CutArray(byte[] orig, int length)
		{
			byte[] array = new byte[length];
			Buffer.BlockCopy(orig, 0, array, 0, length);
			return array;
		}

		private IAsyncResult DoBeginSend(byte[] datagram, int bytes, IPEndPoint endPoint, AsyncCallback requestCallback, object state)
		{
			IAsyncResult asyncResult;
			try
			{
				if (endPoint == null)
				{
					asyncResult = this.socket.BeginSend(datagram, 0, bytes, SocketFlags.None, requestCallback, state);
				}
				else
				{
					asyncResult = this.socket.BeginSendTo(datagram, 0, bytes, SocketFlags.None, endPoint, requestCallback, state);
				}
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode != 10013)
				{
					throw;
				}
				this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
				if (endPoint == null)
				{
					asyncResult = this.socket.BeginSend(datagram, 0, bytes, SocketFlags.None, requestCallback, state);
				}
				else
				{
					asyncResult = this.socket.BeginSendTo(datagram, 0, bytes, SocketFlags.None, endPoint, requestCallback, state);
				}
			}
			return asyncResult;
		}

		public IAsyncResult BeginSend(byte[] datagram, int bytes, AsyncCallback requestCallback, object state)
		{
			return this.BeginSend(datagram, bytes, null, requestCallback, state);
		}

		public IAsyncResult BeginSend(byte[] datagram, int bytes, IPEndPoint endPoint, AsyncCallback requestCallback, object state)
		{
			this.CheckDisposed();
			if (datagram == null)
			{
				throw new ArgumentNullException("datagram");
			}
			return this.DoBeginSend(datagram, bytes, endPoint, requestCallback, state);
		}

		public IAsyncResult BeginSend(byte[] datagram, int bytes, string hostname, int port, AsyncCallback requestCallback, object state)
		{
			return this.BeginSend(datagram, bytes, new IPEndPoint(Dns.GetHostAddresses(hostname)[0], port), requestCallback, state);
		}

		public int EndSend(IAsyncResult asyncResult)
		{
			this.CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult is a null reference");
			}
			return this.socket.EndSend(asyncResult);
		}

		public IAsyncResult BeginReceive(AsyncCallback callback, object state)
		{
			this.CheckDisposed();
			this.recvbuffer = new byte[8192];
			EndPoint endPoint;
			if (this.family == AddressFamily.InterNetwork)
			{
				endPoint = new IPEndPoint(IPAddress.Any, 0);
			}
			else
			{
				endPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
			}
			return this.socket.BeginReceiveFrom(this.recvbuffer, 0, 8192, SocketFlags.None, ref endPoint, callback, state);
		}

		public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP)
		{
			this.CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult is a null reference");
			}
			EndPoint endPoint;
			if (this.family == AddressFamily.InterNetwork)
			{
				endPoint = new IPEndPoint(IPAddress.Any, 0);
			}
			else
			{
				endPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
			}
			int num = this.socket.EndReceiveFrom(asyncResult, ref endPoint);
			remoteEP = (IPEndPoint)endPoint;
			byte[] array = new byte[num];
			Array.Copy(this.recvbuffer, array, num);
			return array;
		}

		protected bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
			}
		}

		public Socket Client
		{
			get
			{
				return this.socket;
			}
			set
			{
				this.socket = value;
			}
		}

		public int Available
		{
			get
			{
				return this.socket.Available;
			}
		}

		public bool DontFragment
		{
			get
			{
				return this.socket.DontFragment;
			}
			set
			{
				this.socket.DontFragment = value;
			}
		}

		public bool EnableBroadcast
		{
			get
			{
				return this.socket.EnableBroadcast;
			}
			set
			{
				this.socket.EnableBroadcast = value;
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				return this.socket.ExclusiveAddressUse;
			}
			set
			{
				this.socket.ExclusiveAddressUse = value;
			}
		}

		public bool MulticastLoopback
		{
			get
			{
				return this.socket.MulticastLoopback;
			}
			set
			{
				this.socket.MulticastLoopback = value;
			}
		}

		public short Ttl
		{
			get
			{
				return this.socket.Ttl;
			}
			set
			{
				this.socket.Ttl = value;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (disposing)
			{
				if (this.socket != null)
				{
					this.socket.Close();
				}
				this.socket = null;
			}
		}

		~UdpClient()
		{
			this.Dispose(false);
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		private bool disposed;

		private bool active;

		private Socket socket;

		private AddressFamily family;

		private byte[] recvbuffer;
	}
}
