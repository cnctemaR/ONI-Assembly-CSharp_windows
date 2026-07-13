using System;
using System.Collections.Generic;

namespace UnityEngine.AdaptivePerformance
{
	public abstract class AdaptivePerformanceLoaderHelper : AdaptivePerformanceLoader
	{
		public override T GetLoadedSubsystem<T>()
		{
			Type typeFromHandle = typeof(T);
			ISubsystem subsystem;
			this.m_SubsystemInstanceMap.TryGetValue(typeFromHandle, out subsystem);
			return subsystem as T;
		}

		protected void StartSubsystem<T>() where T : class, ISubsystem
		{
			T loadedSubsystem = this.GetLoadedSubsystem<T>();
			bool flag = loadedSubsystem != null;
			if (flag)
			{
				loadedSubsystem.Start();
			}
		}

		protected void StopSubsystem<T>() where T : class, ISubsystem
		{
			T loadedSubsystem = this.GetLoadedSubsystem<T>();
			bool flag = loadedSubsystem != null;
			if (flag)
			{
				loadedSubsystem.Stop();
			}
		}

		protected void DestroySubsystem<T>() where T : class, ISubsystem
		{
			T loadedSubsystem = this.GetLoadedSubsystem<T>();
			bool flag = loadedSubsystem != null;
			if (flag)
			{
				bool running = loadedSubsystem.running;
				if (running)
				{
					loadedSubsystem.Stop();
				}
				Type typeFromHandle = typeof(T);
				bool flag2 = this.m_SubsystemInstanceMap.ContainsKey(typeFromHandle);
				if (flag2)
				{
					this.m_SubsystemInstanceMap.Remove(typeFromHandle);
				}
				loadedSubsystem.Destroy();
			}
		}

		protected void CreateSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id) where TDescriptor : ISubsystemDescriptor where TSubsystem : ISubsystem
		{
			bool flag = descriptors == null;
			if (flag)
			{
				throw new ArgumentNullException("descriptors");
			}
			SubsystemManager.GetSubsystemDescriptors<TDescriptor>(descriptors);
			bool flag2 = descriptors.Count > 0;
			if (flag2)
			{
				foreach (TDescriptor tdescriptor in descriptors)
				{
					ISubsystem subsystem = null;
					bool flag3 = string.Compare(tdescriptor.id, id, true) == 0;
					if (flag3)
					{
						subsystem = tdescriptor.Create();
					}
					bool flag4 = subsystem != null;
					if (flag4)
					{
						this.m_SubsystemInstanceMap[typeof(TSubsystem)] = subsystem;
						break;
					}
				}
			}
		}

		public override bool Deinitialize()
		{
			this.m_SubsystemInstanceMap.Clear();
			return base.Deinitialize();
		}

		protected Dictionary<Type, ISubsystem> m_SubsystemInstanceMap = new Dictionary<Type, ISubsystem>();
	}
}
