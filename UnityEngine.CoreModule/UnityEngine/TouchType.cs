using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Describes whether a touch is direct, indirect (or remote), or from a stylus.</para>
	/// </summary>
	public enum TouchType
	{
		/// <summary>
		///   <para>A direct touch on a device.</para>
		/// </summary>
		Direct,
		/// <summary>
		///   <para>An Indirect, or remote, touch on a device.</para>
		/// </summary>
		Indirect,
		/// <summary>
		///   <para>A touch from a stylus on a device.</para>
		/// </summary>
		Stylus
	}
}
