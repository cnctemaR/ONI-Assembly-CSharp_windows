using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveTransparency : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveTransparency);
			}
		}

		protected override void OnDisabled()
		{
			this.OnDestroy();
		}

		private void OnDestroy()
		{
			AdaptivePerformanceRenderSettings.SkipTransparentObjects = false;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.SkipTransparentObjects = this.Scale < 1f;
			}
		}
	}
}
