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
		public RequestCacheLevel PolicyLevel
		{
			get
			{
				return (RequestCacheLevel)base[FtpCachePolicyElement.policyLevelProp];
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

		[MonoTODO]
		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected override void Reset(ConfigurationElement parentElement)
		{
			throw new NotImplementedException();
		}

		private static ConfigurationProperty policyLevelProp = new ConfigurationProperty("policyLevel", typeof(RequestCacheLevel), RequestCacheLevel.Default);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
