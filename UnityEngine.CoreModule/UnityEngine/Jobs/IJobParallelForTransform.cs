using System;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Jobs
{
	[JobProducerType(typeof(IJobParallelForTransformExtensions.TransformParallelForLoopStruct<>))]
	public interface IJobParallelForTransform
	{
		/// <summary>
		///   <para>Execute.</para>
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="transform">TransformAccessArray.</param>
		void Execute(int index, TransformAccess transform);
	}
}
