using System;

namespace Unity.Jobs.LowLevel.Unsafe
{
	/// <summary>
	///   <para>Struct containing information about a range the job is allowed to work on.</para>
	/// </summary>
	public struct JobRanges
	{
		/// <summary>
		///   <para>The size of the batch.</para>
		/// </summary>
		public int BatchSize;

		/// <summary>
		///   <para>Number of jobs.</para>
		/// </summary>
		public int NumJobs;

		/// <summary>
		///   <para>Total iteration count.</para>
		/// </summary>
		public int TotalIterationCount;

		/// <summary>
		///   <para>Number of phases.</para>
		/// </summary>
		public int NumPhases;

		/// <summary>
		///   <para>Number of indices per phase.</para>
		/// </summary>
		public int IndicesPerPhase;

		/// <summary>
		///   <para>The start and end index of the job range.</para>
		/// </summary>
		public IntPtr StartEndIndex;

		/// <summary>
		///   <para>Phase Data.</para>
		/// </summary>
		public IntPtr PhaseData;
	}
}
