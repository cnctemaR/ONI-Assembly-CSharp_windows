using System;

namespace UnityEngine.AdaptivePerformance
{
	[Serializable]
	public class AdaptivePerformanceScalerProfile : AdaptivePerformanceScalerSettings
	{
		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		[SerializeField]
		[Tooltip("Name of the scaler profile.")]
		private string m_Name = "Default Scaler Profile";
	}
}
