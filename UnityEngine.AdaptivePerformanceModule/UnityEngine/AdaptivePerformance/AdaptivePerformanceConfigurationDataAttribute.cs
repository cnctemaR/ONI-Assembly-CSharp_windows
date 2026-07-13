using System;

namespace UnityEngine.AdaptivePerformance
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class AdaptivePerformanceConfigurationDataAttribute : Attribute
	{
		public string displayName { get; set; }

		public string buildSettingsKey { get; set; }

		private AdaptivePerformanceConfigurationDataAttribute()
		{
		}

		public AdaptivePerformanceConfigurationDataAttribute(string displayName, string buildSettingsKey)
		{
			this.displayName = displayName;
			this.buildSettingsKey = buildSettingsKey;
		}
	}
}
