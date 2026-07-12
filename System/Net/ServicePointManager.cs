using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Security;
using System.Threading;

namespace System.Net
{
	public class ServicePointManager
	{
		static ServicePointManager()
		{
			ConnectionManagementSection connectionManagementSection = ConfigurationManager.GetSection("system.net/connectionManagement") as ConnectionManagementSection;
			if (connectionManagementSection != null)
			{
				ServicePointManager.manager = new ConnectionManagementData(null);
				foreach (object obj in connectionManagementSection.ConnectionManagement)
				{
					ConnectionManagementElement connectionManagementElement = (ConnectionManagementElement)obj;
					ServicePointManager.manager.Add(connectionManagementElement.Address, connectionManagementElement.MaxConnection);
				}
				ServicePointManager.defaultConnectionLimit = (int)ServicePointManager.manager.GetMaxConnections("*");
				return;
			}
			ServicePointManager.manager = (ConnectionManagementData)ConfigurationSettings.GetConfig("system.net/connectionManagement");
			if (ServicePointManager.manager != null)
			{
				ServicePointManager.defaultConnectionLimit = (int)ServicePointManager.manager.GetMaxConnections("*");
			}
		}

		private ServicePointManager()
		{
		}

		[Obsolete("Use ServerCertificateValidationCallback instead", false)]
		public static ICertificatePolicy CertificatePolicy
		{
			get
			{
				if (ServicePointManager.policy == null)
				{
					Interlocked.CompareExchange<ICertificatePolicy>(ref ServicePointManager.policy, new DefaultCertificatePolicy(), null);
				}
				return ServicePointManager.policy;
			}
			set
			{
				ServicePointManager.policy = value;
			}
		}

		internal static ICertificatePolicy GetLegacyCertificatePolicy()
		{
			return ServicePointManager.policy;
		}

		[MonoTODO("CRL checks not implemented")]
		public static bool CheckCertificateRevocationList
		{
			get
			{
				return ServicePointManager._checkCRL;
			}
			set
			{
				ServicePointManager._checkCRL = false;
			}
		}

		public static int DefaultConnectionLimit
		{
			get
			{
				return ServicePointManager.defaultConnectionLimit;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				ServicePointManager.defaultConnectionLimit = value;
				if (ServicePointManager.manager != null)
				{
					ServicePointManager.manager.Add("*", ServicePointManager.defaultConnectionLimit);
				}
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		public static int DnsRefreshTimeout
		{
			get
			{
				return ServicePointManager.dnsRefreshTimeout;
			}
			set
			{
				ServicePointManager.dnsRefreshTimeout = Math.Max(-1, value);
			}
		}

		[MonoTODO]
		public static bool EnableDnsRoundRobin
		{
			get
			{
				throw ServicePointManager.GetMustImplement();
			}
			set
			{
				throw ServicePointManager.GetMustImplement();
			}
		}

		public static int MaxServicePointIdleTime
		{
			get
			{
				return ServicePointManager.maxServicePointIdleTime;
			}
			set
			{
				if (value < -2 || value > 2147483647)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				ServicePointManager.maxServicePointIdleTime = value;
			}
		}

		public static int MaxServicePoints
		{
			get
			{
				return ServicePointManager.maxServicePoints;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("value");
				}
				ServicePointManager.maxServicePoints = value;
			}
		}

		[MonoTODO]
		public static bool ReusePort
		{
			get
			{
				return false;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public static SecurityProtocolType SecurityProtocol
		{
			get
			{
				return ServicePointManager._securityProtocol;
			}
			set
			{
				ServicePointManager._securityProtocol = value;
			}
		}

		internal static ServerCertValidationCallback ServerCertValidationCallback
		{
			get
			{
				return ServicePointManager.server_cert_cb;
			}
		}

		public static RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				if (ServicePointManager.server_cert_cb == null)
				{
					return null;
				}
				return ServicePointManager.server_cert_cb.ValidationCallback;
			}
			set
			{
				if (value == null)
				{
					ServicePointManager.server_cert_cb = null;
					return;
				}
				ServicePointManager.server_cert_cb = new ServerCertValidationCallback(value);
			}
		}

		[MonoTODO("Always returns EncryptionPolicy.RequireEncryption.")]
		public static EncryptionPolicy EncryptionPolicy
		{
			get
			{
				return EncryptionPolicy.RequireEncryption;
			}
		}

		public static bool Expect100Continue
		{
			get
			{
				return ServicePointManager.expectContinue;
			}
			set
			{
				ServicePointManager.expectContinue = value;
			}
		}

		public static bool UseNagleAlgorithm
		{
			get
			{
				return ServicePointManager.useNagle;
			}
			set
			{
				ServicePointManager.useNagle = value;
			}
		}

		internal static bool DisableStrongCrypto
		{
			get
			{
				return false;
			}
		}

		internal static bool DisableSendAuxRecord
		{
			get
			{
				return false;
			}
		}

		public static void SetTcpKeepAlive(bool enabled, int keepAliveTime, int keepAliveInterval)
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
			ServicePointManager.tcp_keepalive = enabled;
			ServicePointManager.tcp_keepalive_time = keepAliveTime;
			ServicePointManager.tcp_keepalive_interval = keepAliveInterval;
		}

