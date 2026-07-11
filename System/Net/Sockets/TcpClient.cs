using System;

namespace System.Net.Sockets
{
	public class TcpClient : IDisposable
	{
		public TcpClient()
		{
			this.Init(AddressFamily.InterNetwork);
			this.client.Bind(new IPEndPoint(IPAddress.Any, 0));
		}

		public TcpClient(AddressFamily family)
		{
			if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
			}
			this.Init(family);
			IPAddress ipaddress = IPAddress.Any;
			if (family == AddressFamily.InterNetworkV6)
			{
				ipaddress = IPAddress.IPv6Any;
			}
			this.client.Bind(new IPEndPoint(ipaddress, 0));
		}

		public TcpClient(IPEndPoint local_end_point)
		{
			this.Init(local_end_point.AddressFamily);
			this.client.Bind(local_end_point);
		}

		public TcpClient(string hostname, int port)
		{
			this.Connect(hostname, port);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Init(AddressFamily family)
		{
			this.active = false;
			if (this.client != null)
			{
				this.client.Close();
				this.client = null;
			}
			this.client = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
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
				return this.client;
			}
			set
			{
				this.client = value;
				this.stream = null;
			}
		}

		public int Available
		{
			get
			{
				return this.client.Available;
			}
		}

		public bool Connected
		{
			get
			{
				return this.client.Connected;
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				return this.client.ExclusiveAddressUse;
			}
			set
			{
				this.client.ExclusiveAddressUse = value;
			}
		}

		internal void SetTcpClient(Socket s)
		{
			this.Client = s;
		}

		public LingerOption LingerState
		{
			get
			{
				if ((this.values & TcpClient.Properties.LingerState) != (TcpClient.Properties)0U)
				{
					return this.linger_state;
				}
				return (LingerOption)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.linger_state = value;
					this.values |= TcpClient.Properties.LingerState;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
			}
		}

