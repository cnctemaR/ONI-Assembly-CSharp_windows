using System;
using UnityEngine.Rendering;

namespace UnityEngine
{
	/// <summary>
	///   <para>Fully describes setup of RenderTarget.</para>
	/// </summary>
	public struct RenderTargetSetup
	{
		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face, RenderBufferLoadAction[] colorLoad, RenderBufferStoreAction[] colorStore, RenderBufferLoadAction depthLoad, RenderBufferStoreAction depthStore)
		{
			this.color = color;
			this.depth = depth;
			this.mipLevel = mip;
			this.cubemapFace = face;
			this.depthSlice = 0;
			this.colorLoad = colorLoad;
			this.colorStore = colorStore;
			this.depthLoad = depthLoad;
			this.depthStore = depthStore;
		}

		/// <summary>
		///   <para>Constructs RenderTargetSetup.</para>
		/// </summary>
		/// <param name="color">Color Buffer(s) to set.</param>
		/// <param name="depth">Depth Buffer to set.</param>
		/// <param name="mipLevel">Mip Level to render to.</param>
		/// <param name="face">Cubemap face to render to.</param>
		/// <param name="mip"></param>
		public RenderTargetSetup(RenderBuffer color, RenderBuffer depth)
		{
			this = new RenderTargetSetup(new RenderBuffer[] { color }, depth);
		}

		/// <summary>
		///   <para>Constructs RenderTargetSetup.</para>
		/// </summary>
		/// <param name="color">Color Buffer(s) to set.</param>
		/// <param name="depth">Depth Buffer to set.</param>
		/// <param name="mipLevel">Mip Level to render to.</param>
		/// <param name="face">Cubemap face to render to.</param>
		/// <param name="mip"></param>
		public RenderTargetSetup(RenderBuffer color, RenderBuffer depth, int mipLevel)
		{
			this = new RenderTargetSetup(new RenderBuffer[] { color }, depth, mipLevel);
		}

		/// <summary>
		///   <para>Constructs RenderTargetSetup.</para>
		/// </summary>
		/// <param name="color">Color Buffer(s) to set.</param>
		/// <param name="depth">Depth Buffer to set.</param>
		/// <param name="mipLevel">Mip Level to render to.</param>
		/// <param name="face">Cubemap face to render to.</param>
		/// <param name="mip"></param>
		public RenderTargetSetup(RenderBuffer color, RenderBuffer depth, int mipLevel, CubemapFace face)
		{
			this = new RenderTargetSetup(new RenderBuffer[] { color }, depth, mipLevel, face);
		}

		public RenderTargetSetup(RenderBuffer color, RenderBuffer depth, int mipLevel, CubemapFace face, int depthSlice)
		{
			this = new RenderTargetSetup(new RenderBuffer[] { color }, depth, mipLevel, face);
			this.depthSlice = depthSlice;
		}

		/// <summary>
		///   <para>Constructs RenderTargetSetup.</para>
		/// </summary>
		/// <param name="color">Color Buffer(s) to set.</param>
		/// <param name="depth">Depth Buffer to set.</param>
		/// <param name="mipLevel">Mip Level to render to.</param>
		/// <param name="face">Cubemap face to render to.</param>
		/// <param name="mip"></param>
		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth)
		{
			this = new RenderTargetSetup(color, depth, 0, CubemapFace.Unknown);
		}

		/// <summary>
		///   <para>Constructs RenderTargetSetup.</para>
		/// </summary>
		/// <param name="color">Color Buffer(s) to set.</param>
		/// <param name="depth">Depth Buffer to set.</param>
		/// <param name="mipLevel">Mip Level to render to.</param>
		/// <param name="face">Cubemap face to render to.</param>
		/// <param name="mip"></param>
		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mipLevel)
		{
			this = new RenderTargetSetup(color, depth, mipLevel, CubemapFace.Unknown);
		}

		/// <summary>
		///   <para>Constructs RenderTargetSetup.</para>
		/// </summary>
		/// <param name="color">Color Buffer(s) to set.</param>
		/// <param name="depth">Depth Buffer to set.</param>
		/// <param name="mipLevel">Mip Level to render to.</param>
		/// <param name="face">Cubemap face to render to.</param>
		/// <param name="mip"></param>
		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face)
		{
			this = new RenderTargetSetup(color, depth, mip, face, RenderTargetSetup.LoadActions(color), RenderTargetSetup.StoreActions(color), depth.loadAction, depth.storeAction);
		}

		internal static RenderBufferLoadAction[] LoadActions(RenderBuffer[] buf)
		{
			RenderBufferLoadAction[] array = new RenderBufferLoadAction[buf.Length];
			for (int i = 0; i < buf.Length; i++)
			{
				array[i] = buf[i].loadAction;
				buf[i].loadAction = RenderBufferLoadAction.Load;
			}
			return array;
		}

		internal static RenderBufferStoreAction[] StoreActions(RenderBuffer[] buf)
		{
			RenderBufferStoreAction[] array = new RenderBufferStoreAction[buf.Length];
			for (int i = 0; i < buf.Length; i++)
			{
				array[i] = buf[i].storeAction;
				buf[i].storeAction = RenderBufferStoreAction.Store;
			}
			return array;
		}

		/// <summary>
		///   <para>Color Buffers to set.</para>
		/// </summary>
		public RenderBuffer[] color;

		/// <summary>
		///   <para>Depth Buffer to set.</para>
		/// </summary>
		public RenderBuffer depth;

		/// <summary>
		///   <para>Mip Level to render to.</para>
		/// </summary>
		public int mipLevel;

		/// <summary>
		///   <para>Cubemap face to render to.</para>
		/// </summary>
		public CubemapFace cubemapFace;

		/// <summary>
		///   <para>Slice of a Texture3D or Texture2DArray to set as a render target.</para>
		/// </summary>
		public int depthSlice;

		/// <summary>
		///   <para>Load Actions for Color Buffers. It will override any actions set on RenderBuffers themselves.</para>
		/// </summary>
		public RenderBufferLoadAction[] colorLoad;

		/// <summary>
		///   <para>Store Actions for Color Buffers. It will override any actions set on RenderBuffers themselves.</para>
		/// </summary>
		public RenderBufferStoreAction[] colorStore;

		/// <summary>
		///   <para>Load Action for Depth Buffer. It will override any actions set on RenderBuffer itself.</para>
		/// </summary>
		public RenderBufferLoadAction depthLoad;

		/// <summary>
		///   <para>Store Actions for Depth Buffer. It will override any actions set on RenderBuffer itself.</para>
		/// </summary>
		public RenderBufferStoreAction depthStore;
	}
}
