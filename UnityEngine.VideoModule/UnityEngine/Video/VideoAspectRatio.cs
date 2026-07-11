using System;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	/// <summary>
	///   <para>Methods used to fit a video in the target area.</para>
	/// </summary>
	[RequiredByNativeCode]
	public enum VideoAspectRatio
	{
		/// <summary>
		///   <para>Preserve the pixel size without adjusting for target area.</para>
		/// </summary>
		NoScaling,
		/// <summary>
		///   <para>Resize proportionally so that height fits the target area, cropping or adding black bars on each side if needed.</para>
		/// </summary>
		FitVertically,
		/// <summary>
		///   <para>Resize proportionally so that width fits the target area, cropping or adding black bars above and below if needed.</para>
		/// </summary>
		FitHorizontally,
		/// <summary>
		///   <para>Resize proportionally so that content fits the target area, adding black bars if needed.</para>
		/// </summary>
		FitInside,
		/// <summary>
		///   <para>Resize proportionally so that content fits the target area, cropping if needed.</para>
		/// </summary>
		FitOutside,
		/// <summary>
		///   <para>Resize non-proportionally to fit the target area.</para>
		/// </summary>
		Stretch
	}
}
