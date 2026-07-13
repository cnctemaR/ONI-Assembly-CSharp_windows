using System;

namespace UnityEngine.AdaptivePerformance.Basic
{
	[AdaptivePerformanceConfigurationData("Basic", "com.unity.adaptivePerformance.basic.provider_settings")]
	[Serializable]
	public class BasicProviderSettings : IAdaptivePerformanceSettings
	{
		private void Awake()
		{
			BasicProviderSettings.m_Instance = this;
		}

		internal static BasicProviderSettings GetSettings()
		{
			return BasicProviderSettings.m_Instance;
		}

		private static BasicProviderSettings m_Instance;
	}
}
