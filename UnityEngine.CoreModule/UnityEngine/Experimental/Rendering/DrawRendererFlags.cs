using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Flags controlling RenderLoop.DrawRenderers.</para>
	/// </summary>
	[Flags]
	public enum DrawRendererFlags
	{
		/// <summary>
		///   <para>No flags are set.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>When set, enables dynamic batching.</para>
		/// </summary>
		EnableDynamicBatching = 1,
		/// <summary>
		///   <para>When set, enables GPU instancing.</para>
		/// </summary>
		EnableInstancing = 2
	}
}
