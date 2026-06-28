using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Configuration;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System.Net.Sockets
{
	public class Socket : IDisposable
	{
		private Socket(AddressFamily family, SocketType type, ProtocolType proto, IntPtr sock)
		{
			this.address_family = family;
			this.socket_type = type;
			this.protocol_type = proto;
			this.socket = sock;
			this.connected = true;
		}

		[global::System.MonoTODO]
		public Socket(SocketInformation socketInformation)
		{
			throw new NotImplementedException("SocketInformation not figured out yet");
		}

		public Socket(AddressFamily family, SocketType type, ProtocolType proto)
		{
			this.address_family = family;
			this.socket_type = type;
			this.protocol_type = proto;
			int num;
			this.socket = this.Socket_internal(family, type, proto, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			this.SocketDefaults();
		}

		static Socket()
		{
			Socket.CheckProtocolSupport();
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private static void AddSockets(ArrayList sockets, IList list, string name)
		{
			if (list != null)
			{
				foreach (object obj in list)
				{
					Socket socket = (Socket)obj;
					if (socket == null)
					{
						throw new ArgumentNullException("name", "Contains a null element");
					}
					sockets.Add(socket);
				}
			}
			sockets.Add(null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Select_internal(ref Socket[] sockets, int microSeconds, out int error);

		public static void Select(IList checkRead, IList checkWrite, IList checkError, int microSeconds)
		{
			ArrayList arrayList = new ArrayList();
			Socket.AddSockets(arrayList, checkRead, "checkRead");
			Socket.AddSockets(arrayList, checkWrite, "checkWrite");
			Socket.AddSockets(arrayList, checkError, "checkError");
			if (arrayList.Count == 3)
			{
				throw new ArgumentNullException("checkRead, checkWrite, checkError", "All the lists are null or empty.");
			}
			Socket[] array = (Socket[])arrayList.ToArray(typeof(Socket));
			int num;
			Socket.Select_internal(ref array, microSeconds, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			if (array == null)
			{
				if (checkRead != null)
				{
					checkRead.Clear();
				}
				if (checkWrite != null)
				{
					checkWrite.Clear();
				}
				if (checkError != null)
				{
					checkError.Clear();
				}
				return;
			}
			int num2 = 0;
			int num3 = array.Length;
			IList list = checkRead;
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				Socket socket = array[i];
				if (socket == null)
				{
					if (list != null)
					{
						int num5 = list.Count - num4;
						for (int j = 0; j < num5; j++)
						{
							list.RemoveAt(num4);
						}
					}
					list = ((num2 != 0) ? checkError : checkWrite);
					num4 = 0;
					num2++;
				}
				else
				{
					if (num2 == 1 && list == checkWrite && !socket.connected && (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error) == 0)
					{
						socket.connected = true;
					}
					if (list != null && num4 < list.Count)
					{
						while ((Socket)list[num4] != socket)
						{
							list.RemoveAt(num4);
						}
					}
					num4++;
				}
			}
		}

		private void SocketDefaults()
		{
			try
			{
				if (this.address_family == AddressFamily.InterNetwork)
				{
					this.DontFragment = false;
				}
			}
			catch (SocketException)
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Available_internal(IntPtr socket, out int error);

		public int Available
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				int num2;
				int num = Socket.Available_internal(this.socket, out num2);
				if (num2 != 0)
				{
					throw new SocketException(num2);
				}
				return num;
			}
		}

		public bool DontFragment
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				bool flag;
				if (this.address_family == AddressFamily.InterNetwork)
				{
					flag = (int)this.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment) != 0;
				}
				else
				{
					if (this.address_family != AddressFamily.InterNetworkV6)
					{
						throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
					}
					flag = (int)this.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DontFragment) != 0;
				}
				return flag;
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.address_family == AddressFamily.InterNetwork)
				{
					this.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment, (!value) ? 0 : 1);
				}
				else
				{
					if (this.address_family != AddressFamily.InterNetworkV6)
					{
						throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
					}
					this.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DontFragment, (!value) ? 0 : 1);
				}
			}
		}

		public bool EnableBroadcast
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.protocol_type != ProtocolType.Udp)
				{
					throw new SocketException(10042);
				}
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast) != 0;
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.protocol_type != ProtocolType.Udp)
				{
					throw new SocketException(10042);
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, (!value) ? 0 : 1);
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse) != 0;
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.isbound)
				{
					throw new InvalidOperationException("Bind has already been called for this socket");
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, (!value) ? 0 : 1);
			}
		}

		public bool IsBound
		{
			get
			{
				return this.isbound;
			}
		}

		public LingerOption LingerState
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				return (LingerOption)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
			}
		}

		public bool MulticastLoopback
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.protocol_type == ProtocolType.Tcp)
				{
					throw new SocketException(10042);
				}
				bool flag;
				if (this.address_family == AddressFamily.InterNetwork)
				{
					flag = (int)this.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback) != 0;
				}
				else
				{
					if (this.address_family != AddressFamily.InterNetworkV6)
					{
						throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
					}
					flag = (int)this.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback) != 0;
				}
				return flag;
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.protocol_type == ProtocolType.Tcp)
				{
					throw new SocketException(10042);
				}
				if (this.address_family == AddressFamily.InterNetwork)
				{
					this.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, (!value) ? 0 : 1);
				}
				else
				{
					if (this.address_family != AddressFamily.InterNetworkV6)
					{
						throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
					}
					this.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback, (!value) ? 0 : 1);
				}
			}
		}

		[global::System.MonoTODO("This doesn't do anything on Mono yet")]
		public bool UseOnlyOverlappedIO
		{
			get
			{
				return this.useoverlappedIO;
			}
			set
			{
				this.useoverlappedIO = value;
			}
		}

		public IntPtr Handle
		{
			get
			{
				return this.socket;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SocketAddress LocalEndPoint_internal(IntPtr socket, out int error);

		public EndPoint LocalEndPoint
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.seed_endpoint == null)
				{
					return null;
				}
				int num;
				SocketAddress socketAddress = Socket.LocalEndPoint_internal(this.socket, out num);
				if (num != 0)
				{
					throw new SocketException(num);
				}
				return this.seed_endpoint.Create(socketAddress);
			}
		}

		public SocketType SocketType
		{
			get
			{
				return this.socket_type;
			}
		}

		public int SendTimeout
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified for a set operation is less than -1");
				}
				if (value == -1)
				{
					value = 0;
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified for a set operation is less than -1");
				}
				if (value == -1)
				{
					value = 0;
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
			}
		}

		public bool AcceptAsync(SocketAsyncEventArgs e)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (!this.IsBound)
			{
				throw new InvalidOperationException("You must call the Bind method before performing this operation.");
			}
			if (!this.islistening)
			{
				throw new InvalidOperationException("You must call the Listen method before performing this operation.");
			}
			if (e.BufferList != null)
			{
				throw new ArgumentException("Multiple buffers cannot be used with this method.");
			}
			if (e.Count < 0)
			{
				throw new ArgumentOutOfRangeException("e.Count");
			}
			Socket acceptSocket = e.AcceptSocket;
			if (acceptSocket != null)
			{
				if (acceptSocket.IsBound || acceptSocket.Connected)
				{
					throw new InvalidOperationException("AcceptSocket: The socket must not be bound or connected.");
				}
			}
			else
			{
				e.AcceptSocket = new Socket(this.AddressFamily, this.SocketType, this.ProtocolType);
			}
			try
			{
				e.DoOperation(SocketAsyncOperation.Accept, this);
			}
			catch
			{
				((IDisposable)e).Dispose();
				throw;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Accept_internal(IntPtr sock, out int error, bool blocking);

		public Socket Accept()
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			int num = 0;
			IntPtr intPtr = (IntPtr)(-1);
			this.blocking_thread = Thread.CurrentThread;
			try
			{
				intPtr = Socket.Accept_internal(this.socket, out num, this.blocking);
			}
			catch (ThreadAbortException)
			{
				if (this.disposed)
				{
					Thread.ResetAbort();
					num = 10004;
				}
			}
			finally
			{
				this.blocking_thread = null;
			}
			if (num != 0)
			{
				throw new SocketException(num);
			}
			return new Socket(this.AddressFamily, this.SocketType, this.ProtocolType, intPtr)
			{
				seed_endpoint = this.seed_endpoint,
				Blocking = this.Blocking
			};
		}

		internal void Accept(Socket acceptSocket)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			int num = 0;
			IntPtr intPtr = (IntPtr)(-1);
			this.blocking_thread = Thread.CurrentThread;
			try
			{
				intPtr = Socket.Accept_internal(this.socket, out num, this.blocking);
			}
			catch (ThreadAbortException)
			{
				if (this.disposed)
				{
					Thread.ResetAbort();
					num = 10004;
				}
			}
			finally
			{
				this.blocking_thread = null;
			}
			if (num != 0)
			{
				throw new SocketException(num);
			}
			acceptSocket.address_family = this.AddressFamily;
			acceptSocket.socket_type = this.SocketType;
			acceptSocket.protocol_type = this.ProtocolType;
			acceptSocket.socket = intPtr;
			acceptSocket.connected = true;
			acceptSocket.seed_endpoint = this.seed_endpoint;
			acceptSocket.Blocking = this.Blocking;
		}

		public IAsyncResult BeginAccept(AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (!this.isbound || !this.islistening)
			{
				throw new InvalidOperationException();
			}
			Socket.SocketAsyncResult socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.Accept);
			Socket.Worker worker = new Socket.Worker(socketAsyncResult);
			Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.Accept);
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginAccept(int receiveSize, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (receiveSize < 0)
			{
				throw new ArgumentOutOfRangeException("receiveSize", "receiveSize is less than zero");
			}
			Socket.SocketAsyncResult socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.AcceptReceive);
			Socket.Worker worker = new Socket.Worker(socketAsyncResult);
			Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.AcceptReceive);
			socketAsyncResult.Buffer = new byte[receiveSize];
			socketAsyncResult.Offset = 0;
			socketAsyncResult.Size = receiveSize;
			socketAsyncResult.SockFlags = SocketFlags.None;
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginAccept(Socket acceptSocket, int receiveSize, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (receiveSize < 0)
			{
				throw new ArgumentOutOfRangeException("receiveSize", "receiveSize is less than zero");
			}
			if (acceptSocket != null)
			{
				if (acceptSocket.disposed && acceptSocket.closed)
				{
					throw new ObjectDisposedException(acceptSocket.GetType().ToString());
				}
				if (acceptSocket.IsBound)
				{
					throw new InvalidOperationException();
				}
				if (acceptSocket.ProtocolType != ProtocolType.Tcp)
				{
					throw new SocketException(10022);
				}
			}
			Socket.SocketAsyncResult socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.AcceptReceive);
			Socket.Worker worker = new Socket.Worker(socketAsyncResult);
			Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.AcceptReceive);
			socketAsyncResult.Buffer = new byte[receiveSize];
			socketAsyncResult.Offset = 0;
			socketAsyncResult.Size = receiveSize;
			socketAsyncResult.SockFlags = SocketFlags.None;
			socketAsyncResult.AcceptSocket = acceptSocket;
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginConnect(EndPoint end_point, AsyncCallback callback, object state)
		{
			return this.BeginConnect(end_point, callback, state, false);
		}

		internal IAsyncResult BeginConnect(EndPoint end_point, AsyncCallback callback, object state, bool bypassSocketSecurity)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (end_point == null)
			{
				throw new ArgumentNullException("end_point");
			}
			Socket.SocketAsyncResult socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.Connect);
			socketAsyncResult.EndPoint = end_point;
			if (end_point is IPEndPoint)
			{
				IPEndPoint ipendPoint = (IPEndPoint)end_point;
				if (ipendPoint.Address.Equals(IPAddress.Any) || ipendPoint.Address.Equals(IPAddress.IPv6Any))
				{
					socketAsyncResult.Complete(new SocketException(10049), true);
					return socketAsyncResult;
				}
			}
			int num = 0;
			if (!this.blocking)
			{
				SocketAddress socketAddress = end_point.Serialize();
				Socket.Connect_internal(this.socket, socketAddress, out num);
				if (num == 0)
				{
					this.connected = true;
					socketAsyncResult.Complete(true);
				}
				else if (num != 10036 && num != 10035)
				{
					this.connected = false;
					socketAsyncResult.Complete(new SocketException(num), true);
				}
			}
			if (this.blocking || num == 10036 || num == 10035)
			{
				this.connected = false;
				Socket.Worker worker = new Socket.Worker(socketAsyncResult, bypassSocketSecurity);
				Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.Connect);
				socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			}
			return socketAsyncResult;
		}

		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.ToString().Length == 0)
			{
				throw new ArgumentException("The length of the IP address is zero");
			}
			if (this.islistening)
			{
				throw new InvalidOperationException();
			}
			IPEndPoint ipendPoint = new IPEndPoint(address, port);
			return this.BeginConnect(ipendPoint, callback, state);
		}

		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (addresses == null)
			{
				throw new ArgumentNullException("addresses");
			}
			if (this.AddressFamily != AddressFamily.InterNetwork && this.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException("This method is only valid for addresses in the InterNetwork or InterNetworkV6 families");
			}
			if (this.islistening)
			{
				throw new InvalidOperationException();
			}
			Socket.SocketAsyncResult socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.Connect);
			socketAsyncResult.Addresses = addresses;
			socketAsyncResult.Port = port;
			this.connected = false;
			Socket.Worker worker = new Socket.Worker(socketAsyncResult);
			Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.Connect);
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginConnect(string host, int port, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			if (this.address_family != AddressFamily.InterNetwork && this.address_family != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException("This method is valid only for sockets in the InterNetwork and InterNetworkV6 families");
			}
			if (this.islistening)
			{
				throw new InvalidOperationException();
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
			return this.BeginConnect(hostAddresses, port, callback, state);
		}

		public IAsyncResult BeginDisconnect(bool reuseSocket, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			Socket.SocketAsyncResult socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.Disconnect);
			socketAsyncResult.ReuseSocket = reuseSocket;
			Socket.Worker worker = new Socket.Worker(socketAsyncResult);
			Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.Disconnect);
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socket_flags, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			Queue queue = this.readQ;
			Socket.SocketAsyncResult socketAsyncResult;
			lock (queue)
			{
				socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.Receive);
				socketAsyncResult.Buffer = buffer;
				socketAsyncResult.Offset = offset;
				socketAsyncResult.Size = size;
				socketAsyncResult.SockFlags = socket_flags;
				this.readQ.Enqueue(socketAsyncResult);
				if (this.readQ.Count == 1)
				{
					Socket.Worker worker = new Socket.Worker(socketAsyncResult);
					Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.Receive);
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags flags, out SocketError error, AsyncCallback callback, object state)
		{
			error = SocketError.Success;
			return this.BeginReceive(buffer, offset, size, flags, callback, state);
		}

		[CLSCompliant(false)]
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			Queue queue = this.readQ;
			Socket.SocketAsyncResult socketAsyncResult;
			lock (queue)
			{
				socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.ReceiveGeneric);
				socketAsyncResult.Buffers = buffers;
				socketAsyncResult.SockFlags = socketFlags;
				this.readQ.Enqueue(socketAsyncResult);
				if (this.readQ.Count == 1)
				{
					Socket.Worker worker = new Socket.Worker(socketAsyncResult);
					Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.ReceiveGeneric);
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		[CLSCompliant(false)]
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			errorCode = SocketError.Success;
			return this.BeginReceive(buffers, socketFlags, callback, state);
		}

		public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socket_flags, ref EndPoint remote_end, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be >= 0");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", "size must be >= 0");
			}
			if (offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset, size", "offset + size exceeds the buffer length");
			}
			Queue queue = this.readQ;
			Socket.SocketAsyncResult socketAsyncResult;
			lock (queue)
			{
				socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.ReceiveFrom);
				socketAsyncResult.Buffer = buffer;
				socketAsyncResult.Offset = offset;
				socketAsyncResult.Size = size;
				socketAsyncResult.SockFlags = socket_flags;
				socketAsyncResult.EndPoint = remote_end;
				this.readQ.Enqueue(socketAsyncResult);
				if (this.readQ.Count == 1)
				{
					Socket.Worker worker = new Socket.Worker(socketAsyncResult);
					Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.ReceiveFrom);
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		[global::System.MonoTODO]
		public IAsyncResult BeginReceiveMessageFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			throw new NotImplementedException();
		}

		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socket_flags, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be >= 0");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", "size must be >= 0");
			}
			if (offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset, size", "offset + size exceeds the buffer length");
			}
			if (!this.connected)
			{
				throw new SocketException(10057);
			}
			Queue queue = this.writeQ;
			Socket.SocketAsyncResult socketAsyncResult;
			lock (queue)
			{
				socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.Send);
				socketAsyncResult.Buffer = buffer;
				socketAsyncResult.Offset = offset;
				socketAsyncResult.Size = size;
				socketAsyncResult.SockFlags = socket_flags;
				this.writeQ.Enqueue(socketAsyncResult);
				if (this.writeQ.Count == 1)
				{
					Socket.Worker worker = new Socket.Worker(socketAsyncResult);
					Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.Send);
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			if (!this.connected)
			{
				errorCode = SocketError.NotConnected;
				throw new SocketException((int)errorCode);
			}
			errorCode = SocketError.Success;
			return this.BeginSend(buffer, offset, size, socketFlags, callback, state);
		}

		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			if (!this.connected)
			{
				throw new SocketException(10057);
			}
			Queue queue = this.writeQ;
			Socket.SocketAsyncResult socketAsyncResult;
			lock (queue)
			{
				socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.SendGeneric);
				socketAsyncResult.Buffers = buffers;
				socketAsyncResult.SockFlags = socketFlags;
				this.writeQ.Enqueue(socketAsyncResult);
				if (this.writeQ.Count == 1)
				{
					Socket.Worker worker = new Socket.Worker(socketAsyncResult);
					Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.SendGeneric);
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		[CLSCompliant(false)]
		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			if (!this.connected)
			{
				errorCode = SocketError.NotConnected;
				throw new SocketException((int)errorCode);
			}
			errorCode = SocketError.Success;
			return this.BeginSend(buffers, socketFlags, callback, state);
		}

		public IAsyncResult BeginSendFile(string fileName, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (!this.connected)
			{
				throw new NotSupportedException();
			}
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException();
			}
			return this.BeginSendFile(fileName, null, null, TransmitFileOptions.UseDefaultWorkerThread, callback, state);
		}

		public IAsyncResult BeginSendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (!this.connected)
			{
				throw new NotSupportedException();
			}
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException();
			}
			Socket.SendFileHandler sendFileHandler = new Socket.SendFileHandler(this.SendFile);
			return new Socket.SendFileAsyncResult(sendFileHandler, sendFileHandler.BeginInvoke(fileName, preBuffer, postBuffer, flags, callback, state));
		}

		public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, SocketFlags socket_flags, EndPoint remote_end, AsyncCallback callback, object state)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be >= 0");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", "size must be >= 0");
			}
			if (offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset, size", "offset + size exceeds the buffer length");
			}
			Queue queue = this.writeQ;
			Socket.SocketAsyncResult socketAsyncResult;
			lock (queue)
			{
				socketAsyncResult = new Socket.SocketAsyncResult(this, state, callback, Socket.SocketOperation.SendTo);
				socketAsyncResult.Buffer = buffer;
				socketAsyncResult.Offset = offset;
				socketAsyncResult.Size = size;
				socketAsyncResult.SockFlags = socket_flags;
				socketAsyncResult.EndPoint = remote_end;
				this.writeQ.Enqueue(socketAsyncResult);
				if (this.writeQ.Count == 1)
				{
					Socket.Worker worker = new Socket.Worker(socketAsyncResult);
					Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(worker.SendTo);
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Bind_internal(IntPtr sock, SocketAddress sa, out int error);

		public void Bind(EndPoint local_end)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (local_end == null)
			{
				throw new ArgumentNullException("local_end");
			}
			if (Environment.SocketSecurityEnabled && Socket.current_bind_count >= this.max_bind_count)
			{
				throw new SecurityException("Too many sockets are bound, maximum count in the webplayer is " + this.max_bind_count);
			}
			int num;
			Socket.Bind_internal(this.socket, local_end.Serialize(), out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			if (num == 0)
			{
				this.isbound = true;
			}
			if (Environment.SocketSecurityEnabled)
			{
				Socket.current_bind_count++;
			}
			this.seed_endpoint = local_end;
		}

		public bool ConnectAsync(SocketAsyncEventArgs e)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (this.islistening)
			{
				throw new InvalidOperationException("You may not perform this operation after calling the Listen method.");
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP", "Value cannot be null.");
			}
			if (e.BufferList != null)
			{
				throw new ArgumentException("Multiple buffers cannot be used with this method.");
			}
			e.DoOperation(SocketAsyncOperation.Connect, this);
			return true;
		}

		public void Connect(IPAddress address, int port)
		{
			this.Connect(new IPEndPoint(address, port));
		}

		public void Connect(IPAddress[] addresses, int port)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (addresses == null)
			{
				throw new ArgumentNullException("addresses");
			}
			if (this.AddressFamily != AddressFamily.InterNetwork && this.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException("This method is only valid for addresses in the InterNetwork or InterNetworkV6 families");
			}
			if (this.islistening)
			{
				throw new InvalidOperationException();
			}
			int num = 0;
			foreach (IPAddress ipaddress in addresses)
			{
				IPEndPoint ipendPoint = new IPEndPoint(ipaddress, port);
				SocketAddress socketAddress = ipendPoint.Serialize();
				Socket.Connect_internal(this.socket, socketAddress, out num);
				if (num == 0)
				{
					this.connected = true;
					this.seed_endpoint = ipendPoint;
					return;
				}
				if (num == 10036 || num == 10035)
				{
					if (!this.blocking)
					{
						this.Poll(-1, SelectMode.SelectWrite);
						num = (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);
						if (num == 0)
						{
							this.connected = true;
							this.seed_endpoint = ipendPoint;
							return;
						}
					}
				}
			}
			if (num != 0)
			{
				throw new SocketException(num);
			}
		}

		public void Connect(string host, int port)
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
			this.Connect(hostAddresses, port);
		}

		public bool DisconnectAsync(SocketAsyncEventArgs e)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			e.DoOperation(SocketAsyncOperation.Disconnect, this);
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Disconnect_internal(IntPtr sock, bool reuse, out int error);

		public void Disconnect(bool reuseSocket)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			int num = 0;
			Socket.Disconnect_internal(this.socket, reuseSocket, out num);
			if (num == 0)
			{
				this.connected = false;
				if (reuseSocket)
				{
				}
				return;
			}
			if (num == 50)
			{
				throw new PlatformNotSupportedException();
			}
			throw new SocketException(num);
		}

		[global::System.MonoTODO("Not implemented")]
		public SocketInformation DuplicateAndClose(int targetProcessId)
		{
			throw new NotImplementedException();
		}

		public Socket EndAccept(IAsyncResult result)
		{
			byte[] array;
			int num;
			return this.EndAccept(out array, out num, result);
		}

		public Socket EndAccept(out byte[] buffer, IAsyncResult asyncResult)
		{
			int num;
			return this.EndAccept(out buffer, out num, asyncResult);
		}

		public Socket EndAccept(out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Socket.SocketAsyncResult socketAsyncResult = asyncResult as Socket.SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw this.InvalidAsyncOp("EndAccept");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
			buffer = socketAsyncResult.Buffer;
			bytesTransferred = socketAsyncResult.Total;
			return socketAsyncResult.Socket;
		}

		public void EndConnect(IAsyncResult result)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			Socket.SocketAsyncResult socketAsyncResult = result as Socket.SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "result");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw this.InvalidAsyncOp("EndConnect");
			}
			if (!result.IsCompleted)
			{
				result.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
		}

		public void EndDisconnect(IAsyncResult asyncResult)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Socket.SocketAsyncResult socketAsyncResult = asyncResult as Socket.SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw this.InvalidAsyncOp("EndDisconnect");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
		}

		public int EndReceive(IAsyncResult result)
		{
			SocketError socketError;
			return this.EndReceive(result, out socketError);
		}

		public int EndReceive(IAsyncResult asyncResult, out SocketError errorCode)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Socket.SocketAsyncResult socketAsyncResult = asyncResult as Socket.SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw this.InvalidAsyncOp("EndReceive");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			errorCode = socketAsyncResult.ErrorCode;
			socketAsyncResult.CheckIfThrowDelayedException();
			return socketAsyncResult.Total;
		}

		public int EndReceiveFrom(IAsyncResult result, ref EndPoint end_point)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			Socket.SocketAsyncResult socketAsyncResult = result as Socket.SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "result");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw this.InvalidAsyncOp("EndReceiveFrom");
			}
			if (!result.IsCompleted)
			{
				result.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
			end_point = socketAsyncResult.EndPoint;
			return socketAsyncResult.Total;
		}

		[global::System.MonoTODO]
		public int EndReceiveMessageFrom(IAsyncResult asyncResult, ref SocketFlags socketFlags, ref EndPoint endPoint, out IPPacketInformation ipPacketInformation)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (endPoint == null)
			{
				throw new ArgumentNullException("endPoint");
			}
			Socket.SocketAsyncResult socketAsyncResult = asyncResult as Socket.SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw this.InvalidAsyncOp("EndReceiveMessageFrom");
			}
			throw new NotImplementedException();
		}

		public int EndSend(IAsyncResult result)
		{
			SocketError socketError;
			return this.EndSend(result, out socketError);
		}

		public int EndSend(IAsyncResult asyncResult, out SocketError errorCode)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Socket.SocketAsyncResult socketAsyncResult = asyncResult as Socket.SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "result");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw this.InvalidAsyncOp("EndSend");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			errorCode = socketAsyncResult.ErrorCode;
			socketAsyncResult.CheckIfThrowDelayedException();
			return socketAsyncResult.Total;
		}

		public void EndSendFile(IAsyncResult asyncResult)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Socket.SendFileAsyncResult sendFileAsyncResult = asyncResult as Socket.SendFileAsyncResult;
			if (sendFileAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			sendFileAsyncResult.Delegate.EndInvoke(sendFileAsyncResult.Original);
		}

		private Exception InvalidAsyncOp(string method)
		{
			return new InvalidOperationException(method + " can only be called once per asynchronous operation");
		}

		public int EndSendTo(IAsyncResult result)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			Socket.SocketAsyncResult socketAsyncResult = result as Socket.SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "result");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw this.InvalidAsyncOp("EndSendTo");
			}
			if (!result.IsCompleted)
			{
				result.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
			return socketAsyncResult.Total;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSocketOption_arr_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, ref byte[] byte_val, out int error);

		public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (optionValue == null)
			{
				throw new SocketException(10014, "Error trying to dereference an invalid pointer");
			}
			int num;
			Socket.GetSocketOption_arr_internal(this.socket, optionLevel, optionName, ref optionValue, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
		}

		public byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int length)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			byte[] array = new byte[length];
			int num;
			Socket.GetSocketOption_arr_internal(this.socket, optionLevel, optionName, ref array, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int WSAIoctl(IntPtr sock, int ioctl_code, byte[] input, byte[] output, out int error);

		public int IOControl(int ioctl_code, byte[] in_value, byte[] out_value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			int num2;
			int num = Socket.WSAIoctl(this.socket, ioctl_code, in_value, out_value, out num2);
			if (num2 != 0)
			{
				throw new SocketException(num2);
			}
			if (num == -1)
			{
				throw new InvalidOperationException("Must use Blocking property instead.");
			}
			return num;
		}

		[global::System.MonoTODO]
		public int IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Listen_internal(IntPtr sock, int backlog, out int error);

		public void Listen(int backlog)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (!this.isbound)
			{
				throw new SocketException(10022);
			}
			if (Environment.SocketSecurityEnabled)
			{
				SecurityException ex = new SecurityException("Listening on TCP sockets is not allowed in the webplayer");
				Console.WriteLine("Throwing the following securityexception: " + ex);
				throw ex;
			}
			int num;
			Socket.Listen_internal(this.socket, backlog, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			this.islistening = true;
		}

		public bool Poll(int time_us, SelectMode mode)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (mode != SelectMode.SelectRead && mode != SelectMode.SelectWrite && mode != SelectMode.SelectError)
			{
				throw new NotSupportedException("'mode' parameter is not valid.");
			}
			int num;
			bool flag = Socket.Poll_internal(this.socket, mode, time_us, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			if (mode == SelectMode.SelectWrite && flag && !this.connected && (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error) == 0)
			{
				this.connected = true;
			}
			return flag;
		}

		public int Receive(byte[] buffer)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			SocketError socketError;
			int num = this.Receive_nochecks(buffer, 0, buffer.Length, SocketFlags.None, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException((int)socketError);
			}
			return num;
		}

		public int Receive(byte[] buffer, SocketFlags flags)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			SocketError socketError;
			int num = this.Receive_nochecks(buffer, 0, buffer.Length, flags, out socketError);
			if (socketError == SocketError.Success)
			{
				return num;
			}
			if (socketError == SocketError.WouldBlock && this.blocking)
			{
				throw new SocketException((int)socketError, "Operation timed out.");
			}
			throw new SocketException((int)socketError);
		}

		public int Receive(byte[] buffer, int size, SocketFlags flags)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (size < 0 || size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			SocketError socketError;
			int num = this.Receive_nochecks(buffer, 0, size, flags, out socketError);
			if (socketError == SocketError.Success)
			{
				return num;
			}
			if (socketError == SocketError.WouldBlock && this.blocking)
			{
				throw new SocketException((int)socketError, "Operation timed out.");
			}
			throw new SocketException((int)socketError);
		}

		public int Receive(byte[] buffer, int offset, int size, SocketFlags flags)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			SocketError socketError;
			int num = this.Receive_nochecks(buffer, offset, size, flags, out socketError);
			if (socketError == SocketError.Success)
			{
				return num;
			}
			if (socketError == SocketError.WouldBlock && this.blocking)
			{
				throw new SocketException((int)socketError, "Operation timed out.");
			}
			throw new SocketException((int)socketError);
		}

		public int Receive(byte[] buffer, int offset, int size, SocketFlags flags, out SocketError error)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return this.Receive_nochecks(buffer, offset, size, flags, out error);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Receive_internal(IntPtr sock, Socket.WSABUF[] bufarray, SocketFlags flags, out int error);

		public int Receive(IList<ArraySegment<byte>> buffers)
		{
			SocketError socketError;
			int num = this.Receive(buffers, SocketFlags.None, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException((int)socketError);
			}
			return num;
		}

		[CLSCompliant(false)]
		public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			SocketError socketError;
			int num = this.Receive(buffers, socketFlags, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException((int)socketError);
			}
			return num;
		}

		[CLSCompliant(false)]
		public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffers == null || buffers.Count == 0)
			{
				throw new ArgumentNullException("buffers");
			}
			int count = buffers.Count;
			Socket.WSABUF[] array = new Socket.WSABUF[count];
			GCHandle[] array2 = new GCHandle[count];
			for (int i = 0; i < count; i++)
			{
				ArraySegment<byte> arraySegment = buffers[i];
				array2[i] = GCHandle.Alloc(arraySegment.Array, GCHandleType.Pinned);
				array[i].len = arraySegment.Count;
				array[i].buf = Marshal.UnsafeAddrOfPinnedArrayElement(arraySegment.Array, arraySegment.Offset);
			}
			int num2;
			int num;
			try
			{
				num = Socket.Receive_internal(this.socket, array, socketFlags, out num2);
			}
			finally
			{
				for (int j = 0; j < count; j++)
				{
					if (array2[j].IsAllocated)
					{
						array2[j].Free();
					}
				}
			}
			errorCode = (SocketError)num2;
			return num;
		}

		public bool ReceiveFromAsync(SocketAsyncEventArgs e)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (e.BufferList != null)
			{
				throw new NotSupportedException("Mono doesn't support using BufferList at this point.");
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP", "Value cannot be null.");
			}
			e.DoOperation(SocketAsyncOperation.ReceiveFrom, this);
			return true;
		}

		public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			return this.ReceiveFrom_nochecks(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP);
		}

		public int ReceiveFrom(byte[] buffer, SocketFlags flags, ref EndPoint remoteEP)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			return this.ReceiveFrom_nochecks(buffer, 0, buffer.Length, flags, ref remoteEP);
		}

		public int ReceiveFrom(byte[] buffer, int size, SocketFlags flags, ref EndPoint remoteEP)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			if (size < 0 || size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return this.ReceiveFrom_nochecks(buffer, 0, size, flags, ref remoteEP);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RecvFrom_internal(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, ref SocketAddress sockaddr, out int error);

		public int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags flags, ref EndPoint remoteEP)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return this.ReceiveFrom_nochecks(buffer, offset, size, flags, ref remoteEP);
		}

		internal int ReceiveFrom_nochecks(byte[] buf, int offset, int size, SocketFlags flags, ref EndPoint remote_end)
		{
			int num;
			return this.ReceiveFrom_nochecks_exc(buf, offset, size, flags, ref remote_end, true, out num);
		}

		internal int ReceiveFrom_nochecks_exc(byte[] buf, int offset, int size, SocketFlags flags, ref EndPoint remote_end, bool throwOnError, out int error)
		{
			SocketAddress socketAddress = remote_end.Serialize();
			int num = Socket.RecvFrom_internal(this.socket, buf, offset, size, flags, ref socketAddress, out error);
			SocketError socketError = (SocketError)error;
			if (socketError != SocketError.Success)
			{
				if (socketError != SocketError.WouldBlock && socketError != SocketError.InProgress)
				{
					this.connected = false;
				}
				else if (socketError == SocketError.WouldBlock && this.blocking)
				{
					if (throwOnError)
					{
						throw new SocketException(10060, "Operation timed out");
					}
					error = 10060;
					return 0;
				}
				if (throwOnError)
				{
					throw new SocketException(error);
				}
				return 0;
			}
			else
			{
				if (Environment.SocketSecurityEnabled && !Socket.CheckEndPoint(socketAddress))
				{
					buf.Initialize();
					throw new SecurityException("Unable to connect, as no valid crossdomain policy was found");
				}
				this.connected = true;
				this.isbound = true;
				if (socketAddress != null)
				{
					remote_end = remote_end.Create(socketAddress);
				}
				this.seed_endpoint = remote_end;
				return num;
			}
		}

		[global::System.MonoTODO("Not implemented")]
		public bool ReceiveMessageFromAsync(SocketAsyncEventArgs e)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			throw new NotImplementedException();
		}

		[global::System.MonoTODO("Not implemented")]
		public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			throw new NotImplementedException();
		}

		[global::System.MonoTODO("Not implemented")]
		public bool SendPacketsAsync(SocketAsyncEventArgs e)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			throw new NotImplementedException();
		}

		public int Send(byte[] buf)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			SocketError socketError;
			int num = this.Send_nochecks(buf, 0, buf.Length, SocketFlags.None, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException((int)socketError);
			}
			return num;
		}

		public int Send(byte[] buf, SocketFlags flags)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			SocketError socketError;
			int num = this.Send_nochecks(buf, 0, buf.Length, flags, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException((int)socketError);
			}
			return num;
		}

		public int Send(byte[] buf, int size, SocketFlags flags)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			if (size < 0 || size > buf.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			SocketError socketError;
			int num = this.Send_nochecks(buf, 0, size, flags, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException((int)socketError);
			}
			return num;
		}

		public int Send(byte[] buf, int offset, int size, SocketFlags flags)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buf.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buf.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			SocketError socketError;
			int num = this.Send_nochecks(buf, offset, size, flags, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException((int)socketError);
			}
			return num;
		}

		public int Send(byte[] buf, int offset, int size, SocketFlags flags, out SocketError error)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buf.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buf.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return this.Send_nochecks(buf, offset, size, flags, out error);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Send_internal(IntPtr sock, Socket.WSABUF[] bufarray, SocketFlags flags, out int error);

		public int Send(IList<ArraySegment<byte>> buffers)
		{
			SocketError socketError;
			int num = this.Send(buffers, SocketFlags.None, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException((int)socketError);
			}
			return num;
		}

		public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			SocketError socketError;
			int num = this.Send(buffers, socketFlags, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException((int)socketError);
			}
			return num;
		}

		[CLSCompliant(false)]
		public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			if (buffers.Count == 0)
			{
				throw new ArgumentException("Buffer is empty", "buffers");
			}
			int count = buffers.Count;
			Socket.WSABUF[] array = new Socket.WSABUF[count];
			GCHandle[] array2 = new GCHandle[count];
			for (int i = 0; i < count; i++)
			{
				ArraySegment<byte> arraySegment = buffers[i];
				array2[i] = GCHandle.Alloc(arraySegment.Array, GCHandleType.Pinned);
				array[i].len = arraySegment.Count;
				array[i].buf = Marshal.UnsafeAddrOfPinnedArrayElement(arraySegment.Array, arraySegment.Offset);
			}
			int num2;
			int num;
			try
			{
				num = Socket.Send_internal(this.socket, array, socketFlags, out num2);
			}
			finally
			{
				for (int j = 0; j < count; j++)
				{
					if (array2[j].IsAllocated)
					{
						array2[j].Free();
					}
				}
			}
			errorCode = (SocketError)num2;
			return num;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SendFile(IntPtr sock, string filename, byte[] pre_buffer, byte[] post_buffer, TransmitFileOptions flags);

		public void SendFile(string fileName)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (!this.connected)
			{
				throw new NotSupportedException();
			}
			if (!this.blocking)
			{
				throw new InvalidOperationException();
			}
			this.SendFile(fileName, null, null, TransmitFileOptions.UseDefaultWorkerThread);
		}

		public void SendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (!this.connected)
			{
				throw new NotSupportedException();
			}
			if (!this.blocking)
			{
				throw new InvalidOperationException();
			}
			if (Socket.SendFile(this.socket, fileName, preBuffer, postBuffer, flags))
			{
				return;
			}
			SocketException ex = new SocketException();
			if (ex.ErrorCode == 2 || ex.ErrorCode == 3)
			{
				throw new FileNotFoundException();
			}
			throw ex;
		}

		public bool SendToAsync(SocketAsyncEventArgs e)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP", "Value cannot be null.");
			}
			e.DoOperation(SocketAsyncOperation.SendTo, this);
			return true;
		}

		public int SendTo(byte[] buffer, EndPoint remote_end)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remote_end == null)
			{
				throw new ArgumentNullException("remote_end");
			}
			return this.SendTo_nochecks(buffer, 0, buffer.Length, SocketFlags.None, remote_end);
		}

		public int SendTo(byte[] buffer, SocketFlags flags, EndPoint remote_end)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remote_end == null)
			{
				throw new ArgumentNullException("remote_end");
			}
			return this.SendTo_nochecks(buffer, 0, buffer.Length, flags, remote_end);
		}

		public int SendTo(byte[] buffer, int size, SocketFlags flags, EndPoint remote_end)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remote_end == null)
			{
				throw new ArgumentNullException("remote_end");
			}
			if (size < 0 || size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return this.SendTo_nochecks(buffer, 0, size, flags, remote_end);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SendTo_internal_real(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, SocketAddress sa, out int error);

		private static int SendTo_internal(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, SocketAddress sa, out int error)
		{
			if (Environment.SocketSecurityEnabled && !Socket.CheckEndPoint(sa))
			{
				SecurityException ex = new SecurityException("SendTo request refused by Unity webplayer security model");
				Console.WriteLine("Throwing the following security exception: " + ex);
				throw ex;
			}
			return Socket.SendTo_internal_real(sock, buffer, offset, count, flags, sa, out error);
		}

		public int SendTo(byte[] buffer, int offset, int size, SocketFlags flags, EndPoint remote_end)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remote_end == null)
			{
				throw new ArgumentNullException("remote_end");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return this.SendTo_nochecks(buffer, offset, size, flags, remote_end);
		}

		internal int SendTo_nochecks(byte[] buffer, int offset, int size, SocketFlags flags, EndPoint remote_end)
		{
			SocketAddress socketAddress = remote_end.Serialize();
			int num2;
			int num = Socket.SendTo_internal(this.socket, buffer, offset, size, flags, socketAddress, out num2);
			SocketError socketError = (SocketError)num2;
			if (socketError != SocketError.Success)
			{
				if (socketError != SocketError.WouldBlock && socketError != SocketError.InProgress)
				{
					this.connected = false;
				}
				throw new SocketException(num2);
			}
			this.connected = true;
			this.isbound = true;
			this.seed_endpoint = remote_end;
			return num;
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (optionValue == null)
			{
				throw new SocketException(10014, "Error trying to dereference an invalid pointer");
			}
			int num;
			Socket.SetSocketOption_internal(this.socket, optionLevel, optionName, null, optionValue, 0, out num);
			if (num == 0)
			{
				return;
			}
			if (num == 10022)
			{
				throw new ArgumentException();
			}
			throw new SocketException(num);
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (optionValue == null)
			{
				throw new ArgumentNullException("optionValue");
			}
			int num;
			if (optionLevel == SocketOptionLevel.Socket && optionName == SocketOptionName.Linger)
			{
				LingerOption lingerOption = optionValue as LingerOption;
				if (lingerOption == null)
				{
					throw new ArgumentException("A 'LingerOption' value must be specified.", "optionValue");
				}
				Socket.SetSocketOption_internal(this.socket, optionLevel, optionName, lingerOption, null, 0, out num);
			}
			else if (optionLevel == SocketOptionLevel.IP && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
			{
				MulticastOption multicastOption = optionValue as MulticastOption;
				if (multicastOption == null)
				{
					throw new ArgumentException("A 'MulticastOption' value must be specified.", "optionValue");
				}
				Socket.SetSocketOption_internal(this.socket, optionLevel, optionName, multicastOption, null, 0, out num);
			}
			else
			{
				if (optionLevel != SocketOptionLevel.IPv6 || (optionName != SocketOptionName.AddMembership && optionName != SocketOptionName.DropMembership))
				{
					throw new ArgumentException("Invalid value specified.", "optionValue");
				}
				IPv6MulticastOption pv6MulticastOption = optionValue as IPv6MulticastOption;
				if (pv6MulticastOption == null)
				{
					throw new ArgumentException("A 'IPv6MulticastOption' value must be specified.", "optionValue");
				}
				Socket.SetSocketOption_internal(this.socket, optionLevel, optionName, pv6MulticastOption, null, 0, out num);
			}
			if (num == 0)
			{
				return;
			}
			if (num == 10022)
			{
				throw new ArgumentException();
			}
			throw new SocketException(num);
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			int num = ((!optionValue) ? 0 : 1);
			int num2;
			Socket.SetSocketOption_internal(this.socket, optionLevel, optionName, null, null, num, out num2);
			if (num2 == 0)
			{
				return;
			}
			if (num2 == 10022)
			{
				throw new ArgumentException();
			}
			throw new SocketException(num2);
		}

		internal static void CheckProtocolSupport()
		{
			if (Socket.ipv4Supported == -1)
			{
				try
				{
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Close();
					Socket.ipv4Supported = 1;
				}
				catch
				{
					Socket.ipv4Supported = 0;
				}
			}
			if (Socket.ipv6Supported == -1)
			{
				global::System.Net.Configuration.SettingsSection settingsSection = (global::System.Net.Configuration.SettingsSection)ConfigurationManager.GetSection("system.net/settings");
				if (settingsSection != null)
				{
					Socket.ipv6Supported = ((!settingsSection.Ipv6.Enabled) ? 0 : (-1));
				}
				if (Socket.ipv6Supported != 0)
				{
					try
					{
						Socket socket2 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
						socket2.Close();
						Socket.ipv6Supported = 1;
					}
					catch
					{
						Socket.ipv6Supported = 0;
					}
				}
			}
		}

		public static bool SupportsIPv4
		{
			get
			{
				Socket.CheckProtocolSupport();
				return Socket.ipv4Supported == 1;
			}
		}

		[Obsolete("Use OSSupportsIPv6 instead")]
		public static bool SupportsIPv6
		{
			get
			{
				Socket.CheckProtocolSupport();
				return Socket.ipv6Supported == 1;
			}
		}

		public static bool OSSupportsIPv6
		{
			get
			{
				global::System.Net.NetworkInformation.NetworkInterface[] allNetworkInterfaces = global::System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
				foreach (global::System.Net.NetworkInformation.NetworkInterface networkInterface in allNetworkInterfaces)
				{
					if (networkInterface.Supports(global::System.Net.NetworkInformation.NetworkInterfaceComponent.IPv6))
					{
						return true;
					}
				}
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr Socket_internal(AddressFamily family, SocketType type, ProtocolType proto, out int error);

		~Socket()
		{
			this.Dispose(false);
		}

		public AddressFamily AddressFamily
		{
			get
			{
				return this.address_family;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Blocking_internal(IntPtr socket, bool block, out int error);

		public bool Blocking
		{
			get
			{
				return this.blocking;
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				int num;
				Socket.Blocking_internal(this.socket, value, out num);
				if (num != 0)
				{
					throw new SocketException(num);
				}
				this.blocking = value;
			}
		}

		public bool Connected
		{
			get
			{
				return this.connected;
			}
			internal set
			{
				this.connected = value;
			}
		}

		public ProtocolType ProtocolType
		{
			get
			{
				return this.protocol_type;
			}
		}

		public bool NoDelay
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				this.ThrowIfUpd();
				return (int)this.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug) != 0;
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				this.ThrowIfUpd();
				this.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, (!value) ? 0 : 1);
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified for a set operation is less than zero");
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
			}
		}

		public int SendBufferSize
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified for a set operation is less than zero");
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
			}
		}

		public short Ttl
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				short num;
				if (this.address_family == AddressFamily.InterNetwork)
				{
					num = (short)((int)this.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress));
				}
				else
				{
					if (this.address_family != AddressFamily.InterNetworkV6)
					{
						throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
					}
					num = (short)((int)this.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit));
				}
				return num;
			}
			set
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.address_family == AddressFamily.InterNetwork)
				{
					this.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, (int)value);
				}
				else
				{
					if (this.address_family != AddressFamily.InterNetworkV6)
					{
						throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
					}
					this.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit, (int)value);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SocketAddress RemoteEndPoint_internal(IntPtr socket, out int error);

		public EndPoint RemoteEndPoint
		{
			get
			{
				if (this.disposed && this.closed)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.seed_endpoint == null)
				{
					return null;
				}
				int num;
				SocketAddress socketAddress = Socket.RemoteEndPoint_internal(this.socket, out num);
				if (num != 0)
				{
					throw new SocketException(num);
				}
				return this.seed_endpoint.Create(socketAddress);
			}
		}

		private void Linger(IntPtr handle)
		{
			if (!this.connected || this.linger_timeout <= 0)
			{
				return;
			}
			int num;
			Socket.Shutdown_internal(handle, SocketShutdown.Receive, out num);
			if (num != 0)
			{
				return;
			}
			int num2 = this.linger_timeout / 1000;
			int num3 = this.linger_timeout % 1000;
			if (num3 > 0)
			{
				Socket.Poll_internal(handle, SelectMode.SelectRead, num3 * 1000, out num);
				if (num != 0)
				{
					return;
				}
			}
			if (num2 > 0)
			{
				LingerOption lingerOption = new LingerOption(true, num2);
				Socket.SetSocketOption_internal(handle, SocketOptionLevel.Socket, SocketOptionName.Linger, lingerOption, null, 0, out num);
			}
		}

		protected virtual void Dispose(bool explicitDisposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			bool flag = this.connected;
			this.connected = false;
			if ((int)this.socket != -1)
			{
				if (Environment.SocketSecurityEnabled && Socket.current_bind_count > 0)
				{
					Socket.current_bind_count--;
				}
				this.closed = true;
				IntPtr intPtr = this.socket;
				this.socket = (IntPtr)(-1);
				Thread thread = this.blocking_thread;
				if (thread != null)
				{
					thread.Abort();
					this.blocking_thread = null;
				}
				if (flag)
				{
					this.Linger(intPtr);
				}
				int num;
				Socket.Close_internal(intPtr, out num);
				if (num != 0)
				{
					throw new SocketException(num);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Close_internal(IntPtr socket, out int error);

		public void Close()
		{
			this.linger_timeout = 0;
			((IDisposable)this).Dispose();
		}

		public void Close(int timeout)
		{
			this.linger_timeout = timeout;
			((IDisposable)this).Dispose();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Connect_internal_real(IntPtr sock, SocketAddress sa, out int error);

		private static void Connect_internal(IntPtr sock, SocketAddress sa, out int error)
		{
			Socket.Connect_internal(sock, sa, out error, true);
		}

		private static void Connect_internal(IntPtr sock, SocketAddress sa, out int error, bool requireSocketPolicyFile)
		{
			if (requireSocketPolicyFile && !Socket.CheckEndPoint(sa))
			{
				throw new SecurityException("Unable to connect, as no valid crossdomain policy was found");
			}
			Socket.Connect_internal_real(sock, sa, out error);
		}

		internal static bool CheckEndPoint(SocketAddress sa)
		{
			if (!Environment.SocketSecurityEnabled)
			{
				return true;
			}
			bool flag;
			try
			{
				IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Loopback, 123);
				IPEndPoint ipendPoint2 = (IPEndPoint)ipendPoint.Create(sa);
				if (Socket.check_socket_policy == null)
				{
					Socket.check_socket_policy = Socket.GetUnityCrossDomainHelperMethod("CheckSocketEndPoint");
				}
				flag = (bool)Socket.check_socket_policy.Invoke(null, new object[]
				{
					ipendPoint2.Address.ToString(),
					ipendPoint2.Port
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unexpected error while trying to CheckEndPoint() : " + ex);
				flag = false;
			}
			return flag;
		}

		private static MethodInfo GetUnityCrossDomainHelperMethod(string methodname)
		{
			Type type = Type.GetType("UnityEngine.UnityCrossDomainHelper, CrossDomainPolicyParser, Version=1.0.0.0, Culture=neutral");
			if (type == null)
			{
				throw new SecurityException("Cant find type UnityCrossDomainHelper");
			}
			MethodInfo method = type.GetMethod(methodname);
			if (method == null)
			{
				throw new SecurityException("Cant find " + methodname);
			}
			return method;
		}

		public void Connect(EndPoint remoteEP)
		{
			this.Connect(remoteEP, true);
		}

		internal void Connect(EndPoint remoteEP, bool requireSocketPolicy)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			IPEndPoint ipendPoint = remoteEP as IPEndPoint;
			if (ipendPoint != null && (ipendPoint.Address.Equals(IPAddress.Any) || ipendPoint.Address.Equals(IPAddress.IPv6Any)))
			{
				throw new SocketException(10049);
			}
			if (this.islistening)
			{
				throw new InvalidOperationException();
			}
			SocketAddress socketAddress = remoteEP.Serialize();
			int num = 0;
			this.blocking_thread = Thread.CurrentThread;
			try
			{
				Socket.Connect_internal(this.socket, socketAddress, out num, requireSocketPolicy);
			}
			catch (ThreadAbortException)
			{
				if (this.disposed)
				{
					Thread.ResetAbort();
					num = 10004;
				}
			}
			finally
			{
				this.blocking_thread = null;
			}
			if (num != 0)
			{
				throw new SocketException(num);
			}
			this.connected = true;
			this.isbound = true;
			this.seed_endpoint = remoteEP;
		}

		public bool ReceiveAsync(SocketAsyncEventArgs e)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (e.BufferList != null)
			{
				throw new NotSupportedException("Mono doesn't support using BufferList at this point.");
			}
			e.DoOperation(SocketAsyncOperation.Receive, this);
			return true;
		}

		public bool SendAsync(SocketAsyncEventArgs e)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (e.Buffer == null && e.BufferList == null)
			{
				throw new ArgumentException("Either e.Buffer or e.BufferList must be valid buffers.");
			}
			e.DoOperation(SocketAsyncOperation.Send, this);
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Poll_internal(IntPtr socket, SelectMode mode, int timeout, out int error);

		internal bool Poll(int time_us, SelectMode mode, out int socket_error)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (mode != SelectMode.SelectRead && mode != SelectMode.SelectWrite && mode != SelectMode.SelectError)
			{
				throw new NotSupportedException("'mode' parameter is not valid.");
			}
			int num;
			bool flag = Socket.Poll_internal(this.socket, mode, time_us, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			socket_error = (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);
			if (mode == SelectMode.SelectWrite && flag && socket_error == 0)
			{
				this.connected = true;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Receive_internal(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, out int error);

		internal int Receive_nochecks(byte[] buf, int offset, int size, SocketFlags flags, out SocketError error)
		{
			if (this.protocol_type == ProtocolType.Udp && Environment.SocketSecurityEnabled)
			{
				IPAddress ipaddress = IPAddress.Any;
				if (this.address_family == AddressFamily.InterNetworkV6)
				{
					ipaddress = IPAddress.IPv6Any;
				}
				EndPoint endPoint = new IPEndPoint(ipaddress, 0);
				int num = 0;
				int num2 = this.ReceiveFrom_nochecks_exc(buf, offset, size, flags, ref endPoint, false, out num);
				error = (SocketError)num;
				return num2;
			}
			int num4;
			int num3 = Socket.Receive_internal(this.socket, buf, offset, size, flags, out num4);
			error = (SocketError)num4;
			if (error != SocketError.Success && error != SocketError.WouldBlock && error != SocketError.InProgress)
			{
				this.connected = false;
			}
			else
			{
				this.connected = true;
			}
			return num3;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSocketOption_obj_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, out object obj_val, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Send_internal(IntPtr sock, byte[] buf, int offset, int count, SocketFlags flags, out int error);

		internal int Send_nochecks(byte[] buf, int offset, int size, SocketFlags flags, out SocketError error)
		{
			if (size == 0)
			{
				error = SocketError.Success;
				return 0;
			}
			int num2;
			int num = Socket.Send_internal(this.socket, buf, offset, size, flags, out num2);
			error = (SocketError)num2;
			if (error != SocketError.Success && error != SocketError.WouldBlock && error != SocketError.InProgress)
			{
				this.connected = false;
			}
			else
			{
				this.connected = true;
			}
			return num;
		}

		public object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			object obj;
			int num;
			Socket.GetSocketOption_obj_internal(this.socket, optionLevel, optionName, out obj, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			if (optionName == SocketOptionName.Linger)
			{
				return (LingerOption)obj;
			}
			if (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership)
			{
				return (MulticastOption)obj;
			}
			if (obj is int)
			{
				return (int)obj;
			}
			return obj;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Shutdown_internal(IntPtr socket, SocketShutdown how, out int error);

		public void Shutdown(SocketShutdown how)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (!this.connected)
			{
				throw new SocketException(10057);
			}
			int num;
			Socket.Shutdown_internal(this.socket, how, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSocketOption_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, object obj_val, byte[] byte_val, int int_val, out int error);

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
		{
			if (this.disposed && this.closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			int num;
			Socket.SetSocketOption_internal(this.socket, optionLevel, optionName, null, null, optionValue, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
		}

		private void ThrowIfUpd()
		{
			if (this.protocol_type == ProtocolType.Udp)
			{
				throw new SocketException(10042);
			}
		}

		private Queue readQ = new Queue(2);

		private Queue writeQ = new Queue(2);

		private bool islistening;

		private bool useoverlappedIO;

		private readonly int MinListenPort = 7100;

		private readonly int MaxListenPort = 7150;

		private static int ipv4Supported = -1;

		private static int ipv6Supported = -1;

		private int linger_timeout;

		private IntPtr socket;

		private AddressFamily address_family;

		private SocketType socket_type;

		private ProtocolType protocol_type;

		internal bool blocking = true;

		private Thread blocking_thread;

		private bool isbound;

		private static int current_bind_count;

		private readonly int max_bind_count = 50;

		private bool connected;

		private bool closed;

		internal bool disposed;

		internal EndPoint seed_endpoint;

		private static MethodInfo check_socket_policy;

		private enum SocketOperation
		{
			Accept,
			Connect,
			Receive,
			ReceiveFrom,
			Send,
			SendTo,
			UsedInManaged1,
			UsedInManaged2,
			UsedInProcess,
			UsedInConsole2,
			Disconnect,
			AcceptReceive,
			ReceiveGeneric,
			SendGeneric
		}

		private struct WSABUF
		{
			public int len;

			public IntPtr buf;
		}

		[StructLayout(LayoutKind.Sequential)]
		private sealed class SocketAsyncResult : IAsyncResult
		{
			public SocketAsyncResult(Socket sock, object state, AsyncCallback callback, Socket.SocketOperation operation)
			{
				this.Sock = sock;
				this.blocking = sock.blocking;
				this.handle = sock.socket;
				this.state = state;
				this.callback = callback;
				this.operation = operation;
				this.SockFlags = SocketFlags.None;
			}

			public void CheckIfThrowDelayedException()
			{
				if (this.delayedException != null)
				{
					this.Sock.connected = false;
					throw this.delayedException;
				}
				if (this.error != 0)
				{
					this.Sock.connected = false;
					throw new SocketException(this.error);
				}
			}

			private void CompleteAllOnDispose(Queue queue)
			{
				object[] array = queue.ToArray();
				queue.Clear();
				foreach (Socket.SocketAsyncResult socketAsyncResult in array)
				{
					WaitCallback waitCallback = new WaitCallback(socketAsyncResult.CompleteDisposed);
					ThreadPool.QueueUserWorkItem(waitCallback, null);
				}
				if (array.Length == 0)
				{
					this.Buffer = null;
				}
			}

			private void CompleteDisposed(object unused)
			{
				this.Complete();
			}

			public void Complete()
			{
				if (this.operation != Socket.SocketOperation.Receive && this.Sock.disposed)
				{
					this.delayedException = new ObjectDisposedException(this.Sock.GetType().ToString());
				}
				this.IsCompleted = true;
				Queue queue = null;
				if (this.operation == Socket.SocketOperation.Receive || this.operation == Socket.SocketOperation.ReceiveFrom || this.operation == Socket.SocketOperation.ReceiveGeneric)
				{
					queue = this.Sock.readQ;
				}
				else if (this.operation == Socket.SocketOperation.Send || this.operation == Socket.SocketOperation.SendTo || this.operation == Socket.SocketOperation.SendGeneric)
				{
					queue = this.Sock.writeQ;
				}
				if (queue != null)
				{
					Socket.SocketAsyncCall socketAsyncCall = null;
					Socket.SocketAsyncResult socketAsyncResult = null;
					Queue queue2 = queue;
					lock (queue2)
					{
						queue.Dequeue();
						if (queue.Count > 0)
						{
							socketAsyncResult = (Socket.SocketAsyncResult)queue.Peek();
							if (!this.Sock.disposed)
							{
								Socket.Worker worker = new Socket.Worker(socketAsyncResult);
								socketAsyncCall = this.GetDelegate(worker, socketAsyncResult.operation);
							}
							else
							{
								this.CompleteAllOnDispose(queue);
							}
						}
					}
					if (socketAsyncCall != null)
					{
						socketAsyncCall.BeginInvoke(null, socketAsyncResult);
					}
				}
				if (this.callback != null)
				{
					this.callback(this);
				}
				this.Buffer = null;
			}

			private Socket.SocketAsyncCall GetDelegate(Socket.Worker worker, Socket.SocketOperation op)
			{
				switch (op)
				{
				case Socket.SocketOperation.Receive:
					return new Socket.SocketAsyncCall(worker.Receive);
				case Socket.SocketOperation.ReceiveFrom:
					return new Socket.SocketAsyncCall(worker.ReceiveFrom);
				case Socket.SocketOperation.Send:
					return new Socket.SocketAsyncCall(worker.Send);
				case Socket.SocketOperation.SendTo:
					return new Socket.SocketAsyncCall(worker.SendTo);
				default:
					return null;
				}
			}

			public void Complete(bool synch)
			{
				this.completed_sync = synch;
				this.Complete();
			}

			public void Complete(int total)
			{
				this.total = total;
				this.Complete();
			}

			public void Complete(Exception e, bool synch)
			{
				this.completed_sync = synch;
				this.delayedException = e;
				this.Complete();
			}

			public void Complete(Exception e)
			{
				this.delayedException = e;
				this.Complete();
			}

			public void Complete(Socket s)
			{
				this.acc_socket = s;
				this.Complete();
			}

			public void Complete(Socket s, int total)
			{
				this.acc_socket = s;
				this.total = total;
				this.Complete();
			}

			public object AsyncState
			{
				get
				{
					return this.state;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					lock (this)
					{
						if (this.waithandle == null)
						{
							this.waithandle = new ManualResetEvent(this.completed);
						}
					}
					return this.waithandle;
				}
				set
				{
					this.waithandle = value;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return this.completed_sync;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.completed;
				}
				set
				{
					this.completed = value;
					lock (this)
					{
						if (this.waithandle != null && value)
						{
							((ManualResetEvent)this.waithandle).Set();
						}
					}
				}
			}

			public Socket Socket
			{
				get
				{
					return this.acc_socket;
				}
			}

			public int Total
			{
				get
				{
					return this.total;
				}
				set
				{
					this.total = value;
				}
			}

			public SocketError ErrorCode
			{
				get
				{
					SocketException ex = this.delayedException as SocketException;
					if (ex != null)
					{
						return ex.SocketErrorCode;
					}
					if (this.error != 0)
					{
						return (SocketError)this.error;
					}
					return SocketError.Success;
				}
			}

			public Socket Sock;

			public IntPtr handle;

			private object state;

			private AsyncCallback callback;

			private WaitHandle waithandle;

			private Exception delayedException;

			public EndPoint EndPoint;

			public byte[] Buffer;

			public int Offset;

			public int Size;

			public SocketFlags SockFlags;

			public Socket AcceptSocket;

			public IPAddress[] Addresses;

			public int Port;

			public IList<ArraySegment<byte>> Buffers;

			public bool ReuseSocket;

			private Socket acc_socket;

			private int total;

			private bool completed_sync;

			private bool completed;

			public bool blocking;

			internal int error;

			private Socket.SocketOperation operation;

			public object ares;

			public int EndCalled;
		}

		private sealed class Worker
		{
			public Worker(Socket.SocketAsyncResult ares)
				: this(ares, true)
			{
			}

			public Worker(Socket.SocketAsyncResult ares, bool requireSocketSecurity)
			{
				this.result = ares;
				this.requireSocketSecurity = requireSocketSecurity;
			}

			public void Accept()
			{
				Socket socket = null;
				try
				{
					socket = this.result.Sock.Accept();
				}
				catch (Exception ex)
				{
					this.result.Complete(ex);
					return;
				}
				this.result.Complete(socket);
			}

			public void AcceptReceive()
			{
				Socket socket = null;
				try
				{
					if (this.result.AcceptSocket == null)
					{
						socket = this.result.Sock.Accept();
					}
					else
					{
						socket = this.result.AcceptSocket;
						this.result.Sock.Accept(socket);
					}
				}
				catch (Exception ex)
				{
					this.result.Complete(ex);
					return;
				}
				int num = 0;
				if (this.result.Size > 0)
				{
					try
					{
						SocketError socketError;
						num = socket.Receive_nochecks(this.result.Buffer, this.result.Offset, this.result.Size, this.result.SockFlags, out socketError);
					}
					catch (Exception ex2)
					{
						this.result.Complete(ex2);
						return;
					}
				}
				this.result.Complete(socket, num);
			}

			public void Connect()
			{
				if (this.result.EndPoint != null)
				{
					try
					{
						if (!this.result.Sock.Blocking)
						{
							int num;
							this.result.Sock.Poll(-1, SelectMode.SelectWrite, out num);
							if (num != 0)
							{
								this.result.Complete(new SocketException(num));
								return;
							}
							this.result.Sock.connected = true;
						}
						else
						{
							this.result.Sock.seed_endpoint = this.result.EndPoint;
							this.result.Sock.Connect(this.result.EndPoint, this.requireSocketSecurity);
							this.result.Sock.connected = true;
						}
					}
					catch (Exception ex)
					{
						this.result.Complete(ex);
						return;
					}
					this.result.Complete();
				}
				else if (this.result.Addresses != null)
				{
					int num2 = 10036;
					foreach (IPAddress ipaddress in this.result.Addresses)
					{
						IPEndPoint ipendPoint = new IPEndPoint(ipaddress, this.result.Port);
						SocketAddress socketAddress = ipendPoint.Serialize();
						try
						{
							Socket.Connect_internal(this.result.Sock.socket, socketAddress, out num2, this.requireSocketSecurity);
						}
						catch (Exception ex2)
						{
							this.result.Complete(ex2);
							return;
						}
						if (num2 == 0)
						{
							this.result.Sock.connected = true;
							this.result.Sock.seed_endpoint = ipendPoint;
							this.result.Complete();
							return;
						}
						if (num2 == 10036 || num2 == 10035)
						{
							if (!this.result.Sock.Blocking)
							{
								int num3;
								this.result.Sock.Poll(-1, SelectMode.SelectWrite, out num3);
								if (num3 == 0)
								{
									this.result.Sock.connected = true;
									this.result.Sock.seed_endpoint = ipendPoint;
									this.result.Complete();
									return;
								}
							}
						}
					}
					this.result.Complete(new SocketException(num2));
				}
				else
				{
					this.result.Complete(new SocketException(10049));
				}
			}

			public void Disconnect()
			{
				try
				{
					this.result.Sock.Disconnect(this.result.ReuseSocket);
				}
				catch (Exception ex)
				{
					this.result.Complete(ex);
					return;
				}
				this.result.Complete();
			}

			public void Receive()
			{
				this.result.Complete();
			}

			public void ReceiveFrom()
			{
				int num = 0;
				try
				{
					num = this.result.Sock.ReceiveFrom_nochecks(this.result.Buffer, this.result.Offset, this.result.Size, this.result.SockFlags, ref this.result.EndPoint);
				}
				catch (Exception ex)
				{
					this.result.Complete(ex);
					return;
				}
				this.result.Complete(num);
			}

			public void ReceiveGeneric()
			{
				int num = 0;
				try
				{
					SocketError socketError;
					num = this.result.Sock.Receive(this.result.Buffers, this.result.SockFlags, out socketError);
				}
				catch (Exception ex)
				{
					this.result.Complete(ex);
					return;
				}
				this.result.Complete(num);
			}

			private void UpdateSendValues(int last_sent)
			{
				if (this.result.error == 0)
				{
					this.send_so_far += last_sent;
					this.result.Offset += last_sent;
					this.result.Size -= last_sent;
				}
			}

			public void Send()
			{
				if (this.result.error == 0)
				{
					this.UpdateSendValues(this.result.Total);
					if (this.result.Sock.disposed)
					{
						this.result.Complete();
						return;
					}
					if (this.result.Size > 0)
					{
						Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(this.Send);
						socketAsyncCall.BeginInvoke(null, this.result);
						return;
					}
					this.result.Total = this.send_so_far;
				}
				this.result.Complete();
			}

			public void SendTo()
			{
				try
				{
					int num = this.result.Sock.SendTo_nochecks(this.result.Buffer, this.result.Offset, this.result.Size, this.result.SockFlags, this.result.EndPoint);
					this.UpdateSendValues(num);
					if (this.result.Size > 0)
					{
						Socket.SocketAsyncCall socketAsyncCall = new Socket.SocketAsyncCall(this.SendTo);
						socketAsyncCall.BeginInvoke(null, this.result);
						return;
					}
					this.result.Total = this.send_so_far;
				}
				catch (Exception ex)
				{
					this.result.Complete(ex);
					return;
				}
				this.result.Complete();
			}

			public void SendGeneric()
			{
				int num = 0;
				try
				{
					SocketError socketError;
					num = this.result.Sock.Send(this.result.Buffers, this.result.SockFlags, out socketError);
				}
				catch (Exception ex)
				{
					this.result.Complete(ex);
					return;
				}
				this.result.Complete(num);
			}

			private Socket.SocketAsyncResult result;

			private bool requireSocketSecurity;

			private int send_so_far;
		}

		private sealed class SendFileAsyncResult : IAsyncResult
		{
			public SendFileAsyncResult(Socket.SendFileHandler d, IAsyncResult ares)
			{
				this.d = d;
				this.ares = ares;
			}

			public object AsyncState
			{
				get
				{
					return this.ares.AsyncState;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.ares.AsyncWaitHandle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return this.ares.CompletedSynchronously;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.ares.IsCompleted;
				}
			}

			public Socket.SendFileHandler Delegate
			{
				get
				{
					return this.d;
				}
			}

			public IAsyncResult Original
			{
				get
				{
					return this.ares;
				}
			}

			private IAsyncResult ares;

			private Socket.SendFileHandler d;
		}

		private delegate void SocketAsyncCall();

		private delegate void SendFileHandler(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags);
	}
}
