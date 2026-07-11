using System;
using UnityEngine.Bindings;

namespace UnityEngine.Animations
{
	/// <summary>
	///   <para>Represents the axes used in 3D space.</para>
	/// </summary>
	[Flags]
	[NativeType("Runtime/Animation/Constraints/ConstraintEnums.h")]
	public enum Axis
	{
		/// <summary>
		///   <para>Represents the case when no axis is specified.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>Represents the X axis.</para>
		/// </summary>
		X = 1,
		/// <summary>
		///   <para>Represents the Y axis.</para>
		/// </summary>
		Y = 2,
		/// <summary>
		///   <para>Represents the Z axis.</para>
		/// </summary>
		Z = 4
	}
}
