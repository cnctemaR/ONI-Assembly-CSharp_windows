using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveShadowDistance : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveShadowDistance);
			}
		}

		protected override void OnDisabled()
		{
			AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier = this.m_DefaultShadowDistance;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultShadowDistance = AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier = 1f * this.Scale;
			}
		}

		private float m_DefaultShadowDistance;
	}
}
