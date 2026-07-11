using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Select how gradients will be evaluated.</para>
	/// </summary>
	public enum GradientMode
	{
		/// <summary>
		///   <para>Find the 2 keys adjacent to the requested evaluation time, and linearly interpolate between them to obtain a blended color.</para>
		/// </summary>
		Blend,
		/// <summary>
		///   <para>Return a fixed color, by finding the first key whose time value is greater than the requested evaluation time.</para>
		/// </summary>
		Fixed
	}
}
