using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptivePerformanceGeneralSettings : ScriptableObject
	{
		public AdaptivePerformanceManagerSettings Manager
		{
			get
			{
				return this.m_LoaderManagerInstance;
			}
			set
			{
				this.m_LoaderManagerInstance = value;
			}
		}

		public bool IsProviderInitialized
		{
			get
			{
				return this.m_ProviderIntialized;
			}
		}

		public bool IsProviderStarted
		{
			get
			{
				return this.m_ProviderStarted;
			}
		}

		public static AdaptivePerformanceGeneralSettings Instance
		{
			get
			{
				return AdaptivePerformanceGeneralSettings.s_RuntimeSettingsInstance;
			}
			set
			{
				AdaptivePerformanceGeneralSettings.s_RuntimeSettingsInstance = value;
			}
		}

		public AdaptivePerformanceManagerSettings AssignedSettings
		{
			get
			{
				return this.m_LoaderManagerInstance;
			}
			set
			{
				this.m_LoaderManagerInstance = value;
			}
		}

		public bool InitManagerOnStart
		{
			get
			{
				return this.m_InitManagerOnStart;
			}
			set
			{
				this.m_InitManagerOnStart = value;
			}
		}

		private void Awake()
		{
			AdaptivePerformanceGeneralSettings.s_RuntimeSettingsInstance = this;
			Application.quitting += AdaptivePerformanceGeneralSettings.Quit;
			Object.DontDestroyOnLoad(AdaptivePerformanceGeneralSettings.s_RuntimeSettingsInstance);
		}

		private static void Quit()
		{
			AdaptivePerformanceGeneralSettings instance = AdaptivePerformanceGeneralSettings.Instance;
			bool flag = instance == null;
			if (!flag)
			{
				instance.DeInitAdaptivePerformance();
			}
		}

		private void OnDestroy()
		{
			this.DeInitAdaptivePerformance();
			AdaptivePerformanceGeneralSettings.s_RuntimeSettingsInstance = null;
		}

		[RequiredByNativeCode(true)]
		internal static void AttemptInitializeAdaptivePerformanceGeneralSettingsOnLoad()
		{
			AdaptivePerformanceGeneralSettings instance = AdaptivePerformanceGeneralSettings.Instance;
			bool flag = instance == null || !instance.InitManagerOnStart;
			if (!flag)
			{
				instance.InitAdaptivePerformance();
			}
		}

		[RequiredByNativeCode(true)]
		internal static void AttemptStartAdaptivePerformanceGeneralSettingsOnBeforeSplashScreen()
		{
			AdaptivePerformanceGeneralSettings instance = AdaptivePerformanceGeneralSettings.Instance;
			bool flag = instance == null || !instance.InitManagerOnStart;
			if (!flag)
			{
				instance.StartAdaptivePerformance();
			}
		}

		internal void InitAdaptivePerformance()
		{
			bool providerIntialized = this.m_ProviderIntialized;
			if (!providerIntialized)
			{
				bool flag = AdaptivePerformanceGeneralSettings.Instance == null;
				if (!flag)
				{
					this.m_AdaptivePerformanceManager = AdaptivePerformanceGeneralSettings.Instance.m_LoaderManagerInstance;
					bool flag2 = this.m_AdaptivePerformanceManager == null;
					if (flag2)
					{
						Debug.LogError("Assigned GameObject for Adaptive Performance Management loading is invalid. No Adaptive Performance Providers will be automatically loaded.");
					}
					else
					{
						this.m_AdaptivePerformanceManager.automaticLoading = false;
						this.m_AdaptivePerformanceManager.automaticRunning = false;
						this.m_AdaptivePerformanceManager.InitializeLoaderSync();
						bool flag3 = this.m_AdaptivePerformanceManager.activeLoader == null;
						if (!flag3)
						{
							this.m_ProviderIntialized = true;
						}
					}
				}
			}
		}

		internal void StartAdaptivePerformance()
		{
			bool flag = !this.m_ProviderIntialized || this.m_ProviderStarted;
			if (!flag)
			{
				bool flag2 = this.m_AdaptivePerformanceManager == null || this.m_AdaptivePerformanceManager.activeLoader == null;
				if (!flag2)
				{
					this.m_AdaptivePerformanceManager.StartSubsystems();
					this.m_ProviderStarted = true;
				}
			}
		}

		internal void StopAdaptivePerformance()
		{
			bool flag = !this.m_ProviderIntialized || !this.m_ProviderStarted;
			if (!flag)
			{
				bool flag2 = this.m_AdaptivePerformanceManager == null || this.m_AdaptivePerformanceManager.activeLoader == null;
				if (!flag2)
				{
					this.m_AdaptivePerformanceManager.StopSubsystems();
					this.m_ProviderStarted = false;
				}
			}
		}

		internal void DeInitAdaptivePerformance()
		{
			bool flag = !this.m_ProviderIntialized;
			if (!flag)
			{
				bool providerStarted = this.m_ProviderStarted;
				if (providerStarted)
				{
					this.StopAdaptivePerformance();
				}
				bool flag2 = this.m_AdaptivePerformanceManager != null;
				if (flag2)
				{
					this.m_AdaptivePerformanceManager.DeinitializeLoader();
					this.m_AdaptivePerformanceManager = null;
				}
				this.m_ProviderIntialized = false;
			}
		}

		public static string k_SettingsKey = "com.unity.adaptiveperformance.loader_settings";

		internal static AdaptivePerformanceGeneralSettings s_RuntimeSettingsInstance = null;

		[SerializeField]
		internal AdaptivePerformanceManagerSettings m_LoaderManagerInstance = null;

		[SerializeField]
		[Tooltip("Enable this to automatically start up Adaptive Performance at runtime.")]
		internal bool m_InitManagerOnStart = true;

		[SerializeField]
		[VisibleToOtherModules(new string[] { "UnityEditor.AdaptivePerformanceModule" })]
		internal string m_LastSelectedProvider = "";

		private AdaptivePerformanceManagerSettings m_AdaptivePerformanceManager = null;

		private bool m_ProviderIntialized = false;

		private bool m_ProviderStarted = false;
	}
}
