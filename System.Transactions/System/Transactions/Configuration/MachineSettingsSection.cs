using System;
using System.Configuration;

namespace System.Transactions.Configuration
{
	public class MachineSettingsSection : ConfigurationSection
	{
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "10675199.02:48:05.4775807")]
		[ConfigurationProperty("maxTimeout", DefaultValue = "00:10:00")]
		public TimeSpan MaxTimeout
		{
			get
			{
				return (TimeSpan)base["maxTimeout"];
			}
			set
			{
				base["maxTimeout"] = value;
			}
		}
	}
}
