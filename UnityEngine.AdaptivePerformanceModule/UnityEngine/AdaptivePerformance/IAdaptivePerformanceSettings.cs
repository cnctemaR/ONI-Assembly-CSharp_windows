using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.AdaptivePerformance
{
	public class IAdaptivePerformanceSettings : ScriptableObject
	{
		public bool logging
		{
			get
			{
				return this.m_Logging;
			}
			set
			{
				this.m_Logging = value;
			}
		}

		public bool automaticPerformanceMode
		{
			get
			{
				return this.m_AutomaticPerformanceModeEnabled;
			}
			set
			{
				this.m_AutomaticPerformanceModeEnabled = value;
			}
		}

		public bool automaticGameMode
		{
			get
			{
				return this.m_AutomaticGameModeEnabled;
			}
			set
			{
				this.m_AutomaticGameModeEnabled = value;
			}
		}

		public bool enableBoostOnStartup
		{
			get
			{
				return this.m_EnableBoostOnStartup;
			}
			set
			{
				this.m_EnableBoostOnStartup = value;
			}
		}

		public int statsLoggingFrequencyInFrames
		{
			get
			{
				return this.m_StatsLoggingFrequencyInFrames;
			}
			set
			{
				this.m_StatsLoggingFrequencyInFrames = value;
			}
		}

		public AdaptivePerformanceIndexerSettings indexerSettings
		{
			get
			{
				return this.m_IndexerSettings;
			}
			set
			{
				this.m_IndexerSettings = value;
			}
		}

		public AdaptivePerformanceScalerSettings scalerSettings
		{
			get
			{
				return this.m_ScalerSettings;
			}
			set
			{
				this.m_ScalerSettings = value;
			}
		}

		public void LoadScalerProfile(string scalerProfileName)
		{
			bool flag = scalerProfileName == null || scalerProfileName.Length <= 0;
			if (flag)
			{
				APLog.Debug("Scaler profile name empty. Can not load and apply profile.", Array.Empty<object>());
			}
			else
			{
				bool flag2 = this.m_scalerProfileList.Length == 0;
				if (flag2)
				{
					APLog.Debug("No scaler profiles available. Can not load and apply profile. Add more profiles in the Adaptive Performance settings.", Array.Empty<object>());
				}
				else
				{
					bool flag3 = this.m_scalerProfileList.Length == 1;
					if (flag3)
					{
						APLog.Debug("Only default scaler profile available. Reset all scalers to default profile.", Array.Empty<object>());
					}
					for (int i = 0; i < this.m_scalerProfileList.Length; i++)
					{
						AdaptivePerformanceScalerProfile adaptivePerformanceScalerProfile = this.m_scalerProfileList[i];
						bool flag4 = adaptivePerformanceScalerProfile == null;
						if (flag4)
						{
							APLog.Debug("Scaler profile is null. Can not load and apply profile. Check Adaptive Performance settings.", Array.Empty<object>());
							return;
						}
						bool flag5 = adaptivePerformanceScalerProfile.Name == null || adaptivePerformanceScalerProfile.Name.Length <= 0;
						if (flag5)
						{
							APLog.Debug("Scaler profile name is null or empty. Can not load and apply profile. Check Adaptive Performance settings.", Array.Empty<object>());
							return;
						}
						bool flag6 = adaptivePerformanceScalerProfile.Name == scalerProfileName;
						if (flag6)
						{
							this.scalerSettings.ApplySettings(adaptivePerformanceScalerProfile);
							break;
						}
					}
					bool flag7 = this.ApplyScalerProfileToAllScalers();
					if (flag7)
					{
						APLog.Debug("Scaler profile " + scalerProfileName + " loaded.", Array.Empty<object>());
					}
				}
			}
		}

		private bool ApplyScalerProfileToAllScalers()
		{
			bool flag = false;
			bool flag2 = Holder.Instance == null || Holder.Instance.Indexer == null;
			bool flag3;
			if (flag2)
			{
				flag3 = flag;
			}
			else
			{
				List<AdaptivePerformanceScaler> list = new List<AdaptivePerformanceScaler>();
				List<AdaptivePerformanceScaler> list2 = new List<AdaptivePerformanceScaler>();
				Holder.Instance.Indexer.GetUnappliedScalers(ref list2);
				list.AddRange(list2);
				Holder.Instance.Indexer.GetAppliedScalers(ref list2);
				list.AddRange(list2);
				Holder.Instance.Indexer.GetDisabledScalers(ref list2);
				list.AddRange(list2);
				bool flag4 = list.Count <= 0;
				if (flag4)
				{
					APLog.Debug("No scalers found. No scaler profile applied.", Array.Empty<object>());
					flag3 = flag;
				}
				else
				{
					PropertyInfo[] properties = typeof(AdaptivePerformanceScalerSettings).GetProperties();
					PropertyInfo[] array = properties;
					for (int i = 0; i < array.Length; i++)
					{
						PropertyInfo property = array[i];
						AdaptivePerformanceScaler adaptivePerformanceScaler = list.Find((AdaptivePerformanceScaler s) => s.GetType().ToString().Contains(property.Name));
						bool flag5 = adaptivePerformanceScaler;
						if (flag5)
						{
							PropertyInfo property2 = typeof(AdaptivePerformanceScalerSettings).GetProperty(property.Name);
							object value = property2.GetValue(this.scalerSettings);
							adaptivePerformanceScaler.Deactivate();
							adaptivePerformanceScaler.ApplyDefaultSetting((AdaptivePerformanceScalerSettingsBase)value);
							adaptivePerformanceScaler.Activate();
							flag = true;
						}
					}
					flag3 = flag;
				}
			}
			return flag3;
		}

		public string[] GetAvailableScalerProfiles()
		{
			string[] array = new string[this.m_scalerProfileList.Length];
			bool flag = this.m_scalerProfileList.Length == 0;
			string[] array2;
			if (flag)
			{
				APLog.Debug("No scaler profiles available. You can not load and apply profiles. Add more profiles in the Adaptive Performance settings.", Array.Empty<object>());
				array2 = array;
			}
			else
			{
				for (int i = 0; i < this.m_scalerProfileList.Length; i++)
				{
					AdaptivePerformanceScalerProfile adaptivePerformanceScalerProfile = this.m_scalerProfileList[i];
					array[i] = adaptivePerformanceScalerProfile.Name;
				}
				array2 = array;
			}
			return array2;
		}

		public int defaultScalerProfilerIndex
		{
			get
			{
				return this.m_DefaultScalerProfilerIndex;
			}
			set
			{
				this.m_DefaultScalerProfilerIndex = value;
			}
		}

		public void OnEnable()
		{
			bool flag = this.k_AssetVersion < 3;
			if (flag)
			{
				this.k_AssetVersion = 2;
			}
		}

		[SerializeField]
		[Tooltip("Enable Logging in Devmode")]
		private bool m_Logging = true;

		[SerializeField]
		[Tooltip("Automatic Performance Mode")]
		private bool m_AutomaticPerformanceModeEnabled = true;

		[SerializeField]
		[Tooltip("Automatic Game Mode")]
		private bool m_AutomaticGameModeEnabled = false;

		[SerializeField]
		[Tooltip("Enables the CPU and GPU boost mode before engine startup to decrease startup time.")]
		private bool m_EnableBoostOnStartup = true;

		[SerializeField]
		[Tooltip("Logging Frequency (Development mode only)")]
		private int m_StatsLoggingFrequencyInFrames = 50;

		[Tooltip("Indexer Settings")]
		[SerializeField]
		private AdaptivePerformanceIndexerSettings m_IndexerSettings;

		[SerializeField]
		[Tooltip("Scaler Settings")]
		private AdaptivePerformanceScalerSettings m_ScalerSettings;

		[SerializeField]
		private AdaptivePerformanceScalerProfile[] m_scalerProfileList = new AdaptivePerformanceScalerProfile[]
		{
			new AdaptivePerformanceScalerProfile()
		};

		[SerializeField]
		internal int m_DefaultScalerProfilerIndex = 0;

		[SerializeField]
		private int k_AssetVersion = 3;
	}
}
