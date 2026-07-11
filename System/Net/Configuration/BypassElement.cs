using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class BypassElement : ConfigurationElement
	{
		public BypassElement()
		{
		}

		public BypassElement(string address)
		{
			this.Address = address;
		}

		static BypassElement()
		{
			BypassElement.properties.Add(BypassElement.addressProp);
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

		private static ConfigurationProperty addressProp = new ConfigurationProperty("Address", typeof(string), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
	}
}
