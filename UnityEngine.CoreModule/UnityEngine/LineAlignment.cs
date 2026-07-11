using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Control the direction lines face, when using the LineRenderer or TrailRenderer.</para>
	/// </summary>
	public enum LineAlignment
	{
		/// <summary>
		///   <para>Lines face the camera.</para>
		/// </summary>
		View,
		/// <summary>
		///   <para>Lines face the direction of the Transform Component.</para>
		/// </summary>
		[Obsolete("Enum member Local has been deprecated. Use TransformZ instead (UnityUpgradable) -> TransformZ", false)]
		Local,
		/// <summary>
		///   <para>Lines face the Z axis of the Transform Component.</para>
		/// </summary>
		TransformZ = 1
	}
}
