using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[RayTracingShaderHelpURL]
	public sealed class RayTracingShader : Object
	{
		public float maxRecursionDepth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RayTracingShader.get_maxRecursionDepth_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetFloat", HasExplicitThis = true)]
		public void SetFloat(int nameID, float val)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.SetFloat_Injected(intPtr, nameID, val);
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetInt", HasExplicitThis = true)]
		public void SetInt(int nameID, int val)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.SetInt_Injected(intPtr, nameID, val);
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetVector", HasExplicitThis = true)]
		public void SetVector(int nameID, Vector4 val)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.SetVector_Injected(intPtr, nameID, ref val);
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetMatrix", HasExplicitThis = true)]
		public void SetMatrix(int nameID, Matrix4x4 val)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.SetMatrix_Injected(intPtr, nameID, ref val);
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetFloatArray", HasExplicitThis = true)]
		private unsafe void SetFloatArray(int nameID, float[] values)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(values);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				RayTracingShader.SetFloatArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetIntArray", HasExplicitThis = true)]
		private unsafe void SetIntArray(int nameID, int[] values)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = new Span<int>(values);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				RayTracingShader.SetIntArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetVectorArray", HasExplicitThis = true)]
		public unsafe void SetVectorArray(int nameID, Vector4[] values)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector4> span = new Span<Vector4>(values);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				RayTracingShader.SetVectorArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetMatrixArray", HasExplicitThis = true)]
		public unsafe void SetMatrixArray(int nameID, Matrix4x4[] values)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(values);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				RayTracingShader.SetMatrixArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[NativeMethod(Name = "RayTracingShaderScripting::SetTexture", HasExplicitThis = true, IsFreeFunction = true)]
		public void SetTexture(int nameID, [NotNull] Texture texture)
		{
			if (texture == null)
			{
				ThrowHelper.ThrowArgumentNullException(texture, "texture");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Texture>(texture);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(texture, "texture");
			}
			RayTracingShader.SetTexture_Injected(intPtr, nameID, intPtr2);
		}

		[NativeMethod(Name = "RayTracingShaderScripting::SetBuffer", HasExplicitThis = true, IsFreeFunction = true)]
		public void SetBuffer(int nameID, [NotNull] ComputeBuffer buffer)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			RayTracingShader.SetBuffer_Injected(intPtr, nameID, intPtr2);
		}

		[NativeMethod(Name = "RayTracingShaderScripting::SetBuffer", HasExplicitThis = true, IsFreeFunction = true)]
		private void SetGraphicsBuffer(int nameID, [NotNull] GraphicsBuffer buffer)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			RayTracingShader.SetGraphicsBuffer_Injected(intPtr, nameID, intPtr2);
		}

		[NativeMethod(Name = "RayTracingShaderScripting::SetBuffer", HasExplicitThis = true, IsFreeFunction = true)]
		private void SetGraphicsBufferHandle(int nameID, GraphicsBufferHandle bufferHandle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.SetGraphicsBufferHandle_Injected(intPtr, nameID, ref bufferHandle);
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetConstantBuffer", HasExplicitThis = true)]
		private void SetConstantComputeBuffer(int nameID, [NotNull] ComputeBuffer buffer, int offset, int size)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			RayTracingShader.SetConstantComputeBuffer_Injected(intPtr, nameID, intPtr2, offset, size);
		}

		[FreeFunction(Name = "RayTracingShaderScripting::SetConstantBuffer", HasExplicitThis = true)]
		private void SetConstantGraphicsBuffer(int nameID, [NotNull] GraphicsBuffer buffer, int offset, int size)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			RayTracingShader.SetConstantGraphicsBuffer_Injected(intPtr, nameID, intPtr2, offset, size);
		}

		[NativeMethod(Name = "RayTracingShaderScripting::SetAccelerationStructure", HasExplicitThis = true, IsFreeFunction = true)]
		public void SetAccelerationStructure(int nameID, [NotNull] RayTracingAccelerationStructure accelerationStructure)
		{
			if (accelerationStructure == null)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelerationStructure);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			RayTracingShader.SetAccelerationStructure_Injected(intPtr, nameID, intPtr2);
		}

		public unsafe void SetShaderPass(string passName)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(passName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = passName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				RayTracingShader.SetShaderPass_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeMethod(Name = "RayTracingShaderScripting::SetTextureFromGlobal", HasExplicitThis = true, IsFreeFunction = true)]
		public void SetTextureFromGlobal(int nameID, int globalTextureNameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.SetTextureFromGlobal_Injected(intPtr, nameID, globalTextureNameID);
		}

		[NativeMethod(Name = "RayTracingShaderScripting::Dispatch", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		public unsafe void Dispatch(string rayGenFunctionName, int width, int height, int depth, Camera camera = null)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(rayGenFunctionName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = rayGenFunctionName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				RayTracingShader.Dispatch_Injected(intPtr, ref managedSpanWrapper, width, height, depth, Object.MarshalledUnityObject.Marshal<Camera>(camera));
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeMethod(Name = "RayTracingShaderScripting::DispatchIndirect", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
		public unsafe void DispatchIndirect(string rayGenFunctionName, [NotNull] GraphicsBuffer argsBuffer, uint argsOffset = 0U, Camera camera = null)
		{
			if (argsBuffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(rayGenFunctionName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = rayGenFunctionName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
				}
				RayTracingShader.DispatchIndirect_Injected(intPtr, ref managedSpanWrapper, intPtr2, argsOffset, Object.MarshalledUnityObject.Marshal<Camera>(camera));
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void SetBuffer(int nameID, GraphicsBuffer buffer)
		{
			this.SetGraphicsBuffer(nameID, buffer);
		}

		public void SetBuffer(int nameID, GraphicsBufferHandle bufferHandle)
		{
			this.SetGraphicsBufferHandle(nameID, bufferHandle);
		}

		public LocalKeywordSpace keywordSpace
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LocalKeywordSpace localKeywordSpace;
				RayTracingShader.get_keywordSpace_Injected(intPtr, out localKeywordSpace);
				return localKeywordSpace;
			}
		}

		[FreeFunction("RayTracingShaderScripting::EnableKeyword", HasExplicitThis = true)]
		public unsafe void EnableKeyword(string keyword)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
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
				RayTracingShader.EnableKeyword_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("RayTracingShaderScripting::DisableKeyword", HasExplicitThis = true)]
		public unsafe void DisableKeyword(string keyword)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
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
				RayTracingShader.DisableKeyword_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("RayTracingShaderScripting::IsKeywordEnabled", HasExplicitThis = true)]
		public unsafe bool IsKeywordEnabled(string keyword)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
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
				flag = RayTracingShader.IsKeywordEnabled_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction("RayTracingShaderScripting::EnableKeyword", HasExplicitThis = true)]
		private void EnableLocalKeyword(LocalKeyword keyword)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.EnableLocalKeyword_Injected(intPtr, ref keyword);
		}

		[FreeFunction("RayTracingShaderScripting::DisableKeyword", HasExplicitThis = true)]
		private void DisableLocalKeyword(LocalKeyword keyword)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.DisableLocalKeyword_Injected(intPtr, ref keyword);
		}

		[FreeFunction("RayTracingShaderScripting::SetKeyword", HasExplicitThis = true)]
		private void SetLocalKeyword(LocalKeyword keyword, bool value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.SetLocalKeyword_Injected(intPtr, ref keyword, value);
		}

		[FreeFunction("RayTracingShaderScripting::IsKeywordEnabled", HasExplicitThis = true)]
		private bool IsLocalKeywordEnabled(LocalKeyword keyword)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingShader.IsLocalKeywordEnabled_Injected(intPtr, ref keyword);
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

		[FreeFunction("RayTracingShaderScripting::GetShaderKeywords", HasExplicitThis = true)]
		private string[] GetShaderKeywords()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingShader.GetShaderKeywords_Injected(intPtr);
		}

		[FreeFunction("RayTracingShaderScripting::SetShaderKeywords", HasExplicitThis = true)]
		private void SetShaderKeywords(string[] names)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.SetShaderKeywords_Injected(intPtr, names);
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

		[FreeFunction("RayTracingShaderScripting::GetEnabledKeywords", HasExplicitThis = true)]
		private LocalKeyword[] GetEnabledKeywords()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingShader.GetEnabledKeywords_Injected(intPtr);
		}

		[FreeFunction("RayTracingShaderScripting::SetEnabledKeywords", HasExplicitThis = true)]
		private void SetEnabledKeywords(LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingShader.SetEnabledKeywords_Injected(intPtr, keywords);
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

		private RayTracingShader()
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

		public void SetTexture(string name, Texture texture)
		{
			this.SetTexture(Shader.PropertyToID(name), texture);
		}

		public void SetBuffer(string name, ComputeBuffer buffer)
		{
			this.SetBuffer(Shader.PropertyToID(name), buffer);
		}

		public void SetBuffer(string name, GraphicsBuffer buffer)
		{
			this.SetBuffer(Shader.PropertyToID(name), buffer);
		}

		public void SetBuffer(string name, GraphicsBufferHandle bufferHandle)
		{
			this.SetBuffer(Shader.PropertyToID(name), bufferHandle);
		}

		public void SetConstantBuffer(int nameID, ComputeBuffer buffer, int offset, int size)
		{
			this.SetConstantComputeBuffer(nameID, buffer, offset, size);
		}

		public void SetConstantBuffer(string name, ComputeBuffer buffer, int offset, int size)
		{
			this.SetConstantComputeBuffer(Shader.PropertyToID(name), buffer, offset, size);
		}

		public void SetConstantBuffer(int nameID, GraphicsBuffer buffer, int offset, int size)
		{
			this.SetConstantGraphicsBuffer(nameID, buffer, offset, size);
		}

		public void SetConstantBuffer(string name, GraphicsBuffer buffer, int offset, int size)
		{
			this.SetConstantGraphicsBuffer(Shader.PropertyToID(name), buffer, offset, size);
		}

		public void SetAccelerationStructure(string name, RayTracingAccelerationStructure accelerationStructure)
		{
			this.SetAccelerationStructure(Shader.PropertyToID(name), accelerationStructure);
		}

		public void SetTextureFromGlobal(string name, string globalTextureName)
		{
			this.SetTextureFromGlobal(Shader.PropertyToID(name), Shader.PropertyToID(globalTextureName));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxRecursionDepth_Injected(IntPtr _unity_self);

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
		private static extern void SetTexture_Injected(IntPtr _unity_self, int nameID, IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGraphicsBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGraphicsBufferHandle_Injected(IntPtr _unity_self, int nameID, [In] ref GraphicsBufferHandle bufferHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConstantComputeBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConstantGraphicsBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAccelerationStructure_Injected(IntPtr _unity_self, int nameID, IntPtr accelerationStructure);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShaderPass_Injected(IntPtr _unity_self, ref ManagedSpanWrapper passName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTextureFromGlobal_Injected(IntPtr _unity_self, int nameID, int globalTextureNameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Dispatch_Injected(IntPtr _unity_self, ref ManagedSpanWrapper rayGenFunctionName, int width, int height, int depth, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DispatchIndirect_Injected(IntPtr _unity_self, ref ManagedSpanWrapper rayGenFunctionName, IntPtr argsBuffer, uint argsOffset, IntPtr camera);

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
		private static extern string[] GetShaderKeywords_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShaderKeywords_Injected(IntPtr _unity_self, string[] names);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LocalKeyword[] GetEnabledKeywords_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEnabledKeywords_Injected(IntPtr _unity_self, LocalKeyword[] keywords);
	}
}
