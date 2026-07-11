using System;
using System.Configuration;
using Unity;

namespace System.Net.Configuration
{
	public sealed class SettingsSection : ConfigurationSection
	{
		static SettingsSection()
		{
			SettingsSection.properties.Add(SettingsSection.httpWebRequestProp);
			SettingsSection.properties.Add(SettingsSection.ipv6Prop);
			SettingsSection.properties.Add(SettingsSection.performanceCountersProp);
			SettingsSection.properties.Add(SettingsSection.servicePointManagerProp);
			SettingsSection.properties.Add(SettingsSection.socketProp);
			SettingsSection.properties.Add(SettingsSection.webProxyScriptProp);
		}

		[ConfigurationProperty("httpWebRequest")]
		public HttpWebRequestElement HttpWebRequest
		{
			get
			{
				return (HttpWebRequestElement)base[SettingsSection.httpWebRequestProp];
			}
		}

		[ConfigurationProperty("ipv6")]
		public Ipv6Element Ipv6
		{
			get
			{
				return (Ipv6Element)base[SettingsSection.ipv6Prop];
			}
		}

		[ConfigurationProperty("performanceCounters")]
		public PerformanceCountersElement PerformanceCounters
		{
			get
			{
				return (PerformanceCountersElement)base[SettingsSection.performanceCountersProp];
			}
		}

		[ConfigurationProperty("servicePointManager")]
		public ServicePointManagerElement ServicePointManager
		{
			get
			{
				return (ServicePointManagerElement)base[SettingsSection.servicePointManagerProp];
			}
		}

		[ConfigurationProperty("socket")]
		public SocketElement Socket
		{
			get
			{
				return (SocketElement)base[SettingsSection.socketProp];
			}
		}

		[ConfigurationProperty("webProxyScript")]
		public WebProxyScriptElement WebProxyScript
		{
			get
			{
				return (WebProxyScriptElement)base[SettingsSection.webProxyScriptProp];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SettingsSection.properties;
			}
		}

		public HttpListenerElement HttpListener
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public WebUtilityElement WebUtility
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty httpWebRequestProp = new ConfigurationProperty("httpWebRequest", typeof(HttpWebRequestElement));

		private static ConfigurationProperty ipv6Prop = new ConfigurationProperty("ipv6", typeof(Ipv6Element));

		private static ConfigurationProperty performanceCountersProp = new ConfigurationProperty("performanceCounters", typeof(PerformanceCountersElement));

		private static ConfigurationProperty servicePointManagerProp = new ConfigurationProperty("servicePointManager", typeof(ServicePointManagerElement));

		private static ConfigurationProperty webProxyScriptProp = new ConfigurationProperty("webProxyScript", typeof(WebProxyScriptElement));

		private static ConfigurationProperty socketProp = new ConfigurationProperty("socket", typeof(SocketElement));
	}
}