		public static ServicePoint FindServicePoint(Uri address)
		{
			return ServicePointManager.FindServicePoint(address, null);
		}

		public static ServicePoint FindServicePoint(string uriString, IWebProxy proxy)
		{
			return ServicePointManager.FindServicePoint(new Uri(uriString), proxy);
		}

		public static ServicePoint FindServicePoint(Uri address, IWebProxy proxy)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			Uri uri = new Uri(address.Scheme + "://" + address.Authority);
			bool flag = false;
			bool flag2 = false;
			if (proxy != null && !proxy.IsBypassed(address))
			{
				flag = true;
				bool flag3 = address.Scheme == "https";
				address = proxy.GetProxy(address);
				if (address.Scheme != "http")
				{
					throw new NotSupportedException("Proxy scheme not supported.");
				}
				if (flag3 && address.Scheme == "http")
				{
					flag2 = true;
				}
			}
			address = new Uri(address.Scheme + "://" + address.Authority);
			ServicePointManager.SPKey spkey = new ServicePointManager.SPKey(uri, flag ? address : null, flag2);
			ConcurrentDictionary<ServicePointManager.SPKey, ServicePoint> concurrentDictionary = ServicePointManager.servicePoints;
			ServicePoint servicePoint2;
			lock (concurrentDictionary)
			{
				ServicePoint servicePoint;
				if (ServicePointManager.servicePoints.TryGetValue(spkey, out servicePoint))
				{
					servicePoint2 = servicePoint;
				}
				else
				{
					if (ServicePointManager.maxServicePoints > 0 && ServicePointManager.servicePoints.Count >= ServicePointManager.maxServicePoints)
					{
						throw new InvalidOperationException("maximum number of service points reached");
					}
					string text = address.ToString();
					int maxConnections = (int)ServicePointManager.manager.GetMaxConnections(text);
					servicePoint = new ServicePoint(spkey, address, maxConnections, ServicePointManager.maxServicePointIdleTime);
					servicePoint.Expect100Continue = ServicePointManager.expectContinue;
					servicePoint.UseNagleAlgorithm = ServicePointManager.useNagle;
					servicePoint.UsesProxy = flag;
					servicePoint.UseConnect = flag2;
					servicePoint.SetTcpKeepAlive(ServicePointManager.tcp_keepalive, ServicePointManager.tcp_keepalive_time, ServicePointManager.tcp_keepalive_interval);
					servicePoint2 = ServicePointManager.servicePoints.GetOrAdd(spkey, servicePoint);
				}
			}
			return servicePoint2;
		}

		internal static void CloseConnectionGroup(string connectionGroupName)
		{
			ConcurrentDictionary<ServicePointManager.SPKey, ServicePoint> concurrentDictionary = ServicePointManager.servicePoints;
			lock (concurrentDictionary)
			{
				foreach (ServicePoint servicePoint in ServicePointManager.servicePoints.Values)
				{
					servicePoint.CloseConnectionGroup(connectionGroupName);
				}
			}
		}

		internal static void RemoveServicePoint(ServicePoint sp)
		{
			ServicePoint servicePoint;
			ServicePointManager.servicePoints.TryRemove(sp.Key, out servicePoint);
		}

		private static ConcurrentDictionary<ServicePointManager.SPKey, ServicePoint> servicePoints = new ConcurrentDictionary<ServicePointManager.SPKey, ServicePoint>();

		private static ICertificatePolicy policy;

		private static int defaultConnectionLimit = 2;

		private static int maxServicePointIdleTime = 100000;

		private static int maxServicePoints = 0;

		private static int dnsRefreshTimeout = 120000;

		private static bool _checkCRL = false;

		private static SecurityProtocolType _securityProtocol = SecurityProtocolType.SystemDefault;

		private static bool expectContinue = true;

		private static bool useNagle;

		private static ServerCertValidationCallback server_cert_cb;

		private static bool tcp_keepalive;

		private static int tcp_keepalive_time;

		private static int tcp_keepalive_interval;

		public const int DefaultNonPersistentConnectionLimit = 4;

		public const int DefaultPersistentConnectionLimit = 2;

		private const string configKey = "system.net/connectionManagement";

		private static ConnectionManagementData manager;

		internal class SPKey
		{
			public SPKey(Uri uri, Uri proxy, bool use_connect)
			{
				this.uri = uri;
				this.proxy = proxy;
				this.use_connect = use_connect;
			}

			public Uri Uri
			{
				get
				{
					return this.uri;
				}
			}

			public bool UseConnect
			{
				get
				{
					return this.use_connect;
				}
			}

			public bool UsesProxy
			{
				get
				{
					return this.proxy != null;
				}
			}

			public override int GetHashCode()
			{
				return ((23 * 31 + (this.use_connect ? 1 : 0)) * 31 + this.uri.GetHashCode()) * 31 + ((this.proxy != null) ? this.proxy.GetHashCode() : 0);
			}

			public override bool Equals(object obj)
			{
				ServicePointManager.SPKey spkey = obj as ServicePointManager.SPKey;
				return obj != null && this.uri.Equals(spkey.uri) && this.use_connect == spkey.use_connect && this.UsesProxy == spkey.UsesProxy && (!this.UsesProxy || this.proxy.Equals(spkey.proxy));
			}

			private Uri uri;

			private Uri proxy;

			private bool use_connect;
		}
	}
}
