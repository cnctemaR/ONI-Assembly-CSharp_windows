using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Compute Shader asset.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	public sealed class ComputeShader : Object
	{
		private ComputeShader()
		{
		}

		/// <summary>
		///   <para>Find ComputeShader kernel index.</para>
		/// </summary>
		/// <param name="name">Name of kernel function.</param>
		/// <returns>
		///   <para>The Kernel index, or logs a "FindKernel failed" error message if the kernel is not found.</para>
		/// </returns>
		[NativeMethod(Name = "ComputeShaderScripting::FindKernel", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int FindKernel(string name);

		/// <summary>
		///   <para>Checks whether a shader contains a given kernel.</para>
		/// </summary>
		/// <param name="name">The name of the kernel to look for.</param>
		/// <returns>
		///   <para>True if the kernel is found, false otherwise.</para>
		/// </returns>
		[FreeFunction(Name = "ComputeShaderScripting::HasKernel", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasKernel(string name);

		/// <summary>
		///   <para>Set a float parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		[FreeFunction(Name = "ComputeShaderScripting::SetValue<float>", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetFloat(int nameID, float val);

		/// <summary>
		///   <para>Set an integer parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		[FreeFunction(Name = "ComputeShaderScripting::SetValue<int>", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetInt(int nameID, int val);

		/// <summary>
		///   <para>Set a vector parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		[FreeFunction(Name = "ComputeShaderScripting::SetValue<Vector4f>", HasExplicitThis = true)]
		public void SetVector(int nameID, Vector4 val)
		{
			this.SetVector_Injected(nameID, ref val);
		}

		/// <summary>
		///   <para>Set a Matrix parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		[FreeFunction(Name = "ComputeShaderScripting::SetValue<Matrix4x4f>", HasExplicitThis = true)]
		public void SetMatrix(int nameID, Matrix4x4 val)
		{
			this.SetMatrix_Injected(nameID, ref val);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetArray<float>", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatArray(int nameID, float[] values);

		[FreeFunction(Name = "ComputeShaderScripting::SetArray<int>", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIntArray(int nameID, int[] values);

		/// <summary>
		///   <para>Set a vector array parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="values">Value to set.</param>
		[FreeFunction(Name = "ComputeShaderScripting::SetArray<Vector4f>", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetVectorArray(int nameID, Vector4[] values);

		/// <summary>
		///   <para>Set a Matrix array parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="values">Value to set.</param>
		[FreeFunction(Name = "ComputeShaderScripting::SetArray<Matrix4x4f>", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetMatrixArray(int nameID, Matrix4x4[] values);

		/// <summary>
		///   <para>Set a texture parameter.</para>
		/// </summary>
		/// <param name="kernelIndex">For which kernel the texture is being set. See FindKernel.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="name">Name of the buffer variable in shader code.</param>
		/// <param name="texture">Texture to set.</param>
		[NativeMethod(Name = "ComputeShaderScripting::SetTexture", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTexture(int kernelIndex, int nameID, [NotNull] Texture texture);

		/// <summary>
		///   <para>Set a texture parameter from a global texture property.</para>
		/// </summary>
		/// <param name="kernelIndex">For which kernel the texture is being set. See FindKernel.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="name">Name of the buffer variable in shader code.</param>
		/// <param name="globalTextureName">Global texture property to assign to shader.</param>
		/// <param name="globalTextureNameID">Property name ID, use Shader.PropertyToID to get it.</param>
		[NativeMethod(Name = "ComputeShaderScripting::SetTextureFromGlobal", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTextureFromGlobal(int kernelIndex, int nameID, int globalTextureNameID);

		/// <summary>
		///   <para>Sets an input or output compute buffer.</para>
		/// </summary>
		/// <param name="kernelIndex">For which kernel the buffer is being set. See FindKernel.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="name">Name of the buffer variable in shader code.</param>
		/// <param name="buffer">Buffer to set.</param>
		[FreeFunction(Name = "ComputeShaderScripting::SetBuffer", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetBuffer(int kernelIndex, int nameID, [NotNull] ComputeBuffer buffer);

		[NativeMethod(Name = "ComputeShaderScripting::GetKernelThreadGroupSizes", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetKernelThreadGroupSizes(int kernelIndex, out uint x, out uint y, out uint z);

		/// <summary>
		///   <para>Execute a compute shader.</para>
		/// </summary>
		/// <param name="kernelIndex">Which kernel to execute. A single compute shader asset can have multiple kernel entry points.</param>
		/// <param name="threadGroupsX">Number of work groups in the X dimension.</param>
		/// <param name="threadGroupsY">Number of work groups in the Y dimension.</param>
		/// <param name="threadGroupsZ">Number of work groups in the Z dimension.</param>
		[NativeName("DispatchComputeShader")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispatch(int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ);

		[FreeFunction(Name = "ComputeShaderScripting::DispatchIndirect", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DispatchIndirect(int kernelIndex, [NotNull] ComputeBuffer argsBuffer, uint argsOffset);

		/// <summary>
		///   <para>Set a float parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		public void SetFloat(string name, float val)
		{
			this.SetFloat(Shader.PropertyToID(name), val);
		}

		/// <summary>
		///   <para>Set an integer parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		public void SetInt(string name, int val)
		{
			this.SetInt(Shader.PropertyToID(name), val);
		}

		/// <summary>
		///   <para>Set a vector parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		public void SetVector(string name, Vector4 val)
		{
			this.SetVector(Shader.PropertyToID(name), val);
		}

		/// <summary>
		///   <para>Set a Matrix parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		public void SetMatrix(string name, Matrix4x4 val)
		{
			this.SetMatrix(Shader.PropertyToID(name), val);
		}

		/// <summary>
		///   <para>Set a vector array parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="values">Value to set.</param>
		public void SetVectorArray(string name, Vector4[] values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), values);
		}

		/// <summary>
		///   <para>Set a Matrix array parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="values">Value to set.</param>
		public void SetMatrixArray(string name, Matrix4x4[] values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), values);
		}

		/// <summary>
		///   <para>Set multiple consecutive float parameters at once.</para>
		/// </summary>
		/// <param name="name">Array variable name in the shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="values">Value array to set.</param>
		public void SetFloats(string name, params float[] values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), values);
		}

		/// <summary>
		///   <para>Set multiple consecutive float parameters at once.</para>
		/// </summary>
		/// <param name="name">Array variable name in the shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="values">Value array to set.</param>
		public void SetFloats(int nameID, params float[] values)
		{
			this.SetFloatArray(nameID, values);
		}

		/// <summary>
		///   <para>Set multiple consecutive integer parameters at once.</para>
		/// </summary>
		/// <param name="name">Array variable name in the shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="values">Value array to set.</param>
		public void SetInts(string name, params int[] values)
		{
			this.SetIntArray(Shader.PropertyToID(name), values);
		}

		/// <summary>
		///   <para>Set multiple consecutive integer parameters at once.</para>
		/// </summary>
		/// <param name="name">Array variable name in the shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="values">Value array to set.</param>
		public void SetInts(int nameID, params int[] values)
		{
			this.SetIntArray(nameID, values);
		}

		/// <summary>
		///   <para>Set a bool parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		public void SetBool(string name, bool val)
		{
			this.SetInt(Shader.PropertyToID(name), (!val) ? 0 : 1);
		}

		/// <summary>
		///   <para>Set a bool parameter.</para>
		/// </summary>
		/// <param name="name">Variable name in shader code.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="val">Value to set.</param>
		public void SetBool(int nameID, bool val)
		{
			this.SetInt(nameID, (!val) ? 0 : 1);
		}

		/// <summary>
		///   <para>Set a texture parameter.</para>
		/// </summary>
		/// <param name="kernelIndex">For which kernel the texture is being set. See FindKernel.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="name">Name of the buffer variable in shader code.</param>
		/// <param name="texture">Texture to set.</param>
		public void SetTexture(int kernelIndex, string name, Texture texture)
		{
			this.SetTexture(kernelIndex, Shader.PropertyToID(name), texture);
		}

		/// <summary>
		///   <para>Set a texture parameter from a global texture property.</para>
		/// </summary>
		/// <param name="kernelIndex">For which kernel the texture is being set. See FindKernel.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="name">Name of the buffer variable in shader code.</param>
		/// <param name="globalTextureName">Global texture property to assign to shader.</param>
		/// <param name="globalTextureNameID">Property name ID, use Shader.PropertyToID to get it.</param>
		public void SetTextureFromGlobal(int kernelIndex, string name, string globalTextureName)
		{
			this.SetTextureFromGlobal(kernelIndex, Shader.PropertyToID(name), Shader.PropertyToID(globalTextureName));
		}

		/// <summary>
		///   <para>Sets an input or output compute buffer.</para>
		/// </summary>
		/// <param name="kernelIndex">For which kernel the buffer is being set. See FindKernel.</param>
		/// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
		/// <param name="name">Name of the buffer variable in shader code.</param>
		/// <param name="buffer">Buffer to set.</param>
		public void SetBuffer(int kernelIndex, string name, ComputeBuffer buffer)
		{
			this.SetBuffer(kernelIndex, Shader.PropertyToID(name), buffer);
		}

		/// <summary>
		///   <para>Execute a compute shader.</para>
		/// </summary>
		/// <param name="kernelIndex">Which kernel to execute. A single compute shader asset can have multiple kernel entry points.</param>
		/// <param name="argsBuffer">Buffer with dispatch arguments.</param>
		/// <param name="argsOffset">The byte offset into the buffer, where the draw arguments start.</param>
		public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer, [DefaultValue("0")] uint argsOffset)
		{
			if (argsBuffer == null)
			{
				throw new ArgumentNullException("argsBuffer");
			}
			if (argsBuffer.m_Ptr == IntPtr.Zero)
			{
				throw new ObjectDisposedException("argsBuffer");
			}
			this.Internal_DispatchIndirect(kernelIndex, argsBuffer, argsOffset);
		}

		[ExcludeFromDocs]
		public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer)
		{
			this.DispatchIndirect(kernelIndex, argsBuffer, 0U);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVector_Injected(int nameID, ref Vector4 val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMatrix_Injected(int nameID, ref Matrix4x4 val);
	}
}
