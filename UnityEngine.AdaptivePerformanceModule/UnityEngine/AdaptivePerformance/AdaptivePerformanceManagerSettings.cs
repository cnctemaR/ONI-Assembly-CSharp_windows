using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.AdaptivePerformance
{
	public sealed class AdaptivePerformanceManagerSettings : ScriptableObject
	{
		public bool automaticLoading
		{
			get
			{
				return this.m_AutomaticLoading;
			}
			set
			{
				this.m_AutomaticLoading = value;
			}
		}

		public bool automaticRunning
		{
			get
			{
				return this.m_AutomaticRunning;
			}
			set
			{
				this.m_AutomaticRunning = value;
			}
		}

		public List<AdaptivePerformanceLoader> loaders
		{
			get
			{
				return this.m_Loaders;
			}
			set
			{
				this.m_Loaders = value;
			}
		}

		public bool isInitializationComplete
		{
			get
			{
				return this.m_InitializationComplete;
			}
		}

		[HideInInspector]
		public AdaptivePerformanceLoader activeLoader
		{
			get
			{
				return AdaptivePerformanceManagerSettings.s_ActiveLoader;
			}
			private set
			{
				AdaptivePerformanceManagerSettings.s_ActiveLoader = value;
			}
		}

		public T ActiveLoaderAs<T>() where T : AdaptivePerformanceLoader
		{
			return this.activeLoader as T;
		}

		internal void InitializeLoaderSync()
		{
			bool flag = this.isInitializationComplete && this.activeLoader != null;
			if (flag)
			{
				Debug.LogWarning("Adaptive Performance Management has already initialized an active loader in this scene.Please make sure to stop all subsystems and deinitialize the active loader before initializing a new one.");
			}
			else
			{
				foreach (AdaptivePerformanceLoader adaptivePerformanceLoader in this.loaders)
				{
					bool flag2 = adaptivePerformanceLoader != null;
					if (flag2)
					{
						bool flag3 = adaptivePerformanceLoader.Initialize();
						if (flag3)
						{
							this.activeLoader = adaptivePerformanceLoader;
							this.m_InitializationComplete = true;
							return;
						}
					}
				}
				this.activeLoader = null;
			}
		}

		internal IEnumerator InitializeLoader()
		{
			bool flag = this.isInitializationComplete && this.activeLoader != null;
			if (flag)
			{
				Debug.LogWarning("Adaptive Performance Management has already initialized an active loader in this scene.Please make sure to stop all subsystems and deinitialize the active loader before initializing a new one.");
				yield break;
			}
			foreach (AdaptivePerformanceLoader loader in this.loaders)
			{
				bool flag2 = loader != null;
				if (flag2)
				{
					bool flag3 = loader.Initialize();
					if (flag3)
					{
						this.activeLoader = loader;
						this.m_InitializationComplete = true;
						yield break;
					}
				}
				yield return null;
				loader = null;
			}
			List<AdaptivePerformanceLoader>.Enumerator enumerator = default(List<AdaptivePerformanceLoader>.Enumerator);
			this.activeLoader = null;
			yield break;
			yield break;
		}

		internal void StartSubsystems()
		{
			bool flag = !this.m_InitializationComplete;
			if (flag)
			{
				Debug.LogWarning("Call to StartSubsystems without an initialized manager.Please make sure to wait for initialization to complete before calling this API.");
			}
			else
			{
				bool flag2 = this.activeLoader == null;
				if (!flag2)
				{
					this.activeLoader.Start();
				}
			}
		}

		internal void StopSubsystems()
		{
			bool flag = !this.m_InitializationComplete;
			if (flag)
			{
				Debug.LogWarning("Call to StopSubsystems without an initialized manager.Please make sure to wait for initialization to complete before calling this API.");
			}
			else
			{
				bool flag2 = this.activeLoader == null;
				if (!flag2)
				{
					this.activeLoader.Stop();
				}
			}
		}

		internal void DeinitializeLoader()
		{
			bool flag = !this.m_InitializationComplete;
			if (flag)
			{
				Debug.LogWarning("Call to DeinitializeLoader without an initialized manager.Please make sure to wait for initialization to complete before calling this API.");
			}
			else
			{
				this.StopSubsystems();
				bool flag2 = this.activeLoader != null;
				if (flag2)
				{
					this.activeLoader.Deinitialize();
					this.activeLoader = null;
				}
				this.m_InitializationComplete = false;
			}
		}

		private void OnDisable()
		{
			bool flag = this.automaticLoading && this.automaticRunning;
			if (flag)
			{
				this.StopSubsystems();
			}
		}

		private void OnDestroy()
		{
			bool automaticLoading = this.automaticLoading;
			if (automaticLoading)
			{
				this.DeinitializeLoader();
			}
		}

		[HideInInspector]
		private bool m_InitializationComplete = false;

		[SerializeField]
		[Tooltip("Determines if the Adaptive Performance Manager instance is responsible for creating and destroying the appropriate loader instance.")]
		private bool m_AutomaticLoading = false;

		[Tooltip("Determines if the Adaptive Performance Manager instance is responsible for starting and stopping subsystems for the active loader instance.")]
		[SerializeField]
		private bool m_AutomaticRunning = false;

		[SerializeField]
		[Tooltip("List of Adaptive Performance Loader instances arranged in desired load order.")]
		private List<AdaptivePerformanceLoader> m_Loaders = new List<AdaptivePerformanceLoader>();

		[HideInInspector]
		private static AdaptivePerformanceLoader s_ActiveLoader;
	}
}
