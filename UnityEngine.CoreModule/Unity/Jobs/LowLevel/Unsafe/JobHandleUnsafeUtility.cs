using System;

namespace Unity.Jobs.LowLevel.Unsafe
{
	/// <summary>
	///   <para>JobHandle Unsafe Utilities.</para>
	/// </summary>
	public static class JobHandleUnsafeUtility
	{
		/// <summary>
		///   <para>Combines multiple dependencies into a single one using an unsafe array of job handles.
		/// See Also: JobHandle.CombineDependencies.</para>
		/// </summary>
		/// <param name="jobs"></param>
		/// <param name="count"></param>
		public unsafe static JobHandle CombineDependencies(JobHandle* jobs, int count)
		{
			return JobHandle.CombineDependenciesInternalPtr((void*)jobs, count);
		}
	}
}
