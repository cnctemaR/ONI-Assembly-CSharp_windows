using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class SocketElement : ConfigurationElement
	{
		public SocketElement()
		{
			SocketElement.alwaysUseCompletionPortsForAcceptProp = new ConfigurationProperty("alwaysUseCompletionPortsForAccept", typeof(bool), false);
			SocketElement.alwaysUseCompletionPortsForConnectProp = new ConfigurationProperty("alwaysUseCompletionPortsForConnect", typeof(bool), false);
			SocketElement.properties = new ConfigurationPropertyCollection();
			SocketElement.properties.Add(SocketElement.alwaysUseCompletionPortsForAcceptProp);
			SocketElement.properties.Add(SocketElement.alwaysUseCompletionPortsForConnectProp);
		}

		[ConfigurationProperty("alwaysUseCompletionPortsForAccept", DefaultValue = "False")]
		public bool AlwaysUseCompletionPortsForAccept
		{
			get
			{
				return (bool)base[SocketElement.alwaysUseCompletionPortsForAcceptProp];
			}
			set
			{
				base[SocketElement.alwaysUseCompletionPortsForAcceptProp] = value;
			}
		}

		[ConfigurationProperty("alwaysUseCompletionPortsForConnect", DefaultValue = "False")]
		public bool AlwaysUseCompletionPortsForConnect
		{
			get
			{
				return (bool)base[SocketElement.alwaysUseCompletionPortsForConnectProp];
			}
			set
			{
				base[SocketElement.alwaysUseCompletionPortsForConnectProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SocketElement.properties;
			}
		}

		[global::System.MonoTODO]
		protected override void PostDeserialize()
		{
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty alwaysUseCompletionPortsForAcceptProp;

		private static ConfigurationProperty alwaysUseCompletionPortsForConnectProp;
	}
}
