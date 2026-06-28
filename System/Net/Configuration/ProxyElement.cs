using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class ProxyElement : ConfigurationElement
	{
		static ProxyElement()
		{
			ProxyElement.properties.Add(ProxyElement.bypassOnLocalProp);
			ProxyElement.properties.Add(ProxyElement.proxyAddressProp);
			ProxyElement.properties.Add(ProxyElement.scriptLocationProp);
			ProxyElement.properties.Add(ProxyElement.useSystemDefaultProp);
		}

		[ConfigurationProperty("autoDetect", DefaultValue = "Unspecified")]
		public ProxyElement.AutoDetectValues AutoDetect
		{
			get
			{
				return (ProxyElement.AutoDetectValues)((int)base[ProxyElement.autoDetectProp]);
			}
			set
			{
				base[ProxyElement.autoDetectProp] = value;
			}
		}

		[ConfigurationProperty("bypassonlocal", DefaultValue = "Unspecified")]
		public ProxyElement.BypassOnLocalValues BypassOnLocal
		{
			get
			{
				return (ProxyElement.BypassOnLocalValues)((int)base[ProxyElement.bypassOnLocalProp]);
			}
			set
			{
				base[ProxyElement.bypassOnLocalProp] = value;
			}
		}

		[ConfigurationProperty("proxyaddress")]
		public global::System.Uri ProxyAddress
		{
			get
			{
				return (global::System.Uri)base[ProxyElement.proxyAddressProp];
			}
			set
			{
				base[ProxyElement.proxyAddressProp] = value;
			}
		}

		[ConfigurationProperty("scriptLocation")]
		public global::System.Uri ScriptLocation
		{
			get
			{
				return (global::System.Uri)base[ProxyElement.scriptLocationProp];
			}
			set
			{
				base[ProxyElement.scriptLocationProp] = value;
			}
		}

		[ConfigurationProperty("usesystemdefault", DefaultValue = "Unspecified")]
		public ProxyElement.UseSystemDefaultValues UseSystemDefault
		{
			get
			{
				return (ProxyElement.UseSystemDefaultValues)((int)base[ProxyElement.useSystemDefaultProp]);
			}
			set
			{
				base[ProxyElement.useSystemDefaultProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ProxyElement.properties;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty autoDetectProp = new ConfigurationProperty("autoDetect", typeof(ProxyElement.AutoDetectValues), ProxyElement.AutoDetectValues.Unspecified);

		private static ConfigurationProperty bypassOnLocalProp = new ConfigurationProperty("bypassonlocal", typeof(ProxyElement.BypassOnLocalValues), ProxyElement.BypassOnLocalValues.Unspecified);

		private static ConfigurationProperty proxyAddressProp = new ConfigurationProperty("proxyaddress", typeof(global::System.Uri), null);

		private static ConfigurationProperty scriptLocationProp = new ConfigurationProperty("scriptLocation", typeof(global::System.Uri), null);

		private static ConfigurationProperty useSystemDefaultProp = new ConfigurationProperty("UseSystemDefault", typeof(ProxyElement.UseSystemDefaultValues), ProxyElement.UseSystemDefaultValues.Unspecified);

		public enum BypassOnLocalValues
		{
			Unspecified = -1,
			True = 1,
			False = 0
		}

		public enum UseSystemDefaultValues
		{
			Unspecified = -1,
			True = 1,
			False = 0
		}

		public enum AutoDetectValues
		{
			Unspecified = -1,
			True = 1,
			False = 0
		}
	}
}
