using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class BypassElement : ConfigurationElement
	{
		static BypassElement()
		{
			BypassElement.properties.Add(BypassElement.addressProp);
		}

		public BypassElement()
		{
		}

		public BypassElement(string address)
		{
			this.Address = address;
		}

		[ConfigurationProperty("address", Options = ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey)]
		public string Address
		{
			get
			{
				return (string)base[BypassElement.addressProp];
			}
			set
			{
				base[BypassElement.addressProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return BypassElement.properties;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty addressProp = new ConfigurationProperty("address", typeof(string), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
	}
}
