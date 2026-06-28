using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class PerformanceCountersElement : ConfigurationElement
	{
		static PerformanceCountersElement()
		{
			PerformanceCountersElement.properties.Add(PerformanceCountersElement.enabledProp);
		}

		[ConfigurationProperty("enabled", DefaultValue = "False")]
		public bool Enabled
		{
			get
			{
				return (bool)base[PerformanceCountersElement.enabledProp];
			}
			set
			{
				base[PerformanceCountersElement.enabledProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return PerformanceCountersElement.properties;
			}
		}

		private static ConfigurationProperty enabledProp = new ConfigurationProperty("enabled", typeof(bool), false);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
