using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveMSAA : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveMSAA);
			}
		}

		protected override void OnDisabled()
		{
			AdaptivePerformanceRenderSettings.AntiAliasingQualityBias = this.m_DefaultAA;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultAA = AdaptivePerformanceRenderSettings.AntiAliasingQualityBias;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.AntiAliasingQualityBias = (int)(2f * this.Scale);
			}
		}

		private int m_DefaultAA;
	}
}
