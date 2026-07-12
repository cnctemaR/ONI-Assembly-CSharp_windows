using System;

namespace UnityEngine.ParticleSystemJobs
{
	public static class IJobParticleSystemParallelForBatchExtensions
	{
		public static void EarlyJobInit<T>() where T : struct, IJobParticleSystemParallelForBatch
		{
			ParticleSystemParallelForBatchJobStruct<T>.Initialize();
		}

		internal unsafe static IntPtr GetReflectionData<T>() where T : struct, IJobParticleSystemParallelForBatch
		{
			ParticleSystemParallelForBatchJobStruct<T>.Initialize();
			return *ParticleSystemParallelForBatchJobStruct<T>.jobReflectionData.Data;
		}
	}
}
