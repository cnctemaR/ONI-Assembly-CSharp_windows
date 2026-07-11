using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Describes current plane alignment in mixed reality space.</para>
	/// </summary>
	[Flags]
	[UsedByNativeCode]
	public enum PlaneAlignment
	{
		/// <summary>
		///   <para>Plane has horizontal alignment.</para>
		/// </summary>
		Horizontal = 1,
		/// <summary>
		///   <para>Plane has vertical alignment.</para>
		/// </summary>
		Vertical = 2,
		/// <summary>
		///   <para>Plane is not alligned along cardinal (X, Y or Z) axis.</para>
		/// </summary>
		NonAxis = 4
	}
}
