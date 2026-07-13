using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveLut : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveLut);
			}
		}

		protected override void OnDisabled()
		{
			AdaptivePerformanceRenderSettings.LutBias = this.m_DefaultLutBias;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultLutBias = AdaptivePerformanceRenderSettings.LutBias;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.LutBias = this.Scale;
			}
		}

		private float m_DefaultLutBias;
	}
}
