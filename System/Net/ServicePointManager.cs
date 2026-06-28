using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Mono.Security.Protocol.Tls;
using Mono.Security.X509;
using Mono.Security.X509.Extensions;

namespace System.Net
{
	public class ServicePointManager
	{
		private ServicePointManager()
		{
		}

		static ServicePointManager()
		{
			object section = ConfigurationManager.GetSection("system.net/connectionManagement");
			global::System.Net.Configuration.ConnectionManagementSection connectionManagementSection = section as global::System.Net.Configuration.ConnectionManagementSection;
			if (connectionManagementSection != null)
			{
				ServicePointManager.manager = new global::System.Net.Configuration.ConnectionManagementData(null);
				foreach (object obj in connectionManagementSection.ConnectionManagement)
				{
					global::System.Net.Configuration.ConnectionManagementElement connectionManagementElement = (global::System.Net.Configuration.ConnectionManagementElement)obj;
					ServicePointManager.manager.Add(connectionManagementElement.Address, connectionManagementElement.MaxConnection);
				}
				ServicePointManager.defaultConnectionLimit = (int)ServicePointManager.manager.GetMaxConnections("*");
				return;
			}
			ServicePointManager.manager = (global::System.Net.Configuration.ConnectionManagementData)global::System.Configuration.ConfigurationSettings.GetConfig("system.net/connectionManagement");
			if (ServicePointManager.manager != null)
			{
				ServicePointManager.defaultConnectionLimit = (int)ServicePointManager.manager.GetMaxConnections("*");
			}
		}

		[Obsolete("Use ServerCertificateValidationCallback instead", false)]
		public static ICertificatePolicy CertificatePolicy
		{
			get
			{
				return ServicePointManager.policy;
			}
			set
			{
				ServicePointManager.policy = value;
			}
		}

		[global::System.MonoTODO("CRL checks not implemented")]
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
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[global::System.MonoTODO]
		public static int DnsRefreshTimeout
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

		[global::System.MonoTODO]
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
				ServicePointManager.RecycleServicePoints();
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

		public static global::System.Net.Security.RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				return ServicePointManager.server_cert_cb;
			}
			set
			{
				ServicePointManager.server_cert_cb = value;
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

		public static ServicePoint FindServicePoint(global::System.Uri address)
		{
			return ServicePointManager.FindServicePoint(address, GlobalProxySelection.Select);
		}

		public static ServicePoint FindServicePoint(string uriString, IWebProxy proxy)
		{
			return ServicePointManager.FindServicePoint(new global::System.Uri(uriString), proxy);
		}

		public static ServicePoint FindServicePoint(global::System.Uri address, IWebProxy proxy)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			ServicePointManager.RecycleServicePoints();
			bool flag = false;
			bool flag2 = false;
			if (proxy != null && !proxy.IsBypassed(address))
			{
				flag = true;
				bool flag3 = address.Scheme == "https";
				address = proxy.GetProxy(address);
				if (address.Scheme != "http" && !flag3)
				{
					throw new NotSupportedException("Proxy scheme not supported.");
				}
				if (flag3 && address.Scheme == "http")
				{
					flag2 = true;
				}
			}
			address = new global::System.Uri(address.Scheme + "://" + address.Authority);
			ServicePoint servicePoint = null;
			global::System.Collections.Specialized.HybridDictionary hybridDictionary = ServicePointManager.servicePoints;
			lock (hybridDictionary)
			{
				ServicePointManager.SPKey spkey = new ServicePointManager.SPKey(address, flag2);
				servicePoint = ServicePointManager.servicePoints[spkey] as ServicePoint;
				if (servicePoint != null)
				{
					return servicePoint;
				}
				if (ServicePointManager.maxServicePoints > 0 && ServicePointManager.servicePoints.Count >= ServicePointManager.maxServicePoints)
				{
					throw new InvalidOperationException("maximum number of service points reached");
				}
				string text = address.ToString();
				int maxConnections = (int)ServicePointManager.manager.GetMaxConnections(text);
				servicePoint = new ServicePoint(address, maxConnections, ServicePointManager.maxServicePointIdleTime);
				servicePoint.Expect100Continue = ServicePointManager.expectContinue;
				servicePoint.UseNagleAlgorithm = ServicePointManager.useNagle;
				servicePoint.UsesProxy = flag;
				servicePoint.UseConnect = flag2;
				ServicePointManager.servicePoints.Add(spkey, servicePoint);
			}
			return servicePoint;
		}

