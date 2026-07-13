using System;

namespace UnityEngine.AdaptivePerformance
{
	[Serializable]
	public class AdaptivePerformanceIndexerSettings
	{
		public bool active
		{
			get
			{
				return this.m_Active;
			}
			set
			{
				bool flag = this.m_Active == value;
				if (!flag)
				{
					this.m_Active = value;
				}
			}
		}

		public float thermalActionDelay
		{
			get
			{
				return this.m_ThermalActionDelay;
			}
			set
			{
				this.m_ThermalActionDelay = value;
			}
		}

		public float performanceActionDelay
		{
			get
			{
				return this.m_PerformanceActionDelay;
			}
			set
			{
				this.m_PerformanceActionDelay = value;
			}
		}

		private const string m_FeatureName = "Indexer";

		[SerializeField]
		[Tooltip("Active")]
		private bool m_Active = true;

		[SerializeField]
		[Tooltip("Thermal Action Delay")]
		private float m_ThermalActionDelay = 10f;

		[Tooltip("Performance Action Delay")]
		[SerializeField]
		private float m_PerformanceActionDelay = 4f;
	}
}
