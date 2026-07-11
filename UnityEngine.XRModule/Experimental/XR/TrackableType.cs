using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>A trackable is feature in the physical environment that a device is able to track, such as a plane.</para>
	/// </summary>
	[UsedByNativeCode]
	[Flags]
	public enum TrackableType
	{
		/// <summary>
		///   <para>No trackable.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>The boundary of a BoundedPlane</para>
		/// </summary>
		PlaneWithinPolygon = 1,
		/// <summary>
		///   <para>Within the BoundedPlane.Size of a BoundedPlane</para>
		/// </summary>
		PlaneWithinBounds = 2,
		/// <summary>
		///   <para>The infinite plane of a BoundedPlane</para>
		/// </summary>
		PlaneWithinInfinity = 4,
		/// <summary>
		///   <para>An estimated plane.</para>
		/// </summary>
		PlaneEstimated = 8,
		/// <summary>
		///   <para>Any of the plane types.</para>
		/// </summary>
		Planes = 15,
		/// <summary>
		///   <para>A feature point.</para>
		/// </summary>
		FeaturePoint = 16,
		/// <summary>
		///   <para>All trackables (planes and point cloud)</para>
		/// </summary>
		All = 31
	}
}
