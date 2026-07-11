using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>List of graphics commands to execute.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[NativeType("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
	public sealed class CommandBuffer : IDisposable
	{
		/// <summary>
		///   <para>Create a new empty command buffer.</para>
		/// </summary>
		public CommandBuffer()
		{
			this.m_Ptr = IntPtr.Zero;
			CommandBuffer.InitBuffer(this);
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitBuffer(CommandBuffer buf);

		private IntPtr CreateGPUFence_Internal(SynchronisationStage stage)
		{
			IntPtr intPtr;
			CommandBuffer.INTERNAL_CALL_CreateGPUFence_Internal(this, stage, out intPtr);
			return intPtr;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateGPUFence_Internal(CommandBuffer self, SynchronisationStage stage, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void WaitOnGPUFence_Internal(IntPtr fencePtr, SynchronisationStage stage);

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ReleaseBuffer();

		public void Release()
		{
			this.Dispose();
		}

		[ExcludeFromDocs]
		public GPUFence CreateGPUFence()
		{
			SynchronisationStage synchronisationStage = SynchronisationStage.PixelProcessing;
			return this.CreateGPUFence(synchronisationStage);
		}

		/// <summary>
		///   <para>Creates a GPUFence which will be passed after the last Blit, Clear, Draw, Dispatch or Texture Copy command prior to this call has been completed on the GPU.</para>
		/// </summary>
		/// <param name="stage">On some platforms there is a significant gap between the vertex processing completing and the pixel processing begining for a given draw call. This parameter allows for the fence to be passed after either the vertex or pixel processing for the proceeding draw has completed. If a compute shader dispatch was the last task submitted then this parameter is ignored.</param>
		/// <returns>
		///   <para>Returns a new GPUFence.</para>
		/// </returns>
		public GPUFence CreateGPUFence([DefaultValue("SynchronisationStage.PixelProcessing")] SynchronisationStage stage)
		{
			GPUFence gpufence = default(GPUFence);
			gpufence.m_Ptr = this.CreateGPUFence_Internal(stage);
			gpufence.InitPostAllocation();
			gpufence.Validate();
			return gpufence;
		}

		[ExcludeFromDocs]
		public void WaitOnGPUFence(GPUFence fence)
		{
			SynchronisationStage synchronisationStage = SynchronisationStage.VertexProcessing;
			this.WaitOnGPUFence(fence, synchronisationStage);
		}

		/// <summary>
		///   <para>Instructs the GPU to wait until the given GPUFence is passed.</para>
		/// </summary>
		/// <param name="fence">The GPUFence that the GPU will be instructed to wait upon.</param>
		/// <param name="stage">On some platforms there is a significant gap between the vertex processing completing and the pixel processing completing for a given draw call. This parameter allows for requested wait to be before the next items vertex or pixel processing begins. Some platforms can not differentiate between the start of vertex and pixel processing, these platforms will wait before the next items vertex processing. If a compute shader dispatch is the next item to be submitted then this parameter is ignored.</param>
		public void WaitOnGPUFence(GPUFence fence, [DefaultValue("SynchronisationStage.VertexProcessing")] SynchronisationStage stage)
		{
			fence.Validate();
			if (fence.IsFencePending())
			{
				this.WaitOnGPUFence_Internal(fence.m_Ptr, stage);
			}
		}

		/// <summary>
		///   <para>Adds a command to set a float parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="val">Value to set.</param>
		public void SetComputeFloatParam(ComputeShader computeShader, string name, float val)
		{
			this.SetComputeFloatParam(computeShader, Shader.PropertyToID(name), val);
		}

		/// <summary>
		///   <para>Adds a command to set a float parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="val">Value to set.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeFloatParam(ComputeShader computeShader, int nameID, float val);

		/// <summary>
		///   <para>Adds a command to set an integer parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="val">Value to set.</param>
		public void SetComputeIntParam(ComputeShader computeShader, string name, int val)
		{
			this.SetComputeIntParam(computeShader, Shader.PropertyToID(name), val);
		}

		/// <summary>
		///   <para>Adds a command to set an integer parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="val">Value to set.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeIntParam(ComputeShader computeShader, int nameID, int val);

		/// <summary>
		///   <para>Adds a command to set a vector parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="val">Value to set.</param>
		public void SetComputeVectorParam(ComputeShader computeShader, string name, Vector4 val)
		{
			this.SetComputeVectorParam(computeShader, Shader.PropertyToID(name), val);
		}

		/// <summary>
		///   <para>Adds a command to set a vector parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="val">Value to set.</param>
		public void SetComputeVectorParam(ComputeShader computeShader, int nameID, Vector4 val)
		{
			CommandBuffer.INTERNAL_CALL_SetComputeVectorParam(this, computeShader, nameID, ref val);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetComputeVectorParam(CommandBuffer self, ComputeShader computeShader, int nameID, ref Vector4 val);

		/// <summary>
		///   <para>Adds a command to set a vector array parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Property name.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="values">Value to set.</param>
		public void SetComputeVectorArrayParam(ComputeShader computeShader, string name, Vector4[] values)
		{
			this.SetComputeVectorArrayParam(computeShader, Shader.PropertyToID(name), values);
		}

		/// <summary>
		///   <para>Adds a command to set a vector array parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Property name.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="values">Value to set.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeVectorArrayParam(ComputeShader computeShader, int nameID, Vector4[] values);

		/// <summary>
		///   <para>Adds a command to set a matrix parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="val">Value to set.</param>
		public void SetComputeMatrixParam(ComputeShader computeShader, string name, Matrix4x4 val)
		{
			this.SetComputeMatrixParam(computeShader, Shader.PropertyToID(name), val);
		}

		/// <summary>
		///   <para>Adds a command to set a matrix parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="val">Value to set.</param>
		public void SetComputeMatrixParam(ComputeShader computeShader, int nameID, Matrix4x4 val)
		{
			CommandBuffer.INTERNAL_CALL_SetComputeMatrixParam(this, computeShader, nameID, ref val);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetComputeMatrixParam(CommandBuffer self, ComputeShader computeShader, int nameID, ref Matrix4x4 val);

		/// <summary>
		///   <para>Adds a command to set a matrix array parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="values">Value to set.</param>
		public void SetComputeMatrixArrayParam(ComputeShader computeShader, string name, Matrix4x4[] values)
		{
			this.SetComputeMatrixArrayParam(computeShader, Shader.PropertyToID(name), values);
		}

		/// <summary>
		///   <para>Adds a command to set a matrix array parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="values">Value to set.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeMatrixArrayParam(ComputeShader computeShader, int nameID, Matrix4x4[] values);

		/// <summary>
		///   <para>Adds a command to set multiple consecutive float parameters on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="values">Values to set.</param>
		public void SetComputeFloatParams(ComputeShader computeShader, string name, params float[] values)
		{
			this.Internal_SetComputeFloats(computeShader, Shader.PropertyToID(name), values);
		}

		/// <summary>
		///   <para>Adds a command to set multiple consecutive float parameters on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="values">Values to set.</param>
		public void SetComputeFloatParams(ComputeShader computeShader, int nameID, params float[] values)
		{
			this.Internal_SetComputeFloats(computeShader, nameID, values);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetComputeFloats(ComputeShader computeShader, int nameID, float[] values);

		/// <summary>
		///   <para>Adds a command to set multiple consecutive integer parameters on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="values">Values to set.</param>
		public void SetComputeIntParams(ComputeShader computeShader, string name, params int[] values)
		{
			this.Internal_SetComputeInts(computeShader, Shader.PropertyToID(name), values);
		}

		/// <summary>
		///   <para>Adds a command to set multiple consecutive integer parameters on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="name">Name of the variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="values">Values to set.</param>
		public void SetComputeIntParams(ComputeShader computeShader, int nameID, params int[] values)
		{
			this.Internal_SetComputeInts(computeShader, nameID, values);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetComputeInts(ComputeShader computeShader, int nameID, int[] values);

		/// <summary>
		///   <para>Adds a command to set a texture parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="kernelIndex">Which kernel the texture is being set for. See ComputeShader.FindKernel.</param>
		/// <param name="name">Name of the texture variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="rt">Texture value or identifier to set, see RenderTargetIdentifier.</param>
		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, Shader.PropertyToID(name), ref rt);
		}

		/// <summary>
		///   <para>Adds a command to set a texture parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="kernelIndex">Which kernel the texture is being set for. See ComputeShader.FindKernel.</param>
		/// <param name="name">Name of the texture variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="rt">Texture value or identifier to set, see RenderTargetIdentifier.</param>
		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, RenderTargetIdentifier rt)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, nameID, ref rt);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, ref RenderTargetIdentifier rt);

		/// <summary>
		///   <para>Adds a command to set an input or output buffer parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="kernelIndex">Which kernel the buffer is being set for. See ComputeShader.FindKernel.</param>
		/// <param name="name">Name of the buffer variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="buffer">Buffer to set.</param>
		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, string name, ComputeBuffer buffer)
		{
			this.SetComputeBufferParam(computeShader, kernelIndex, Shader.PropertyToID(name), buffer);
		}

		/// <summary>
		///   <para>Adds a command to set an input or output buffer parameter on a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to set parameter for.</param>
		/// <param name="kernelIndex">Which kernel the buffer is being set for. See ComputeShader.FindKernel.</param>
		/// <param name="name">Name of the buffer variable in shader code.</param>
		/// <param name="nameID">Property name ID. Use Shader.PropertyToID to get this ID.</param>
		/// <param name="buffer">Buffer to set.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, int nameID, ComputeBuffer buffer);

		/// <summary>
		///   <para>Add a command to execute a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to execute.</param>
		/// <param name="kernelIndex">Kernel index to execute, see ComputeShader.FindKernel.</param>
		/// <param name="threadGroupsX">Number of work groups in the X dimension.</param>
		/// <param name="threadGroupsY">Number of work groups in the Y dimension.</param>
		/// <param name="threadGroupsZ">Number of work groups in the Z dimension.</param>
		/// <param name="indirectBuffer">ComputeBuffer with dispatch arguments.</param>
		/// <param name="argsOffset">Byte offset indicating the location of the dispatch arguments in the buffer.</param>
		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
		{
			this.Internal_DispatchCompute(computeShader, kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DispatchCompute(ComputeShader computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ);

		/// <summary>
		///   <para>Add a command to execute a ComputeShader.</para>
		/// </summary>
		/// <param name="computeShader">ComputeShader to execute.</param>
		/// <param name="kernelIndex">Kernel index to execute, see ComputeShader.FindKernel.</param>
		/// <param name="threadGroupsX">Number of work groups in the X dimension.</param>
		/// <param name="threadGroupsY">Number of work groups in the Y dimension.</param>
		/// <param name="threadGroupsZ">Number of work groups in the Z dimension.</param>
		/// <param name="indirectBuffer">ComputeBuffer with dispatch arguments.</param>
		/// <param name="argsOffset">Byte offset indicating the location of the dispatch arguments in the buffer.</param>
		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, ComputeBuffer indirectBuffer, uint argsOffset)
		{
			this.Internal_DispatchComputeIndirect(computeShader, kernelIndex, indirectBuffer, argsOffset);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DispatchComputeIndirect(ComputeShader computeShader, int kernelIndex, ComputeBuffer indirectBuffer, uint argsOffset);

		/// <summary>
		///   <para>Generate mipmap levels of a render texture.</para>
		/// </summary>
		/// <param name="rt">The render texture requiring mipmaps generation.</param>
		public void GenerateMips(RenderTexture rt)
		{
			if (rt == null)
			{
				throw new ArgumentNullException("rt");
			}
			this.Internal_GenerateMips(rt);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GenerateMips(RenderTexture rt);

		/// <summary>
		///   <para>Adds a command to copy ComputeBuffer counter value.</para>
		/// </summary>
		/// <param name="src">Append/consume buffer to copy the counter from.</param>
		/// <param name="dst">A buffer to copy the counter to.</param>
		/// <param name="dstOffsetBytes">Target byte offset in dst buffer.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyCounterValue(ComputeBuffer src, ComputeBuffer dst, uint dstOffsetBytes);

		/// <summary>
		///   <para>Name of this command buffer.</para>
		/// </summary>
		public extern string name
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Size of this command buffer in bytes (Read Only).</para>
		/// </summary>
		public extern int sizeInBytes
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Clear all commands in the buffer.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();

		[ExcludeFromDocs]
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			this.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, materialPropertyBlock);
		}

		[ExcludeFromDocs]
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = -1;
			this.DrawMesh(mesh, matrix, material, submeshIndex, num, materialPropertyBlock);
		}

		[ExcludeFromDocs]
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = -1;
			int num2 = 0;
			this.DrawMesh(mesh, matrix, material, num2, num, materialPropertyBlock);
		}

		/// <summary>
		///   <para>Add a "draw mesh" command.</para>
		/// </summary>
		/// <param name="mesh">Mesh to draw.</param>
		/// <param name="matrix">Transformation matrix to use.</param>
		/// <param name="material">Material to use.</param>
		/// <param name="submeshIndex">Which subset of the mesh to render.</param>
		/// <param name="shaderPass">Which pass of the shader to use (default is -1, which renders all passes).</param>
		/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass, [DefaultValue("null")] MaterialPropertyBlock properties)
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

		private void Internal_DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties)
		{
			CommandBuffer.INTERNAL_CALL_Internal_DrawMesh(this, mesh, ref matrix, material, submeshIndex, shaderPass, properties);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawMesh(CommandBuffer self, Mesh mesh, ref Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties);

		[ExcludeFromDocs]
		public void DrawRenderer(Renderer renderer, Material material, int submeshIndex)
		{
			int num = -1;
			this.DrawRenderer(renderer, material, submeshIndex, num);
		}

		[ExcludeFromDocs]
		public void DrawRenderer(Renderer renderer, Material material)
		{
			int num = -1;
			int num2 = 0;
			this.DrawRenderer(renderer, material, num2, num);
		}

		/// <summary>
		///   <para>Add a "draw renderer" command.</para>
		/// </summary>
		/// <param name="renderer">Renderer to draw.</param>
		/// <param name="material">Material to use.</param>
		/// <param name="submeshIndex">Which subset of the mesh to render.</param>
		/// <param name="shaderPass">Which pass of the shader to use (default is -1, which renders all passes).</param>
		public void DrawRenderer(Renderer renderer, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass)
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawRenderer(Renderer renderer, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass);

		[ExcludeFromDocs]
		private void Internal_DrawRenderer(Renderer renderer, Material material, int submeshIndex)
		{
			int num = -1;
			this.Internal_DrawRenderer(renderer, material, submeshIndex, num);
		}

		[ExcludeFromDocs]
		private void Internal_DrawRenderer(Renderer renderer, Material material)
		{
			int num = -1;
			int num2 = 0;
			this.Internal_DrawRenderer(renderer, material, num2, num);
		}

		[ExcludeFromDocs]
		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			this.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, materialPropertyBlock);
		}

		[ExcludeFromDocs]
		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = 1;
			this.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, num, materialPropertyBlock);
		}

		/// <summary>
		///   <para>Add a "draw procedural geometry" command.</para>
		/// </summary>
		/// <param name="matrix">Transformation matrix to use.</param>
		/// <param name="material">Material to use.</param>
		/// <param name="shaderPass">Which pass of the shader to use (or -1 for all passes).</param>
		/// <param name="topology">Topology of the procedural geometry.</param>
		/// <param name="vertexCount">Vertex count to render.</param>
		/// <param name="instanceCount">Instance count to render.</param>
		/// <param name="properties">Additional material properties to apply just before rendering. See MaterialPropertyBlock.</param>
		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, [DefaultValue("1")] int instanceCount, [DefaultValue("null")] MaterialPropertyBlock properties)
		{
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			this.Internal_DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
		}

		private void Internal_DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties)
		{
			CommandBuffer.INTERNAL_CALL_Internal_DrawProcedural(this, ref matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawProcedural(CommandBuffer self, ref Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties);

		[ExcludeFromDocs]
		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, materialPropertyBlock);
		}

		[ExcludeFromDocs]
		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = 0;
			this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, num, materialPropertyBlock);
		}

		/// <summary>
		///   <para>Add a "draw procedural geometry" command.</para>
		/// </summary>
		/// <param name="matrix">Transformation matrix to use.</param>
		/// <param name="material">Material to use.</param>
		/// <param name="shaderPass">Which pass of the shader to use (or -1 for all passes).</param>
		/// <param name="topology">Topology of the procedural geometry.</param>
		/// <param name="properties">Additional material properties to apply just before rendering. See MaterialPropertyBlock.</param>
		/// <param name="bufferWithArgs">Buffer with draw arguments.</param>
		/// <param name="argsOffset">Byte offset where in the buffer the draw arguments are.</param>
		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset, [DefaultValue("null")] MaterialPropertyBlock properties)
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

		private void Internal_DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			CommandBuffer.INTERNAL_CALL_Internal_DrawProceduralIndirect(this, ref matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawProceduralIndirect(CommandBuffer self, ref Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties);

		[ExcludeFromDocs]
		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			this.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, count, materialPropertyBlock);
		}

		[ExcludeFromDocs]
		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = matrices.Length;
			this.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, num, materialPropertyBlock);
		}

		/// <summary>
		///   <para>Add a "draw mesh with instancing" command.
		///
		/// The command will not immediately fail and throw an exception if Material.enableInstancing is false, but it will log an error and skips rendering each time the command is being executed if such a condition is detected.
		///
		/// InvalidOperationException will be thrown if the current platform doesn't support this API (i.e. if GPU instancing is not available). See SystemInfo.supportsInstancing.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
		/// <param name="material">Material to use.</param>
		/// <param name="shaderPass">Which pass of the shader to use, or -1 which renders all passes.</param>
		/// <param name="matrices">The array of object transformation matrices.</param>
		/// <param name="count">The number of instances to be drawn.</param>
		/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, [DefaultValue("matrices.Length")] int count, [DefaultValue("null")] MaterialPropertyBlock properties)
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties);

		[ExcludeFromDocs]
		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, materialPropertyBlock);
		}

		[ExcludeFromDocs]
		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs)
		{
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = 0;
			this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, num, materialPropertyBlock);
		}

		/// <summary>
		///   <para>Add a "draw mesh with indirect instancing" command.</para>
		/// </summary>
		/// <param name="mesh">The Mesh to draw.</param>
		/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
		/// <param name="material">Material to use.</param>
		/// <param name="shaderPass">Which pass of the shader to use, or -1 which renders all passes.</param>
		/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
		/// <param name="bufferWithArgs">The GPU buffer containing the arguments for how many instances of this mesh to draw.</param>
		/// <param name="argsOffset">The byte offset into the buffer, where the draw arguments start.</param>
		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset, [DefaultValue("null")] MaterialPropertyBlock properties)
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties);

		/// <summary>
		///   <para>Set random write target for level pixel shaders.</para>
		/// </summary>
		/// <param name="index">Index of the random write target in the shader.</param>
		/// <param name="buffer">ComputeBuffer to set as write targe.</param>
		/// <param name="preserveCounterValue">Whether to leave the append/consume counter value unchanged.</param>
		/// <param name="rt">RenderTargetIdentifier to set as write target.</param>
		public void SetRandomWriteTarget(int index, RenderTargetIdentifier rt)
		{
			this.SetRandomWriteTarget_Texture(index, ref rt);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRandomWriteTarget_Texture(int index, ref RenderTargetIdentifier rt);

		[ExcludeFromDocs]
		public void SetRandomWriteTarget(int index, ComputeBuffer buffer)
		{
			bool flag = false;
			this.SetRandomWriteTarget(index, buffer, flag);
		}

		/// <summary>
		///   <para>Set random write target for level pixel shaders.</para>
		/// </summary>
		/// <param name="index">Index of the random write target in the shader.</param>
		/// <param name="buffer">ComputeBuffer to set as write targe.</param>
		/// <param name="preserveCounterValue">Whether to leave the append/consume counter value unchanged.</param>
		/// <param name="rt">RenderTargetIdentifier to set as write target.</param>
		public void SetRandomWriteTarget(int index, ComputeBuffer buffer, [DefaultValue("false")] bool preserveCounterValue)
		{
			this.SetRandomWriteTarget_Buffer(index, buffer, preserveCounterValue);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRandomWriteTarget_Buffer(int index, ComputeBuffer uav, bool preserveCounterValue);

		/// <summary>
		///   <para>Clear random write targets for level pixel shaders.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearRandomWriteTargets();

		/// <summary>
		///   <para>Adds a command to copy a texture into another texture.</para>
		/// </summary>
		/// <param name="src">Source texture or identifier, see RenderTargetIdentifier.</param>
		/// <param name="dst">Destination texture or identifier, see RenderTargetIdentifier.</param>
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
		public void CopyTexture(RenderTargetIdentifier src, RenderTargetIdentifier dst)
		{
			this.CopyTexture_Internal(ref src, -1, -1, -1, -1, -1, -1, ref dst, -1, -1, -1, -1, 1);
		}

		/// <summary>
		///   <para>Adds a command to copy a texture into another texture.</para>
		/// </summary>
		/// <param name="src">Source texture or identifier, see RenderTargetIdentifier.</param>
		/// <param name="dst">Destination texture or identifier, see RenderTargetIdentifier.</param>
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
		public void CopyTexture(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement)
		{
			this.CopyTexture_Internal(ref src, srcElement, -1, -1, -1, -1, -1, ref dst, dstElement, -1, -1, -1, 2);
		}

		/// <summary>
		///   <para>Adds a command to copy a texture into another texture.</para>
		/// </summary>
		/// <param name="src">Source texture or identifier, see RenderTargetIdentifier.</param>
		/// <param name="dst">Destination texture or identifier, see RenderTargetIdentifier.</param>
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
		public void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, RenderTargetIdentifier dst, int dstElement, int dstMip)
		{
			this.CopyTexture_Internal(ref src, srcElement, srcMip, -1, -1, -1, -1, ref dst, dstElement, dstMip, -1, -1, 3);
		}

		/// <summary>
		///   <para>Adds a command to copy a texture into another texture.</para>
		/// </summary>
		/// <param name="src">Source texture or identifier, see RenderTargetIdentifier.</param>
		/// <param name="dst">Destination texture or identifier, see RenderTargetIdentifier.</param>
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
		public void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY)
		{
			this.CopyTexture_Internal(ref src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, ref dst, dstElement, dstMip, dstX, dstY, 4);
		}

		/// <summary>
		///   <para>Add a command to set the rendering viewport.</para>
		/// </summary>
		/// <param name="pixelRect">Viewport rectangle in pixel coordinates.</param>
		public void SetViewport(Rect pixelRect)
		{
			CommandBuffer.INTERNAL_CALL_SetViewport(this, ref pixelRect);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetViewport(CommandBuffer self, ref Rect pixelRect);

		/// <summary>
		///   <para>Add a command to enable the hardware scissor rectangle.</para>
		/// </summary>
		/// <param name="scissor">Viewport rectangle in pixel coordinates.</param>
		public void EnableScissorRect(Rect scissor)
		{
			CommandBuffer.INTERNAL_CALL_EnableScissorRect(this, ref scissor);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_EnableScissorRect(CommandBuffer self, ref Rect scissor);

		/// <summary>
		///   <para>Add a command to disable the hardware scissor rectangle.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DisableScissorRect();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CopyTexture_Internal(ref RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, ref RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY, int mode);

		/// <summary>
		///   <para>Add a "blit into a render texture" command.</para>
		/// </summary>
		/// <param name="source">Source texture or render target to blit from.</param>
		/// <param name="dest">Destination to blit into.</param>
		/// <param name="mat">Material to use.</param>
		/// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		public void Blit(Texture source, RenderTargetIdentifier dest)
		{
			this.Blit_Texture(source, ref dest, null, -1, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		/// <summary>
		///   <para>Add a "blit into a render texture" command.</para>
		/// </summary>
		/// <param name="source">Source texture or render target to blit from.</param>
		/// <param name="dest">Destination to blit into.</param>
		/// <param name="mat">Material to use.</param>
		/// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		public void Blit(Texture source, RenderTargetIdentifier dest, Vector2 scale, Vector2 offset)
		{
			this.Blit_Texture(source, ref dest, null, -1, scale, offset);
		}

		/// <summary>
		///   <para>Add a "blit into a render texture" command.</para>
		/// </summary>
		/// <param name="source">Source texture or render target to blit from.</param>
		/// <param name="dest">Destination to blit into.</param>
		/// <param name="mat">Material to use.</param>
		/// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		public void Blit(Texture source, RenderTargetIdentifier dest, Material mat)
		{
			this.Blit_Texture(source, ref dest, mat, -1, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		/// <summary>
		///   <para>Add a "blit into a render texture" command.</para>
		/// </summary>
		/// <param name="source">Source texture or render target to blit from.</param>
		/// <param name="dest">Destination to blit into.</param>
		/// <param name="mat">Material to use.</param>
		/// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		public void Blit(Texture source, RenderTargetIdentifier dest, Material mat, int pass)
		{
			this.Blit_Texture(source, ref dest, mat, pass, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		private void Blit_Texture(Texture source, ref RenderTargetIdentifier dest, Material mat, int pass, Vector2 scale, Vector2 offset)
		{
			CommandBuffer.INTERNAL_CALL_Blit_Texture(this, source, ref dest, mat, pass, ref scale, ref offset);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Blit_Texture(CommandBuffer self, Texture source, ref RenderTargetIdentifier dest, Material mat, int pass, ref Vector2 scale, ref Vector2 offset);

		/// <summary>
		///   <para>Add a "blit into a render texture" command.</para>
		/// </summary>
		/// <param name="source">Source texture or render target to blit from.</param>
		/// <param name="dest">Destination to blit into.</param>
		/// <param name="mat">Material to use.</param>
		/// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest)
		{
			this.Blit_Identifier(ref source, ref dest, null, -1, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		/// <summary>
		///   <para>Add a "blit into a render texture" command.</para>
		/// </summary>
		/// <param name="source">Source texture or render target to blit from.</param>
		/// <param name="dest">Destination to blit into.</param>
		/// <param name="mat">Material to use.</param>
		/// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Vector2 scale, Vector2 offset)
		{
			this.Blit_Identifier(ref source, ref dest, null, -1, scale, offset);
		}

		/// <summary>
		///   <para>Add a "blit into a render texture" command.</para>
		/// </summary>
		/// <param name="source">Source texture or render target to blit from.</param>
		/// <param name="dest">Destination to blit into.</param>
		/// <param name="mat">Material to use.</param>
		/// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat)
		{
			this.Blit_Identifier(ref source, ref dest, mat, -1, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		/// <summary>
		///   <para>Add a "blit into a render texture" command.</para>
		/// </summary>
		/// <param name="source">Source texture or render target to blit from.</param>
		/// <param name="dest">Destination to blit into.</param>
		/// <param name="mat">Material to use.</param>
		/// <param name="pass">Shader pass to use (default is -1, meaning "all passes").</param>
		/// <param name="scale">Scale applied to the source texture coordinate.</param>
		/// <param name="offset">Offset applied to the source texture coordinate.</param>
		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int pass)
		{
			this.Blit_Identifier(ref source, ref dest, mat, pass, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}

		private void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, Material mat, int pass, Vector2 scale, Vector2 offset)
		{
			CommandBuffer.INTERNAL_CALL_Blit_Identifier(this, ref source, ref dest, mat, pass, ref scale, ref offset);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Blit_Identifier(CommandBuffer self, ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, Material mat, int pass, ref Vector2 scale, ref Vector2 offset);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetTemporaryRT(int nameID, int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("FilterMode.Point")] FilterMode filter, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing, [DefaultValue("false")] bool enableRandomWrite, [DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [DefaultValue("false")] bool useDynamicScale);

		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite, RenderTextureMemoryless memorylessMode)
		{
			bool flag = false;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite, memorylessMode, flag);
		}

		/// <summary>
		///   <para>Add a "get a temporary render texture" command.</para>
		/// </summary>
		/// <param name="nameID">Shader property name for this texture.</param>
		/// <param name="width">Width in pixels, or -1 for "camera pixel width".</param>
		/// <param name="height">Height in pixels, or -1 for "camera pixel height".</param>
		/// <param name="depthBuffer">Depth buffer bits (0, 16 or 24).</param>
		/// <param name="filter">Texture filtering mode (default is Point).</param>
		/// <param name="format">Format of the render texture (default is ARGB32).</param>
		/// <param name="readWrite">Color space conversion mode.</param>
		/// <param name="antiAliasing">Anti-aliasing (default is no anti-aliasing).</param>
		/// <param name="enableRandomWrite">Should random-write access into the texture be enabled (default is false).</param>
		/// <param name="desc">Use this RenderTextureDescriptor for the settings when creating the temporary RenderTexture.</param>
		/// <param name="memorylessMode">Render texture memoryless mode.</param>
		/// <param name="useDynamicScale"></param>
		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite)
		{
			bool flag = false;
			RenderTextureMemoryless renderTextureMemoryless = RenderTextureMemoryless.None;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite, renderTextureMemoryless, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			bool flag = false;
			RenderTextureMemoryless renderTextureMemoryless = RenderTextureMemoryless.None;
			bool flag2 = false;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, flag2, renderTextureMemoryless, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			bool flag = false;
			RenderTextureMemoryless renderTextureMemoryless = RenderTextureMemoryless.None;
			bool flag2 = false;
			int num = 1;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, num, flag2, renderTextureMemoryless, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format)
		{
			bool flag = false;
			RenderTextureMemoryless renderTextureMemoryless = RenderTextureMemoryless.None;
			bool flag2 = false;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, renderTextureReadWrite, num, flag2, renderTextureMemoryless, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter)
		{
			bool flag = false;
			RenderTextureMemoryless renderTextureMemoryless = RenderTextureMemoryless.None;
			bool flag2 = false;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, renderTextureFormat, renderTextureReadWrite, num, flag2, renderTextureMemoryless, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer)
		{
			bool flag = false;
			RenderTextureMemoryless renderTextureMemoryless = RenderTextureMemoryless.None;
			bool flag2 = false;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			FilterMode filterMode = FilterMode.Point;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filterMode, renderTextureFormat, renderTextureReadWrite, num, flag2, renderTextureMemoryless, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height)
		{
			bool flag = false;
			RenderTextureMemoryless renderTextureMemoryless = RenderTextureMemoryless.None;
			bool flag2 = false;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			FilterMode filterMode = FilterMode.Point;
			int num2 = 0;
			this.GetTemporaryRT(nameID, width, height, num2, filterMode, renderTextureFormat, renderTextureReadWrite, num, flag2, renderTextureMemoryless, flag);
		}

		/// <summary>
		///   <para>Add a "get a temporary render texture" command.</para>
		/// </summary>
		/// <param name="nameID">Shader property name for this texture.</param>
		/// <param name="width">Width in pixels, or -1 for "camera pixel width".</param>
		/// <param name="height">Height in pixels, or -1 for "camera pixel height".</param>
		/// <param name="depthBuffer">Depth buffer bits (0, 16 or 24).</param>
		/// <param name="filter">Texture filtering mode (default is Point).</param>
		/// <param name="format">Format of the render texture (default is ARGB32).</param>
		/// <param name="readWrite">Color space conversion mode.</param>
		/// <param name="antiAliasing">Anti-aliasing (default is no anti-aliasing).</param>
		/// <param name="enableRandomWrite">Should random-write access into the texture be enabled (default is false).</param>
		/// <param name="desc">Use this RenderTextureDescriptor for the settings when creating the temporary RenderTexture.</param>
		/// <param name="memorylessMode">Render texture memoryless mode.</param>
		/// <param name="useDynamicScale"></param>
		public void GetTemporaryRT(int nameID, RenderTextureDescriptor desc, [DefaultValue("FilterMode.Point")] FilterMode filter)
		{
			CommandBuffer.INTERNAL_CALL_GetTemporaryRT(this, nameID, ref desc, filter);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, RenderTextureDescriptor desc)
		{
			FilterMode filterMode = FilterMode.Point;
			CommandBuffer.INTERNAL_CALL_GetTemporaryRT(this, nameID, ref desc, filterMode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTemporaryRT(CommandBuffer self, int nameID, ref RenderTextureDescriptor desc, FilterMode filter);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetTemporaryRTArray(int nameID, int width, int height, int slices, [DefaultValue("0")] int depthBuffer, [DefaultValue("FilterMode.Point")] FilterMode filter, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing, [DefaultValue("false")] bool enableRandomWrite, [DefaultValue("false")] bool useDynamicScale);

		/// <summary>
		///   <para>Add a "get a temporary render texture array" command.</para>
		/// </summary>
		/// <param name="nameID">Shader property name for this texture.</param>
		/// <param name="width">Width in pixels, or -1 for "camera pixel width".</param>
		/// <param name="height">Height in pixels, or -1 for "camera pixel height".</param>
		/// <param name="slices">Number of slices in texture array.</param>
		/// <param name="depthBuffer">Depth buffer bits (0, 16 or 24).</param>
		/// <param name="filter">Texture filtering mode (default is Point).</param>
		/// <param name="format">Format of the render texture (default is ARGB32).</param>
		/// <param name="readWrite">Color space conversion mode.</param>
		/// <param name="antiAliasing">Anti-aliasing (default is no anti-aliasing).</param>
		/// <param name="enableRandomWrite">Should random-write access into the texture be enabled (default is false).</param>
		/// <param name="useDynamicScale"></param>
		[ExcludeFromDocs]
		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite)
		{
			bool flag = false;
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			bool flag = false;
			bool flag2 = false;
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, readWrite, antiAliasing, flag2, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			bool flag = false;
			bool flag2 = false;
			int num = 1;
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, readWrite, num, flag2, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format)
		{
			bool flag = false;
			bool flag2 = false;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, renderTextureReadWrite, num, flag2, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter)
		{
			bool flag = false;
			bool flag2 = false;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, renderTextureFormat, renderTextureReadWrite, num, flag2, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer)
		{
			bool flag = false;
			bool flag2 = false;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			FilterMode filterMode = FilterMode.Point;
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filterMode, renderTextureFormat, renderTextureReadWrite, num, flag2, flag);
		}

		[ExcludeFromDocs]
		public void GetTemporaryRTArray(int nameID, int width, int height, int slices)
		{
			bool flag = false;
			bool flag2 = false;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			FilterMode filterMode = FilterMode.Point;
			int num2 = 0;
			this.GetTemporaryRTArray(nameID, width, height, slices, num2, filterMode, renderTextureFormat, renderTextureReadWrite, num, flag2, flag);
		}

		/// <summary>
		///   <para>Add a "release a temporary render texture" command.</para>
		/// </summary>
		/// <param name="nameID">Shader property name for this texture.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ReleaseTemporaryRT(int nameID);

		/// <summary>
		///   <para>Adds a "clear render target" command.</para>
		/// </summary>
		/// <param name="clearDepth">Should clear depth buffer?</param>
		/// <param name="clearColor">Should clear color buffer?</param>
		/// <param name="backgroundColor">Color to clear with.</param>
		/// <param name="depth">Depth to clear with (default is 1.0).</param>
		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor, [DefaultValue("1.0f")] float depth)
		{
			CommandBuffer.INTERNAL_CALL_ClearRenderTarget(this, clearDepth, clearColor, ref backgroundColor, depth);
		}

		[ExcludeFromDocs]
		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor)
		{
			float num = 1f;
			CommandBuffer.INTERNAL_CALL_ClearRenderTarget(this, clearDepth, clearColor, ref backgroundColor, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClearRenderTarget(CommandBuffer self, bool clearDepth, bool clearColor, ref Color backgroundColor, float depth);

		/// <summary>
		///   <para>Add a "set global shader float property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalFloat(string name, float value)
		{
			this.SetGlobalFloat(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Add a "set global shader float property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalFloat(int nameID, float value);

		/// <summary>
		///   <para>Sets the given global integer property for all shaders.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalInt(string name, int value)
		{
			this.SetGlobalInt(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Sets the given global integer property for all shaders.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalInt(int nameID, int value);

		/// <summary>
		///   <para>Add a "set global shader vector property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalVector(string name, Vector4 value)
		{
			this.SetGlobalVector(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Add a "set global shader vector property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalVector(int nameID, Vector4 value)
		{
			CommandBuffer.INTERNAL_CALL_SetGlobalVector(this, nameID, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalVector(CommandBuffer self, int nameID, ref Vector4 value);

		/// <summary>
		///   <para>Add a "set global shader color property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalColor(string name, Color value)
		{
			this.SetGlobalColor(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Add a "set global shader color property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalColor(int nameID, Color value)
		{
			CommandBuffer.INTERNAL_CALL_SetGlobalColor(this, nameID, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalColor(CommandBuffer self, int nameID, ref Color value);

		/// <summary>
		///   <para>Add a "set global shader matrix property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalMatrix(string name, Matrix4x4 value)
		{
			this.SetGlobalMatrix(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Add a "set global shader matrix property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalMatrix(int nameID, Matrix4x4 value)
		{
			CommandBuffer.INTERNAL_CALL_SetGlobalMatrix(this, nameID, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalMatrix(CommandBuffer self, int nameID, ref Matrix4x4 value);

		/// <summary>
		///   <para>Adds a command to enable global shader keyword.</para>
		/// </summary>
		/// <param name="keyword">Shader keyword to enable.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void EnableShaderKeyword(string keyword);

		/// <summary>
		///   <para>Adds a command to disable global shader keyword.</para>
		/// </summary>
		/// <param name="keyword">Shader keyword to disable.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DisableShaderKeyword(string keyword);

		/// <summary>
		///   <para>Add a command to set the view matrix.</para>
		/// </summary>
		/// <param name="view">View (world to camera space) matrix.</param>
		public void SetViewMatrix(Matrix4x4 view)
		{
			CommandBuffer.INTERNAL_CALL_SetViewMatrix(this, ref view);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetViewMatrix(CommandBuffer self, ref Matrix4x4 view);

		/// <summary>
		///   <para>Add a command to set the projection matrix.</para>
		/// </summary>
		/// <param name="proj">Projection (camera to clip space) matrix.</param>
		public void SetProjectionMatrix(Matrix4x4 proj)
		{
			CommandBuffer.INTERNAL_CALL_SetProjectionMatrix(this, ref proj);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetProjectionMatrix(CommandBuffer self, ref Matrix4x4 proj);

		/// <summary>
		///   <para>Add a command to set the view and projection matrices.</para>
		/// </summary>
		/// <param name="view">View (world to camera space) matrix.</param>
		/// <param name="proj">Projection (camera to clip space) matrix.</param>
		public void SetViewProjectionMatrices(Matrix4x4 view, Matrix4x4 proj)
		{
			CommandBuffer.INTERNAL_CALL_SetViewProjectionMatrices(this, ref view, ref proj);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetViewProjectionMatrices(CommandBuffer self, ref Matrix4x4 view, ref Matrix4x4 proj);

		/// <summary>
		///   <para>Add a command to set global depth bias.</para>
		/// </summary>
		/// <param name="bias">Constant depth bias.</param>
		/// <param name="slopeBias">Slope-dependent depth bias.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalDepthBias(float bias, float slopeBias);

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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalFloatArrayListImpl(int nameID, object values);

		/// <summary>
		///   <para>Add a "set global shader float array property" command.</para>
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="values"></param>
		/// <param name="nameID"></param>
		public void SetGlobalFloatArray(string propertyName, float[] values)
		{
			this.SetGlobalFloatArray(Shader.PropertyToID(propertyName), values);
		}

		/// <summary>
		///   <para>Add a "set global shader float array property" command.</para>
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="values"></param>
		/// <param name="nameID"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalFloatArray(int nameID, float[] values);

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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalVectorArrayListImpl(int nameID, object values);

		/// <summary>
		///   <para>Add a "set global shader vector array property" command.</para>
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="values"></param>
		/// <param name="nameID"></param>
		public void SetGlobalVectorArray(string propertyName, Vector4[] values)
		{
			this.SetGlobalVectorArray(Shader.PropertyToID(propertyName), values);
		}

		/// <summary>
		///   <para>Add a "set global shader vector array property" command.</para>
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="values"></param>
		/// <param name="nameID"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalVectorArray(int nameID, Vector4[] values);

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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalMatrixArrayListImpl(int nameID, object values);

		/// <summary>
		///   <para>Add a "set global shader matrix array property" command.</para>
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="values"></param>
		/// <param name="nameID"></param>
		public void SetGlobalMatrixArray(string propertyName, Matrix4x4[] values)
		{
			this.SetGlobalMatrixArray(Shader.PropertyToID(propertyName), values);
		}

		/// <summary>
		///   <para>Add a "set global shader matrix array property" command.</para>
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="values"></param>
		/// <param name="nameID"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalMatrixArray(int nameID, Matrix4x4[] values);

		/// <summary>
		///   <para>Add a "set global shader texture property" command, referencing a RenderTexture.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalTexture(string name, RenderTargetIdentifier value)
		{
			this.SetGlobalTexture(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Add a "set global shader texture property" command, referencing a RenderTexture.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalTexture(int nameID, RenderTargetIdentifier value)
		{
			this.SetGlobalTexture_Impl(nameID, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalTexture_Impl(int nameID, ref RenderTargetIdentifier rt);

		/// <summary>
		///   <para>Add a "set global shader buffer property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		public void SetGlobalBuffer(string name, ComputeBuffer value)
		{
			this.SetGlobalBuffer(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Add a "set global shader buffer property" command.</para>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="nameID"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalBuffer(int nameID, ComputeBuffer value);

		/// <summary>
		///   <para>Add a "set shadow sampling mode" command.</para>
		/// </summary>
		/// <param name="shadowmap">Shadowmap render target to change the sampling mode on.</param>
		/// <param name="mode">New sampling mode.</param>
		public void SetShadowSamplingMode(RenderTargetIdentifier shadowmap, ShadowSamplingMode mode)
		{
			this.SetShadowSamplingMode_Impl(ref shadowmap, mode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetShadowSamplingMode_Impl(ref RenderTargetIdentifier shadowmap, ShadowSamplingMode mode);

		/// <summary>
		///   <para>Send a user-defined event to a native code plugin.</para>
		/// </summary>
		/// <param name="callback">Native code callback to queue for Unity's renderer to invoke.</param>
		/// <param name="eventID">User defined id to send to the callback.</param>
		public void IssuePluginEvent(IntPtr callback, int eventID)
		{
			if (callback == IntPtr.Zero)
			{
				throw new ArgumentException("Null callback specified.");
			}
			this.IssuePluginEventInternal(callback, eventID);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void IssuePluginEventInternal(IntPtr callback, int eventID);

		/// <summary>
		///   <para>Adds a command to begin profile sampling.</para>
		/// </summary>
		/// <param name="name">Name of the profile information used for sampling.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BeginSample(string name);

		/// <summary>
		///   <para>Adds a command to begin profile sampling.</para>
		/// </summary>
		/// <param name="name">Name of the profile information used for sampling.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void EndSample(string name);

		/// <summary>
		///   <para>Send a user-defined event to a native code plugin with custom data.</para>
		/// </summary>
		/// <param name="callback">Native code callback to queue for Unity's renderer to invoke.</param>
		/// <param name="data">Custom data to pass to the native plugin callback.</param>
		/// <param name="eventID">Built in or user defined id to send to the callback.</param>
		public void IssuePluginEventAndData(IntPtr callback, int eventID, IntPtr data)
		{
			if (callback == IntPtr.Zero)
			{
				throw new ArgumentException("Null callback specified.");
			}
			this.IssuePluginEventAndDataInternal(callback, eventID, data);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void IssuePluginEventAndDataInternal(IntPtr callback, int eventID, IntPtr data);

		/// <summary>
		///   <para>Send a user-defined blit event to a native code plugin.</para>
		/// </summary>
		/// <param name="callback">Native code callback to queue for Unity's renderer to invoke.</param>
		/// <param name="command">User defined command id to send to the callback.</param>
		/// <param name="source">Source render target.</param>
		/// <param name="dest">Destination render target.</param>
		/// <param name="commandParam">User data command parameters.</param>
		/// <param name="commandFlags">User data command flags.</param>
		public void IssuePluginCustomBlit(IntPtr callback, uint command, RenderTargetIdentifier source, RenderTargetIdentifier dest, uint commandParam, uint commandFlags)
		{
			this.IssuePluginCustomBlitInternal(callback, command, ref source, ref dest, commandParam, commandFlags);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void IssuePluginCustomBlitInternal(IntPtr callback, uint command, ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, uint commandParam, uint commandFlags);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void IssuePluginCustomTextureUpdateInternal(IntPtr callback, Texture targetTexture, uint userData);

		/// <summary>
		///   <para>Send a texture update event to a native code plugin.</para>
		/// </summary>
		/// <param name="callback">Native code callback to queue for Unity's renderer to invoke.</param>
		/// <param name="targetTexture">Texture resource to be updated.</param>
		/// <param name="commandParam">User data to send to the native plugin.</param>
		/// <param name="userData"></param>
		public void IssuePluginCustomTextureUpdate(IntPtr callback, Texture targetTexture, uint userData)
		{
			this.IssuePluginCustomTextureUpdateInternal(callback, targetTexture, userData);
		}

		/// <summary>
		///   <para>Converts and copies a source texture to a destination texture with a different format or dimensions.</para>
		/// </summary>
		/// <param name="src">Source texture.</param>
		/// <param name="dst">Destination texture.</param>
		/// <param name="srcElement">Source element (e.g. cubemap face). Set this to 0 for 2D source textures.</param>
		/// <param name="dstElement">Destination element (e.g. cubemap face or texture array element).</param>
		public void ConvertTexture(RenderTargetIdentifier src, RenderTargetIdentifier dst)
		{
			this.ConvertTexture_Internal(src, 0, dst, 0);
		}

		/// <summary>
		///   <para>Converts and copies a source texture to a destination texture with a different format or dimensions.</para>
		/// </summary>
		/// <param name="src">Source texture.</param>
		/// <param name="dst">Destination texture.</param>
		/// <param name="srcElement">Source element (e.g. cubemap face). Set this to 0 for 2D source textures.</param>
		/// <param name="dstElement">Destination element (e.g. cubemap face or texture array element).</param>
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

		/// <summary>
		///   <para>Add a "set invert culling" command to the buffer.</para>
		/// </summary>
		/// <param name="invertCulling">A boolean indicating whether to invert the backface culling (true) or not (false).</param>
		[NativeMethod("AddSetInvertCulling")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetInvertCulling(bool invertCulling);

		private void ConvertTexture_Internal(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement)
		{
			this.ConvertTexture_Internal_Injected(ref src, srcElement, ref dst, dstElement);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
		public void SetRenderTarget(RenderTargetIdentifier rt)
		{
			this.SetRenderTargetSingle_Internal(rt, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
		public void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
		{
			if (loadAction == RenderBufferLoadAction.Clear)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			this.SetRenderTargetSingle_Internal(rt, loadAction, storeAction, loadAction, storeAction);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
		public void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			if (colorLoadAction == RenderBufferLoadAction.Clear || depthLoadAction == RenderBufferLoadAction.Clear)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			this.SetRenderTargetSingle_Internal(rt, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel)
		{
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetSingle_Internal(new RenderTargetIdentifier(rt, mipLevel, CubemapFace.Unknown, 0), RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace)
		{
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetSingle_Internal(new RenderTargetIdentifier(rt, mipLevel, cubemapFace, 0), RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
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

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth)
		{
			this.SetRenderTargetColorDepth_Internal(color, depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel)
		{
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetColorDepth_Internal(new RenderTargetIdentifier(color, mipLevel, CubemapFace.Unknown, 0), depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace)
		{
			if (mipLevel < 0)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetColorDepth_Internal(new RenderTargetIdentifier(color, mipLevel, cubemapFace, 0), depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
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

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
		public void SetRenderTarget(RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			if (colorLoadAction == RenderBufferLoadAction.Clear || depthLoadAction == RenderBufferLoadAction.Clear)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			this.SetRenderTargetColorDepth_Internal(color, depth, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
		}

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
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

		/// <summary>
		///   <para>Add a "set active render target" command.</para>
		/// </summary>
		/// <param name="rt">Render target to set for both color &amp; depth buffers.</param>
		/// <param name="color">Render target to set as a color buffer.</param>
		/// <param name="colors">Render targets to set as color buffers (MRT).</param>
		/// <param name="depth">Render target to set as a depth buffer.</param>
		/// <param name="mipLevel">The mip level of the render target to render into.</param>
		/// <param name="cubemapFace">The cubemap face of a cubemap render target to render into.</param>
		/// <param name="depthSlice">Slice of a 3D or array render target to set.</param>
		/// <param name="loadAction">Load action that is used for color and depth/stencil buffers.</param>
		/// <param name="storeAction">Store action that is used for color and depth/stencil buffers.</param>
		/// <param name="colorLoadAction">Load action that is used for the color buffer.</param>
		/// <param name="colorStoreAction">Store action that is used for the color buffer.</param>
		/// <param name="depthLoadAction">Load action that is used for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action that is used for the depth/stencil buffer.</param>
		/// <param name="binding"></param>
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ConvertTexture_Internal_Injected(ref RenderTargetIdentifier src, int srcElement, ref RenderTargetIdentifier dst, int dstElement);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTargetSingle_Internal_Injected(ref RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTargetColorDepth_Internal_Injected(ref RenderTargetIdentifier color, ref RenderTargetIdentifier depth, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTargetMulti_Internal_Injected(RenderTargetIdentifier[] colors, ref RenderTargetIdentifier depth, RenderBufferLoadAction[] colorLoadActions, RenderBufferStoreAction[] colorStoreActions, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction);

		internal IntPtr m_Ptr;
	}
}