		public bool NoDelay
		{
			get
			{
				if ((this.values & TcpClient.Properties.NoDelay) != (TcpClient.Properties)0U)
				{
					return this.no_delay;
				}
				return (bool)this.client.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.no_delay = value;
					this.values |= TcpClient.Properties.NoDelay;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, (!value) ? 0 : 1);
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				if ((this.values & TcpClient.Properties.ReceiveBufferSize) != (TcpClient.Properties)0U)
				{
					return this.recv_buffer_size;
				}
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.recv_buffer_size = value;
					this.values |= TcpClient.Properties.ReceiveBufferSize;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				if ((this.values & TcpClient.Properties.ReceiveTimeout) != (TcpClient.Properties)0U)
				{
					return this.recv_timeout;
				}
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.recv_timeout = value;
					this.values |= TcpClient.Properties.ReceiveTimeout;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
			}
		}

		public int SendBufferSize
		{
			get
			{
				if ((this.values & TcpClient.Properties.SendBufferSize) != (TcpClient.Properties)0U)
				{
					return this.send_buffer_size;
				}
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.send_buffer_size = value;
					this.values |= TcpClient.Properties.SendBufferSize;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
			}
		}

		public int SendTimeout
		{
			get
			{
				if ((this.values & TcpClient.Properties.SendTimeout) != (TcpClient.Properties)0U)
				{
					return this.send_timeout;
				}
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.send_timeout = value;
					this.values |= TcpClient.Properties.SendTimeout;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
			}
		}

		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		public void Connect(IPEndPoint remote_end_point)
		{
			try
			{
				this.client.Connect(remote_end_point);
				this.active = true;
			}
			finally
			{
				this.CheckDisposed();
			}
		}

		public void Connect(IPAddress address, int port)
		{
			this.Connect(new IPEndPoint(address, port));
		}

		private void SetOptions()
		{
			TcpClient.Properties properties = this.values;
			this.values = (TcpClient.Properties)0U;
			if ((properties & TcpClient.Properties.LingerState) != (TcpClient.Properties)0U)
			{
				this.LingerState = this.linger_state;
			}
			if ((properties & TcpClient.Properties.NoDelay) != (TcpClient.Properties)0U)
			{
				this.NoDelay = this.no_delay;
			}
			if ((properties & TcpClient.Properties.ReceiveBufferSize) != (TcpClient.Properties)0U)
			{
				this.ReceiveBufferSize = this.recv_buffer_size;
			}
			if ((properties & TcpClient.Properties.ReceiveTimeout) != (TcpClient.Properties)0U)
			{
				this.ReceiveTimeout = this.recv_timeout;
			}
			if ((properties & TcpClient.Properties.SendBufferSize) != (TcpClient.Properties)0U)
			{
				this.SendBufferSize = this.send_buffer_size;
			}
			if ((properties & TcpClient.Properties.SendTimeout) != (TcpClient.Properties)0U)
			{
				this.SendTimeout = this.send_timeout;
			}
		}

		public void Connect(string hostname, int port)
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
			this.Connect(hostAddresses, port);
		}

		public void Connect(IPAddress[] ipAddresses, int port)
		{
			this.CheckDisposed();
			if (ipAddresses == null)
			{
				throw new ArgumentNullException("ipAddresses");
			}
			for (int i = 0; i < ipAddresses.Length; i++)
			{
				try
				{
					IPAddress ipaddress = ipAddresses[i];
					if (ipaddress.Equals(IPAddress.Any) || ipaddress.Equals(IPAddress.IPv6Any))
					{
						throw new SocketException(10049);
					}
					this.Init(ipaddress.AddressFamily);
					if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
					{
						this.client.Bind(new IPEndPoint(IPAddress.Any, 0));
					}
					else
					{
						if (ipaddress.AddressFamily != AddressFamily.InterNetworkV6)
						{
							throw new NotSupportedException("This method is only valid for sockets in the InterNetwork and InterNetworkV6 families");
						}
						this.client.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
					}
					this.Connect(new IPEndPoint(ipaddress, port));
					if (this.values != (TcpClient.Properties)0U)
					{
						this.SetOptions();
					}
					break;
				}
				catch (Exception ex)
				{
					this.Init(AddressFamily.InterNetwork);
					if (i == ipAddresses.Length - 1)
					{
						throw ex;
					}
				}
			}
		}

		public void EndConnect(IAsyncResult asyncResult)
		{
			this.client.EndConnect(asyncResult);
		}

		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback callback, object state)
		{
			return this.client.BeginConnect(address, port, callback, state);
		}

		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback callback, object state)
		{
			return this.client.BeginConnect(addresses, port, callback, state);
		}

		public IAsyncResult BeginConnect(string host, int port, AsyncCallback callback, object state)
		{
			return this.client.BeginConnect(host, port, callback, state);
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
				NetworkStream networkStream = this.stream;
				this.stream = null;
				if (networkStream != null)
				{
					networkStream.Close();
					this.active = false;
				}
				else if (this.client != null)
				{
					this.client.Close();
					this.client = null;
				}
			}
		}

		~TcpClient()
		{
			this.Dispose(false);
		}

		public NetworkStream GetStream()
		{
			NetworkStream networkStream;
			try
			{
				if (this.stream == null)
				{
					this.stream = new NetworkStream(this.client, true);
				}
				networkStream = this.stream;
			}
			finally
			{
				this.CheckDisposed();
			}
			return networkStream;
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		private NetworkStream stream;

		private bool active;

		private Socket client;

		private bool disposed;

		private TcpClient.Properties values;

		private int recv_timeout;

		private int send_timeout;

		private int recv_buffer_size;

		private int send_buffer_size;

		private LingerOption linger_state;

		private bool no_delay;

		private enum Properties : uint
		{
			LingerState = 1U,
			NoDelay,
			ReceiveBufferSize = 4U,
			ReceiveTimeout = 8U,
			SendBufferSize = 16U,
			SendTimeout = 32U
		}
	}
}
