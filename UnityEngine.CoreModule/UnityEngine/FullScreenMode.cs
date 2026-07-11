using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Platform agnostic fullscreen mode. Not all platforms support all modes.</para>
	/// </summary>
	public enum FullScreenMode
	{
		/// <summary>
		///   <para>Exclusive Mode.</para>
		/// </summary>
		ExclusiveFullScreen,
		/// <summary>
		///   <para>Fullscreen window.</para>
		/// </summary>
		FullScreenWindow,
		/// <summary>
		///   <para>Maximized window.</para>
		/// </summary>
		MaximizedWindow,
		/// <summary>
		///   <para>Windowed.</para>
		/// </summary>
		Windowed
	}
}
