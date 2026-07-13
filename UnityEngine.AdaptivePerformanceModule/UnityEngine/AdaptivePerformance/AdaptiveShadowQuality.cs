using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveShadowQuality : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveShadowQuality);
			}
		}

		protected override void OnDisabled()
		{
			AdaptivePerformanceRenderSettings.ShadowQualityBias = this.m_DefaultShadowQualityBias;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultShadowQualityBias = AdaptivePerformanceRenderSettings.ShadowQualityBias;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.ShadowQualityBias = (int)(3f - 3f * this.Scale);
			}
		}

		private int m_DefaultShadowQualityBias;
	}
}
