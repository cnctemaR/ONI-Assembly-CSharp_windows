using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>How much CPU usage to assign to the final lighting calculations at runtime.</para>
	/// </summary>
	public enum RealtimeGICPUUsage
	{
		/// <summary>
		///   <para>25% of the allowed CPU threads are used as worker threads.</para>
		/// </summary>
		Low = 25,
		/// <summary>
		///   <para>50% of the allowed CPU threads are used as worker threads.</para>
		/// </summary>
		Medium = 50,
		/// <summary>
		///   <para>75% of the allowed CPU threads are used as worker threads.</para>
		/// </summary>
		High = 75,
		/// <summary>
		///   <para>100% of the allowed CPU threads are used as worker threads.</para>
		/// </summary>
		Unlimited = 100
	}
}
