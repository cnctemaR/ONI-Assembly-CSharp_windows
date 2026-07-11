using System;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Enumeration of tracked XR nodes which can be updated by XR input.</para>
	/// </summary>
	public enum XRNode
	{
		/// <summary>
		///   <para>Node representing the left eye.</para>
		/// </summary>
		LeftEye,
		/// <summary>
		///   <para>Node representing the right eye.</para>
		/// </summary>
		RightEye,
		/// <summary>
		///   <para>Node representing a point between the left and right eyes.</para>
		/// </summary>
		CenterEye,
		/// <summary>
		///   <para>Node representing the user's head.</para>
		/// </summary>
		Head,
		/// <summary>
		///   <para>Node representing the left hand.</para>
		/// </summary>
		LeftHand,
		/// <summary>
		///   <para>Node representing the right hand.</para>
		/// </summary>
		RightHand,
		/// <summary>
		///   <para>Represents a tracked game Controller not associated with a specific hand.</para>
		/// </summary>
		GameController,
		/// <summary>
		///   <para>Represents a stationary physical device that can be used as a point of reference in the tracked area.</para>
		/// </summary>
		TrackingReference,
		/// <summary>
		///   <para>Represents a physical device that provides tracking data for objects to which it is attached.</para>
		/// </summary>
		HardwareTracker
	}
}
