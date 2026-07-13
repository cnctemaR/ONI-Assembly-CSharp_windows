using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveSorting : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveSorting);
			}
		}

		protected override void OnDisabled()
		{
			AdaptivePerformanceRenderSettings.SkipFrontToBackSorting = this.m_DefaultSorting;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultSorting = AdaptivePerformanceRenderSettings.SkipFrontToBackSorting;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.SkipFrontToBackSorting = this.Scale < 1f;
			}
		}

		private bool m_DefaultSorting;
	}
}
