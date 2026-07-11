using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Flags for VisibleLight.</para>
	/// </summary>
	[Flags]
	public enum VisibleLightFlags
	{
		/// <summary>
		///   <para>No flags are set.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>Light intersects near clipping plane.</para>
		/// </summary>
		IntersectsNearPlane = 1,
		/// <summary>
		///   <para>Light intersects far clipping plane.</para>
		/// </summary>
		IntersectsFarPlane = 2
	}
}
