using System;

namespace UnityEngine.AdaptivePerformance
{
	public abstract class AdaptivePerformanceLoader : ScriptableObject
	{
		public abstract bool Initialized { get; }

		public abstract bool Running { get; }

		public virtual bool Initialize()
		{
			return true;
		}

		public virtual bool Start()
		{
			return true;
		}

		public virtual bool Stop()
		{
			return true;
		}

		public virtual bool Deinitialize()
		{
			return true;
		}

		public abstract T GetLoadedSubsystem<T>() where T : class, ISubsystem;

		public abstract ISubsystem GetDefaultSubsystem();

		public abstract IAdaptivePerformanceSettings GetSettings();
	}
}
