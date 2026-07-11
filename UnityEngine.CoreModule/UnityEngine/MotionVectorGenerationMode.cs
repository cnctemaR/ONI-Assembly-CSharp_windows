using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The type of motion vectors that should be generated.</para>
	/// </summary>
	public enum MotionVectorGenerationMode
	{
		/// <summary>
		///   <para>Use only camera movement to track motion.</para>
		/// </summary>
		Camera,
		/// <summary>
		///   <para>Use a specific pass (if required) to track motion.</para>
		/// </summary>
		Object,
		/// <summary>
		///   <para>Do not track motion. Motion vectors will be 0.</para>
		/// </summary>
		ForceNoMotion
	}
}
