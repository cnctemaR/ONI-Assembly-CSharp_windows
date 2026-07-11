using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;

namespace UnityEngine
{
	/// <summary>
	///   <para>Raw interface to Unity's drawing functions.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/LightProbeProxyVolume.h")]
	[NativeHeader("Runtime/Graphics/ColorGamut.h")]
	[NativeHeader("Runtime/Graphics/CopyTexture.h")]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	public class Graphics
	{
		[FreeFunction("GraphicsScripting::GetMaxDrawMeshInstanceCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMaxDrawMeshInstanceCount();

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ColorGamut GetActiveColorGamut();

		/// <summary>
		///   <para>Returns the currently active color gamut.</para>
		/// </summary>
		public static ColorGamut activeColorGamut
		{
			get
			{
				return Graphics.GetActiveColorGamut();
			}
		}

		/// <summary>
		///   <para>Graphics Tier classification for current device.
		/// Changing this value affects any subsequently loaded shaders. Initially this value is auto-detected from the hardware in use.</para>
		/// </summary>
		[StaticAccessor("GetGfxDevice()", StaticAccessorType.Dot)]
		public static extern GraphicsTier activeTier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("GraphicsScripting::GetActiveColorBuffer")]
		private static RenderBuffer GetActiveColorBuffer()
		{
			RenderBuffer renderBuffer;
			Graphics.GetActiveColorBuffer_Injected(out renderBuffer);
			return renderBuffer;
		}

		[FreeFunction("GraphicsScripting::GetActiveDepthBuffer")]
		private static RenderBuffer GetActiveDepthBuffer()
		{
			RenderBuffer renderBuffer;
			Graphics.GetActiveDepthBuffer_Injected(out renderBuffer);
			return renderBuffer;
		}

		[FreeFunction("GraphicsScripting::SetNullRT")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetNullRT();

		[NativeMethod(Name = "GraphicsScripting::SetRTSimple", IsFreeFunction = true, ThrowsException = true)]
		private static void Internal_SetRTSimple(RenderBuffer color, RenderBuffer depth, int mip, CubemapFace face, int depthSlice)
		{
			Graphics.Internal_SetRTSimple_Injected(ref color, ref depth, mip, face, depthSlice);
		}

		[NativeMethod(Name = "GraphicsScripting::SetMRTSimple", IsFreeFunction = true, ThrowsException = true)]
		private static void Internal_SetMRTSimple([NotNull] RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face, int depthSlice)
		{
			Graphics.Internal_SetMRTSimple_Injected(color, ref depth, mip, face, depthSlice);
		}

		[NativeMethod(Name = "GraphicsScripting::SetMRTFull", IsFreeFunction = true, ThrowsException = true)]
		private static void Internal_SetMRTFullSetup([NotNull] RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face, int depthSlice, [NotNull] RenderBufferLoadAction[] colorLA, [NotNull] RenderBufferStoreAction[] colorSA, RenderBufferLoadAction depthLA, RenderBufferStoreAction depthSA)
		{
			Graphics.Internal_SetMRTFullSetup_Injected(color, ref depth, mip, face, depthSlice, colorLA, colorSA, depthLA, depthSA);
		}

		[NativeMethod(Name = "GraphicsScripting::SetRandomWriteTargetRT", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRandomWriteTargetRT(int index, RenderTexture uav);

		[FreeFunction("GraphicsScripting::SetRandomWriteTargetBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRandomWriteTargetBuffer(int index, ComputeBuffer uav, bool preserveCounterValue);

		/// <summary>
		///   <para>Clear random write targets for level pixel shaders.</para>
		/// </summary>
		[StaticAccessor("GetGfxDevice()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearRandomWriteTargets();

		[FreeFunction("CopyTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Full(Texture src, Texture dst);

		[FreeFunction("CopyTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Slice_AllMips(Texture src, int srcElement, Texture dst, int dstElement);

		[FreeFunction("CopyTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Slice(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip);

		[FreeFunction("CopyTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY);

		[FreeFunction("ConvertTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ConvertTexture_Full(Texture src, Texture dst);

		[FreeFunction("ConvertTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ConvertTexture_Slice(Texture src, int srcElement, Texture dst, int dstElement);

		[FreeFunction("GraphicsScripting::DrawMeshNow")]
		private static void Internal_DrawMeshNow1(Mesh mesh, int subsetIndex, Vector3 position, Quaternion rotation)
		{
			Graphics.Internal_DrawMeshNow1_Injected(mesh, subsetIndex, ref position, ref rotation);
		}

		[FreeFunction("GraphicsScripting::DrawMeshNow")]
		private static void Internal_DrawMeshNow2(Mesh mesh, int subsetIndex, Matrix4x4 matrix)
		{
			Graphics.Internal_DrawMeshNow2_Injected(mesh, subsetIndex, ref matrix);
		}

		[FreeFunction("GraphicsScripting::DrawTexture")]
		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_DrawTexture(ref Internal_DrawTextureArguments args);

		[FreeFunction("GraphicsScripting::DrawMesh")]
		private static void Internal_DrawMesh(Mesh mesh, int submeshIndex, Matrix4x4 matrix, Material material, int layer, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume)
		{
			Graphics.Internal_DrawMesh_Injected(mesh, submeshIndex, ref matrix, material, layer, camera, properties, castShadows, receiveShadows, probeAnchor, lightProbeUsage, lightProbeProxyVolume);
		}

		[FreeFunction("GraphicsScripting::DrawMeshInstanced")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume);

		[FreeFunction("GraphicsScripting::DrawMeshInstancedIndirect")]
		private static void Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume)
		{
			Graphics.Internal_DrawMeshInstancedIndirect_Injected(mesh, submeshIndex, material, ref bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
		}

		[FreeFunction("GraphicsScripting::DrawProcedural")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawProcedural(MeshTopology topology, int vertexCount, int instanceCount);

		[FreeFunction("GraphicsScripting::DrawProceduralIndirect")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset);

		[FreeFunction("GraphicsScripting::BlitMaterial")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BlitMaterial(Texture source, RenderTexture dest, [NotNull] Material mat, int pass, bool setRT);

		[FreeFunction("GraphicsScripting::BlitMultitap")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BlitMultiTap(Texture source, RenderTexture dest, [NotNull] Material mat, [NotNull] Vector2[] offsets);

		[FreeFunction("GraphicsScripting::Blit")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Blit2(Texture source, RenderTexture dest);

		[FreeFunction("GraphicsScripting::Blit")]
		private static void Blit4(Texture source, RenderTexture dest, Vector2 scale, Vector2 offset)
		{
			Graphics.Blit4_Injected(source, dest, ref scale, ref offset);
		}

		[NativeMethod(Name = "GraphicsScripting::CreateGPUFence", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateGPUFenceImpl(SynchronisationStage stage);

		[NativeMethod(Name = "GraphicsScripting::WaitOnGPUFence", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WaitOnGPUFenceImpl(IntPtr fencePtr, SynchronisationStage stage);

		/// <summary>
		///   <para>Execute a command buffer.</para>
		/// </summary>
		/// <param name="buffer">The buffer to execute.</param>
		[NativeMethod(Name = "GraphicsScripting::ExecuteCommandBuffer", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExecuteCommandBuffer([NotNull] CommandBuffer buffer);

		/// <summary>
		///   <para>Executes a command buffer on an async compute queue with the queue selected based on the ComputeQueueType parameter passed.
		///
		/// It is required that all of the commands within the command buffer be of a type suitable for execution on the async compute queues. If the buffer contains any commands that are not appropriate then an error will be logged and displayed in the editor window.  Specifically the following commands are permitted in a CommandBuffer intended for async execution:
		///
		/// CommandBuffer.BeginSample
		///
		/// CommandBuffer.CopyCounterValue
		///
		/// CommandBuffer.CopyTexture
		///
		/// CommandBuffer.CreateGPUFence
		///
		/// CommandBuffer.DispatchCompute
		///
		/// CommandBuffer.EndSample
		///
		/// CommandBuffer.IssuePluginEvent
		///
		/// CommandBuffer.SetComputeBufferParam
		///
		/// CommandBuffer.SetComputeFloatParam
		///
		/// CommandBuffer.SetComputeFloatParams
		///
		/// CommandBuffer.SetComputeTextureParam
		///
		/// CommandBuffer.SetComputeVectorParam
		///
		/// CommandBuffer.WaitOnGPUFence
		///
		/// All of the commands within the buffer are guaranteed to be executed on the same queue. If the target platform does not support async compute queues then the work is dispatched on the graphics queue.</para>
		/// </summary>
		/// <param name="buffer">The CommandBuffer to be executed.</param>
		/// <param name="queueType">Describes the desired async compute queue the suuplied CommandBuffer should be executed on.</param>
		[NativeMethod(Name = "GraphicsScripting::ExecuteCommandBufferAsync", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExecuteCommandBufferAsync([NotNull] CommandBuffer buffer, ComputeQueueType queueType);

		internal static void CheckLoadActionValid(RenderBufferLoadAction load, string bufferType)
		{
			if (load != RenderBufferLoadAction.Load && load != RenderBufferLoadAction.DontCare)
			{
				throw new ArgumentException(UnityString.Format("Bad {0} LoadAction provided.", new object[] { bufferType }));
			}
		}

		internal static void CheckStoreActionValid(RenderBufferStoreAction store, string bufferType)
		{
			if (store != RenderBufferStoreAction.Store && store != RenderBufferStoreAction.DontCare)
			{
				throw new ArgumentException(UnityString.Format("Bad {0} StoreAction provided.", new object[] { bufferType }));
			}
		}

		internal static void SetRenderTargetImpl(RenderTargetSetup setup)
		{
			if (setup.color.Length == 0)
			{
				throw new ArgumentException("Invalid color buffer count for SetRenderTarget");
			}
			if (setup.color.Length != setup.colorLoad.Length)
			{
				throw new ArgumentException("Color LoadAction and Buffer arrays have different sizes");
			}
			if (setup.color.Length != setup.colorStore.Length)
			{
				throw new ArgumentException("Color StoreAction and Buffer arrays have different sizes");
			}
			foreach (RenderBufferLoadAction renderBufferLoadAction in setup.colorLoad)
			{
				Graphics.CheckLoadActionValid(renderBufferLoadAction, "Color");
			}
			foreach (RenderBufferStoreAction renderBufferStoreAction in setup.colorStore)
			{
				Graphics.CheckStoreActionValid(renderBufferStoreAction, "Color");
			}
			Graphics.CheckLoadActionValid(setup.depthLoad, "Depth");
			Graphics.CheckStoreActionValid(setup.depthStore, "Depth");
			if (setup.cubemapFace < CubemapFace.Unknown || setup.cubemapFace > CubemapFace.NegativeZ)
			{
				throw new ArgumentException("Bad CubemapFace provided");
			}
			Graphics.Internal_SetMRTFullSetup(setup.color, setup.depth, setup.mipLevel, setup.cubemapFace, setup.depthSlice, setup.colorLoad, setup.colorStore, setup.depthLoad, setup.depthStore);
		}

		internal static void SetRenderTargetImpl(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
		{
			Graphics.Internal_SetRTSimple(colorBuffer, depthBuffer, mipLevel, face, depthSlice);
		}

		internal static void SetRenderTargetImpl(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
		{
			if (rt)
			{
				Graphics.SetRenderTargetImpl(rt.colorBuffer, rt.depthBuffer, mipLevel, face, depthSlice);
			}
			else
			{
				Graphics.Internal_SetNullRT();
			}
		}

		internal static void SetRenderTargetImpl(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
		{
			Graphics.Internal_SetMRTSimple(colorBuffers, depthBuffer, mipLevel, face, depthSlice);
		}

		/// <summary>
		///   <para>Sets current render target.</para>
		/// </summary>
		/// <param name="rt">RenderTexture to set as active render target.</param>
		/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
		/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
		/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
		/// <param name="colorBuffer">Color buffer to render into.</param>
		/// <param name="depthBuffer">Depth buffer to render into.</param>
		/// <param name="colorBuffers">Color buffers to render into (for multiple render target effects).</param>
		/// <param name="setup">Full render target setup information.</param>
		public static void SetRenderTarget(RenderTexture rt, [DefaultValue("0")] int mipLevel, [DefaultValue("CubemapFace.Unknown")] CubemapFace face, [DefaultValue("0")] int depthSlice)
		{
			Graphics.SetRenderTargetImpl(rt, mipLevel, face, depthSlice);
		}

		/// <summary>
		///   <para>Sets current render target.</para>
		/// </summary>
		/// <param name="rt">RenderTexture to set as active render target.</param>
		/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
		/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
		/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
		/// <param name="colorBuffer">Color buffer to render into.</param>
		/// <param name="depthBuffer">Depth buffer to render into.</param>
		/// <param name="colorBuffers">Color buffers to render into (for multiple render target effects).</param>
		/// <param name="setup">Full render target setup information.</param>
		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, [DefaultValue("0")] int mipLevel, [DefaultValue("CubemapFace.Unknown")] CubemapFace face, [DefaultValue("0")] int depthSlice)
		{
			Graphics.SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, depthSlice);
		}

		/// <summary>
		///   <para>Sets current render target.</para>
		/// </summary>
		/// <param name="rt">RenderTexture to set as active render target.</param>
		/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
		/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
		/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
		/// <param name="colorBuffer">Color buffer to render into.</param>
		/// <param name="depthBuffer">Depth buffer to render into.</param>
		/// <param name="colorBuffers">Color buffers to render into (for multiple render target effects).</param>
		/// <param name="setup">Full render target setup information.</param>
		public static void SetRenderTarget(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer)
		{
			Graphics.SetRenderTargetImpl(colorBuffers, depthBuffer, 0, CubemapFace.Unknown, 0);
		}

		/// <summary>
		///   <para>Sets current render target.</para>
		/// </summary>
		/// <param name="rt">RenderTexture to set as active render target.</param>
		/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
		/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
		/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
		/// <param name="colorBuffer">Color buffer to render into.</param>
		/// <param name="depthBuffer">Depth buffer to render into.</param>
		/// <param name="colorBuffers">Color buffers to render into (for multiple render target effects).</param>
		/// <param name="setup">Full render target setup information.</param>
		public static void SetRenderTarget(RenderTargetSetup setup)
		{
			Graphics.SetRenderTargetImpl(setup);
		}

		/// <summary>
		///   <para>Currently active color buffer (Read Only).</para>
		/// </summary>
		public static RenderBuffer activeColorBuffer
		{
			get
			{
				return Graphics.GetActiveColorBuffer();
			}
		}

		/// <summary>
		///   <para>Currently active depth/stencil buffer (Read Only).</para>
		/// </summary>
		public static RenderBuffer activeDepthBuffer
		{
			get
			{
				return Graphics.GetActiveDepthBuffer();
			}
		}

		/// <summary>
		///   <para>Set random write target for level pixel shaders.</para>
		/// </summary>
		/// <param name="index">Index of the random write target in the shader.</param>
		/// <param name="uav">RenderTexture to set as write target.</param>
		/// <param name="preserveCounterValue">Whether to leave the append/consume counter value unchanged.</param>
		public static void SetRandomWriteTarget(int index, RenderTexture uav)
		{
			if (index < 0 || index >= SystemInfo.supportedRandomWriteTargetCount)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("must be non-negative less than {0}.", SystemInfo.supportedRandomWriteTargetCount));
			}
			Graphics.Internal_SetRandomWriteTargetRT(index, uav);
		}

		/// <summary>
		///   <para>Set random write target for level pixel shaders.</para>
		/// </summary>
		/// <param name="index">Index of the random write target in the shader.</param>
		/// <param name="uav">RenderTexture to set as write target.</param>
		/// <param name="preserveCounterValue">Whether to leave the append/consume counter value unchanged.</param>
		public static void SetRandomWriteTarget(int index, ComputeBuffer uav, [DefaultValue("false")] bool preserveCounterValue)
		{
			if (uav == null)
			{
				throw new ArgumentNullException("uav");
			}
			if (uav.m_Ptr == IntPtr.Zero)
			{
				throw new ObjectDisposedException("uav");
			}
			if (index < 0 || index >= SystemInfo.supportedRandomWriteTargetCount)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("must be non-negative less than {0}.", SystemInfo.supportedRandomWriteTargetCount));
			}
			Graphics.Internal_SetRandomWriteTargetBuffer(index, uav, preserveCounterValue);
		}

		/// <summary>
		///   <para>Copy texture contents.</para>
		/// </summary>
		/// <param name="src">Source texture.</param>
		/// <param name="dst">Destination texture.</param>
		/// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
		/// <param name="srcMip">Source texture mipmap level.</param>
		/// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
		/// <param name="dstMip">Destination texture mipmap level.</param>
		/// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
		/// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
		/// <param name="srcWidth">Width of source texture region to copy.</param>
		/// <param name="srcHeight">Height of source texture region to copy.</param>
		/// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
		/// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
		public static void CopyTexture(Texture src, Texture dst)
		{
			Graphics.CopyTexture_Full(src, dst);
		}

		public static void CopyTexture(Texture src, int srcElement, Texture dst, int dstElement)
		{
			Graphics.CopyTexture_Slice_AllMips(src, srcElement, dst, dstElement);
		}

		/// <summary>
		///   <para>Copy texture contents.</para>
		/// </summary>
		/// <param name="src">Source texture.</param>
		/// <param name="dst">Destination texture.</param>
		/// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
		/// <param name="srcMip">Source texture mipmap level.</param>
		/// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
		/// <param name="dstMip">Destination texture mipmap level.</param>
		/// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
		/// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
		/// <param name="srcWidth">Width of source texture region to copy.</param>
		/// <param name="srcHeight">Height of source texture region to copy.</param>
		/// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
		/// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
		public static void CopyTexture(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip)
		{
			Graphics.CopyTexture_Slice(src, srcElement, srcMip, dst, dstElement, dstMip);
		}

		/// <summary>
		///   <para>Copy texture contents.</para>
		/// </summary>
		/// <param name="src">Source texture.</param>
		/// <param name="dst">Destination texture.</param>
		/// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
		/// <param name="srcMip">Source texture mipmap level.</param>
		/// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
		/// <param name="dstMip">Destination texture mipmap level.</param>
		/// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
		/// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
		/// <param name="srcWidth">Width of source texture region to copy.</param>
		/// <param name="srcHeight">Height of source texture region to copy.</param>
		/// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
		/// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
		public static void CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY)
		{
			Graphics.CopyTexture_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dst, dstElement, dstMip, dstX, dstY);
		}

		/// <summary>
		///   <para>This function provides an efficient way to convert between textures of different formats and dimensions.
		/// The destination texture format should be uncompressed and correspond to a supported RenderTextureFormat.</para>
		/// </summary>
		/// <param name="src">Source texture.</param>
		/// <param name="dst">Destination texture.</param>
		/// <param name="srcElement">Source element (e.g. cubemap face).  Set this to 0 for 2d source textures.</param>
		/// <param name="dstElement">Destination element (e.g. cubemap face or texture array element).</param>
		/// <returns>
		///   <para>True if the call succeeded.</para>
		/// </returns>
		public static bool ConvertTexture(Texture src, Texture dst)
		{
			return Graphics.ConvertTexture_Full(src, dst);
		}

		/// <summary>
		///   <para>This function provides an efficient way to convert between textures of different formats and dimensions.
		/// The destination texture format should be uncompressed and correspond to a supported RenderTextureFormat.</para>
		/// </summary>
		/// <param name="src">Source texture.</param>
		/// <param name="dst">Destination texture.</param>
		/// <param name="srcElement">Source element (e.g. cubemap face).  Set this to 0 for 2d source textures.</param>
		/// <param name="dstElement">Destination element (e.g. cubemap face or texture array element).</param>
		/// <returns>
		///   <para>True if the call succeeded.</para>
		/// </returns>
		public static bool ConvertTexture(Texture src, int srcElement, Texture dst, int dstElement)
		{
			return Graphics.ConvertTexture_Slice(src, srcElement, dst, dstElement);
		}

		/// <summary>
		///   <para>Creates a GPUFence which will be passed after the last Blit, Clear, Draw, Dispatch or Texture Copy command prior to this call has been completed on the GPU.</para>
		/// </summary>
		/// <param name="stage">On some platforms there is a significant gap between the vertex processing completing and the pixel processing begining for a given draw call. This parameter allows for the fence to be passed after either the vertex or pixel processing for the proceeding draw has completed. If a compute shader dispatch was the last task submitted then this parameter is ignored.</param>
		/// <returns>
		///   <para>Returns a new GPUFence.</para>
		/// </returns>
		public static GPUFence CreateGPUFence([DefaultValue("SynchronisationStage.PixelProcessing")] SynchronisationStage stage)
		{
			GPUFence gpufence = default(GPUFence);
			gpufence.m_Ptr = Graphics.CreateGPUFenceImpl(stage);
			gpufence.InitPostAllocation();
			gpufence.Validate();
			return gpufence;
		}

		/// <summary>
		///   <para>Instructs the GPU's processing of the graphics queue to wait until the given GPUFence is passed.</para>
		/// </summary>
		/// <param name="fence">The GPUFence that the GPU will be instructed to wait upon before proceeding with its processing of the graphics queue.</param>
		/// <param name="stage">On some platforms there is a significant gap between the vertex processing completing and the pixel processing begining for a given draw call. This parameter allows for requested wait to be before the next items vertex or pixel processing begins. If a compute shader dispatch is the next item to be submitted then this parameter is ignored.</param>
		public static void WaitOnGPUFence(GPUFence fence, [DefaultValue("SynchronisationStage.VertexProcessing")] SynchronisationStage stage)
		{
			fence.Validate();
			if (fence.IsFencePending())
			{
				Graphics.WaitOnGPUFenceImpl(fence.m_Ptr, stage);
			}
		}

		[ExcludeFromDocs]
		public static GPUFence CreateGPUFence()
		{
			return Graphics.CreateGPUFence(SynchronisationStage.PixelProcessing);
		}

		[ExcludeFromDocs]
		public static void WaitOnGPUFence(GPUFence fence)
		{
			Graphics.WaitOnGPUFence(fence, SynchronisationStage.VertexProcessing);
		}

		private static void DrawTextureImpl(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat, int pass)
		{
			Internal_DrawTextureArguments internal_DrawTextureArguments = default(Internal_DrawTextureArguments);
			internal_DrawTextureArguments.screenRect = screenRect;
			internal_DrawTextureArguments.sourceRect = sourceRect;
			internal_DrawTextureArguments.leftBorder = leftBorder;
			internal_DrawTextureArguments.rightBorder = rightBorder;
			internal_DrawTextureArguments.topBorder = topBorder;
			internal_DrawTextureArguments.bottomBorder = bottomBorder;
			internal_DrawTextureArguments.color = color;
			internal_DrawTextureArguments.pass = pass;
			internal_DrawTextureArguments.texture = texture;
			internal_DrawTextureArguments.mat = mat;
			Graphics.Internal_DrawTexture(ref internal_DrawTextureArguments);
		}

		/// <summary>
		///   <para>Draw a texture in screen coordinates.</para>
		/// </summary>
		/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
		/// <param name="texture">Texture to draw.</param>
		/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
		/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
		/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
		/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
		/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
		/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
		/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
		}

		/// <summary>
		///   <para>Draw a texture in screen coordinates.</para>
		/// </summary>
		/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
		/// <param name="texture">Texture to draw.</param>
		/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
		/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
		/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
		/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
		/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
		/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
		/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
		{
			Color32 color = new Color32(128, 128, 128, 128);
			Graphics.DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
		}

		/// <summary>
		///   <para>Draw a texture in screen coordinates.</para>
		/// </summary>
		/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
		/// <param name="texture">Texture to draw.</param>
		/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
		/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
		/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
		/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
		/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
		/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
		/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.DrawTexture(screenRect, texture, new Rect(0f, 0f, 1f, 1f), leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
		}

		/// <summary>
		///   <para>Draw a texture in screen coordinates.</para>
		/// </summary>
		/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
		/// <param name="texture">Texture to draw.</param>
		/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
		/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
		/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
		/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
		/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
		/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
		/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		public static void DrawTexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.DrawTexture(screenRect, texture, 0, 0, 0, 0, mat, pass);
		}

		/// <summary>
		///   <para>Draw a mesh immediately.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="position">Position of the mesh.</param>
		/// <param name="rotation">Rotation of the mesh.</param>
		/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
		/// <param name="materialIndex">Subset of the mesh to draw.</param>
		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			Graphics.Internal_DrawMeshNow1(mesh, materialIndex, position, rotation);
		}

		/// <summary>
		///   <para>Draw a mesh immediately.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="position">Position of the mesh.</param>
		/// <param name="rotation">Rotation of the mesh.</param>
		/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
		/// <param name="materialIndex">Subset of the mesh to draw.</param>
		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix, int materialIndex)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			Graphics.Internal_DrawMeshNow2(mesh, materialIndex, matrix);
		}

		/// <summary>
		///   <para>Draw a mesh immediately.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="position">Position of the mesh.</param>
		/// <param name="rotation">Rotation of the mesh.</param>
		/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
		/// <param name="materialIndex">Subset of the mesh to draw.</param>
		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation)
		{
			Graphics.DrawMeshNow(mesh, position, rotation, -1);
		}

		/// <summary>
		///   <para>Draw a mesh immediately.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="position">Position of the mesh.</param>
		/// <param name="rotation">Rotation of the mesh.</param>
		/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
		/// <param name="materialIndex">Subset of the mesh to draw.</param>
		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix)
		{
			Graphics.DrawMeshNow(mesh, matrix, -1);
		}

		/// <summary>
		///   <para>Draw a mesh.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="position">Position of the mesh.</param>
		/// <param name="rotation">Rotation of the mesh.</param>
		/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
		/// <param name="material">Material to use.</param>
		/// <param name="layer"> to use.</param>
		/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
		/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
		/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
		/// <param name="castShadows">Should the mesh cast shadows?</param>
		/// <param name="receiveShadows">Should the mesh receive shadows?</param>
		/// <param name="useLightProbes">Should the mesh use light probes?</param>
		/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
		/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
		/// <param name="lightProbeProxyVolume"></param>
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, (!useLightProbes) ? LightProbeUsage.Off : LightProbeUsage.BlendProbes, null);
		}

		/// <summary>
		///   <para>Draw a mesh.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="position">Position of the mesh.</param>
		/// <param name="rotation">Rotation of the mesh.</param>
		/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
		/// <param name="material">Material to use.</param>
		/// <param name="layer"> to use.</param>
		/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
		/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
		/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
		/// <param name="castShadows">Should the mesh cast shadows?</param>
		/// <param name="receiveShadows">Should the mesh receive shadows?</param>
		/// <param name="useLightProbes">Should the mesh use light probes?</param>
		/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
		/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
		/// <param name="lightProbeProxyVolume"></param>
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor, [DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, (!useLightProbes) ? LightProbeUsage.Off : LightProbeUsage.BlendProbes, null);
		}

		/// <summary>
		///   <para>Draw a mesh.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="position">Position of the mesh.</param>
		/// <param name="rotation">Rotation of the mesh.</param>
		/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
		/// <param name="material">Material to use.</param>
		/// <param name="layer"> to use.</param>
		/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
		/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
		/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
		/// <param name="castShadows">Should the mesh cast shadows?</param>
		/// <param name="receiveShadows">Should the mesh receive shadows?</param>
		/// <param name="useLightProbes">Should the mesh use light probes?</param>
		/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
		/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
		/// <param name="lightProbeProxyVolume"></param>
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, (!useLightProbes) ? LightProbeUsage.Off : LightProbeUsage.BlendProbes, null);
		}

		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
		{
			if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
			{
				throw new ArgumentException("Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.", "lightProbeProxyVolume");
			}
			Graphics.Internal_DrawMesh(mesh, submeshIndex, matrix, material, layer, camera, properties, castShadows, receiveShadows, probeAnchor, lightProbeUsage, lightProbeProxyVolume);
		}

		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, [DefaultValue("matrices.Length")] int count, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
		{
			if (!SystemInfo.supportsInstancing)
			{
				throw new InvalidOperationException("Instancing is not supported.");
			}
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
			{
				throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
			}
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			if (!material.enableInstancing)
			{
				throw new InvalidOperationException("Material needs to enable instancing for use with DrawMeshInstanced.");
			}
			if (matrices == null)
			{
				throw new ArgumentNullException("matrices");
			}
			if (count < 0 || count > Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length))
			{
				throw new ArgumentOutOfRangeException("count", string.Format("Count must be in the range of 0 to {0}.", Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length)));
			}
			if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
			{
				throw new ArgumentException("Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.", "lightProbeProxyVolume");
			}
			if (count > 0)
			{
				Graphics.Internal_DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
			}
		}

		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
		{
			if (matrices == null)
			{
				throw new ArgumentNullException("matrices");
			}
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, NoAllocHelpers.ExtractArrayFromListT<Matrix4x4>(matrices), matrices.Count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
		}

		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
		{
			if (!SystemInfo.supportsInstancing)
			{
				throw new InvalidOperationException("Instancing is not supported.");
			}
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
			{
				throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
			}
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			if (bufferWithArgs == null)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
			{
				throw new ArgumentException("Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.", "lightProbeProxyVolume");
			}
			Graphics.Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
		}

		/// <summary>
		///   <para>Draws a fully procedural geometry on the GPU.</para>
		/// </summary>
		/// <param name="topology"></param>
		/// <param name="vertexCount"></param>
		/// <param name="instanceCount"></param>
		public static void DrawProcedural(MeshTopology topology, int vertexCount, [DefaultValue("1")] int instanceCount)
		{
			Graphics.Internal_DrawProcedural(topology, vertexCount, instanceCount);
		}

		/// <summary>
		///   <para>Draws a fully procedural geometry on the GPU.</para>
		/// </summary>
		/// <param name="topology">Topology of the procedural geometry.</param>
		/// <param name="bufferWithArgs">Buffer with draw arguments.</param>
		/// <param name="argsOffset">Byte offset where in the buffer the draw arguments are.</param>
		public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset)
		{
			if (bufferWithArgs == null)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			Graphics.Internal_DrawProceduralIndirect(topology, bufferWithArgs, argsOffset);
		}

		/// <summary>
		///   <para>Copies source texture into destination render texture with a shader.</para>
		/// </summary>
		/// <param name="source">Source texture.</param>
		/// <param name="dest">The destination RenderTexture. Set this to null to blit directly to screen. See description for more information.</param>
		/// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		public static void Blit(Texture source, RenderTexture dest)
		{
			Graphics.Blit2(source, dest);
		}

		/// <summary>
		///   <para>Copies source texture into destination render texture with a shader.</para>
		/// </summary>
		/// <param name="source">Source texture.</param>
		/// <param name="dest">The destination RenderTexture. Set this to null to blit directly to screen. See description for more information.</param>
		/// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		public static void Blit(Texture source, RenderTexture dest, Vector2 scale, Vector2 offset)
		{
			Graphics.Blit4(source, dest, scale, offset);
		}

		/// <summary>
		///   <para>Copies source texture into destination render texture with a shader.</para>
		/// </summary>
		/// <param name="source">Source texture.</param>
		/// <param name="dest">The destination RenderTexture. Set this to null to blit directly to screen. See description for more information.</param>
		/// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		public static void Blit(Texture source, RenderTexture dest, Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.Internal_BlitMaterial(source, dest, mat, pass, true);
		}

		public static void Blit(Texture source, RenderTexture dest, Material mat)
		{
			Graphics.Blit(source, dest, mat, -1);
		}

		/// <summary>
		///   <para>Copies source texture into destination render texture with a shader.</para>
		/// </summary>
		/// <param name="source">Source texture.</param>
		/// <param name="dest">The destination RenderTexture. Set this to null to blit directly to screen. See description for more information.</param>
		/// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		public static void Blit(Texture source, Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.Internal_BlitMaterial(source, null, mat, pass, false);
		}

		public static void Blit(Texture source, Material mat)
		{
			Graphics.Blit(source, mat, -1);
		}

		/// <summary>
		///   <para>Copies source texture into destination, for multi-tap shader.</para>
		/// </summary>
		/// <param name="source">Source texture.</param>
		/// <param name="dest">Destination RenderTexture, or null to blit directly to screen.</param>
		/// <param name="mat">Material to use for copying. Material's shader should do some post-processing effect.</param>
		/// <param name="offsets">Variable number of filtering offsets. Offsets are given in pixels.</param>
		public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, params Vector2[] offsets)
		{
			if (offsets.Length == 0)
			{
				throw new ArgumentException("empty offsets list passed.", "offsets");
			}
			Graphics.Internal_BlitMultiTap(source, dest, mat, offsets);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, null, 0, null, ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, 0, null, ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, null, ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, null, 0, null, ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, 0, null, ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, null, ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, true, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, LightProbeUsage.BlendProbes, null);
		}

		/// <summary>
		///   <para>Draw a mesh.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="position">Position of the mesh.</param>
		/// <param name="rotation">Rotation of the mesh.</param>
		/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
		/// <param name="material">Material to use.</param>
		/// <param name="layer"> to use.</param>
		/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
		/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
		/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
		/// <param name="castShadows">Should the mesh cast shadows?</param>
		/// <param name="receiveShadows">Should the mesh receive shadows?</param>
		/// <param name="useLightProbes">Should the mesh use light probes?</param>
		/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
		/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
		/// <param name="lightProbeProxyVolume"></param>
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor, [DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, (!useLightProbes) ? LightProbeUsage.Off : LightProbeUsage.BlendProbes, null);
		}

		/// <summary>
		///   <para>Draw a mesh.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="position">Position of the mesh.</param>
		/// <param name="rotation">Rotation of the mesh.</param>
		/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
		/// <param name="material">Material to use.</param>
		/// <param name="layer"> to use.</param>
		/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
		/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
		/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
		/// <param name="castShadows">Should the mesh cast shadows?</param>
		/// <param name="receiveShadows">Should the mesh receive shadows?</param>
		/// <param name="useLightProbes">Should the mesh use light probes?</param>
		/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
		/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
		/// <param name="lightProbeProxyVolume"></param>
		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage)
		{
			Graphics.Internal_DrawMesh(mesh, submeshIndex, matrix, material, layer, camera, properties, castShadows, receiveShadows, probeAnchor, lightProbeUsage, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, matrices.Length, null, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, null, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, LightProbeUsage.BlendProbes, null);
		}

		/// <summary>
		///   <para>Draw the same mesh multiple times using GPU instancing.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
		/// <param name="material">Material to use.</param>
		/// <param name="matrices">The array of object transformation matrices.</param>
		/// <param name="count">The number of instances to be drawn.</param>
		/// <param name="properties">Additional material properties to apply. See MaterialPropertyBlock.</param>
		/// <param name="castShadows">Should the meshes cast shadows?</param>
		/// <param name="receiveShadows">Should the meshes receive shadows?</param>
		/// <param name="layer"> to use.</param>
		/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be drawn in the given camera only.</param>
		/// <param name="lightProbeUsage">LightProbeUsage for the instances.</param>
		/// <param name="lightProbeProxyVolume"></param>
		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, null, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage)
		{
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs)
		{
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, 0, null, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, null, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, true, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, 0, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
		{
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, null, LightProbeUsage.BlendProbes, null);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
		{
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, LightProbeUsage.BlendProbes, null);
		}

		/// <summary>
		///   <para>Draw the same mesh multiple times using GPU instancing.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
		/// <param name="material">Material to use.</param>
		/// <param name="bounds">The bounding volume surrounding the instances you intend to draw.</param>
		/// <param name="bufferWithArgs">The GPU buffer containing the arguments for how many instances of this mesh to draw.</param>
		/// <param name="argsOffset">The byte offset into the buffer, where the draw arguments start.</param>
		/// <param name="properties">Additional material properties to apply. See MaterialPropertyBlock.</param>
		/// <param name="castShadows">Should the mesh cast shadows?</param>
		/// <param name="receiveShadows">Should the mesh receive shadows?</param>
		/// <param name="layer"> to use.</param>
		/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be drawn in the given camera only.</param>
		/// <param name="lightProbeUsage">LightProbeUsage for the instances.</param>
		/// <param name="lightProbeProxyVolume"></param>
		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage)
		{
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
		}

		/// <summary>
		///   <para>Draw a texture in screen coordinates.</para>
		/// </summary>
		/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
		/// <param name="texture">Texture to draw.</param>
		/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
		/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
		/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
		/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
		/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
		/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
		/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat)
		{
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, -1);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color)
		{
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, null, -1);
		}

		/// <summary>
		///   <para>Draw a texture in screen coordinates.</para>
		/// </summary>
		/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
		/// <param name="texture">Texture to draw.</param>
		/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
		/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
		/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
		/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
		/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
		/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
		/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
		{
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat, -1);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, null, -1);
		}

