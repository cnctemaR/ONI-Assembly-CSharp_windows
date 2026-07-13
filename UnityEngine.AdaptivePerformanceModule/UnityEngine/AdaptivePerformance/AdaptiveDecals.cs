using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveDecals : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveDecals);
			}
		}

		protected override void OnDisabled()
		{
			AdaptivePerformanceRenderSettings.DecalsDrawDistance = this.m_DefaultDecalsDistance;
		}

		protected override void OnEnabled()
		{
			this.m_DefaultDecalsDistance = AdaptivePerformanceRenderSettings.DecalsDrawDistance;
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			if (flag)
			{
				AdaptivePerformanceRenderSettings.DecalsDrawDistance = (float)((int)(this.m_DefaultDecalsDistance * this.Scale));
			}
		}

		private float m_DefaultDecalsDistance;
	}
}
