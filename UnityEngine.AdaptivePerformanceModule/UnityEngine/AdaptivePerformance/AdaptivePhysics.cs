using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptivePhysics : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptivePhysics);
			}
		}

		protected override void OnDisabled()
		{
			Time.fixedDeltaTime = this.m_fixedDeltaTimeDefault;
		}

		protected override void OnEnabled()
		{
			this.m_fixedDeltaTimeDefault = Time.fixedDeltaTime;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				Time.fixedDeltaTime = this.m_fixedDeltaTimeDefault / this.Scale;
			}
		}

		private float m_fixedDeltaTimeDefault;
	}
}
