using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveBatching : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveBatching);
			}
		}

		protected override void OnDisabled()
		{
			AdaptivePerformanceRenderSettings.SkipDynamicBatching = this.m_DefaultState;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultState = AdaptivePerformanceRenderSettings.SkipDynamicBatching;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.SkipDynamicBatching = this.Scale < 1f;
			}
		}

		private bool m_DefaultState;
	}
}