		internal static void RecycleServicePoints()
		{
			ArrayList arrayList = new ArrayList();
			global::System.Collections.Specialized.HybridDictionary hybridDictionary = ServicePointManager.servicePoints;
			lock (hybridDictionary)
			{
				IDictionaryEnumerator dictionaryEnumerator = ServicePointManager.servicePoints.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					ServicePoint servicePoint = (ServicePoint)dictionaryEnumerator.Value;
					if (servicePoint.AvailableForRecycling)
					{
						arrayList.Add(dictionaryEnumerator.Key);
					}
				}
				for (int i = 0; i < arrayList.Count; i++)
				{
					ServicePointManager.servicePoints.Remove(arrayList[i]);
				}
				if (ServicePointManager.maxServicePoints != 0 && ServicePointManager.servicePoints.Count > ServicePointManager.maxServicePoints)
				{
					SortedList sortedList = new SortedList(ServicePointManager.servicePoints.Count);
					dictionaryEnumerator = ServicePointManager.servicePoints.GetEnumerator();
					while (dictionaryEnumerator.MoveNext())
					{
						ServicePoint servicePoint2 = (ServicePoint)dictionaryEnumerator.Value;
						if (servicePoint2.CurrentConnections == 0)
						{
							while (sortedList.ContainsKey(servicePoint2.IdleSince))
							{
								servicePoint2.IdleSince = servicePoint2.IdleSince.AddMilliseconds(1.0);
							}
							sortedList.Add(servicePoint2.IdleSince, servicePoint2.Address);
						}
					}
					int num = 0;
					while (num < sortedList.Count && ServicePointManager.servicePoints.Count > ServicePointManager.maxServicePoints)
					{
						ServicePointManager.servicePoints.Remove(sortedList.GetByIndex(num));
						num++;
					}
				}
			}
		}

		public const int DefaultNonPersistentConnectionLimit = 4;

		public const int DefaultPersistentConnectionLimit = 2;

		private const string configKey = "system.net/connectionManagement";

		private static global::System.Collections.Specialized.HybridDictionary servicePoints = new global::System.Collections.Specialized.HybridDictionary();

		private static ICertificatePolicy policy = new DefaultCertificatePolicy();

		private static int defaultConnectionLimit = 2;

		private static int maxServicePointIdleTime = 900000;

		private static int maxServicePoints = 0;

		private static bool _checkCRL = false;

		private static SecurityProtocolType _securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

		private static bool expectContinue = true;

		private static bool useNagle;

		private static global::System.Net.Security.RemoteCertificateValidationCallback server_cert_cb;

		private static global::System.Net.Configuration.ConnectionManagementData manager;

		private class SPKey
		{
			public SPKey(global::System.Uri uri, bool use_connect)
			{
				this.uri = uri;
				this.use_connect = use_connect;
			}

			public global::System.Uri Uri
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

			public override int GetHashCode()
			{
				return this.uri.GetHashCode() + ((!this.use_connect) ? 0 : 1);
			}

			public override bool Equals(object obj)
			{
				ServicePointManager.SPKey spkey = obj as ServicePointManager.SPKey;
				return obj != null && this.uri.Equals(spkey.uri) && spkey.use_connect == this.use_connect;
			}

			private global::System.Uri uri;

			private bool use_connect;
		}

		internal class ChainValidationHelper
		{
			public ChainValidationHelper(object sender)
			{
				this.sender = sender;
			}

			public string Host
			{
				get
				{
					if (this.host == null && this.sender is HttpWebRequest)
					{
						this.host = ((HttpWebRequest)this.sender).Address.Host;
					}
					return this.host;
				}
				set
				{
					this.host = value;
				}
			}

			internal ValidationResult ValidateChain(Mono.Security.X509.X509CertificateCollection certs)
			{
				bool flag = false;
				if (certs == null || certs.Count == 0)
				{
					return null;
				}
				ICertificatePolicy certificatePolicy = ServicePointManager.CertificatePolicy;
				global::System.Net.Security.RemoteCertificateValidationCallback serverCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback;
				global::System.Security.Cryptography.X509Certificates.X509Chain x509Chain = new global::System.Security.Cryptography.X509Certificates.X509Chain();
				x509Chain.ChainPolicy = new global::System.Security.Cryptography.X509Certificates.X509ChainPolicy();
				for (int i = 1; i < certs.Count; i++)
				{
					global::System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate = new global::System.Security.Cryptography.X509Certificates.X509Certificate2(certs[i].RawData);
					x509Chain.ChainPolicy.ExtraStore.Add(x509Certificate);
				}
				global::System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate2 = new global::System.Security.Cryptography.X509Certificates.X509Certificate2(certs[0].RawData);
				int num = 0;
				global::System.Net.Security.SslPolicyErrors sslPolicyErrors = global::System.Net.Security.SslPolicyErrors.None;
				try
				{
					if (!x509Chain.Build(x509Certificate2))
					{
						sslPolicyErrors |= ServicePointManager.ChainValidationHelper.GetErrorsFromChain(x509Chain);
					}
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine("ERROR building certificate chain: {0}", ex);
					Console.Error.WriteLine("Please, report this problem to the Mono team");
					sslPolicyErrors |= global::System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors;
				}
				if (!ServicePointManager.ChainValidationHelper.CheckCertificateUsage(x509Certificate2))
				{
					sslPolicyErrors |= global::System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors;
					num = -2146762490;
				}
				if (!ServicePointManager.ChainValidationHelper.CheckServerIdentity(certs[0], this.Host))
				{
					sslPolicyErrors |= global::System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch;
					num = -2146762481;
				}
				bool flag2 = false;
				if (ServicePointManager.ChainValidationHelper.is_macosx)
				{
					try
					{
						Mono.Security.X509.OSX509Certificates.SecTrustResult secTrustResult = Mono.Security.X509.OSX509Certificates.TrustEvaluateSsl(certs);
						flag2 = secTrustResult == Mono.Security.X509.OSX509Certificates.SecTrustResult.Proceed || secTrustResult == Mono.Security.X509.OSX509Certificates.SecTrustResult.Unspecified;
					}
					catch
					{
					}
					if (flag2)
					{
						num = 0;
						sslPolicyErrors = global::System.Net.Security.SslPolicyErrors.None;
					}
				}
				if (certificatePolicy != null && (!(certificatePolicy is DefaultCertificatePolicy) || serverCertificateValidationCallback == null))
				{
					ServicePoint servicePoint = null;
					HttpWebRequest httpWebRequest = this.sender as HttpWebRequest;
					if (httpWebRequest != null)
					{
						servicePoint = httpWebRequest.ServicePoint;
					}
					if (num == 0 && sslPolicyErrors != global::System.Net.Security.SslPolicyErrors.None)
					{
						num = ServicePointManager.ChainValidationHelper.GetStatusFromChain(x509Chain);
					}
					flag2 = certificatePolicy.CheckValidationResult(servicePoint, x509Certificate2, httpWebRequest, num);
					flag = !flag2 && !(certificatePolicy is DefaultCertificatePolicy);
				}
				if (serverCertificateValidationCallback != null)
				{
					flag2 = serverCertificateValidationCallback(this.sender, x509Certificate2, x509Chain, sslPolicyErrors);
					flag = !flag2;
				}
				return new ValidationResult(flag2, flag, num);
			}

			private static int GetStatusFromChain(global::System.Security.Cryptography.X509Certificates.X509Chain chain)
			{
				long num = 0L;
				foreach (global::System.Security.Cryptography.X509Certificates.X509ChainStatus x509ChainStatus in chain.ChainStatus)
				{
					global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags status = x509ChainStatus.Status;
					if (status != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotTimeValid) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762495));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotTimeNested) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762494));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.Revoked) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762484));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotSignatureValid) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146869244));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotValidForUsage) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762480));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762487));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.RevocationStatusUnknown) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146885614));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.Cyclic) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762486));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidExtension) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762485));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidPolicyConstraints) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762483));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidBasicConstraints) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146869223));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidNameConstraints) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762476));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasNotSupportedNameConstraint) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762476));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasNotDefinedNameConstraint) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762476));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasNotPermittedNameConstraint) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762476));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasExcludedNameConstraint) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762476));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.PartialChain) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762486));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.CtlNotTimeValid) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762495));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.CtlNotSignatureValid) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146869244));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.CtlNotValidForUsage) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762480));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.OfflineRevocation) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146885614));
						}
						else if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoIssuanceChainPolicy) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							num = (long)((ulong)(-2146762489));
						}
						else
						{
							num = (long)((ulong)(-2146762485));
						}
						break;
					}
				}
				return (int)num;
			}

			private static global::System.Net.Security.SslPolicyErrors GetErrorsFromChain(global::System.Security.Cryptography.X509Certificates.X509Chain chain)
			{
				global::System.Net.Security.SslPolicyErrors sslPolicyErrors = global::System.Net.Security.SslPolicyErrors.None;
				foreach (global::System.Security.Cryptography.X509Certificates.X509ChainStatus x509ChainStatus in chain.ChainStatus)
				{
					if (x509ChainStatus.Status != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						sslPolicyErrors |= global::System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors;
						break;
					}
				}
				return sslPolicyErrors;
			}

			private static bool CheckCertificateUsage(global::System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
			{
				bool flag;
				try
				{
					if (cert.Version < 3)
					{
						flag = true;
					}
					else
					{
						global::System.Security.Cryptography.X509Certificates.X509KeyUsageExtension x509KeyUsageExtension = (global::System.Security.Cryptography.X509Certificates.X509KeyUsageExtension)cert.Extensions["2.5.29.15"];
						global::System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = (global::System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension)cert.Extensions["2.5.29.37"];
						if (x509KeyUsageExtension != null && x509EnhancedKeyUsageExtension != null)
						{
							if ((x509KeyUsageExtension.KeyUsages & ServicePointManager.ChainValidationHelper.s_flags) == global::System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.None)
							{
								flag = false;
							}
							else
							{
								flag = x509EnhancedKeyUsageExtension.EnhancedKeyUsages["1.3.6.1.5.5.7.3.1"] != null || x509EnhancedKeyUsageExtension.EnhancedKeyUsages["2.16.840.1.113730.4.1"] != null;
							}
						}
						else if (x509KeyUsageExtension != null)
						{
							flag = (x509KeyUsageExtension.KeyUsages & ServicePointManager.ChainValidationHelper.s_flags) != global::System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.None;
						}
						else if (x509EnhancedKeyUsageExtension != null)
						{
							flag = x509EnhancedKeyUsageExtension.EnhancedKeyUsages["1.3.6.1.5.5.7.3.1"] != null || x509EnhancedKeyUsageExtension.EnhancedKeyUsages["2.16.840.1.113730.4.1"] != null;
						}
						else
						{
							global::System.Security.Cryptography.X509Certificates.X509Extension x509Extension = cert.Extensions["2.16.840.1.113730.1.1"];
							if (x509Extension != null)
							{
								string text = x509Extension.NetscapeCertType(false);
								flag = text.IndexOf("SSL Server Authentication") != -1;
							}
							else
							{
								flag = true;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine("ERROR processing certificate: {0}", ex);
					Console.Error.WriteLine("Please, report this problem to the Mono team");
					flag = false;
				}
				return flag;
			}

			private static bool CheckServerIdentity(Mono.Security.X509.X509Certificate cert, string targetHost)
			{
				bool flag;
				try
				{
					Mono.Security.X509.X509Extension x509Extension = cert.Extensions["2.5.29.17"];
					if (x509Extension != null)
					{
						SubjectAltNameExtension subjectAltNameExtension = new SubjectAltNameExtension(x509Extension);
						foreach (string text in subjectAltNameExtension.DNSNames)
						{
							if (ServicePointManager.ChainValidationHelper.Match(targetHost, text))
							{
								return true;
							}
						}
						foreach (string text2 in subjectAltNameExtension.IPAddresses)
						{
							if (text2 == targetHost)
							{
								return true;
							}
						}
					}
					flag = ServicePointManager.ChainValidationHelper.CheckDomainName(cert.SubjectName, targetHost);
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine("ERROR processing certificate: {0}", ex);
					Console.Error.WriteLine("Please, report this problem to the Mono team");
					flag = false;
				}
				return flag;
			}

			private static bool CheckDomainName(string subjectName, string targetHost)
			{
				string text = string.Empty;
				global::System.Text.RegularExpressions.Regex regex = new global::System.Text.RegularExpressions.Regex("CN\\s*=\\s*([^,]*)");
				global::System.Text.RegularExpressions.MatchCollection matchCollection = regex.Matches(subjectName);
				if (matchCollection.Count == 1 && matchCollection[0].Success)
				{
					text = matchCollection[0].Groups[1].Value.ToString();
				}
				return ServicePointManager.ChainValidationHelper.Match(targetHost, text);
			}

			private static bool Match(string hostname, string pattern)
			{
				int num = pattern.IndexOf('*');
				if (num == -1)
				{
					return string.Compare(hostname, pattern, true, CultureInfo.InvariantCulture) == 0;
				}
				if (num != pattern.Length - 1 && pattern[num + 1] != '.')
				{
					return false;
				}
				int num2 = pattern.IndexOf('*', num + 1);
				if (num2 != -1)
				{
					return false;
				}
				string text = pattern.Substring(num + 1);
				int num3 = hostname.Length - text.Length;
				if (num3 <= 0)
				{
					return false;
				}
				if (string.Compare(hostname, num3, text, 0, text.Length, true, CultureInfo.InvariantCulture) != 0)
				{
					return false;
				}
				if (num == 0)
				{
					int num4 = hostname.IndexOf('.');
					return num4 == -1 || num4 >= hostname.Length - text.Length;
				}
				string text2 = pattern.Substring(0, num);
				return string.Compare(hostname, 0, text2, 0, text2.Length, true, CultureInfo.InvariantCulture) == 0;
			}

			private object sender;

			private string host;

			private static bool is_macosx = File.Exists("/System/Library/Frameworks/Security.framework/Security");

			private static global::System.Security.Cryptography.X509Certificates.X509KeyUsageFlags s_flags = global::System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.KeyAgreement | global::System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.KeyEncipherment | global::System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.DigitalSignature;
		}
	}
}
