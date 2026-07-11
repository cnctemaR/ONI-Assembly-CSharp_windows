using System;

namespace Unity.Jobs.LowLevel.Unsafe
{
	/// <summary>
	///   <para>ScheduleMode options for scheduling a manage job.</para>
	/// </summary>
	public enum ScheduleMode
	{
		/// <summary>
		///   <para>Schedule job as independent.</para>
		/// </summary>
		Run,
		/// <summary>
		///   <para>Schedule job as batched.</para>
		/// </summary>
		Batched
	}
}
