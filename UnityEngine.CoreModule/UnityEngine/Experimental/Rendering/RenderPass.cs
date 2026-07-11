using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Object encapsulating the duration of a single renderpass that contains one or more subpasses.
	///
	/// The RenderPass object provides a new way to switch rendertargets in the context of a Scriptable Rendering Pipeline. As opposed to the SetRenderTargets function, the RenderPass object specifies a clear beginning and an end for the rendering, alongside explicit load/store actions on the rendering surfaces.
	///
	/// The RenderPass object also allows running multiple subpasses within the same renderpass, where the pixel shaders have a read access to the current pixel value within the renderpass. This allows for efficient implementation of various rendering methods on tile-based GPUs, such as deferred rendering.
	///
	/// RenderPasses are natively implemented on Metal (iOS) and Vulkan, but the API is fully functional on all rendering backends via emulation (using legacy SetRenderTargets calls and reading the current pixel values via texel fetches).
	///
	/// A quick example on how to use the RenderPass API within the Scriptable Render Pipeline to implement deferred rendering:
	///
	/// The RenderPass mechanism has the following limitations:
	/// - All attachments must have the same resolution and MSAA sample count
	/// - The rendering results of previous subpasses are only available within the same screen-space pixel
	///   coordinate via the UNITY_READ_FRAMEBUFFER_INPUT(x) macro in the shader; the attachments cannot be bound
	///   as textures or otherwise accessed until the renderpass has ended
	/// - iOS Metal does not allow reading from the Z-Buffer, so an additional render target is needed to work around that
	/// - The maximum amount of attachments allowed per RenderPass is currently 8 + depth, but note that various GPUs may
	///   have stricter limits.</para>
	/// </summary>
	public class RenderPass : IDisposable
	{
		/// <summary>
		///   <para>Create a RenderPass and start it within the ScriptableRenderContext.</para>
		/// </summary>
		/// <param name="ctx">The ScriptableRenderContext object currently being rendered.</param>
		/// <param name="w">The width of the RenderPass surfaces in pixels.</param>
		/// <param name="h">The height of the RenderPass surfaces in pixels.</param>
		/// <param name="samples">MSAA sample count; set to 1 to disable antialiasing.</param>
		/// <param name="colors">Array of color attachments to use within this RenderPass.</param>
		/// <param name="depth">The attachment to be used as the depthstencil buffer for this RenderPass, or null to disable depthstencil.</param>
		public RenderPass(ScriptableRenderContext ctx, int w, int h, int samples, RenderPassAttachment[] colors, RenderPassAttachment depth = null)
		{
			this.width = w;
			this.height = h;
			this.sampleCount = samples;
			this.colorAttachments = colors;
			this.depthAttachment = depth;
			this.context = ctx;
			ScriptableRenderContext.BeginRenderPassInternal(ctx.Internal_GetPtr(), w, h, samples, colors, depth);
		}

		/// <summary>
		///   <para>Read only: array of RenderPassAttachment objects currently bound into this RenderPass.</para>
		/// </summary>
		public RenderPassAttachment[] colorAttachments { get; private set; }

		/// <summary>
		///   <para>Read only: The depth/stencil attachment used in this RenderPass, or null if none.</para>
		/// </summary>
		public RenderPassAttachment depthAttachment { get; private set; }

		/// <summary>
		///   <para>Read only: The width of the RenderPass surfaces in pixels.</para>
		/// </summary>
		public int width { get; private set; }

		/// <summary>
		///   <para>Read only: The height of the RenderPass surfaces in pixels.</para>
		/// </summary>
		public int height { get; private set; }

		/// <summary>
		///   <para>Read only: MSAA sample count for this RenderPass.</para>
		/// </summary>
		public int sampleCount { get; private set; }

		/// <summary>
		///   <para>Read only: The ScriptableRenderContext object this RenderPass was created for.</para>
		/// </summary>
		public ScriptableRenderContext context { get; private set; }

		/// <summary>
		///   <para>End the RenderPass.</para>
		/// </summary>
		public void Dispose()
		{
			ScriptableRenderContext.EndRenderPassInternal(this.context.Internal_GetPtr());
		}

		/// <summary>
		///   <para>This class encapsulates a single subpass within a RenderPass. RenderPasses can never be standalone, they must always contain at least one SubPass. See Also: RenderPass.</para>
		/// </summary>
		public class SubPass : IDisposable
		{
			/// <summary>
			///   <para>Create a subpass and start it.</para>
			/// </summary>
			/// <param name="renderPass">The RenderPass object this subpass is part of.</param>
			/// <param name="colors">Array of attachments to be used as the color render targets in this subpass. All attachments in this array must also be declared in the RenderPass constructor.</param>
			/// <param name="inputs">Array of attachments to be used as input attachments in this subpass. All attachments in this array must also be declared in the RenderPass constructor.</param>
			/// <param name="readOnlyDepth">If true, the depth attachment is read-only in this subpass. Some renderers require this in order to be able to use the depth attachment as input.</param>
			public SubPass(RenderPass renderPass, RenderPassAttachment[] colors, RenderPassAttachment[] inputs, bool readOnlyDepth = false)
			{
				ScriptableRenderContext.BeginSubPassInternal(renderPass.context.Internal_GetPtr(), (colors == null) ? new RenderPassAttachment[0] : colors, (inputs == null) ? new RenderPassAttachment[0] : inputs, readOnlyDepth);
			}

			/// <summary>
			///   <para>End the subpass.</para>
			/// </summary>
			public void Dispose()
			{
			}
		}
	}
}
