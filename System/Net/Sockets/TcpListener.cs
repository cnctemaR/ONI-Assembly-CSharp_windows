using System;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
	public class TcpListener
	{
		public TcpListener(IPEndPoint localEP)
		{
			bool on = Logging.On;
			if (localEP == null)
			{
				throw new ArgumentNullException("localEP");
			}
			this.m_ServerSocketEP = localEP;
			this.m_ServerSocket = new Socket(this.m_ServerSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			bool on2 = Logging.On;
		}

		public TcpListener(IPAddress localaddr, int port)
		{
			bool on = Logging.On;
			if (localaddr == null)
			{
				throw new ArgumentNullException("localaddr");
			}
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.m_ServerSocketEP = new IPEndPoint(localaddr, port);
			this.m_ServerSocket = new Socket(this.m_ServerSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			bool on2 = Logging.On;
		}

		[Obsolete("This method has been deprecated. Please use TcpListener(IPAddress localaddr, int port) instead. http://go.microsoft.com/fwlink/?linkid=14202")]
		public TcpListener(int port)
		{
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.m_ServerSocketEP = new IPEndPoint(IPAddress.Any, port);
			this.m_ServerSocket = new Socket(this.m_ServerSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		}

		public static TcpListener Create(int port)
		{
			bool on = Logging.On;
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			TcpListener tcpListener = new TcpListener(IPAddress.IPv6Any, port);
			tcpListener.Server.DualMode = true;
			bool on2 = Logging.On;
			return tcpListener;
		}

		public Socket Server
		{
			get
			{
				return this.m_ServerSocket;
			}
		}

		protected bool Active
		{
			get
			{
				return this.m_Active;
			}
		}

		public EndPoint LocalEndpoint
		{
			get
			{
				if (!this.m_Active)
				{
					return this.m_ServerSocketEP;
				}
				return this.m_ServerSocket.LocalEndPoint;
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				return this.m_ServerSocket.ExclusiveAddressUse;
			}
			set
			{
				if (this.m_Active)
				{
					throw new InvalidOperationException(global::SR.GetString("The TcpListener must not be listening before performing this operation."));
				}
				this.m_ServerSocket.ExclusiveAddressUse = value;
				this.m_ExclusiveAddressUse = value;
			}
		}

		public void AllowNatTraversal(bool allowed)
		{
			if (this.m_Active)
			{
				throw new InvalidOperationException(global::SR.GetString("The TcpListener must not be listening before performing this operation."));
			}
			if (allowed)
			{
				this.m_ServerSocket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
				return;
			}
			this.m_ServerSocket.SetIPProtectionLevel(IPProtectionLevel.EdgeRestricted);
		}

		public void Start()
		{
			this.Start(int.MaxValue);
		}

		public void Start(int backlog)
		{
			if (backlog > 2147483647 || backlog < 0)
			{
				throw new ArgumentOutOfRangeException("backlog");
			}
			bool on = Logging.On;
			if (this.m_ServerSocket == null)
			{
				throw new InvalidOperationException(global::SR.GetString("The socket handle is not valid."));
			}
			if (this.m_Active)
			{
				bool on2 = Logging.On;
				return;
			}
			this.m_ServerSocket.Bind(this.m_ServerSocketEP);
			try
			{
				this.m_ServerSocket.Listen(backlog);
			}
			catch (SocketException)
			{
				this.Stop();
				throw;
			}
			this.m_Active = true;
			bool on3 = Logging.On;
		}

		public void Stop()
		{
			bool on = Logging.On;
			if (this.m_ServerSocket != null)
			{
				this.m_ServerSocket.Close();
				this.m_ServerSocket = null;
			}
			this.m_Active = false;
			this.m_ServerSocket = new Socket(this.m_ServerSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			if (this.m_ExclusiveAddressUse)
			{
				this.m_ServerSocket.ExclusiveAddressUse = true;
			}
			bool on2 = Logging.On;
		}

		public bool Pending()
		{
			if (!this.m_Active)
			{
				throw new InvalidOperationException(global::SR.GetString("Not listening. You must call the Start() method before calling this method."));
			}
			return this.m_ServerSocket.Poll(0, SelectMode.SelectRead);
		}

		public Socket AcceptSocket()
		{
			bool on = Logging.On;
			if (!this.m_Active)
			{
				throw new InvalidOperationException(global::SR.GetString("Not listening. You must call the Start() method before calling this method."));
			}
			Socket socket = this.m_ServerSocket.Accept();
			bool on2 = Logging.On;
			return socket;
		}

		public TcpClient AcceptTcpClient()
		{
			bool on = Logging.On;
			if (!this.m_Active)
			{
				throw new InvalidOperationException(global::SR.GetString("Not listening. You must call the Start() method before calling this method."));
			}
			TcpClient tcpClient = new TcpClient(this.m_ServerSocket.Accept());
			bool on2 = Logging.On;
			return tcpClient;
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
		{
			bool on = Logging.On;
			if (!this.m_Active)
			{
				throw new InvalidOperationException(global::SR.GetString("Not listening. You must call the Start() method before calling this method."));
			}
			IAsyncResult asyncResult = this.m_ServerSocket.BeginAccept(callback, state);
			bool on2 = Logging.On;
			return asyncResult;
		}

		public Socket EndAcceptSocket(IAsyncResult asyncResult)
		{
			bool on = Logging.On;
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			SocketAsyncResult socketAsyncResult = asyncResult as SocketAsyncResult;
			object obj = ((socketAsyncResult == null) ? null : socketAsyncResult.socket);
			if (obj == null)
			{
				throw new ArgumentException(global::SR.GetString("The IAsyncResult object was not returned from the corresponding asynchronous method on this class."), "asyncResult");
			}
			Socket socket = obj.EndAccept(asyncResult);
			bool on2 = Logging.On;
			return socket;
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state)
		{
			bool on = Logging.On;
			if (!this.m_Active)
			{
				throw new InvalidOperationException(global::SR.GetString("Not listening. You must call the Start() method before calling this method."));
			}
			IAsyncResult asyncResult = this.m_ServerSocket.BeginAccept(callback, state);
			bool on2 = Logging.On;
			return asyncResult;
		}

		public TcpClient EndAcceptTcpClient(IAsyncResult asyncResult)
		{
			bool on = Logging.On;
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			SocketAsyncResult socketAsyncResult = asyncResult as SocketAsyncResult;
			object obj = ((socketAsyncResult == null) ? null : socketAsyncResult.socket);
			if (obj == null)
			{
				throw new ArgumentException(global::SR.GetString("The IAsyncResult object was not returned from the corresponding asynchronous method on this class."), "asyncResult");
			}
			Socket socket = obj.EndAccept(asyncResult);
			bool on2 = Logging.On;
			return new TcpClient(socket);
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public Task<Socket> AcceptSocketAsync()
		{
			return Task<Socket>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginAcceptSocket), new Func<IAsyncResult, Socket>(this.EndAcceptSocket), null);
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public Task<TcpClient> AcceptTcpClientAsync()
		{
			return Task<TcpClient>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginAcceptTcpClient), new Func<IAsyncResult, TcpClient>(this.EndAcceptTcpClient), null);
		}

		private IPEndPoint m_ServerSocketEP;

		private Socket m_ServerSocket;

		private bool m_Active;

		private bool m_ExclusiveAddressUse;
	}
}
