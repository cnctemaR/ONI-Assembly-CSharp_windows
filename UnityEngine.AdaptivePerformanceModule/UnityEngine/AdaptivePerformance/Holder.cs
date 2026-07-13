using System;
using System.Diagnostics;

namespace UnityEngine.AdaptivePerformance
{
	public static class Holder
	{
		public static IAdaptivePerformance Instance
		{
			get
			{
				return Holder.m_Instance;
			}
			internal set
			{
				bool flag = value == null;
				if (flag)
				{
					LifecycleEventHandler lifecycleEventHandler = Holder.LifecycleEventHandler;
					if (lifecycleEventHandler != null)
					{
						lifecycleEventHandler(Holder.m_Instance, LifecycleChangeType.Destroyed);
					}
				}
				else
				{
					LifecycleEventHandler lifecycleEventHandler2 = Holder.LifecycleEventHandler;
					if (lifecycleEventHandler2 != null)
					{
						lifecycleEventHandler2(value, LifecycleChangeType.Created);
					}
				}
				Holder.m_Instance = value;
			}
		}

		public static void Initialize()
		{
			bool flag = Holder.Instance != null;
			if (!flag)
			{
				AdaptivePerformanceInitializer.Initialize();
				bool flag2 = Holder.Instance != null;
				if (flag2)
				{
					Holder.Instance.InitializeAdaptivePerformance();
				}
			}
		}

		public static void Deinitialize()
		{
			bool flag = Holder.Instance != null;
			if (flag)
			{
				Holder.Instance.DeinitializeAdaptivePerformance();
			}
			AdaptivePerformanceInitializer.Deinitialize();
			Holder.Instance = null;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event LifecycleEventHandler LifecycleEventHandler;

		private static IAdaptivePerformance m_Instance;
	}
}
