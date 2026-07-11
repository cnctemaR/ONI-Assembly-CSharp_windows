using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The ConfigurableJoint attempts to attain position / velocity targets based on this flag.</para>
	/// </summary>
	[Obsolete("JointDriveMode is no longer supported")]
	[Flags]
	public enum JointDriveMode
	{
		/// <summary>
		///   <para>Don't apply any forces to reach the target.</para>
		/// </summary>
		[Obsolete("JointDriveMode.None is no longer supported")]
		None = 0,
		/// <summary>
		///   <para>Try to reach the specified target position.</para>
		/// </summary>
		[Obsolete("JointDriveMode.Position is no longer supported")]
		Position = 1,
		/// <summary>
		///   <para>Try to reach the specified target velocity.</para>
		/// </summary>
		[Obsolete("JointDriveMode.Velocity is no longer supported")]
		Velocity = 2,
		/// <summary>
		///   <para>Try to reach the specified target position and velocity.</para>
		/// </summary>
		[Obsolete("JointDriveMode.PositionAndvelocity is no longer supported")]
		PositionAndVelocity = 3
	}
}
