using System;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Represents the size of physical space available for XR.</para>
	/// </summary>
	public enum TrackingSpaceType
	{
		/// <summary>
		///   <para>Represents a small space where movement may be constrained or positional tracking is unavailable.</para>
		/// </summary>
		Stationary,
		/// <summary>
		///   <para>Represents a space large enough for free movement.</para>
		/// </summary>
		RoomScale
	}
}
