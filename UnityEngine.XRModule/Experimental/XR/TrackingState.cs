using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Current tracking state of the device.</para>
	/// </summary>
	[UsedByNativeCode]
	public enum TrackingState
	{
		/// <summary>
		///   <para>Tracking state is unknown.</para>
		/// </summary>
		Unknown,
		/// <summary>
		///   <para>Tracking is currently working.</para>
		/// </summary>
		Tracking,
		/// <summary>
		///   <para>Tracking is not available.</para>
		/// </summary>
		Unavailable
	}
}
