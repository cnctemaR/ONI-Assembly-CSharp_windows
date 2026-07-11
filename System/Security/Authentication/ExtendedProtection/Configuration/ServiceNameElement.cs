using System;
using System.Configuration;

namespace System.Security.Authentication.ExtendedProtection.Configuration
{
	public sealed class ServiceNameElement : ConfigurationElement
	{
		static ServiceNameElement()
		{
			ServiceNameElement.properties.Add(ServiceNameElement.name);
		}

		[ConfigurationProperty("name")]
		public string Name
		{
			get
			{
				return (string)base[ServiceNameElement.name];
			}
			set
			{
				base[ServiceNameElement.name] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ServiceNameElement.properties;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty name = ConfigUtil.BuildProperty(typeof(ServiceNameElement), "Name");
	}
}
