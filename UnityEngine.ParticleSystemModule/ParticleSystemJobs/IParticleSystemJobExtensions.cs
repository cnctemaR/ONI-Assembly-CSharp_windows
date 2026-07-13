using System;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.ParticleSystemJobs
{
	public static class IParticleSystemJobExtensions
	{
		public static JobHandle Schedule<T>(this T jobData, ParticleSystem ps, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParticleSystem
		{
			bool flag = ParticleSystem.UserJobCanBeScheduled();
			if (flag)
			{
				JobsUtility.JobScheduleParameters jobScheduleParameters = ParticleSystemJobUtility.CreateScheduleParams<T>(ref jobData, ps, dependsOn, IJobParticleSystemExtensions.GetReflectionData<T>());
				JobHandle jobHandle = ParticleSystem.ScheduleManagedJob(ref jobScheduleParameters, ps.GetManagedJobData());
				ps.SetManagedJobHandle(jobHandle);
				return jobHandle;
			}
			throw new InvalidOperationException(IParticleSystemJobExtensions.k_UserJobScheduledOutsideOfCallbackErrorMsg);
		}

		public static JobHandle Schedule<T>(this T jobData, ParticleSystem ps, int minIndicesPerJobCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParticleSystemParallelFor
		{
			bool flag = ParticleSystem.UserJobCanBeScheduled();
			if (flag)
			{
				JobsUtility.JobScheduleParameters jobScheduleParameters = ParticleSystemJobUtility.CreateScheduleParams<T>(ref jobData, ps, dependsOn, IJobParticleSystemParallelForExtensions.GetReflectionData<T>());
				JobHandle jobHandle = JobsUtility.ScheduleParallelForDeferArraySize(ref jobScheduleParameters, minIndicesPerJobCount, ps.GetManagedJobData(), null);
				ps.SetManagedJobHandle(jobHandle);
				return jobHandle;
			}
			throw new InvalidOperationException(IParticleSystemJobExtensions.k_UserJobScheduledOutsideOfCallbackErrorMsg);
		}

		public static JobHandle ScheduleBatch<T>(this T jobData, ParticleSystem ps, int innerLoopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParticleSystemParallelForBatch
		{
			bool flag = ParticleSystem.UserJobCanBeScheduled();
			if (flag)
			{
				JobsUtility.JobScheduleParameters jobScheduleParameters = ParticleSystemJobUtility.CreateScheduleParams<T>(ref jobData, ps, dependsOn, IJobParticleSystemParallelForBatchExtensions.GetReflectionData<T>());
				JobHandle jobHandle = JobsUtility.ScheduleParallelForDeferArraySize(ref jobScheduleParameters, innerLoopBatchCount, ps.GetManagedJobData(), null);
				ps.SetManagedJobHandle(jobHandle);
				return jobHandle;
			}
			throw new InvalidOperationException(IParticleSystemJobExtensions.k_UserJobScheduledOutsideOfCallbackErrorMsg);
		}

		private static readonly string k_UserJobScheduledOutsideOfCallbackErrorMsg = "Particle System jobs can only be scheduled in MonoBehaviour.OnParticleUpdateJobScheduled()";
	}
}
