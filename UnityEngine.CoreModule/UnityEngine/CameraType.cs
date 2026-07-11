using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Describes different types of camera.</para>
	/// </summary>
	[Flags]
	public enum CameraType
	{
		/// <summary>
		///   <para>Used to indicate a regular in-game camera.</para>
		/// </summary>
		Game = 1,
		/// <summary>
		///   <para>Used to indicate that a camera is used for rendering the Scene View in the Editor.</para>
		/// </summary>
		SceneView = 2,
		/// <summary>
		///   <para>Used to indicate a camera that is used for rendering previews in the Editor.</para>
		/// </summary>
		Preview = 4,
		/// <summary>
		///   <para>Used to indicate that a camera is used for rendering VR (in edit mode) in the Editor.</para>
		/// </summary>
		VR = 8,
		/// <summary>
		///   <para>Used to indicate a camera that is used for rendering reflection probes.</para>
		/// </summary>
		Reflection = 16
	}
}
