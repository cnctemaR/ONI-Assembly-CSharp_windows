using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveShadowCascade : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveShadowCascade);
			}
		}

		protected override void OnDisabled()
		{
			AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias = this.m_DefaultCascadeCount;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultCascadeCount = AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias = (int)(2f * this.Scale);
			}
		}

		private int m_DefaultCascadeCount;
	}
}
