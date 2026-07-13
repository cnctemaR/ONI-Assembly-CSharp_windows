using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveLayerCulling : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveLayerCulling);
			}
		}

		protected override void OnDisabled()
		{
			this.init = false;
			bool flag = !Camera.main || this.m_defaultDistances == null;
			if (!flag)
			{
				Camera.main.layerCullDistances = this.m_defaultDistances;
			}
		}

		protected override void OnEnabled()
		{
			this.AsignDefaultValues();
		}

		protected override void OnLevel()
		{
			bool flag = !Camera.main;
			if (!flag)
			{
				this.AsignDefaultValues();
				bool flag2 = base.ScaleChanged();
				if (flag2)
				{
					for (int i = 31; i >= 0; i--)
					{
						bool flag3 = this.m_defaultDistances[i] == 0f;
						if (!flag3)
						{
							this.m_scaledDistances[i] = this.m_defaultDistances[i] * this.Scale;
						}
					}
					Camera.main.layerCullDistances = this.m_scaledDistances;
				}
			}
		}

		private void AsignDefaultValues()
		{
			bool flag = this.m_cachedCamera == null || this.m_cachedCamera != Camera.main;
			if (flag)
			{
				this.m_cachedCamera = Camera.main;
				this.init = false;
			}
			bool flag2 = this.init || !Camera.main;
			if (!flag2)
			{
				this.m_defaultDistances = Camera.main.layerCullDistances;
				this.init = true;
			}
		}

		private float[] m_defaultDistances = new float[32];

		private float[] m_scaledDistances = new float[32];

		private bool init = false;

		private Camera m_cachedCamera;
	}
}
