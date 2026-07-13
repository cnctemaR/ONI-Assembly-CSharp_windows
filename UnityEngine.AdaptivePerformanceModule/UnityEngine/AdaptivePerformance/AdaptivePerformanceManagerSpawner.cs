using System;
using System.Reflection;

namespace UnityEngine.AdaptivePerformance
{
	internal class AdaptivePerformanceManagerSpawner : ScriptableObject
	{
		public GameObject ManagerGameObject
		{
			get
			{
				return this.m_ManagerGameObject;
			}
		}

		private void OnEnable()
		{
			bool flag = this.m_ManagerGameObject != null;
			if (!flag)
			{
				this.m_ManagerGameObject = GameObject.Find("AdaptivePerformanceManager");
			}
		}

		public void Initialize(bool isCheckingProvider)
		{
			bool flag = this.m_ManagerGameObject != null;
			if (!flag)
			{
				this.m_ManagerGameObject = new GameObject("AdaptivePerformanceManager");
				AdaptivePerformanceManager adaptivePerformanceManager = this.m_ManagerGameObject.AddComponent<AdaptivePerformanceManager>();
				if (isCheckingProvider)
				{
					bool flag2 = adaptivePerformanceManager.Indexer == null;
					if (flag2)
					{
						this.Deinitialize();
						return;
					}
				}
				Holder.Instance = adaptivePerformanceManager;
				this.InstallScalers();
				Object.DontDestroyOnLoad(this.m_ManagerGameObject);
				IAdaptivePerformanceSettings settings = adaptivePerformanceManager.Settings;
				bool flag3 = settings == null;
				if (!flag3)
				{
					string[] availableScalerProfiles = settings.GetAvailableScalerProfiles();
					bool flag4 = availableScalerProfiles.Length == 0;
					if (flag4)
					{
						APLog.Debug("No Scaler Profiles available. Did you remove all profiles manually from the provider Settings?", Array.Empty<object>());
					}
					else
					{
						settings.LoadScalerProfile(availableScalerProfiles[settings.defaultScalerProfilerIndex]);
					}
				}
			}
		}

		public void Deinitialize()
		{
			bool flag = this.m_ManagerGameObject == null;
			if (!flag)
			{
				Object.DestroyImmediate(this.m_ManagerGameObject);
				this.m_ManagerGameObject = null;
			}
		}

		private void InstallScalers()
		{
			Type typeFromHandle = typeof(AdaptivePerformanceScaler);
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					bool flag = typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract;
					if (flag)
					{
						ScriptableObject.CreateInstance(type);
					}
				}
			}
		}

		public const string AdaptivePerformanceManagerObjectName = "AdaptivePerformanceManager";

		private GameObject m_ManagerGameObject;
	}
}
