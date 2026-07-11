using System;

namespace Unity.Jobs.LowLevel.Unsafe
{
	/// <summary>
	///   <para>Determines what the job is used for (ParallelFor or a single job).</para>
	/// </summary>
	public enum JobType
	{
		/// <summary>
		///   <para>A single job.</para>
		/// </summary>
		Single,
		/// <summary>
		///   <para>A parallel for job.</para>
		/// </summary>
		ParallelFor
	}
}
