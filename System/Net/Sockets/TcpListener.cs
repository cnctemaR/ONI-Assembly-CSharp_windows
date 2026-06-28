using System;

namespace System.Net.Sockets
{
	public class TcpListener
	{
		[Obsolete("Use TcpListener (IPAddress address, int port) instead")]
		public TcpListener(int port)
		{
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.Init(AddressFamily.InterNetwork, new IPEndPoint(IPAddress.Any, port));
		}

		public TcpListener(IPEndPoint local_end_point)
		{
			if (local_end_point == null)
			{
				throw new ArgumentNullException("local_end_point");
			}
			this.Init(local_end_point.AddressFamily, local_end_point);
		}

		public TcpListener(IPAddress listen_ip, int port)
		{
			if (listen_ip == null)
			{
				throw new ArgumentNullException("listen_ip");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.Init(listen_ip.AddressFamily, new IPEndPoint(listen_ip, port));
		}

		private void Init(AddressFamily family, EndPoint ep)
		{
			this.active = false;
			this.server = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
			this.savedEP = ep;
		}

		protected bool Active
		{
			get
			{
				return this.active;
			}
		}

		public EndPoint LocalEndpoint
		{
			get
			{
				if (this.active)
				{
					return this.server.LocalEndPoint;
				}
				return this.savedEP;
			}
		}

		public Socket Server
		{
			get
			{
				return this.server;
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				if (this.server == null)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.active)
				{
					throw new InvalidOperationException("The TcpListener has been started");
				}
				return this.server.ExclusiveAddressUse;
			}
			set
			{
				if (this.server == null)
				{
					throw new ObjectDisposedException(base.GetType().ToString());
				}
				if (this.active)
				{
					throw new InvalidOperationException("The TcpListener has been started");
				}
				this.server.ExclusiveAddressUse = value;
			}
		}

		public Socket AcceptSocket()
		{
			if (!this.active)
			{
				throw new InvalidOperationException("Socket is not listening");
			}
			return this.server.Accept();
		}

		public TcpClient AcceptTcpClient()
		{
			if (!this.active)
			{
				throw new InvalidOperationException("Socket is not listening");
			}
			Socket socket = this.server.Accept();
			TcpClient tcpClient = new TcpClient();
			tcpClient.SetTcpClient(socket);
			return tcpClient;
		}

		~TcpListener()
		{
			if (this.active)
			{
				this.Stop();
			}
		}

		public bool Pending()
		{
			if (!this.active)
			{
				throw new InvalidOperationException("Socket is not listening");
			}
			return this.server.Poll(0, SelectMode.SelectRead);
		}

		public void Start()
		{
			this.Start(5);
		}

		public void Start(int backlog)
		{
			if (this.active)
			{
				return;
			}
			if (this.server == null)
			{
				throw new InvalidOperationException("Invalid server socket");
			}
			this.server.Bind(this.savedEP);
			this.server.Listen(backlog);
			this.active = true;
		}

		public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
		{
			if (this.server == null)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			return this.server.BeginAccept(callback, state);
		}

		public IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state)
		{
			if (this.server == null)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			return this.server.BeginAccept(callback, state);
		}

		public Socket EndAcceptSocket(IAsyncResult asyncResult)
		{
			return this.server.EndAccept(asyncResult);
		}

		public TcpClient EndAcceptTcpClient(IAsyncResult asyncResult)
		{
			Socket socket = this.server.EndAccept(asyncResult);
			TcpClient tcpClient = new TcpClient();
			tcpClient.SetTcpClient(socket);
			return tcpClient;
		}

		public void Stop()
		{
			if (this.active)
			{
				this.server.Close();
				this.server = null;
			}
			this.Init(AddressFamily.InterNetwork, this.savedEP);
		}

		private bool active;

		private Socket server;

		private EndPoint savedEP;
	}
}
