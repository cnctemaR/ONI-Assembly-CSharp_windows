using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>This enum describes what should be done on the render target when the GPU is done rendering into it.</para>
	/// </summary>
	public enum RenderBufferStoreAction
	{
		/// <summary>
		///   <para>The RenderBuffer contents need to be stored to RAM. If the surface has MSAA enabled, this stores the non-resolved surface.</para>
		/// </summary>
		Store,
		/// <summary>
		///   <para>Resolve the (MSAA'd) surface. Currently only used with the RenderPass API.</para>
		/// </summary>
		Resolve,
		/// <summary>
		///   <para>Resolve the (MSAA'd) surface, but also store the multisampled version. Currently only used with the RenderPass API.</para>
		/// </summary>
		StoreAndResolve,
		/// <summary>
		///   <para>The contents of the RenderBuffer are not needed and can be discarded. Tile-based GPUs will skip writing out the surface contents altogether, providing performance boost.</para>
		/// </summary>
		DontCare
	}
}
