using System;
using System.Configuration;
using System.Net.Cache;
using System.Xml;

namespace System.Net.Configuration
{
	public sealed class FtpCachePolicyElement : ConfigurationElement
	{
		static FtpCachePolicyElement()
		{
			FtpCachePolicyElement.properties.Add(FtpCachePolicyElement.policyLevelProp);
		}

		[ConfigurationProperty("policyLevel", DefaultValue = "Default")]
		public global::System.Net.Cache.RequestCacheLevel PolicyLevel
		{
			get
			{
				return (global::System.Net.Cache.RequestCacheLevel)((int)base[FtpCachePolicyElement.policyLevelProp]);
			}
			set
			{
				base[FtpCachePolicyElement.policyLevelProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return FtpCachePolicyElement.properties;
			}
		}

		[global::System.MonoTODO]
		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected override void Reset(ConfigurationElement parentElement)
		{
			throw new NotImplementedException();
		}

		private static ConfigurationProperty policyLevelProp = new ConfigurationProperty("policyLevel", typeof(global::System.Net.Cache.RequestCacheLevel), global::System.Net.Cache.RequestCacheLevel.Default);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
