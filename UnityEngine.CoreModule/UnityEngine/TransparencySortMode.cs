using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Transparent object sorting mode of a Camera.</para>
	/// </summary>
	public enum TransparencySortMode
	{
		/// <summary>
		///   <para>Default transparency sorting mode.</para>
		/// </summary>
		Default,
		/// <summary>
		///   <para>Perspective transparency sorting mode.</para>
		/// </summary>
		Perspective,
		/// <summary>
		///   <para>Orthographic transparency sorting mode.</para>
		/// </summary>
		Orthographic,
		/// <summary>
		///   <para>Sort objects based on distance along a custom axis.</para>
		/// </summary>
		CustomAxis
	}
}
