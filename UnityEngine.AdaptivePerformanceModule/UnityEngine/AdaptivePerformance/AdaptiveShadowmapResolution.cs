using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveShadowmapResolution : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveShadowmapResolution);
			}
		}

		protected override void OnDisabled()
		{
			AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = this.m_DefaultShadowmapResolution;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultShadowmapResolution = AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = 1f * this.Scale;
			}
		}

		private float m_DefaultShadowmapResolution;
	}
}
