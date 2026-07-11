using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>A declaration of a single color or depth rendering surface to be attached into a RenderPass.</para>
	/// </summary>
	[NativeType("Runtime/Graphics/ScriptableRenderLoop/ScriptableRenderContext.h")]
	public class RenderPassAttachment : Object
	{
		/// <summary>
		///   <para>Create a RenderPassAttachment to be used with RenderPass.</para>
		/// </summary>
		/// <param name="fmt">The format of this attachment.</param>
		public RenderPassAttachment(RenderTextureFormat fmt)
		{
			RenderPassAttachment.Internal_CreateAttachment(this);
			this.loadAction = RenderBufferLoadAction.DontCare;
			this.storeAction = RenderBufferStoreAction.DontCare;
			this.format = fmt;
			this.loadStoreTarget = new RenderTargetIdentifier(BuiltinRenderTextureType.None);
			this.resolveTarget = new RenderTargetIdentifier(BuiltinRenderTextureType.None);
			this.clearColor = new Color(0f, 0f, 0f, 0f);
			this.clearDepth = 1f;
		}

		/// <summary>
		///   <para>The load action to be used on this attachment when the RenderPass starts.</para>
		/// </summary>
		public extern RenderBufferLoadAction loadAction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			private set;
		}

		/// <summary>
		///   <para>The store action to use with this attachment when the RenderPass ends. Only used when either BindSurface or BindResolveSurface has been called.</para>
		/// </summary>
		public extern RenderBufferStoreAction storeAction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			private set;
		}

		/// <summary>
		///   <para>The RenderTextureFormat of this attachment.</para>
		/// </summary>
		public extern RenderTextureFormat format
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			private set;
		}

		private RenderTargetIdentifier loadStoreTarget
		{
			get
			{
				RenderTargetIdentifier renderTargetIdentifier;
				this.get_loadStoreTarget_Injected(out renderTargetIdentifier);
				return renderTargetIdentifier;
			}
			set
			{
				this.set_loadStoreTarget_Injected(ref value);
			}
		}

		private RenderTargetIdentifier resolveTarget
		{
			get
			{
				RenderTargetIdentifier renderTargetIdentifier;
				this.get_resolveTarget_Injected(out renderTargetIdentifier);
				return renderTargetIdentifier;
			}
			set
			{
				this.set_resolveTarget_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The currently assigned clear color for this attachment. Default is black.</para>
		/// </summary>
		public Color clearColor
		{
			get
			{
				Color color;
				this.get_clearColor_Injected(out color);
				return color;
			}
			private set
			{
				this.set_clearColor_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Currently assigned depth clear value for this attachment. Default value is 1.0.</para>
		/// </summary>
		public extern float clearDepth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			private set;
		}

		/// <summary>
		///   <para>Currently assigned stencil clear value for this attachment. Default is 0.</para>
		/// </summary>
		public extern uint clearStencil
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			private set;
		}

		/// <summary>
		///   <para>Binds this RenderPassAttachment to the given target surface.</para>
		/// </summary>
		/// <param name="tgt">The surface to use as the backing storage for this RenderPassAttachment.</param>
		/// <param name="loadExistingContents">Whether to read in the existing contents of the surface when the RenderPass starts.</param>
		/// <param name="storeResults">Whether to store the rendering results of the attachment when the RenderPass ends.</param>
		public void BindSurface(RenderTargetIdentifier tgt, bool loadExistingContents, bool storeResults)
		{
			this.loadStoreTarget = tgt;
			if (loadExistingContents && this.loadAction != RenderBufferLoadAction.Clear)
			{
				this.loadAction = RenderBufferLoadAction.Load;
			}
			if (storeResults)
			{
				if (this.storeAction == RenderBufferStoreAction.StoreAndResolve || this.storeAction == RenderBufferStoreAction.Resolve)
				{
					this.storeAction = RenderBufferStoreAction.StoreAndResolve;
				}
				else
				{
					this.storeAction = RenderBufferStoreAction.Store;
				}
			}
		}

		/// <summary>
		///   <para>When the renderpass that uses this attachment ends, resolve the MSAA surface into the given target.</para>
		/// </summary>
		/// <param name="tgt">The target surface to receive the MSAA-resolved pixels.</param>
		public void BindResolveSurface(RenderTargetIdentifier tgt)
		{
			this.resolveTarget = tgt;
			if (this.storeAction == RenderBufferStoreAction.StoreAndResolve || this.storeAction == RenderBufferStoreAction.Store)
			{
				this.storeAction = RenderBufferStoreAction.StoreAndResolve;
			}
			else
			{
				this.storeAction = RenderBufferStoreAction.Resolve;
			}
		}

		/// <summary>
		///   <para>When the RenderPass starts, clear this attachment into the color or depth/stencil values given (depending on the format of this attachment). Changes loadAction to RenderBufferLoadAction.Clear.</para>
		/// </summary>
		/// <param name="clearCol">Color clear value. Ignored on depth/stencil attachments.</param>
		/// <param name="clearDep">Depth clear value. Ignored on color surfaces.</param>
		/// <param name="clearStenc">Stencil clear value. Ignored on color or depth-only surfaces.</param>
		public void Clear(Color clearCol, float clearDep = 1f, uint clearStenc = 0U)
		{
			this.clearColor = clearCol;
			this.clearDepth = clearDep;
			this.clearStencil = clearStenc;
			this.loadAction = RenderBufferLoadAction.Clear;
		}

		[NativeMethod(Name = "RenderPassAttachment::Internal_CreateAttachment", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Internal_CreateAttachment([Writable] RenderPassAttachment self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_loadStoreTarget_Injected(out RenderTargetIdentifier ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_loadStoreTarget_Injected(ref RenderTargetIdentifier value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_resolveTarget_Injected(out RenderTargetIdentifier ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_resolveTarget_Injected(ref RenderTargetIdentifier value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_clearColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_clearColor_Injected(ref Color value);
	}
}
