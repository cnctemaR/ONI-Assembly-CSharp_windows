using System;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Jobs
{
	[JobProducerType(typeof(IJobParallelForExtensions.ParallelForJobStruct<>))]
	public interface IJobParallelFor
	{
		/// <summary>
		///   <para>Implement this method to perform work against a specific iteration index.</para>
		/// </summary>
		/// <param name="index">The index of the Parallel for loop at which to perform work.</param>
		void Execute(int index);
	}
}
