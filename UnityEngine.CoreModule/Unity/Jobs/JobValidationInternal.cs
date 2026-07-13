using System;
using System.Diagnostics;
using Unity.Burst;
using UnityEngine.Bindings;

namespace Unity.Jobs
{
	[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
	internal static class JobValidationInternal
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal static void CheckReflectionDataCorrect<T>(IntPtr reflectionData)
		{
		}

		[BurstDiscard]
		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckReflectionDataCorrectInternal<T>(IntPtr reflectionData, ref bool burstCompiled)
		{
			bool flag = reflectionData == IntPtr.Zero;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Reflection data was not set up by an Initialize() call. Support for burst compiled calls to Schedule depends on the Collections package.\n\nFor generic job types, please include [assembly: RegisterGenericJobType(typeof({0}))] in your source file.", typeof(T)));
			}
			burstCompiled = false;
		}
	}
}