		/// <summary>
		///   <para>Draw a texture in screen coordinates.</para>
		/// </summary>
		/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
		/// <param name="texture">Texture to draw.</param>
		/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
		/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
		/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
		/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
		/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
		/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
		/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
		{
			Graphics.DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat, -1);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			Graphics.DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, null, -1);
		}

		/// <summary>
		///   <para>Draw a texture in screen coordinates.</para>
		/// </summary>
		/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
		/// <param name="texture">Texture to draw.</param>
		/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
		/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
		/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
		/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
		/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
		/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
		/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
		/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Material mat)
		{
			Graphics.DrawTexture(screenRect, texture, mat, -1);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture)
		{
			Graphics.DrawTexture(screenRect, texture, null, -1);
		}

		[ExcludeFromDocs]
		public static void DrawProcedural(MeshTopology topology, int vertexCount)
		{
			Graphics.DrawProcedural(topology, vertexCount, 1);
		}

		[ExcludeFromDocs]
		public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			Graphics.DrawProceduralIndirect(topology, bufferWithArgs, 0);
		}

		[ExcludeFromDocs]
		public static void SetRenderTarget(RenderTexture rt)
		{
			Graphics.SetRenderTarget(rt, 0, CubemapFace.Unknown, 0);
		}

		[ExcludeFromDocs]
		public static void SetRenderTarget(RenderTexture rt, int mipLevel)
		{
			Graphics.SetRenderTarget(rt, mipLevel, CubemapFace.Unknown, 0);
		}

		[ExcludeFromDocs]
		public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face)
		{
			Graphics.SetRenderTarget(rt, mipLevel, face, 0);
		}

		[ExcludeFromDocs]
		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
		{
			Graphics.SetRenderTarget(colorBuffer, depthBuffer, 0, CubemapFace.Unknown, 0);
		}

		[ExcludeFromDocs]
		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel)
		{
			Graphics.SetRenderTarget(colorBuffer, depthBuffer, mipLevel, CubemapFace.Unknown, 0);
		}

		[ExcludeFromDocs]
		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face)
		{
			Graphics.SetRenderTarget(colorBuffer, depthBuffer, mipLevel, face, 0);
		}

		[ExcludeFromDocs]
		public static void SetRandomWriteTarget(int index, ComputeBuffer uav)
		{
			Graphics.SetRandomWriteTarget(index, uav, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveColorBuffer_Injected(out RenderBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveDepthBuffer_Injected(out RenderBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRTSimple_Injected(ref RenderBuffer color, ref RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetMRTSimple_Injected(RenderBuffer[] color, ref RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetMRTFullSetup_Injected(RenderBuffer[] color, ref RenderBuffer depth, int mip, CubemapFace face, int depthSlice, RenderBufferLoadAction[] colorLA, RenderBufferStoreAction[] colorSA, RenderBufferLoadAction depthLA, RenderBufferStoreAction depthSA);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshNow1_Injected(Mesh mesh, int subsetIndex, ref Vector3 position, ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshNow2_Injected(Mesh mesh, int subsetIndex, ref Matrix4x4 matrix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMesh_Injected(Mesh mesh, int submeshIndex, ref Matrix4x4 matrix, Material material, int layer, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshInstancedIndirect_Injected(Mesh mesh, int submeshIndex, Material material, ref Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Blit4_Injected(Texture source, RenderTexture dest, ref Vector2 scale, ref Vector2 offset);

		internal static readonly int kMaxDrawMeshInstanceCount = Graphics.Internal_GetMaxDrawMeshInstanceCount();
	}
}
