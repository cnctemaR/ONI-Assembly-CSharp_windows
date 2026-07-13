using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveLOD : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveLOD);
			}
		}

		protected override void OnDisabled()
		{
			QualitySettings.lodBias = this.m_DefaultLodBias;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultLodBias = QualitySettings.lodBias;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				QualitySettings.lodBias = this.m_DefaultLodBias * this.Scale;
			}
		}

		private float m_DefaultLodBias;
	}
}
