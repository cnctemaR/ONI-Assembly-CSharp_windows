using System;
using System.Collections;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace System.Net
{
	public class ServicePoint
	{
		internal ServicePoint(global::System.Uri uri, int connectionLimit, int maxIdleTime)
		{
			this.uri = uri;
			this.connectionLimit = connectionLimit;
			this.maxIdleTime = maxIdleTime;
			this.currentConnections = 0;
			this.idleSince = DateTime.Now;
		}

		public global::System.Uri Address
		{
			get
			{
				return this.uri;
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		public BindIPEndPoint BindIPEndPointDelegate
		{
			get
			{
				return this.endPointCallback;
			}
			set
			{
				this.endPointCallback = value;
			}
		}

		public X509Certificate Certificate
		{
			get
			{
				return this.certificate;
			}
		}

		public X509Certificate ClientCertificate
		{
			get
			{
				return this.clientCertificate;
			}
		}

		[global::System.MonoTODO]
		public int ConnectionLeaseTimeout
		{
			get
			{
				throw ServicePoint.GetMustImplement();
			}
			set
			{
				throw ServicePoint.GetMustImplement();
			}
		}

		public int ConnectionLimit
		{
			get
			{
				return this.connectionLimit;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.connectionLimit = value;
			}
		}

		public string ConnectionName
		{
			get
			{
				return this.uri.Scheme;
			}
		}

		public int CurrentConnections
		{
			get
			{
				return this.currentConnections;
			}
		}

		public DateTime IdleSince
		{
			get
			{
				return this.idleSince;
			}
			internal set
			{
				object obj = this.locker;
				lock (obj)
				{
					this.idleSince = value;
				}
			}
		}

		public int MaxIdleTime
		{
			get
			{
				return this.maxIdleTime;
			}
			set
			{
				if (value < -1 || value > 2147483647)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.maxIdleTime = value;
			}
		}

		public virtual Version ProtocolVersion
		{
			get
			{
				return this.protocolVersion;
			}
		}

		[global::System.MonoTODO]
		public int ReceiveBufferSize
		{
			get
			{
				throw ServicePoint.GetMustImplement();
			}
			set
			{
				throw ServicePoint.GetMustImplement();
			}
		}

		public bool SupportsPipelining
		{
			get
			{
				return HttpVersion.Version11.Equals(this.protocolVersion);
			}
		}

		public bool Expect100Continue
		{
			get
			{
				return this.SendContinue;
			}
			set
			{
				this.SendContinue = value;
			}
		}

		public bool UseNagleAlgorithm
		{
			get
			{
				return this.useNagle;
			}
			set
			{
				this.useNagle = value;
			}
		}

		internal bool SendContinue
		{
			get
			{
				return this.sendContinue && (this.protocolVersion == null || this.protocolVersion == HttpVersion.Version11);
			}
			set
			{
				this.sendContinue = value;
			}
		}

		internal bool UsesProxy
		{
			get
			{
				return this.usesProxy;
			}
			set
			{
				this.usesProxy = value;
			}
		}

		internal bool UseConnect
		{
			get
			{
				return this.useConnect;
			}
			set
			{
				this.useConnect = value;
			}
		}

		internal bool AvailableForRecycling
		{
			get
			{
				return this.CurrentConnections == 0 && this.maxIdleTime != -1 && DateTime.Now >= this.IdleSince.AddMilliseconds((double)this.maxIdleTime);
			}
		}

		internal Hashtable Groups
		{
			get
			{
				if (this.groups == null)
				{
					this.groups = new Hashtable();
				}
				return this.groups;
			}
		}

		internal IPHostEntry HostEntry
		{
			get
			{
				object obj = this.hostE;
				lock (obj)
				{
					if (this.host != null)
					{
						return this.host;
					}
					string text = this.uri.Host;
					if (this.uri.HostNameType == global::System.UriHostNameType.IPv6 || this.uri.HostNameType == global::System.UriHostNameType.IPv4)
					{
						if (this.uri.HostNameType == global::System.UriHostNameType.IPv6)
						{
							text = text.Substring(1, text.Length - 2);
						}
						this.host = new IPHostEntry();
						this.host.AddressList = new IPAddress[] { IPAddress.Parse(text) };
						return this.host;
					}
					try
					{
						this.host = Dns.GetHostByName(text);
					}
					catch
					{
						return null;
					}
				}
				return this.host;
			}
		}

		internal void SetVersion(Version version)
		{
			this.protocolVersion = version;
		}

		private WebConnectionGroup GetConnectionGroup(string name)
		{
			if (name == null)
			{
				name = string.Empty;
			}
			WebConnectionGroup webConnectionGroup = this.Groups[name] as WebConnectionGroup;
			if (webConnectionGroup != null)
			{
				return webConnectionGroup;
			}
			webConnectionGroup = new WebConnectionGroup(this, name);
			this.Groups[name] = webConnectionGroup;
			return webConnectionGroup;
		}

		internal EventHandler SendRequest(HttpWebRequest request, string groupName)
		{
			object obj = this.locker;
			WebConnection connection;
			lock (obj)
			{
				WebConnectionGroup connectionGroup = this.GetConnectionGroup(groupName);
				connection = connectionGroup.GetConnection(request);
			}
			return connection.SendRequest(request);
		}

		public bool CloseConnectionGroup(string connectionGroupName)
		{
			object obj = this.locker;
			lock (obj)
			{
				WebConnectionGroup connectionGroup = this.GetConnectionGroup(connectionGroupName);
				if (connectionGroup != null)
				{
					connectionGroup.Close();
					return true;
				}
			}
			return false;
		}

		internal void IncrementConnection()
		{
			object obj = this.locker;
			lock (obj)
			{
				this.currentConnections++;
				this.idleSince = DateTime.Now.AddMilliseconds(1000000.0);
			}
		}

		internal void DecrementConnection()
		{
			object obj = this.locker;
			lock (obj)
			{
				this.currentConnections--;
				if (this.currentConnections == 0)
				{
					this.idleSince = DateTime.Now;
				}
			}
		}

		internal void SetCertificates(X509Certificate client, X509Certificate server)
		{
			this.certificate = server;
			this.clientCertificate = client;
		}

		internal bool CallEndPointDelegate(global::System.Net.Sockets.Socket sock, IPEndPoint remote)
		{
			if (this.endPointCallback == null)
			{
				return true;
			}
			int num = 0;
			checked
			{
				for (;;)
				{
					IPEndPoint ipendPoint = null;
					try
					{
						ipendPoint = this.endPointCallback(this, remote, num);
					}
					catch
					{
						return false;
					}
					if (ipendPoint == null)
					{
						break;
					}
					try
					{
						sock.Bind(ipendPoint);
					}
					catch (global::System.Net.Sockets.SocketException)
					{
						num++;
						continue;
					}
					return true;
				}
				return true;
			}
		}

		private global::System.Uri uri;

		private int connectionLimit;

		private int maxIdleTime;

		private int currentConnections;

		private DateTime idleSince;

		private Version protocolVersion;

		private X509Certificate certificate;

		private X509Certificate clientCertificate;

		private IPHostEntry host;

		private bool usesProxy;

		private Hashtable groups;

		private bool sendContinue = true;

		private bool useConnect;

		private object locker = new object();

		private object hostE = new object();

		private bool useNagle;

		private BindIPEndPoint endPointCallback;
	}
}
