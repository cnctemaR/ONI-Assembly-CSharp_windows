using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveViewDistance : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveViewDistance);
			}
		}

		protected override void OnDisabled()
		{
			bool flag = !Camera.main || this.m_DefaultFarClipPlane == -1f;
			if (!flag)
			{
				Camera.main.farClipPlane = this.m_DefaultFarClipPlane;
			}
		}

		protected override void OnEnabled()
		{
			bool flag = !Camera.main;
			if (!flag)
			{
				this.m_DefaultFarClipPlane = Camera.main.farClipPlane;
			}
		}

		protected override void OnLevel()
		{
			bool flag = !Camera.main;
			if (!flag)
			{
				bool flag2 = this.m_DefaultFarClipPlane == -1f;
				if (flag2)
				{
					this.m_DefaultFarClipPlane = Camera.main.farClipPlane;
				}
				bool flag3 = base.ScaleChanged();
				if (flag3)
				{
					Camera.main.farClipPlane = this.Scale;
				}
			}
		}

		private float m_DefaultFarClipPlane = -1f;
	}
}
