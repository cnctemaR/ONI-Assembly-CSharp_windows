using System;

namespace UnityEngine.ParticleSystemJobs
{
	public static class IJobParticleSystemParallelForExtensions
	{
		public static void EarlyJobInit<T>() where T : struct, IJobParticleSystemParallelFor
		{
			ParticleSystemParallelForJobStruct<T>.Initialize();
		}

		internal unsafe static IntPtr GetReflectionData<T>() where T : struct, IJobParticleSystemParallelFor
		{
			ParticleSystemParallelForJobStruct<T>.Initialize();
			return *ParticleSystemParallelForJobStruct<T>.jobReflectionData.Data;
		}
	}
}
