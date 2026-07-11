using System;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Jobs
{
	[JobProducerType(typeof(IJobExtensions.JobStruct<>))]
	public interface IJob
	{
		/// <summary>
		///   <para>Implement this method to perform work on a worker thread.</para>
		/// </summary>
		void Execute();
	}
}
