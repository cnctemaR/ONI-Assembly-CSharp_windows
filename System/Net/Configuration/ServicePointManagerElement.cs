using System;
using System.Configuration;
using System.Net.Security;
using Unity;

namespace System.Net.Configuration
{
	public sealed class ServicePointManagerElement : ConfigurationElement
	{
		static ServicePointManagerElement()
		{
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.checkCertificateNameProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.checkCertificateRevocationListProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.dnsRefreshTimeoutProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.enableDnsRoundRobinProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.expect100ContinueProp);
			ServicePointManagerElement.properties.Add(ServicePointManagerElement.useNagleAlgorithmProp);
		}

		[ConfigurationProperty("checkCertificateName", DefaultValue = "True")]
		public bool CheckCertificateName
		{
			get
			{
				return (bool)base[ServicePointManagerElement.checkCertificateNameProp];
			}
			set
			{
				base[ServicePointManagerElement.checkCertificateNameProp] = value;
			}
		}

		[ConfigurationProperty("checkCertificateRevocationList", DefaultValue = "False")]
		public bool CheckCertificateRevocationList
		{
			get
			{
				return (bool)base[ServicePointManagerElement.checkCertificateRevocationListProp];
			}
			set
			{
				base[ServicePointManagerElement.checkCertificateRevocationListProp] = value;
			}
		}

		[ConfigurationProperty("dnsRefreshTimeout", DefaultValue = "120000")]
		public int DnsRefreshTimeout
		{
			get
			{
				return (int)base[ServicePointManagerElement.dnsRefreshTimeoutProp];
			}
			set
			{
				base[ServicePointManagerElement.dnsRefreshTimeoutProp] = value;
			}
		}

		[ConfigurationProperty("enableDnsRoundRobin", DefaultValue = "False")]
		public bool EnableDnsRoundRobin
		{
			get
			{
				return (bool)base[ServicePointManagerElement.enableDnsRoundRobinProp];
			}
			set
			{
				base[ServicePointManagerElement.enableDnsRoundRobinProp] = value;
			}
		}

		[ConfigurationProperty("expect100Continue", DefaultValue = "True")]
		public bool Expect100Continue
		{
			get
			{
				return (bool)base[ServicePointManagerElement.expect100ContinueProp];
			}
			set
			{
				base[ServicePointManagerElement.expect100ContinueProp] = value;
			}
		}

		[ConfigurationProperty("useNagleAlgorithm", DefaultValue = "True")]
		public bool UseNagleAlgorithm
		{
			get
			{
				return (bool)base[ServicePointManagerElement.useNagleAlgorithmProp];
			}
			set
			{
				base[ServicePointManagerElement.useNagleAlgorithmProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ServicePointManagerElement.properties;
			}
		}

		[MonoTODO]
		protected override void PostDeserialize()
		{
		}

		public EncryptionPolicy EncryptionPolicy
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return EncryptionPolicy.RequireEncryption;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty checkCertificateNameProp = new ConfigurationProperty("checkCertificateName", typeof(bool), true);

		private static ConfigurationProperty checkCertificateRevocationListProp = new ConfigurationProperty("checkCertificateRevocationList", typeof(bool), false);

		private static ConfigurationProperty dnsRefreshTimeoutProp = new ConfigurationProperty("dnsRefreshTimeout", typeof(int), 120000);

		private static ConfigurationProperty enableDnsRoundRobinProp = new ConfigurationProperty("enableDnsRoundRobin", typeof(bool), false);

		private static ConfigurationProperty expect100ContinueProp = new ConfigurationProperty("expect100Continue", typeof(bool), true);

		private static ConfigurationProperty useNagleAlgorithmProp = new ConfigurationProperty("useNagleAlgorithm", typeof(bool), true);
	}
}
