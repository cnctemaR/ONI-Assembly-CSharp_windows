using System;
using System.Configuration;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Serialization.Configuration
{
	public sealed class NetDataContractSerializerSection : ConfigurationSection
	{
		[SecurityCritical]
		[ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
		internal static bool TryUnsafeGetSection(out NetDataContractSerializerSection section)
		{
			section = (NetDataContractSerializerSection)ConfigurationManager.GetSection(ConfigurationStrings.NetDataContractSerializerSectionPath);
			return section != null;
		}

		[ConfigurationProperty("enableUnsafeTypeForwarding", DefaultValue = false)]
		public bool EnableUnsafeTypeForwarding
		{
			get
			{
				return (bool)base["enableUnsafeTypeForwarding"];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ConfigurationPropertyCollection
					{
						new ConfigurationProperty("enableUnsafeTypeForwarding", typeof(bool), false, null, null, ConfigurationPropertyOptions.None)
					};
				}
				return this.properties;
			}
		}

		private ConfigurationPropertyCollection properties;
	}
}
