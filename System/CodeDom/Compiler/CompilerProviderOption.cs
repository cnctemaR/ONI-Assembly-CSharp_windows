using System;
using System.Configuration;

namespace System.CodeDom.Compiler
{
	internal sealed class CompilerProviderOption : ConfigurationElement
	{
		static CompilerProviderOption()
		{
			CompilerProviderOption.properties.Add(CompilerProviderOption.nameProp);
			CompilerProviderOption.properties.Add(CompilerProviderOption.valueProp);
		}

		[ConfigurationProperty("name", DefaultValue = "", Options = ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey)]
		public string Name
		{
			get
			{
				return (string)base[CompilerProviderOption.nameProp];
			}
			set
			{
				base[CompilerProviderOption.nameProp] = value;
			}
		}

		[ConfigurationProperty("value", DefaultValue = "", Options = ConfigurationPropertyOptions.IsRequired)]
		public string Value
		{
			get
			{
				return (string)base[CompilerProviderOption.valueProp];
			}
			set
			{
				base[CompilerProviderOption.valueProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return CompilerProviderOption.properties;
			}
		}

		private static ConfigurationProperty nameProp = new ConfigurationProperty("name", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

		private static ConfigurationProperty valueProp = new ConfigurationProperty("value", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
