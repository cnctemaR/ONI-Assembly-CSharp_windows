using System;
using UnityEngine.Scripting;

namespace UnityEngine.AdaptivePerformance
{
	internal static class AdaptivePerformanceInitializer
	{
		[RequiredByNativeCode(false)]
		public static void AutoInitializeAdaptivePerformanceManaged()
		{
			AdaptivePerformanceInitializer.InitializeSpawner(true);
		}

		public static void Initialize()
		{
			AdaptivePerformanceInitializer.InitializeSpawner(false);
		}

		public static void Deinitialize()
		{
			bool flag = AdaptivePerformanceInitializer.s_Spawner == null;
			if (!flag)
			{
				AdaptivePerformanceInitializer.s_Spawner.Deinitialize();
				Object.Destroy(AdaptivePerformanceInitializer.s_Spawner);
				AdaptivePerformanceInitializer.s_Spawner = null;
			}
		}

		private static void InitializeSpawner(bool isAuto)
		{
			bool flag = AdaptivePerformanceInitializer.s_Spawner == null;
			if (flag)
			{
				AdaptivePerformanceInitializer.s_Spawner = ScriptableObject.CreateInstance<AdaptivePerformanceManagerSpawner>();
			}
			bool flag2 = AdaptivePerformanceInitializer.s_Spawner != null && AdaptivePerformanceInitializer.s_Spawner.ManagerGameObject != null;
			if (!flag2)
			{
				AdaptivePerformanceInitializer.s_Spawner.Initialize(isAuto);
			}
		}

		private static AdaptivePerformanceManagerSpawner s_Spawner;
	}
}
