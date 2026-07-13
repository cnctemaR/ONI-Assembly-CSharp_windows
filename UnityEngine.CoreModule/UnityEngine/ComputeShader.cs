using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[NativeHeader("Runtime/Graphics/RayTracing/RayTracingAccelerationStructure.h")]
	[UsedByNativeCode]
	public sealed class ComputeShader : Object
	{
		[RequiredByNativeCode]
		[NativeMethod(Name = "ComputeShaderScripting::FindKernel", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		public unsafe int FindKernel(string name)
		{
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = ComputeShader.FindKernel_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		[FreeFunction(Name = "ComputeShaderScripting::HasKernel", HasExplicitThis = true)]
		public unsafe bool HasKernel(string name)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = ComputeShader.HasKernel_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetValue<float>", HasExplicitThis = true)]
		public void SetFloat(int nameID, float val)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.SetFloat_Injected(intPtr, nameID, val);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetValue<int>", HasExplicitThis = true)]
		public void SetInt(int nameID, int val)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.SetInt_Injected(intPtr, nameID, val);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetValue<Vector4f>", HasExplicitThis = true)]
		public void SetVector(int nameID, Vector4 val)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.SetVector_Injected(intPtr, nameID, ref val);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetValue<Matrix4x4f>", HasExplicitThis = true)]
		public void SetMatrix(int nameID, Matrix4x4 val)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.SetMatrix_Injected(intPtr, nameID, ref val);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetArray<float>", HasExplicitThis = true)]
		private unsafe void SetFloatArray(int nameID, float[] values)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(values);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ComputeShader.SetFloatArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetArray<int>", HasExplicitThis = true)]
		private unsafe void SetIntArray(int nameID, int[] values)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = new Span<int>(values);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ComputeShader.SetIntArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetArray<Vector4f>", HasExplicitThis = true)]
		public unsafe void SetVectorArray(int nameID, Vector4[] values)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector4> span = new Span<Vector4>(values);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ComputeShader.SetVectorArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetArray<Matrix4x4f>", HasExplicitThis = true)]
		public unsafe void SetMatrixArray(int nameID, Matrix4x4[] values)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(values);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ComputeShader.SetMatrixArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[NativeMethod(Name = "ComputeShaderScripting::SetTexture", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		public void SetTexture(int kernelIndex, int nameID, [NotNull] Texture texture, int mipLevel)
		{
			if (texture == null)
			{
				ThrowHelper.ThrowArgumentNullException(texture, "texture");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Texture>(texture);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(texture, "texture");
			}
			ComputeShader.SetTexture_Injected(intPtr, kernelIndex, nameID, intPtr2, mipLevel);
		}

		[NativeMethod(Name = "ComputeShaderScripting::SetRenderTexture", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		private void SetRenderTexture(int kernelIndex, int nameID, [NotNull] RenderTexture texture, int mipLevel, RenderTextureSubElement element)
		{
			if (texture == null)
			{
				ThrowHelper.ThrowArgumentNullException(texture, "texture");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(texture);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(texture, "texture");
			}
			ComputeShader.SetRenderTexture_Injected(intPtr, kernelIndex, nameID, intPtr2, mipLevel, element);
		}

		[NativeMethod(Name = "ComputeShaderScripting::SetTextureFromGlobal", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		public void SetTextureFromGlobal(int kernelIndex, int nameID, int globalTextureNameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.SetTextureFromGlobal_Injected(intPtr, kernelIndex, nameID, globalTextureNameID);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetBuffer", HasExplicitThis = true)]
		private void Internal_SetBuffer(int kernelIndex, int nameID, [NotNull] ComputeBuffer buffer)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			ComputeShader.Internal_SetBuffer_Injected(intPtr, kernelIndex, nameID, intPtr2);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetBuffer", HasExplicitThis = true)]
		private void Internal_SetGraphicsBuffer(int kernelIndex, int nameID, [NotNull] GraphicsBuffer buffer)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			ComputeShader.Internal_SetGraphicsBuffer_Injected(intPtr, kernelIndex, nameID, intPtr2);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetRayTracingAccelerationStructure", HasExplicitThis = true)]
		private void Internal_SetRayTracingAccelerationStructure(int kernelIndex, int nameID, [NotNull] RayTracingAccelerationStructure accelerationStructure)
		{
			if (accelerationStructure == null)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelerationStructure);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			ComputeShader.Internal_SetRayTracingAccelerationStructure_Injected(intPtr, kernelIndex, nameID, intPtr2);
		}

		public void SetRayTracingAccelerationStructure(int kernelIndex, int nameID, RayTracingAccelerationStructure accelerationStructure)
		{
			this.Internal_SetRayTracingAccelerationStructure(kernelIndex, nameID, accelerationStructure);
		}

		public void SetBuffer(int kernelIndex, int nameID, ComputeBuffer buffer)
		{
			this.Internal_SetBuffer(kernelIndex, nameID, buffer);
		}

		public void SetBuffer(int kernelIndex, int nameID, GraphicsBuffer buffer)
		{
			this.Internal_SetGraphicsBuffer(kernelIndex, nameID, buffer);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetConstantBuffer", HasExplicitThis = true)]
		private void SetConstantComputeBuffer(int nameID, [NotNull] ComputeBuffer buffer, int offset, int size)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			ComputeShader.SetConstantComputeBuffer_Injected(intPtr, nameID, intPtr2, offset, size);
		}

		[FreeFunction(Name = "ComputeShaderScripting::SetConstantBuffer", HasExplicitThis = true)]
		private void SetConstantGraphicsBuffer(int nameID, [NotNull] GraphicsBuffer buffer, int offset, int size)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			ComputeShader.SetConstantGraphicsBuffer_Injected(intPtr, nameID, intPtr2, offset, size);
		}

		[NativeMethod(Name = "ComputeShaderScripting::GetKernelThreadGroupSizes", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		public void GetKernelThreadGroupSizes(int kernelIndex, out uint x, out uint y, out uint z)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.GetKernelThreadGroupSizes_Injected(intPtr, kernelIndex, out x, out y, out z);
		}

		[NativeName("DispatchComputeShader")]
		public void Dispatch(int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.Dispatch_Injected(intPtr, kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
		}

		[FreeFunction(Name = "ComputeShaderScripting::DispatchIndirect", HasExplicitThis = true)]
		private void Internal_DispatchIndirect(int kernelIndex, [NotNull] ComputeBuffer argsBuffer, uint argsOffset)
		{
			if (argsBuffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
			}
			ComputeShader.Internal_DispatchIndirect_Injected(intPtr, kernelIndex, intPtr2, argsOffset);
		}

		[FreeFunction(Name = "ComputeShaderScripting::DispatchIndirect", HasExplicitThis = true)]
		private void Internal_DispatchIndirectGraphicsBuffer(int kernelIndex, [NotNull] GraphicsBuffer argsBuffer, uint argsOffset)
		{
			if (argsBuffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
			}
			ComputeShader.Internal_DispatchIndirectGraphicsBuffer_Injected(intPtr, kernelIndex, intPtr2, argsOffset);
		}

		public LocalKeywordSpace keywordSpace
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LocalKeywordSpace localKeywordSpace;
				ComputeShader.get_keywordSpace_Injected(intPtr, out localKeywordSpace);
				return localKeywordSpace;
			}
		}

		[FreeFunction("ComputeShaderScripting::EnableKeyword", HasExplicitThis = true)]
		public unsafe void EnableKeyword(string keyword)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ComputeShader.EnableKeyword_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("ComputeShaderScripting::DisableKeyword", HasExplicitThis = true)]
		public unsafe void DisableKeyword(string keyword)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ComputeShader.DisableKeyword_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("ComputeShaderScripting::IsKeywordEnabled", HasExplicitThis = true)]
		public unsafe bool IsKeywordEnabled(string keyword)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = ComputeShader.IsKeywordEnabled_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction("ComputeShaderScripting::EnableKeyword", HasExplicitThis = true)]
		private void EnableLocalKeyword(LocalKeyword keyword)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.EnableLocalKeyword_Injected(intPtr, ref keyword);
		}

		[FreeFunction("ComputeShaderScripting::DisableKeyword", HasExplicitThis = true)]
		private void DisableLocalKeyword(LocalKeyword keyword)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.DisableLocalKeyword_Injected(intPtr, ref keyword);
		}

		[FreeFunction("ComputeShaderScripting::SetKeyword", HasExplicitThis = true)]
		private void SetLocalKeyword(LocalKeyword keyword, bool value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.SetLocalKeyword_Injected(intPtr, ref keyword, value);
		}

		[FreeFunction("ComputeShaderScripting::IsKeywordEnabled", HasExplicitThis = true)]
		private bool IsLocalKeywordEnabled(LocalKeyword keyword)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ComputeShader.IsLocalKeywordEnabled_Injected(intPtr, ref keyword);
		}

		public void EnableKeyword(in LocalKeyword keyword)
		{
			this.EnableLocalKeyword(keyword);
		}

		public void DisableKeyword(in LocalKeyword keyword)
		{
			this.DisableLocalKeyword(keyword);
		}

		public void SetKeyword(in LocalKeyword keyword, bool value)
		{
			this.SetLocalKeyword(keyword, value);
		}

		public bool IsKeywordEnabled(in LocalKeyword keyword)
		{
			return this.IsLocalKeywordEnabled(keyword);
		}

		[FreeFunction("ComputeShaderScripting::IsSupported", HasExplicitThis = true)]
		public bool IsSupported(int kernelIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ComputeShader.IsSupported_Injected(intPtr, kernelIndex);
		}

		[FreeFunction("ComputeShaderScripting::GetShaderKeywords", HasExplicitThis = true)]
		private string[] GetShaderKeywords()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ComputeShader.GetShaderKeywords_Injected(intPtr);
		}

		[FreeFunction("ComputeShaderScripting::SetShaderKeywords", HasExplicitThis = true)]
		private void SetShaderKeywords(string[] names)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.SetShaderKeywords_Injected(intPtr, names);
		}

		public string[] shaderKeywords
		{
			get
			{
				return this.GetShaderKeywords();
			}
			set
			{
				this.SetShaderKeywords(value);
			}
		}

		[FreeFunction("ComputeShaderScripting::GetEnabledKeywords", HasExplicitThis = true)]
		private LocalKeyword[] GetEnabledKeywords()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ComputeShader.GetEnabledKeywords_Injected(intPtr);
		}

		[FreeFunction("ComputeShaderScripting::SetEnabledKeywords", HasExplicitThis = true)]
		private void SetEnabledKeywords(LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeShader.SetEnabledKeywords_Injected(intPtr, keywords);
		}

		public LocalKeyword[] enabledKeywords
		{
			get
			{
				return this.GetEnabledKeywords();
			}
			set
			{
				this.SetEnabledKeywords(value);
			}
		}

		private ComputeShader()
		{
		}

		public void SetFloat(string name, float val)
		{
			this.SetFloat(Shader.PropertyToID(name), val);
		}

		public void SetInt(string name, int val)
		{
			this.SetInt(Shader.PropertyToID(name), val);
		}

		public void SetVector(string name, Vector4 val)
		{
			this.SetVector(Shader.PropertyToID(name), val);
		}

		public void SetMatrix(string name, Matrix4x4 val)
		{
			this.SetMatrix(Shader.PropertyToID(name), val);
		}

		public void SetVectorArray(string name, Vector4[] values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), values);
		}

		public void SetMatrixArray(string name, Matrix4x4[] values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), values);
		}

		public void SetFloats(string name, params float[] values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), values);
		}

		public void SetFloats(int nameID, params float[] values)
		{
			this.SetFloatArray(nameID, values);
		}

		public void SetInts(string name, params int[] values)
		{
			this.SetIntArray(Shader.PropertyToID(name), values);
		}

		public void SetInts(int nameID, params int[] values)
		{
			this.SetIntArray(nameID, values);
		}

		public void SetBool(string name, bool val)
		{
			this.SetInt(Shader.PropertyToID(name), val ? 1 : 0);
		}

		public void SetBool(int nameID, bool val)
		{
			this.SetInt(nameID, val ? 1 : 0);
		}

		public void SetTexture(int kernelIndex, int nameID, Texture texture)
		{
			this.SetTexture(kernelIndex, nameID, texture, 0);
		}

		public void SetTexture(int kernelIndex, string name, Texture texture)
		{
			this.SetTexture(kernelIndex, Shader.PropertyToID(name), texture, 0);
		}

		public void SetTexture(int kernelIndex, string name, Texture texture, int mipLevel)
		{
			this.SetTexture(kernelIndex, Shader.PropertyToID(name), texture, mipLevel);
		}

		public void SetTexture(int kernelIndex, int nameID, RenderTexture texture, int mipLevel, RenderTextureSubElement element)
		{
			this.SetRenderTexture(kernelIndex, nameID, texture, mipLevel, element);
		}

		public void SetTexture(int kernelIndex, string name, RenderTexture texture, int mipLevel, RenderTextureSubElement element)
		{
			this.SetRenderTexture(kernelIndex, Shader.PropertyToID(name), texture, mipLevel, element);
		}

		public void SetTextureFromGlobal(int kernelIndex, string name, string globalTextureName)
		{
			this.SetTextureFromGlobal(kernelIndex, Shader.PropertyToID(name), Shader.PropertyToID(globalTextureName));
		}

		public void SetBuffer(int kernelIndex, string name, ComputeBuffer buffer)
		{
			this.SetBuffer(kernelIndex, Shader.PropertyToID(name), buffer);
		}

		public void SetBuffer(int kernelIndex, string name, GraphicsBuffer buffer)
		{
			this.SetBuffer(kernelIndex, Shader.PropertyToID(name), buffer);
		}

		public void SetRayTracingAccelerationStructure(int kernelIndex, string name, RayTracingAccelerationStructure accelerationStructure)
		{
			this.SetRayTracingAccelerationStructure(kernelIndex, Shader.PropertyToID(name), accelerationStructure);
		}

		public void SetConstantBuffer(int nameID, ComputeBuffer buffer, int offset, int size)
		{
			this.SetConstantComputeBuffer(nameID, buffer, offset, size);
		}

		public void SetConstantBuffer(string name, ComputeBuffer buffer, int offset, int size)
		{
			this.SetConstantBuffer(Shader.PropertyToID(name), buffer, offset, size);
		}

		public void SetConstantBuffer(int nameID, GraphicsBuffer buffer, int offset, int size)
		{
			this.SetConstantGraphicsBuffer(nameID, buffer, offset, size);
		}

		public void SetConstantBuffer(string name, GraphicsBuffer buffer, int offset, int size)
		{
			this.SetConstantBuffer(Shader.PropertyToID(name), buffer, offset, size);
		}

		public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer, [DefaultValue("0")] uint argsOffset)
		{
			bool flag = argsBuffer == null;
			if (flag)
			{
				throw new ArgumentNullException("argsBuffer");
			}
			bool flag2 = argsBuffer.m_Ptr == IntPtr.Zero;
			if (flag2)
			{
				throw new ObjectDisposedException("argsBuffer");
			}
			bool flag3 = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal && !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag3)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			this.Internal_DispatchIndirect(kernelIndex, argsBuffer, argsOffset);
		}

		[ExcludeFromDocs]
		public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer)
		{
			this.DispatchIndirect(kernelIndex, argsBuffer, 0U);
		}

		public void DispatchIndirect(int kernelIndex, GraphicsBuffer argsBuffer, [DefaultValue("0")] uint argsOffset)
		{
			bool flag = argsBuffer == null;
			if (flag)
			{
				throw new ArgumentNullException("argsBuffer");
			}
			bool flag2 = argsBuffer.m_Ptr == IntPtr.Zero;
			if (flag2)
			{
				throw new ObjectDisposedException("argsBuffer");
			}
			bool flag3 = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal && !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag3)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			this.Internal_DispatchIndirectGraphicsBuffer(kernelIndex, argsBuffer, argsOffset);
		}

		[ExcludeFromDocs]
		public void DispatchIndirect(int kernelIndex, GraphicsBuffer argsBuffer)
		{
			this.DispatchIndirect(kernelIndex, argsBuffer, 0U);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int FindKernel_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasKernel_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloat_Injected(IntPtr _unity_self, int nameID, float val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInt_Injected(IntPtr _unity_self, int nameID, int val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVector_Injected(IntPtr _unity_self, int nameID, [In] ref Vector4 val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMatrix_Injected(IntPtr _unity_self, int nameID, [In] ref Matrix4x4 val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVectorArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMatrixArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTexture_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr texture, int mipLevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderTexture_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr texture, int mipLevel, RenderTextureSubElement element);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTextureFromGlobal_Injected(IntPtr _unity_self, int kernelIndex, int nameID, int globalTextureNameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetBuffer_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetGraphicsBuffer_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingAccelerationStructure_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr accelerationStructure);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConstantComputeBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConstantGraphicsBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetKernelThreadGroupSizes_Injected(IntPtr _unity_self, int kernelIndex, out uint x, out uint y, out uint z);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Dispatch_Injected(IntPtr _unity_self, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DispatchIndirect_Injected(IntPtr _unity_self, int kernelIndex, IntPtr argsBuffer, uint argsOffset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DispatchIndirectGraphicsBuffer_Injected(IntPtr _unity_self, int kernelIndex, IntPtr argsBuffer, uint argsOffset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_keywordSpace_Injected(IntPtr _unity_self, out LocalKeywordSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsKeywordEnabled_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsLocalKeywordEnabled_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsSupported_Injected(IntPtr _unity_self, int kernelIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetShaderKeywords_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShaderKeywords_Injected(IntPtr _unity_self, string[] names);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LocalKeyword[] GetEnabledKeywords_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEnabledKeywords_Injected(IntPtr _unity_self, LocalKeyword[] keywords);
	}
}
