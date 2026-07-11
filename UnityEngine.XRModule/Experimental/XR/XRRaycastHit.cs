using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Structure describing the result of a XRRaycastSubsystem.Raycast hit.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Modules/XR/Subsystems/Raycast/XRRaycastSubsystem.h")]
	public struct XRRaycastHit
	{
		/// <summary>
		///   <para>The TrackableId of the trackable that was hit by the raycast.</para>
		/// </summary>
		public TrackableId TrackableId { get; set; }

		/// <summary>
		///   <para>The position and rotation of the hit result in device space where the ray hit the trackable.</para>
		/// </summary>
		public Pose Pose { get; set; }

		/// <summary>
		///   <para>The distance, in meters, from the screen to the hit's XRRaycastSubsystemHit.Position.</para>
		/// </summary>
		public float Distance { get; set; }

		/// <summary>
		///   <para>The TrackableType(s) that were hit.</para>
		/// </summary>
		public TrackableType HitType { get; set; }
	}
}
