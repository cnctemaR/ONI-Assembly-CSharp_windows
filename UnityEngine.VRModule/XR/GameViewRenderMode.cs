using System;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Enumeration of available modes for XR rendering in the Game view or in the main window on a host PC. XR rendering only occurs when the Unity Editor is in Play Mode.</para>
	/// </summary>
	public enum GameViewRenderMode
	{
		/// <summary>
		///   <para>Renders the left eye of the XR device in the Game View window or in main window on a host PC.</para>
		/// </summary>
		LeftEye = 1,
		/// <summary>
		///   <para>Renders the right eye of the XR device in the Game View window or in main window on a host PC.</para>
		/// </summary>
		RightEye,
		/// <summary>
		///   <para>Renders both eyes of the XR device side-by-side in the Game view or in the main window on a host PC.</para>
		/// </summary>
		BothEyes,
		/// <summary>
		///   <para>Renders both eyes of the XR device, and the occlusion mesh, side-by-side in the Game view or in the main window on a host PC.</para>
		/// </summary>
		OcclusionMesh
	}
}
