using System;

namespace UnityEngine.ParticleSystemJobs
{
	public static class IJobParticleSystemExtensions
	{
		public static void EarlyJobInit<T>() where T : struct, IJobParticleSystem
		{
			ParticleSystemJobStruct<T>.Initialize();
		}

		internal unsafe static IntPtr GetReflectionData<T>() where T : struct, IJobParticleSystem
		{
			ParticleSystemJobStruct<T>.Initialize();
			return *ParticleSystemJobStruct<T>.jobReflectionData.Data;
		}
	}
}
