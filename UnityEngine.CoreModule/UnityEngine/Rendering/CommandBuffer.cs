using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Export/RenderingCommandBuffer.bindings.h")]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[NativeType("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
	[UsedByNativeCode]
	public class CommandBuffer : IDisposable
	{
		public CommandBuffer()
		{
			this.m_Ptr = CommandBuffer.InitBuffer();
		}

		public void ConvertTexture(RenderTargetIdentifier src, RenderTargetIdentifier dst)
		{
			this.ConvertTexture_Internal(src, 0, dst, 0);
		}

		public void ConvertTexture(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement)
		{
			this.ConvertTexture_Internal(src, srcElement, dst, dstElement);
		}

		public void RequestAsyncReadback(ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback)
		{
			this.Internal_RequestAsyncReadback_1(src, callback);
		}

		public void RequestAsyncReadback(ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback)
		{
			this.Internal_RequestAsyncReadback_2(src, size, offset, callback);
		}

		public void RequestAsyncReadback(Texture src, Action<AsyncGPUReadbackRequest> callback)
		{
			this.Internal_RequestAsyncReadback_3(src, callback);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, Action<AsyncGPUReadbackRequest> callback)
		{
			this.Internal_RequestAsyncReadback_4(src, mipIndex, callback);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback)
		{
			this.Internal_RequestAsyncReadback_5(src, mipIndex, dstFormat, callback);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, Action<AsyncGPUReadbackRequest> callback)
		{
			this.Internal_RequestAsyncReadback_6(src, mipIndex, x, width, y, height, z, depth, callback);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback)
		{
			this.Internal_RequestAsyncReadback_7(src, mipIndex, x, width, y, height, z, depth, dstFormat, callback);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_RequestAsyncReadback_1([NotNull] ComputeBuffer src, [NotNull] Action<AsyncGPUReadbackRequest> callback);

		[NativeMethod("AddRequestAsyncReadback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_RequestAsyncReadback_2([NotNull] ComputeBuffer src, int size, int offset, [NotNull] Action<AsyncGPUReadbackRequest> callback);

		[NativeMethod("AddRequestAsyncReadback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_RequestAsyncReadback_3([NotNull] Texture src, [NotNull] Action<AsyncGPUReadbackRequest> callback);

		[NativeMethod("AddRequestAsyncReadback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_RequestAsyncReadback_4([NotNull] Texture src, int mipIndex, [NotNull] Action<AsyncGPUReadbackRequest> callback);

		[NativeMethod("AddRequestAsyncReadback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_RequestAsyncReadback_5([NotNull] Texture src, int mipIndex, TextureFormat dstFormat, [NotNull] Action<AsyncGPUReadbackRequest> callback);

		[NativeMethod("AddRequestAsyncReadback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_RequestAsyncReadback_6([NotNull] Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, [NotNull] Action<AsyncGPUReadbackRequest> callback);

		[NativeMethod("AddRequestAsyncReadback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_RequestAsyncReadback_7([NotNull] Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, [NotNull] Action<AsyncGPUReadbackRequest> callback);

		[NativeMethod("AddSetInvertCulling")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetInvertCulling(bool invertCulling);

		private void ConvertTexture_Internal(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement)
		{
			this.ConvertTexture_Internal_Injected(ref src, srcElement, ref dst, dstElement);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::InitBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InitBuffer();

		[FreeFunction("RenderingCommandBuffer_Bindings::CreateGPUFence_Internal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr CreateGPUFence_Internal(SynchronisationStage stage);

		[FreeFunction("RenderingCommandBuffer_Bindings::WaitOnGPUFence_Internal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void WaitOnGPUFence_Internal(IntPtr fencePtr, SynchronisationStage stage);

		[FreeFunction("RenderingCommandBuffer_Bindings::ReleaseBuffer", HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ReleaseBuffer();

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeFloatParam", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeFloatParam([NotNull] ComputeShader computeShader, int nameID, float val);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeIntParam", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeIntParam([NotNull] ComputeShader computeShader, int nameID, int val);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeVectorParam", HasExplicitThis = true)]
		public void SetComputeVectorParam([NotNull] ComputeShader computeShader, int nameID, Vector4 val)
		{
			this.SetComputeVectorParam_Injected(computeShader, nameID, ref val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeVectorArrayParam", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeVectorArrayParam([NotNull] ComputeShader computeShader, int nameID, Vector4[] values);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeMatrixParam", HasExplicitThis = true)]
		public void SetComputeMatrixParam([NotNull] ComputeShader computeShader, int nameID, Matrix4x4 val)
		{
			this.SetComputeMatrixParam_Injected(computeShader, nameID, ref val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeMatrixArrayParam", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeMatrixArrayParam([NotNull] ComputeShader computeShader, int nameID, Matrix4x4[] values);

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetComputeFloats", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetComputeFloats([NotNull] ComputeShader computeShader, int nameID, float[] values);

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetComputeInts", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetComputeInts([NotNull] ComputeShader computeShader, int nameID, int[] values);

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetComputeTextureParam", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetComputeTextureParam([NotNull] ComputeShader computeShader, int kernelIndex, int nameID, ref RenderTargetIdentifier rt, int mipLevel);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeBufferParam", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeBufferParam([NotNull] ComputeShader computeShader, int kernelIndex, int nameID, ComputeBuffer buffer);

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DispatchCompute", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DispatchCompute([NotNull] ComputeShader computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ);

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DispatchComputeIndirect", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DispatchComputeIndirect([NotNull] ComputeShader computeShader, int kernelIndex, ComputeBuffer indirectBuffer, uint argsOffset);

		[NativeMethod("AddGenerateMips")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GenerateMips(RenderTexture rt);

		[NativeMethod("AddResolveAntiAliasedSurface")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_ResolveAntiAliasedSurface(RenderTexture rt, RenderTexture target);

		[NativeMethod("AddCopyCounterValue")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyCounterValue(ComputeBuffer src, ComputeBuffer dst, uint dstOffsetBytes);

		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int sizeInBytes
		{
			[NativeMethod("GetBufferSize")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeMethod("ClearCommands")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawMesh", HasExplicitThis = true)]
		private void Internal_DrawMesh([NotNull] Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties)
		{
			this.Internal_DrawMesh_Injected(mesh, ref matrix, material, submeshIndex, shaderPass, properties);
		}

		[NativeMethod("AddDrawRenderer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawRenderer([NotNull] Renderer renderer, Material material, int submeshIndex, int shaderPass);

		private void Internal_DrawRenderer(Renderer renderer, Material material, int submeshIndex)
		{
			this.Internal_DrawRenderer(renderer, material, submeshIndex, -1);
		}

		private void Internal_DrawRenderer(Renderer renderer, Material material)
		{
			this.Internal_DrawRenderer(renderer, material, 0);
		}

		[NativeMethod("AddDrawProcedural")]
		private void Internal_DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties)
		{
			this.Internal_DrawProcedural_Injected(ref matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawProceduralIndirect", HasExplicitThis = true)]
		private void Internal_DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			this.Internal_DrawProceduralIndirect_Injected(ref matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawMeshInstanced", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties);

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawMeshInstancedIndirect", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetRandomWriteTarget_Texture", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRandomWriteTarget_Texture(int index, ref RenderTargetIdentifier rt);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetRandomWriteTarget_Buffer", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRandomWriteTarget_Buffer(int index, ComputeBuffer uav, bool preserveCounterValue);

		[NativeMethod("AddClearRandomWriteTargets")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearRandomWriteTargets();

		[FreeFunction("RenderingCommandBuffer_Bindings::SetViewport", HasExplicitThis = true)]
		public void SetViewport(Rect pixelRect)
		{
			this.SetViewport_Injected(ref pixelRect);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EnableScissorRect", HasExplicitThis = true)]
		public void EnableScissorRect(Rect scissor)
		{
			this.EnableScissorRect_Injected(ref scissor);
		}

		[NativeMethod("AddDisableScissorRect")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DisableScissorRect();

		[FreeFunction("RenderingCommandBuffer_Bindings::CopyTexture_Internal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CopyTexture_Internal(ref RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, ref RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY, int mode);

		[FreeFunction("RenderingCommandBuffer_Bindings::Blit_Texture", HasExplicitThis = true)]
		private void Blit_Texture(Texture source, ref RenderTargetIdentifier dest, Material mat, int pass, Vector2 scale, Vector2 offset)
		{
			this.Blit_Texture_Injected(source, ref dest, mat, pass, ref scale, ref offset);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Blit_Identifier", HasExplicitThis = true)]
		private void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, Material mat, int pass, Vector2 scale, Vector2 offset)
		{
			this.Blit_Identifier_Injected(ref source, ref dest, mat, pass, ref scale, ref offset);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::GetTemporaryRT", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite, RenderTextureMemoryless memorylessMode, bool useDynamicScale);

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite, RenderTextureMemoryless memorylessMode)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite, memorylessMode, false);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite, RenderTextureMemoryless.None);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, false);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, 1);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, RenderTextureReadWrite.Default);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, RenderTextureFormat.Default);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, FilterMode.Point);
		}

		public void GetTemporaryRT(int nameID, int width, int height)
		{
			this.GetTemporaryRT(nameID, width, height, 0);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::GetTemporaryRTWithDescriptor", HasExplicitThis = true)]
		private void GetTemporaryRTWithDescriptor(int nameID, RenderTextureDescriptor desc, FilterMode filter)
		{
			this.GetTemporaryRTWithDescriptor_Injected(nameID, ref desc, filter);
		}

		public void GetTemporaryRT(int nameID, RenderTextureDescriptor desc, FilterMode filter)
		{
			this.GetTemporaryRTWithDescriptor(nameID, desc, filter);
		}

		public void GetTemporaryRT(int nameID, RenderTextureDescriptor desc)
		{
			this.GetTemporaryRT(nameID, desc, FilterMode.Point);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::GetTemporaryRTArray", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite, bool useDynamicScale);

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite, false);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, readWrite, antiAliasing, false);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, readWrite, 1);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, RenderTextureReadWrite.Default);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, RenderTextureFormat.Default);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, FilterMode.Point);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, 0);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::ReleaseTemporaryRT", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ReleaseTemporaryRT(int nameID);

		[FreeFunction("RenderingCommandBuffer_Bindings::ClearRenderTarget", HasExplicitThis = true)]
		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor, float depth)
		{
			this.ClearRenderTarget_Injected(clearDepth, clearColor, ref backgroundColor, depth);
		}

		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor)
		{
			this.ClearRenderTarget(clearDepth, clearColor, backgroundColor, 1f);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalFloat", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalFloat(int nameID, float value);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalInt", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalInt(int nameID, int value);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalVector", HasExplicitThis = true)]
		public void SetGlobalVector(int nameID, Vector4 value)
		{
			this.SetGlobalVector_Injected(nameID, ref value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalColor", HasExplicitThis = true)]
		public void SetGlobalColor(int nameID, Color value)
		{
			this.SetGlobalColor_Injected(nameID, ref value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalMatrix", HasExplicitThis = true)]
		public void SetGlobalMatrix(int nameID, Matrix4x4 value)
		{
			this.SetGlobalMatrix_Injected(nameID, ref value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EnableShaderKeyword", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void EnableShaderKeyword(string keyword);

		[FreeFunction("RenderingCommandBuffer_Bindings::DisableShaderKeyword", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DisableShaderKeyword(string keyword);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetViewMatrix", HasExplicitThis = true)]
		public void SetViewMatrix(Matrix4x4 view)
		{
			this.SetViewMatrix_Injected(ref view);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetProjectionMatrix", HasExplicitThis = true)]
		public void SetProjectionMatrix(Matrix4x4 proj)
		{
			this.SetProjectionMatrix_Injected(ref proj);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetViewProjectionMatrices", HasExplicitThis = true)]
		public void SetViewProjectionMatrices(Matrix4x4 view, Matrix4x4 proj)
		{
			this.SetViewProjectionMatrices_Injected(ref view, ref proj);
		}

		[NativeMethod("AddSetGlobalDepthBias")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalDepthBias(float bias, float slopeBias);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalFloatArrayListImpl", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalFloatArrayListImpl(int nameID, object values);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalVectorArrayListImpl", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalVectorArrayListImpl(int nameID, object values);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalMatrixArrayListImpl", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalMatrixArrayListImpl(int nameID, object values);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalFloatArray", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalFloatArray(int nameID, float[] values);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalVectorArray", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalVectorArray(int nameID, Vector4[] values);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalMatrixArray", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalMatrixArray(int nameID, Matrix4x4[] values);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalTexture_Impl", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalTexture_Impl(int nameID, ref RenderTargetIdentifier rt);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalBuffer", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalBuffer(int nameID, ComputeBuffer value);

		[FreeFunction("RenderingCommandBuffer_Bindings::SetShadowSamplingMode_Impl", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetShadowSamplingMode_Impl(ref RenderTargetIdentifier shadowmap, ShadowSamplingMode mode);

		[FreeFunction("RenderingCommandBuffer_Bindings::IssuePluginEventInternal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void IssuePluginEventInternal(IntPtr callback, int eventID);

		[FreeFunction("RenderingCommandBuffer_Bindings::BeginSample", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BeginSample(string name);

		[FreeFunction("RenderingCommandBuffer_Bindings::EndSample", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void EndSample(string name);

		[FreeFunction("RenderingCommandBuffer_Bindings::IssuePluginEventAndDataInternal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void IssuePluginEventAndDataInternal(IntPtr callback, int eventID, IntPtr data);

		[FreeFunction("RenderingCommandBuffer_Bindings::IssuePluginCustomBlitInternal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void IssuePluginCustomBlitInternal(IntPtr callback, uint command, ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, uint commandParam, uint commandFlags);

		[FreeFunction("RenderingCommandBuffer_Bindings::IssuePluginCustomTextureUpdateInternal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void IssuePluginCustomTextureUpdateInternal(IntPtr callback, Texture targetTexture, uint userData, bool useNewUnityRenderingExtTextureUpdateParamsV2);

		public void SetRenderTarget(RenderTargetIdentifier rt)
		{
			this.SetRenderTargetSingle_Internal(rt, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
		{
			if (loadAction == RenderBufferLoadAction.Clear)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			this.SetRenderTargetSingle_Internal(rt, loadAction, storeAction, loadAction, storeAction);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			if (colorLoadAction == RenderBufferLoadAction.Clear || depthLoadAction == RenderBufferLoadAction.Clear)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			this.SetRenderTargetSingle_Internal(rt, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel)
		{
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetSingle_Internal(new RenderTargetIdentifier(rt, mipLevel, CubemapFace.Unknown, 0), RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace)
		{
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetSingle_Internal(new RenderTargetIdentifier(rt, mipLevel, cubemapFace, 0), RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			if (depthSlice < -1)
			{
				throw new ArgumentException(string.Format("Invalid value for depthSlice ({0})", depthSlice));
			}
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetSingle_Internal(new RenderTargetIdentifier(rt, mipLevel, cubemapFace, depthSlice), RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth)
		{
			this.SetRenderTargetColorDepth_Internal(color, depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel)
		{
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetColorDepth_Internal(new RenderTargetIdentifier(color, mipLevel, CubemapFace.Unknown, 0), depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace)
		{
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetColorDepth_Internal(new RenderTargetIdentifier(color, mipLevel, cubemapFace, 0), depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			if (depthSlice < -1)
			{
				throw new ArgumentException(string.Format("Invalid value for depthSlice ({0})", depthSlice));
			}
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetColorDepth_Internal(new RenderTargetIdentifier(color, mipLevel, cubemapFace, depthSlice), depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			if (colorLoadAction == RenderBufferLoadAction.Clear || depthLoadAction == RenderBufferLoadAction.Clear)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			this.SetRenderTargetColorDepth_Internal(color, depth, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
		}

		public void SetRenderTarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth)
		{
			if (colors.Length < 1)
			{
				throw new ArgumentException(string.Format("colors.Length must be at least 1, but was", colors.Length));
			}
			if (colors.Length > SystemInfo.supportedRenderTargetCount)
			{
				throw new ArgumentException(string.Format("colors.Length is {0} and exceeds the maximum number of supported render targets ({1})", colors.Length, SystemInfo.supportedRenderTargetCount));
			}
			this.SetRenderTargetMulti_Internal(colors, depth, null, null, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetBinding binding)
		{
			if (binding.colorRenderTargets.Length < 1)
			{
				throw new ArgumentException(string.Format("The number of color render targets must be at least 1, but was {0}", binding.colorRenderTargets.Length));
			}
			if (binding.colorRenderTargets.Length > SystemInfo.supportedRenderTargetCount)
			{
				throw new ArgumentException(string.Format("The number of color render targets ({0}) and exceeds the maximum supported number of render targets ({1})", binding.colorRenderTargets.Length, SystemInfo.supportedRenderTargetCount));
			}
			if (binding.colorLoadActions.Length != binding.colorRenderTargets.Length)
			{
				throw new ArgumentException(string.Format("The number of color load actions provided ({0}) does not match the number of color render targets ({1})", binding.colorLoadActions.Length, binding.colorRenderTargets.Length));
			}
			if (binding.colorStoreActions.Length != binding.colorRenderTargets.Length)
			{
				throw new ArgumentException(string.Format("The number of color store actions provided ({0}) does not match the number of color render targets ({1})", binding.colorLoadActions.Length, binding.colorRenderTargets.Length));
			}
			if (binding.depthLoadAction == RenderBufferLoadAction.Clear || Array.IndexOf<RenderBufferLoadAction>(binding.colorLoadActions, RenderBufferLoadAction.Clear) > -1)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			if (binding.colorRenderTargets.Length == 1)
			{
				this.SetRenderTargetColorDepth_Internal(binding.colorRenderTargets[0], binding.depthRenderTarget, binding.colorLoadActions[0], binding.colorStoreActions[0], binding.depthLoadAction, binding.depthStoreAction);
			}
			else
			{
				this.SetRenderTargetMulti_Internal(binding.colorRenderTargets, binding.depthRenderTarget, binding.colorLoadActions, binding.colorStoreActions, binding.depthLoadAction, binding.depthStoreAction);
			}
		}

		private void SetRenderTargetSingle_Internal(RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			this.SetRenderTargetSingle_Internal_Injected(ref rt, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
		}

		private void SetRenderTargetColorDepth_Internal(RenderTargetIdentifier color, RenderTargetIdentifier depth, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			this.SetRenderTargetColorDepth_Internal_Injected(ref color, ref depth, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
		}

		private void SetRenderTargetMulti_Internal(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth, RenderBufferLoadAction[] colorLoadActions, RenderBufferStoreAction[] colorStoreActions, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			this.SetRenderTargetMulti_Internal_Injected(colors, ref depth, colorLoadActions, colorStoreActions, depthLoadAction, depthStoreAction);
		}

		~CommandBuffer()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			this.ReleaseBuffer();
			this.m_Ptr = IntPtr.Zero;
		}

		public void Release()
		{
			this.Dispose();
		}

		public GPUFence CreateGPUFence(SynchronisationStage stage)
		{
			GPUFence gpufence = default(GPUFence);
			gpufence.m_Ptr = this.CreateGPUFence_Internal(stage);
			gpufence.InitPostAllocation();
			gpufence.Validate();
			return gpufence;
		}

		public GPUFence CreateGPUFence()
		{
			return this.CreateGPUFence(SynchronisationStage.PixelProcessing);
		}

		public void WaitOnGPUFence(GPUFence fence, SynchronisationStage stage)
		{
			fence.Validate();
			if (fence.IsFencePending())
			{
				this.WaitOnGPUFence_Internal(fence.m_Ptr, stage);
			}
		}

		public void WaitOnGPUFence(GPUFence fence)
		{
			this.WaitOnGPUFence(fence, SynchronisationStage.VertexProcessing);
		}

		public void SetComputeFloatParam(ComputeShader computeShader, string name, float val)
		{
			this.SetComputeFloatParam(computeShader, Shader.PropertyToID(name), val);
		}

		public void SetComputeIntParam(ComputeShader computeShader, string name, int val)
		{
			this.SetComputeIntParam(computeShader, Shader.PropertyToID(name), val);
		}

		public void SetComputeVectorParam(ComputeShader computeShader, string name, Vector4 val)
		{
			this.SetComputeVectorParam(computeShader, Shader.PropertyToID(name), val);
		}

		public void SetComputeVectorArrayParam(ComputeShader computeShader, string name, Vector4[] values)
		{
			this.SetComputeVectorArrayParam(computeShader, Shader.PropertyToID(name), values);
		}

		public void SetComputeMatrixParam(ComputeShader computeShader, string name, Matrix4x4 val)
		{
			this.SetComputeMatrixParam(computeShader, Shader.PropertyToID(name), val);
		}

		public void SetComputeMatrixArrayParam(ComputeShader computeShader, string name, Matrix4x4[] values)
		{
			this.SetComputeMatrixArrayParam(computeShader, Shader.PropertyToID(name), values);
		}

		public void SetComputeFloatParams(ComputeShader computeShader, string name, params float[] values)
		{
			this.Internal_SetComputeFloats(computeShader, Shader.PropertyToID(name), values);
		}

		public void SetComputeFloatParams(ComputeShader computeShader, int nameID, params float[] values)
		{
			this.Internal_SetComputeFloats(computeShader, nameID, values);
		}

		public void SetComputeIntParams(ComputeShader computeShader, string name, params int[] values)
		{
			this.Internal_SetComputeInts(computeShader, Shader.PropertyToID(name), values);
		}

		public void SetComputeIntParams(ComputeShader computeShader, int nameID, params int[] values)
		{
			this.Internal_SetComputeInts(computeShader, nameID, values);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, Shader.PropertyToID(name), ref rt, 0);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, RenderTargetIdentifier rt)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, nameID, ref rt, 0);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt, int mipLevel)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, Shader.PropertyToID(name), ref rt, mipLevel);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, RenderTargetIdentifier rt, int mipLevel)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, nameID, ref rt, mipLevel);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, string name, ComputeBuffer buffer)
		{
			this.SetComputeBufferParam(computeShader, kernelIndex, Shader.PropertyToID(name), buffer);
		}

		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
		{
			this.Internal_DispatchCompute(computeShader, kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
		}

		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, ComputeBuffer indirectBuffer, uint argsOffset)
		{
			this.Internal_DispatchComputeIndirect(computeShader, kernelIndex, indirectBuffer, argsOffset);
		}

		public void GenerateMips(RenderTexture rt)
		{
			if (rt == null)
			{
				throw new ArgumentNullException("rt");
			}
			this.Internal_GenerateMips(rt);
		}

		public void ResolveAntiAliasedSurface(RenderTexture rt, RenderTexture target = null)
		{
			if (rt == null)
			{
				throw new ArgumentNullException("rt");
			}
			this.Internal_ResolveAntiAliasedSurface(rt, target);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
			{
				submeshIndex = Mathf.Clamp(submeshIndex, 0, mesh.subMeshCount - 1);
				Debug.LogWarning(string.Format("submeshIndex out of range. Clampped to {0}.", submeshIndex));
			}
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			this.Internal_DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, properties);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass)
		{
			this.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, null);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex)
		{
			this.DrawMesh(mesh, matrix, material, submeshIndex, -1);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material)
		{
			this.DrawMesh(mesh, matrix, material, 0);
		}

		public void DrawRenderer(Renderer renderer, Material material, int submeshIndex, int shaderPass)
		{
			if (renderer == null)
			{
				throw new ArgumentNullException("renderer");
			}
			if (submeshIndex < 0)
			{
				submeshIndex = Mathf.Max(submeshIndex, 0);
				Debug.LogWarning(string.Format("submeshIndex out of range. Clampped to {0}.", submeshIndex));
			}
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			this.Internal_DrawRenderer(renderer, material, submeshIndex, shaderPass);
		}

		public void DrawRenderer(Renderer renderer, Material material, int submeshIndex)
		{
			this.DrawRenderer(renderer, material, submeshIndex, -1);
		}

		public void DrawRenderer(Renderer renderer, Material material)
		{
			this.DrawRenderer(renderer, material, 0);
		}

		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties)
		{
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			this.Internal_DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
		}

		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount)
		{
			this.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, null);
		}

		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount)
		{
			this.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, 1);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			if (bufferWithArgs == null)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			this.Internal_DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, null);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, 0);
		}

		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties)
		{
			if (!SystemInfo.supportsInstancing)
			{
				throw new InvalidOperationException("DrawMeshInstanced is not supported.");
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
			if (matrices == null)
			{
				throw new ArgumentNullException("matrices");
			}
			if (count < 0 || count > Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length))
			{
				throw new ArgumentOutOfRangeException("count", string.Format("Count must be in the range of 0 to {0}.", Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length)));
			}
			if (count > 0)
			{
				this.Internal_DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, count, properties);
			}
		}

		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count)
		{
			this.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, count, null);
		}

		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices)
		{
			this.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, matrices.Length);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
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
			this.Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, properties);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, null);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs)
		{
			this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, 0, null);
		}

		public void SetRandomWriteTarget(int index, RenderTargetIdentifier rt)
		{
			this.SetRandomWriteTarget_Texture(index, ref rt);
		}

		public void SetRandomWriteTarget(int index, ComputeBuffer buffer, bool preserveCounterValue)
		{
			this.SetRandomWriteTarget_Buffer(index, buffer, preserveCounterValue);
		}

		public void SetRandomWriteTarget(int index, ComputeBuffer buffer)
		{
			this.SetRandomWriteTarget(index, buffer, false);
		}

		public void CopyTexture(RenderTargetIdentifier src, RenderTargetIdentifier dst)
		{
			this.CopyTexture_Internal(ref src, -1, -1, -1, -1, -1, -1, ref dst, -1, -1, -1, -1, 1);
		}

		public void CopyTexture(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement)
		{
			this.CopyTexture_Internal(ref src, srcElement, -1, -1, -1, -1, -1, ref dst, dstElement, -1, -1, -1, 2);
		}

		public void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, RenderTargetIdentifier dst, int dstElement, int dstMip)
		{
			this.CopyTexture_Internal(ref src, srcElement, srcMip, -1, -1, -1, -1, ref dst, dstElement, dstMip, -1, -1, 3);
		}

		public void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY)
		{
			this.CopyTexture_Internal(ref src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, ref dst, dstElement, dstMip, dstX, dstY, 4);
		}

		public void Blit(Texture source, RenderTargetIdentifier dest)
		{
			this.Blit_Texture(source, ref dest, null, -1, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		public void Blit(Texture source, RenderTargetIdentifier dest, Vector2 scale, Vector2 offset)
		{
			this.Blit_Texture(source, ref dest, null, -1, scale, offset);
		}

		public void Blit(Texture source, RenderTargetIdentifier dest, Material mat)
		{
			this.Blit_Texture(source, ref dest, mat, -1, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		public void Blit(Texture source, RenderTargetIdentifier dest, Material mat, int pass)
		{
			this.Blit_Texture(source, ref dest, mat, pass, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest)
		{
			this.Blit_Identifier(ref source, ref dest, null, -1, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Vector2 scale, Vector2 offset)
		{
			this.Blit_Identifier(ref source, ref dest, null, -1, scale, offset);
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat)
		{
			this.Blit_Identifier(ref source, ref dest, mat, -1, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int pass)
		{
			this.Blit_Identifier(ref source, ref dest, mat, pass, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		public void SetGlobalFloat(string name, float value)
		{
			this.SetGlobalFloat(Shader.PropertyToID(name), value);
		}

		public void SetGlobalInt(string name, int value)
		{
			this.SetGlobalInt(Shader.PropertyToID(name), value);
		}

		public void SetGlobalVector(string name, Vector4 value)
		{
			this.SetGlobalVector(Shader.PropertyToID(name), value);
		}

		public void SetGlobalColor(string name, Color value)
		{
			this.SetGlobalColor(Shader.PropertyToID(name), value);
		}

		public void SetGlobalMatrix(string name, Matrix4x4 value)
		{
			this.SetGlobalMatrix(Shader.PropertyToID(name), value);
		}

		public void SetGlobalFloatArray(string propertyName, List<float> values)
		{
			this.SetGlobalFloatArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalFloatArray(int nameID, List<float> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetGlobalFloatArrayListImpl(nameID, values);
		}

		public void SetGlobalFloatArray(string propertyName, float[] values)
		{
			this.SetGlobalFloatArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalVectorArray(string propertyName, List<Vector4> values)
		{
			this.SetGlobalVectorArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalVectorArray(int nameID, List<Vector4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetGlobalVectorArrayListImpl(nameID, values);
		}

		public void SetGlobalVectorArray(string propertyName, Vector4[] values)
		{
			this.SetGlobalVectorArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalMatrixArray(string propertyName, List<Matrix4x4> values)
		{
			this.SetGlobalMatrixArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetGlobalMatrixArrayListImpl(nameID, values);
		}

		public void SetGlobalMatrixArray(string propertyName, Matrix4x4[] values)
		{
			this.SetGlobalMatrixArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalTexture(string name, RenderTargetIdentifier value)
		{
			this.SetGlobalTexture(Shader.PropertyToID(name), value);
		}

		public void SetGlobalTexture(int nameID, RenderTargetIdentifier value)
		{
			this.SetGlobalTexture_Impl(nameID, ref value);
		}

		public void SetGlobalBuffer(string name, ComputeBuffer value)
		{
			this.SetGlobalBuffer(Shader.PropertyToID(name), value);
		}

		public void SetShadowSamplingMode(RenderTargetIdentifier shadowmap, ShadowSamplingMode mode)
		{
			this.SetShadowSamplingMode_Impl(ref shadowmap, mode);
		}

		public void IssuePluginEvent(IntPtr callback, int eventID)
		{
			if (callback == IntPtr.Zero)
			{
				throw new ArgumentException("Null callback specified.");
			}
			this.IssuePluginEventInternal(callback, eventID);
		}

		public void IssuePluginEventAndData(IntPtr callback, int eventID, IntPtr data)
		{
			if (callback == IntPtr.Zero)
			{
				throw new ArgumentException("Null callback specified.");
			}
			this.IssuePluginEventAndDataInternal(callback, eventID, data);
		}

		public void IssuePluginCustomBlit(IntPtr callback, uint command, RenderTargetIdentifier source, RenderTargetIdentifier dest, uint commandParam, uint commandFlags)
		{
			this.IssuePluginCustomBlitInternal(callback, command, ref source, ref dest, commandParam, commandFlags);
		}

		[Obsolete("Use IssuePluginCustomTextureUpdateV2 to register TextureUpdate callbacks instead. Callbacks will be passed event IDs kUnityRenderingExtEventUpdateTextureBeginV2 or kUnityRenderingExtEventUpdateTextureEndV2, and data parameter of type UnityRenderingExtTextureUpdateParamsV2.", false)]
		public void IssuePluginCustomTextureUpdate(IntPtr callback, Texture targetTexture, uint userData)
		{
			this.IssuePluginCustomTextureUpdateInternal(callback, targetTexture, userData, false);
		}

		[Obsolete("Use IssuePluginCustomTextureUpdateV2 to register TextureUpdate callbacks instead. Callbacks will be passed event IDs kUnityRenderingExtEventUpdateTextureBeginV2 or kUnityRenderingExtEventUpdateTextureEndV2, and data parameter of type UnityRenderingExtTextureUpdateParamsV2.", false)]
		public void IssuePluginCustomTextureUpdateV1(IntPtr callback, Texture targetTexture, uint userData)
		{
			this.IssuePluginCustomTextureUpdateInternal(callback, targetTexture, userData, false);
		}

		public void IssuePluginCustomTextureUpdateV2(IntPtr callback, Texture targetTexture, uint userData)
		{
			this.IssuePluginCustomTextureUpdateInternal(callback, targetTexture, userData, true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ConvertTexture_Internal_Injected(ref RenderTargetIdentifier src, int srcElement, ref RenderTargetIdentifier dst, int dstElement);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetComputeVectorParam_Injected(ComputeShader computeShader, int nameID, ref Vector4 val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetComputeMatrixParam_Injected(ComputeShader computeShader, int nameID, ref Matrix4x4 val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawMesh_Injected(Mesh mesh, ref Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawProcedural_Injected(ref Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawProceduralIndirect_Injected(ref Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetViewport_Injected(ref Rect pixelRect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void EnableScissorRect_Injected(ref Rect scissor);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Blit_Texture_Injected(Texture source, ref RenderTargetIdentifier dest, Material mat, int pass, ref Vector2 scale, ref Vector2 offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Blit_Identifier_Injected(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, Material mat, int pass, ref Vector2 scale, ref Vector2 offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetTemporaryRTWithDescriptor_Injected(int nameID, ref RenderTextureDescriptor desc, FilterMode filter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ClearRenderTarget_Injected(bool clearDepth, bool clearColor, ref Color backgroundColor, float depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalVector_Injected(int nameID, ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalColor_Injected(int nameID, ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalMatrix_Injected(int nameID, ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetViewMatrix_Injected(ref Matrix4x4 view);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetProjectionMatrix_Injected(ref Matrix4x4 proj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetViewProjectionMatrices_Injected(ref Matrix4x4 view, ref Matrix4x4 proj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTargetSingle_Internal_Injected(ref RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTargetColorDepth_Internal_Injected(ref RenderTargetIdentifier color, ref RenderTargetIdentifier depth, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTargetMulti_Internal_Injected(RenderTargetIdentifier[] colors, ref RenderTargetIdentifier depth, RenderBufferLoadAction[] colorLoadActions, RenderBufferStoreAction[] colorStoreActions, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction);

		internal IntPtr m_Ptr;
	}
}
