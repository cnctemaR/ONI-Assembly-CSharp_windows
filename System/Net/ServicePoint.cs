using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System.Net
{
	public class ServicePoint
	{
		internal ServicePoint(Uri uri, int connectionLimit, int maxIdleTime)
		{
			this.uri = uri;
			this.connectionLimit = connectionLimit;
			this.maxIdleTime = maxIdleTime;
			this.currentConnections = 0;
			this.idleSince = DateTime.UtcNow;
		}

		public Uri Address
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

		[MonoTODO]
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
				return this.idleSince.ToLocalTime();
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
				lock (this)
				{
					this.maxIdleTime = value;
					if (this.idleTimer != null)
					{
						this.idleTimer.Change(this.maxIdleTime, this.maxIdleTime);
					}
				}
			}
		}

		public virtual Version ProtocolVersion
		{
			get
			{
				return this.protocolVersion;
			}
		}

		[MonoTODO]
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

		public void SetTcpKeepAlive(bool enabled, int keepAliveTime, int keepAliveInterval)
		{
			if (enabled)
			{
				if (keepAliveTime <= 0)
				{
					throw new ArgumentOutOfRangeException("keepAliveTime", "Must be greater than 0");
				}
				if (keepAliveInterval <= 0)
				{
					throw new ArgumentOutOfRangeException("keepAliveInterval", "Must be greater than 0");
				}
			}
			this.tcp_keepalive = enabled;
			this.tcp_keepalive_time = keepAliveTime;
			this.tcp_keepalive_interval = keepAliveInterval;
		}

		internal void KeepAliveSetup(Socket socket)
		{
			if (!this.tcp_keepalive)
			{
				return;
			}
			byte[] array = new byte[12];
			ServicePoint.PutBytes(array, this.tcp_keepalive ? 1U : 0U, 0);
			ServicePoint.PutBytes(array, (uint)this.tcp_keepalive_time, 4);
			ServicePoint.PutBytes(array, (uint)this.tcp_keepalive_interval, 8);
			socket.IOControl((IOControlCode)((ulong)(-1744830460)), array, null);
		}

		private static void PutBytes(byte[] bytes, uint v, int offset)
		{
			if (BitConverter.IsLittleEndian)
			{
				bytes[offset] = (byte)(v & 255U);
				bytes[offset + 1] = (byte)((v & 65280U) >> 8);
				bytes[offset + 2] = (byte)((v & 16711680U) >> 16);
				bytes[offset + 3] = (byte)((v & 4278190080U) >> 24);
				return;
			}
			bytes[offset + 3] = (byte)(v & 255U);
			bytes[offset + 2] = (byte)((v & 65280U) >> 8);
			bytes[offset + 1] = (byte)((v & 16711680U) >> 16);
			bytes[offset] = (byte)((v & 4278190080U) >> 24);
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

		private WebConnectionGroup GetConnectionGroup(string name)
		{
			if (name == null)
			{
				name = "";
			}
			WebConnectionGroup webConnectionGroup;
			if (this.groups != null && this.groups.TryGetValue(name, out webConnectionGroup))
			{
				return webConnectionGroup;
			}
			webConnectionGroup = new WebConnectionGroup(this, name);
			webConnectionGroup.ConnectionClosed += delegate(object s, EventArgs e)
			{
				this.currentConnections--;
			};
			if (this.groups == null)
			{
				this.groups = new Dictionary<string, WebConnectionGroup>();
			}
			this.groups.Add(name, webConnectionGroup);
			return webConnectionGroup;
		}

		private void RemoveConnectionGroup(WebConnectionGroup group)
		{
			if (this.groups == null || this.groups.Count == 0)
			{
				throw new InvalidOperationException();
			}
			this.groups.Remove(group.Name);
		}

		private bool CheckAvailableForRecycling(out DateTime outIdleSince)
		{
			outIdleSince = DateTime.MinValue;
			List<WebConnectionGroup> list = null;
			List<WebConnectionGroup> list2 = null;
			ServicePoint servicePoint = this;
			TimeSpan timeSpan;
			lock (servicePoint)
			{
				if (this.groups == null || this.groups.Count == 0)
				{
					this.idleSince = DateTime.MinValue;
					return true;
				}
				timeSpan = TimeSpan.FromMilliseconds((double)this.maxIdleTime);
				list = new List<WebConnectionGroup>(this.groups.Values);
			}
			foreach (WebConnectionGroup webConnectionGroup in list)
			{
				if (webConnectionGroup.TryRecycle(timeSpan, ref outIdleSince))
				{
					if (list2 == null)
					{
						list2 = new List<WebConnectionGroup>();
					}
					list2.Add(webConnectionGroup);
				}
			}
			servicePoint = this;
			bool flag2;
			lock (servicePoint)
			{
				this.idleSince = outIdleSince;
				if (list2 != null && this.groups != null)
				{
					foreach (WebConnectionGroup webConnectionGroup2 in list2)
					{
						if (this.groups.ContainsKey(webConnectionGroup2.Name))
						{
							this.RemoveConnectionGroup(webConnectionGroup2);
						}
					}
				}
				if (this.groups != null && this.groups.Count == 0)
				{
					this.groups = null;
				}
				if (this.groups == null)
				{
					if (this.idleTimer != null)
					{
						this.idleTimer.Dispose();
						this.idleTimer = null;
					}
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		private void IdleTimerCallback(object obj)
		{
			DateTime dateTime;
			this.CheckAvailableForRecycling(out dateTime);
		}

		private bool HasTimedOut
		{
			get
			{
				int dnsRefreshTimeout = ServicePointManager.DnsRefreshTimeout;
				return dnsRefreshTimeout != -1 && this.lastDnsResolve + TimeSpan.FromMilliseconds((double)dnsRefreshTimeout) < DateTime.UtcNow;
			}
		}

		internal IPHostEntry HostEntry
		{
			get
			{
				object obj = this.hostE;
				lock (obj)
				{
					string text = this.uri.Host;
					if (this.uri.HostNameType == UriHostNameType.IPv6 || this.uri.HostNameType == UriHostNameType.IPv4)
					{
						if (this.host != null)
						{
							return this.host;
						}
						if (this.uri.HostNameType == UriHostNameType.IPv6)
						{
							text = text.Substring(1, text.Length - 2);
						}
						this.host = new IPHostEntry();
						this.host.AddressList = new IPAddress[] { IPAddress.Parse(text) };
						return this.host;
					}
					else
					{
						if (!this.HasTimedOut && this.host != null)
						{
							return this.host;
						}
						this.lastDnsResolve = DateTime.UtcNow;
						try
						{
							this.host = Dns.GetHostEntry(text);
						}
						catch
						{
							return null;
						}
					}
				}
				return this.host;
			}
		}

		internal void SetVersion(Version version)
		{
			this.protocolVersion = version;
		}

		internal EventHandler SendRequest(HttpWebRequest request, string groupName)
		{
			WebConnection connection;
			lock (this)
			{
				bool flag2;
				connection = this.GetConnectionGroup(groupName).GetConnection(request, out flag2);
				if (flag2)
				{
					this.currentConnections++;
					if (this.idleTimer == null)
					{
						this.idleTimer = new Timer(new TimerCallback(this.IdleTimerCallback), null, this.maxIdleTime, this.maxIdleTime);
					}
				}
			}
			return connection.SendRequest(request);
		}

		public bool CloseConnectionGroup(string connectionGroupName)
		{
			WebConnectionGroup webConnectionGroup = null;
			lock (this)
			{
				webConnectionGroup = this.GetConnectionGroup(connectionGroupName);
				if (webConnectionGroup != null)
				{
					this.RemoveConnectionGroup(webConnectionGroup);
				}
			}
			if (webConnectionGroup != null)
			{
				webConnectionGroup.Close();
				return true;
			}
			return false;
		}

		public X509Certificate Certificate
		{
			get
			{
				object serverCertificateOrBytes = this.m_ServerCertificateOrBytes;
				if (serverCertificateOrBytes != null && serverCertificateOrBytes.GetType() == typeof(byte[]))
				{
					return (X509Certificate)(this.m_ServerCertificateOrBytes = new X509Certificate((byte[])serverCertificateOrBytes));
				}
				return serverCertificateOrBytes as X509Certificate;
			}
		}

		internal void UpdateServerCertificate(X509Certificate certificate)
		{
			if (certificate != null)
			{
				this.m_ServerCertificateOrBytes = certificate.GetRawCertData();
				return;
			}
			this.m_ServerCertificateOrBytes = null;
		}

		public X509Certificate ClientCertificate
		{
			get
			{
				object clientCertificateOrBytes = this.m_ClientCertificateOrBytes;
				if (clientCertificateOrBytes != null && clientCertificateOrBytes.GetType() == typeof(byte[]))
				{
					return (X509Certificate)(this.m_ClientCertificateOrBytes = new X509Certificate((byte[])clientCertificateOrBytes));
				}
				return clientCertificateOrBytes as X509Certificate;
			}
		}

		internal void UpdateClientCertificate(X509Certificate certificate)
		{
			if (certificate != null)
			{
				this.m_ClientCertificateOrBytes = certificate.GetRawCertData();
				return;
			}
			this.m_ClientCertificateOrBytes = null;
		}

		internal bool CallEndPointDelegate(Socket sock, IPEndPoint remote)
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
					catch (SocketException)
					{
						num++;
						continue;
					}
					return true;
				}
				return true;
			}
		}

		internal Socket GetConnection(PooledStream PooledStream, object owner, bool async, out IPAddress address, ref Socket abortSocket, ref Socket abortSocket6)
		{
			throw new NotImplementedException();
		}

		private readonly Uri uri;

		private int connectionLimit;

		private int maxIdleTime;

		private int currentConnections;

		private DateTime idleSince;

		private DateTime lastDnsResolve;

		private Version protocolVersion;

		private IPHostEntry host;

		private bool usesProxy;

		private Dictionary<string, WebConnectionGroup> groups;

		private bool sendContinue = true;

		private bool useConnect;

		private object hostE = new object();

		private bool useNagle;

		private BindIPEndPoint endPointCallback;

		private bool tcp_keepalive;

		private int tcp_keepalive_time;

		private int tcp_keepalive_interval;

		private Timer idleTimer;

		private object m_ServerCertificateOrBytes;

		private object m_ClientCertificateOrBytes;
	}
}
