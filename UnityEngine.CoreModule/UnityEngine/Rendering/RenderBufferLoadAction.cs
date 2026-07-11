using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>This enum describes what should be done on the render target when it is activated (loaded).</para>
	/// </summary>
	public enum RenderBufferLoadAction
	{
		/// <summary>
		///   <para>When this RenderBuffer is activated, preserve the existing contents of it. This setting is expensive on tile-based GPUs and should be avoided whenever possible.</para>
		/// </summary>
		Load,
		/// <summary>
		///   <para>Upon activating the render buffer, clear its contents. Currently only works together with the RenderPass API.</para>
		/// </summary>
		Clear,
		/// <summary>
		///   <para>When this RenderBuffer is activated, the GPU is instructed not to care about the existing contents of that RenderBuffer. On tile-based GPUs this means that the RenderBuffer contents do not need to be loaded into the tile memory, providing a performance boost.</para>
		/// </summary>
		DontCare
	}
}
