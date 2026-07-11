using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enum values for the Camera's targetEye property.</para>
	/// </summary>
	public enum StereoTargetEyeMask
	{
		/// <summary>
		///   <para>Do not render either eye to the HMD.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Render only the Left eye to the HMD.</para>
		/// </summary>
		Left,
		/// <summary>
		///   <para>Render only the right eye to the HMD.</para>
		/// </summary>
		Right,
		/// <summary>
		///   <para>Render both eyes to the HMD.</para>
		/// </summary>
		Both
	}
}
