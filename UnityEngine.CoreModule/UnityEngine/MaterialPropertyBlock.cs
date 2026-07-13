using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/ShaderPropertySheet.h")]
	[NativeHeader("Runtime/Math/SphericalHarmonicsL2.h")]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	public sealed class MaterialPropertyBlock
	{
		[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", true)]
		public void AddFloat(string name, float value)
		{
			this.SetFloat(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", true)]
		public void AddFloat(int nameID, float value)
		{
			this.SetFloat(nameID, value);
		}

		[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", true)]
		public void AddVector(string name, Vector4 value)
		{
			this.SetVector(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", true)]
		public void AddVector(int nameID, Vector4 value)
		{
			this.SetVector(nameID, value);
		}

		[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", true)]
		public void AddColor(string name, Color value)
		{
			this.SetColor(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", true)]
		public void AddColor(int nameID, Color value)
		{
			this.SetColor(nameID, value);
		}

		[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", true)]
		public void AddMatrix(string name, Matrix4x4 value)
		{
			this.SetMatrix(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", true)]
		public void AddMatrix(int nameID, Matrix4x4 value)
		{
			this.SetMatrix(nameID, value);
		}

		[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", true)]
		public void AddTexture(string name, Texture value)
		{
			this.SetTexture(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", true)]
		public void AddTexture(int nameID, Texture value)
		{
			this.SetTexture(nameID, value);
		}

		[NativeName("GetIntFromScript")]
		[ThreadSafe]
		private int GetIntImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.GetIntImpl_Injected(intPtr, name);
		}

		[NativeName("GetFloatFromScript")]
		[ThreadSafe]
		private float GetFloatImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.GetFloatImpl_Injected(intPtr, name);
		}

		[NativeName("GetVectorFromScript")]
		[ThreadSafe]
		private Vector4 GetVectorImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector4 vector;
			MaterialPropertyBlock.GetVectorImpl_Injected(intPtr, name, out vector);
			return vector;
		}

		[ThreadSafe]
		[NativeName("GetColorFromScript")]
		private Color GetColorImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color color;
			MaterialPropertyBlock.GetColorImpl_Injected(intPtr, name, out color);
			return color;
		}

		[ThreadSafe]
		[NativeName("GetMatrixFromScript")]
		private Matrix4x4 GetMatrixImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			MaterialPropertyBlock.GetMatrixImpl_Injected(intPtr, name, out matrix4x);
			return matrix4x;
		}

		[ThreadSafe]
		[NativeName("GetTextureFromScript")]
		private Texture GetTextureImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(MaterialPropertyBlock.GetTextureImpl_Injected(intPtr, name));
		}

		[NativeName("HasPropertyFromScript")]
		private bool HasPropertyImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.HasPropertyImpl_Injected(intPtr, name);
		}

		[NativeName("HasFloatFromScript")]
		private bool HasFloatImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.HasFloatImpl_Injected(intPtr, name);
		}

		[NativeName("HasIntegerFromScript")]
		private bool HasIntImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.HasIntImpl_Injected(intPtr, name);
		}

		[NativeName("HasTextureFromScript")]
		private bool HasTextureImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.HasTextureImpl_Injected(intPtr, name);
		}

		[NativeName("HasMatrixFromScript")]
		private bool HasMatrixImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.HasMatrixImpl_Injected(intPtr, name);
		}

		[NativeName("HasVectorFromScript")]
		private bool HasVectorImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.HasVectorImpl_Injected(intPtr, name);
		}

		[NativeName("HasBufferFromScript")]
		private bool HasBufferImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.HasBufferImpl_Injected(intPtr, name);
		}

		[NativeName("HasConstantBufferFromScript")]
		private bool HasConstantBufferImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.HasConstantBufferImpl_Injected(intPtr, name);
		}

		[NativeName("SetIntFromScript")]
		[ThreadSafe]
		private void SetIntImpl(int name, int value)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.SetIntImpl_Injected(intPtr, name, value);
		}

		[ThreadSafe]
		[NativeName("SetFloatFromScript")]
		private void SetFloatImpl(int name, float value)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.SetFloatImpl_Injected(intPtr, name, value);
		}

		[NativeName("SetVectorFromScript")]
		[ThreadSafe]
		private void SetVectorImpl(int name, Vector4 value)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.SetVectorImpl_Injected(intPtr, name, ref value);
		}

		[ThreadSafe]
		[NativeName("SetColorFromScript")]
		private void SetColorImpl(int name, Color value)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.SetColorImpl_Injected(intPtr, name, ref value);
		}

		[ThreadSafe]
		[NativeName("SetMatrixFromScript")]
		private void SetMatrixImpl(int name, Matrix4x4 value)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.SetMatrixImpl_Injected(intPtr, name, ref value);
		}

		[NativeName("SetTextureFromScript")]
		[ThreadSafe]
		private void SetTextureImpl(int name, [NotNull] Texture value)
		{
			if (value == null)
			{
				ThrowHelper.ThrowArgumentNullException(value, "value");
			}
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Texture>(value);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(value, "value");
			}
			MaterialPropertyBlock.SetTextureImpl_Injected(intPtr, name, intPtr2);
		}

		[ThreadSafe]
		[NativeName("SetRenderTextureFromScript")]
		private void SetRenderTextureImpl(int name, [NotNull] RenderTexture value, RenderTextureSubElement element)
		{
			if (value == null)
			{
				ThrowHelper.ThrowArgumentNullException(value, "value");
			}
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(value);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(value, "value");
			}
			MaterialPropertyBlock.SetRenderTextureImpl_Injected(intPtr, name, intPtr2, element);
		}

		[NativeName("SetBufferFromScript")]
		[ThreadSafe]
		private void SetBufferImpl(int name, ComputeBuffer value)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.SetBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value));
		}

		[ThreadSafe]
		[NativeName("SetBufferFromScript")]
		private void SetGraphicsBufferImpl(int name, GraphicsBuffer value)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.SetGraphicsBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value));
		}

		[NativeName("SetConstantBufferFromScript")]
		[ThreadSafe]
		private void SetConstantBufferImpl(int name, ComputeBuffer value, int offset, int size)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.SetConstantBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
		}

		[ThreadSafe]
		[NativeName("SetConstantBufferFromScript")]
		private void SetConstantGraphicsBufferImpl(int name, GraphicsBuffer value, int offset, int size)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.SetConstantGraphicsBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
		}

		[ThreadSafe]
		[NativeName("SetFloatArrayFromScript")]
		private unsafe void SetFloatArrayImpl(int name, float[] values, int count)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(values);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				MaterialPropertyBlock.SetFloatArrayImpl_Injected(intPtr, name, ref managedSpanWrapper, count);
			}
		}

		[NativeName("SetVectorArrayFromScript")]
		[ThreadSafe]
		private unsafe void SetVectorArrayImpl(int name, Vector4[] values, int count)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector4> span = new Span<Vector4>(values);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				MaterialPropertyBlock.SetVectorArrayImpl_Injected(intPtr, name, ref managedSpanWrapper, count);
			}
		}

		[ThreadSafe]
		[NativeName("SetMatrixArrayFromScript")]
		private unsafe void SetMatrixArrayImpl(int name, Matrix4x4[] values, int count)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(values);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				MaterialPropertyBlock.SetMatrixArrayImpl_Injected(intPtr, name, ref managedSpanWrapper, count);
			}
		}

		[ThreadSafe]
		[NativeName("GetFloatArrayFromScript")]
		private float[] GetFloatArrayImpl(int name)
		{
			float[] array2;
			try
			{
				IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				MaterialPropertyBlock.GetFloatArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				float[] array;
				blittableArrayWrapper.Unmarshal<float>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeName("GetVectorArrayFromScript")]
		[ThreadSafe]
		private Vector4[] GetVectorArrayImpl(int name)
		{
			Vector4[] array2;
			try
			{
				IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				MaterialPropertyBlock.GetVectorArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Vector4[] array;
				blittableArrayWrapper.Unmarshal<Vector4>(ref array);
				array2 = array;
			}
			return array2;
		}

		[ThreadSafe]
		[NativeName("GetMatrixArrayFromScript")]
		private Matrix4x4[] GetMatrixArrayImpl(int name)
		{
			Matrix4x4[] array2;
			try
			{
				IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				MaterialPropertyBlock.GetMatrixArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Matrix4x4[] array;
				blittableArrayWrapper.Unmarshal<Matrix4x4>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeName("GetFloatArrayCountFromScript")]
		[ThreadSafe]
		private int GetFloatArrayCountImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.GetFloatArrayCountImpl_Injected(intPtr, name);
		}

		[NativeName("GetVectorArrayCountFromScript")]
		[ThreadSafe]
		private int GetVectorArrayCountImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.GetVectorArrayCountImpl_Injected(intPtr, name);
		}

		[ThreadSafe]
		[NativeName("GetMatrixArrayCountFromScript")]
		private int GetMatrixArrayCountImpl(int name)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MaterialPropertyBlock.GetMatrixArrayCountImpl_Injected(intPtr, name);
		}

		[NativeName("ExtractFloatArrayFromScript")]
		[ThreadSafe]
		private unsafe void ExtractFloatArrayImpl(int name, [Out] float[] val)
		{
			try
			{
				IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (val != null)
				{
					fixed (float[] array = val)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				MaterialPropertyBlock.ExtractFloatArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
			}
		}

		[ThreadSafe]
		[NativeName("ExtractVectorArrayFromScript")]
		private unsafe void ExtractVectorArrayImpl(int name, [Out] Vector4[] val)
		{
			try
			{
				IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (val != null)
				{
					fixed (Vector4[] array = val)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				MaterialPropertyBlock.ExtractVectorArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				Vector4[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Vector4>(ref array);
			}
		}

		[ThreadSafe]
		[NativeName("ExtractMatrixArrayFromScript")]
		private unsafe void ExtractMatrixArrayImpl(int name, [Out] Matrix4x4[] val)
		{
			try
			{
				IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (val != null)
				{
					fixed (Matrix4x4[] array = val)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				MaterialPropertyBlock.ExtractMatrixArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				Matrix4x4[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Matrix4x4>(ref array);
			}
		}

		[ThreadSafe]
		[FreeFunction("ConvertAndCopySHCoefficientArraysToPropertySheetFromScript")]
		internal unsafe static void Internal_CopySHCoefficientArraysFrom(MaterialPropertyBlock properties, SphericalHarmonicsL2[] lightProbes, int sourceStart, int destStart, int count)
		{
			IntPtr intPtr = ((properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
			Span<SphericalHarmonicsL2> span = new Span<SphericalHarmonicsL2>(lightProbes);
			fixed (SphericalHarmonicsL2* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				MaterialPropertyBlock.Internal_CopySHCoefficientArraysFrom_Injected(intPtr, ref managedSpanWrapper, sourceStart, destStart, count);
			}
		}

		[FreeFunction("CopyProbeOcclusionArrayToPropertySheetFromScript")]
		[ThreadSafe]
		internal unsafe static void Internal_CopyProbeOcclusionArrayFrom(MaterialPropertyBlock properties, Vector4[] occlusionProbes, int sourceStart, int destStart, int count)
		{
			IntPtr intPtr = ((properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
			Span<Vector4> span = new Span<Vector4>(occlusionProbes);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				MaterialPropertyBlock.Internal_CopyProbeOcclusionArrayFrom_Injected(intPtr, ref managedSpanWrapper, sourceStart, destStart, count);
			}
		}

		[NativeMethod(Name = "MaterialPropertyBlockScripting::Create", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateImpl();

		[NativeMethod(Name = "MaterialPropertyBlockScripting::Destroy", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyImpl(IntPtr mpb);

		public bool isEmpty
		{
			[NativeName("IsEmpty")]
			[ThreadSafe]
			get
			{
				IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return MaterialPropertyBlock.get_isEmpty_Injected(intPtr);
			}
		}

		[ThreadSafe]
		private void Clear(bool keepMemory)
		{
			IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MaterialPropertyBlock.Clear_Injected(intPtr, keepMemory);
		}

		public void Clear()
		{
			this.Clear(true);
		}

		private void SetFloatArray(int name, float[] values, int count)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			bool flag3 = values.Length < count;
			if (flag3)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetFloatArrayImpl(name, values, count);
		}

		private void SetVectorArray(int name, Vector4[] values, int count)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			bool flag3 = values.Length < count;
			if (flag3)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetVectorArrayImpl(name, values, count);
		}

		private void SetMatrixArray(int name, Matrix4x4[] values, int count)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			bool flag3 = values.Length < count;
			if (flag3)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetMatrixArrayImpl(name, values, count);
		}

		private void ExtractFloatArray(int name, List<float> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int floatArrayCountImpl = this.GetFloatArrayCountImpl(name);
			bool flag2 = floatArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<float>(values, floatArrayCountImpl);
				this.ExtractFloatArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<float>(values));
			}
		}

		private void ExtractVectorArray(int name, List<Vector4> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int vectorArrayCountImpl = this.GetVectorArrayCountImpl(name);
			bool flag2 = vectorArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<Vector4>(values, vectorArrayCountImpl);
				this.ExtractVectorArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<Vector4>(values));
			}
		}

		private void ExtractMatrixArray(int name, List<Matrix4x4> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int matrixArrayCountImpl = this.GetMatrixArrayCountImpl(name);
			bool flag2 = matrixArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<Matrix4x4>(values, matrixArrayCountImpl);
				this.ExtractMatrixArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(values));
			}
		}

		public MaterialPropertyBlock()
		{
			this.m_Ptr = MaterialPropertyBlock.CreateImpl();
		}

		~MaterialPropertyBlock()
		{
			this.Dispose();
		}

		private void Dispose()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				MaterialPropertyBlock.DestroyImpl(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		public void SetInt(string name, int value)
		{
			this.SetFloatImpl(Shader.PropertyToID(name), (float)value);
		}

		public void SetInt(int nameID, int value)
		{
			this.SetFloatImpl(nameID, (float)value);
		}

		public void SetFloat(string name, float value)
		{
			this.SetFloatImpl(Shader.PropertyToID(name), value);
		}

		public void SetFloat(int nameID, float value)
		{
			this.SetFloatImpl(nameID, value);
		}

		public void SetInteger(string name, int value)
		{
			this.SetIntImpl(Shader.PropertyToID(name), value);
		}

		public void SetInteger(int nameID, int value)
		{
			this.SetIntImpl(nameID, value);
		}

		public void SetVector(string name, Vector4 value)
		{
			this.SetVectorImpl(Shader.PropertyToID(name), value);
		}

		public void SetVector(int nameID, Vector4 value)
		{
			this.SetVectorImpl(nameID, value);
		}

		public void SetColor(string name, Color value)
		{
			this.SetColorImpl(Shader.PropertyToID(name), value);
		}

		public void SetColor(int nameID, Color value)
		{
			this.SetColorImpl(nameID, value);
		}

		public void SetMatrix(string name, Matrix4x4 value)
		{
			this.SetMatrixImpl(Shader.PropertyToID(name), value);
		}

		public void SetMatrix(int nameID, Matrix4x4 value)
		{
			this.SetMatrixImpl(nameID, value);
		}

		public void SetBuffer(string name, ComputeBuffer value)
		{
			this.SetBufferImpl(Shader.PropertyToID(name), value);
		}

		public void SetBuffer(int nameID, ComputeBuffer value)
		{
			this.SetBufferImpl(nameID, value);
		}

		public void SetBuffer(string name, GraphicsBuffer value)
		{
			this.SetGraphicsBufferImpl(Shader.PropertyToID(name), value);
		}

		public void SetBuffer(int nameID, GraphicsBuffer value)
		{
			this.SetGraphicsBufferImpl(nameID, value);
		}

		public void SetTexture(string name, Texture value)
		{
			this.SetTextureImpl(Shader.PropertyToID(name), value);
		}

		public void SetTexture(int nameID, Texture value)
		{
			this.SetTextureImpl(nameID, value);
		}

		public void SetTexture(string name, RenderTexture value, RenderTextureSubElement element)
		{
			this.SetRenderTextureImpl(Shader.PropertyToID(name), value, element);
		}

		public void SetTexture(int nameID, RenderTexture value, RenderTextureSubElement element)
		{
			this.SetRenderTextureImpl(nameID, value, element);
		}

		public void SetConstantBuffer(string name, ComputeBuffer value, int offset, int size)
		{
			this.SetConstantBufferImpl(Shader.PropertyToID(name), value, offset, size);
		}

		public void SetConstantBuffer(int nameID, ComputeBuffer value, int offset, int size)
		{
			this.SetConstantBufferImpl(nameID, value, offset, size);
		}

		public void SetConstantBuffer(string name, GraphicsBuffer value, int offset, int size)
		{
			this.SetConstantGraphicsBufferImpl(Shader.PropertyToID(name), value, offset, size);
		}

		public void SetConstantBuffer(int nameID, GraphicsBuffer value, int offset, int size)
		{
			this.SetConstantGraphicsBufferImpl(nameID, value, offset, size);
		}

		public void SetFloatArray(string name, List<float> values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<float>(values), values.Count);
		}

		public void SetFloatArray(int nameID, List<float> values)
		{
			this.SetFloatArray(nameID, NoAllocHelpers.ExtractArrayFromList<float>(values), values.Count);
		}

		public void SetFloatArray(string name, float[] values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), values, values.Length);
		}

		public void SetFloatArray(int nameID, float[] values)
		{
			this.SetFloatArray(nameID, values, values.Length);
		}

		public void SetVectorArray(string name, List<Vector4> values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<Vector4>(values), values.Count);
		}

		public void SetVectorArray(int nameID, List<Vector4> values)
		{
			this.SetVectorArray(nameID, NoAllocHelpers.ExtractArrayFromList<Vector4>(values), values.Count);
		}

		public void SetVectorArray(string name, Vector4[] values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), values, values.Length);
		}

		public void SetVectorArray(int nameID, Vector4[] values)
		{
			this.SetVectorArray(nameID, values, values.Length);
		}

		public void SetMatrixArray(string name, List<Matrix4x4> values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(values), values.Count);
		}

		public void SetMatrixArray(int nameID, List<Matrix4x4> values)
		{
			this.SetMatrixArray(nameID, NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(values), values.Count);
		}

		public void SetMatrixArray(string name, Matrix4x4[] values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), values, values.Length);
		}

		public void SetMatrixArray(int nameID, Matrix4x4[] values)
		{
			this.SetMatrixArray(nameID, values, values.Length);
		}

		public bool HasProperty(string name)
		{
			return this.HasPropertyImpl(Shader.PropertyToID(name));
		}

		public bool HasProperty(int nameID)
		{
			return this.HasPropertyImpl(nameID);
		}

		public bool HasInt(string name)
		{
			return this.HasFloatImpl(Shader.PropertyToID(name));
		}

		public bool HasInt(int nameID)
		{
			return this.HasFloatImpl(nameID);
		}

		public bool HasFloat(string name)
		{
			return this.HasFloatImpl(Shader.PropertyToID(name));
		}

		public bool HasFloat(int nameID)
		{
			return this.HasFloatImpl(nameID);
		}

		public bool HasInteger(string name)
		{
			return this.HasIntImpl(Shader.PropertyToID(name));
		}

		public bool HasInteger(int nameID)
		{
			return this.HasIntImpl(nameID);
		}

		public bool HasTexture(string name)
		{
			return this.HasTextureImpl(Shader.PropertyToID(name));
		}

		public bool HasTexture(int nameID)
		{
			return this.HasTextureImpl(nameID);
		}

		public bool HasMatrix(string name)
		{
			return this.HasMatrixImpl(Shader.PropertyToID(name));
		}

		public bool HasMatrix(int nameID)
		{
			return this.HasMatrixImpl(nameID);
		}

		public bool HasVector(string name)
		{
			return this.HasVectorImpl(Shader.PropertyToID(name));
		}

		public bool HasVector(int nameID)
		{
			return this.HasVectorImpl(nameID);
		}

		public bool HasColor(string name)
		{
			return this.HasVectorImpl(Shader.PropertyToID(name));
		}

		public bool HasColor(int nameID)
		{
			return this.HasVectorImpl(nameID);
		}

		public bool HasBuffer(string name)
		{
			return this.HasBufferImpl(Shader.PropertyToID(name));
		}

		public bool HasBuffer(int nameID)
		{
			return this.HasBufferImpl(nameID);
		}

		public bool HasConstantBuffer(string name)
		{
			return this.HasConstantBufferImpl(Shader.PropertyToID(name));
		}

		public bool HasConstantBuffer(int nameID)
		{
			return this.HasConstantBufferImpl(nameID);
		}

		public float GetFloat(string name)
		{
			return this.GetFloatImpl(Shader.PropertyToID(name));
		}

		public float GetFloat(int nameID)
		{
			return this.GetFloatImpl(nameID);
		}

		public int GetInt(string name)
		{
			return (int)this.GetFloatImpl(Shader.PropertyToID(name));
		}

		public int GetInt(int nameID)
		{
			return (int)this.GetFloatImpl(nameID);
		}

		public int GetInteger(string name)
		{
			return this.GetIntImpl(Shader.PropertyToID(name));
		}

		public int GetInteger(int nameID)
		{
			return this.GetIntImpl(nameID);
		}

		public Vector4 GetVector(string name)
		{
			return this.GetVectorImpl(Shader.PropertyToID(name));
		}

		public Vector4 GetVector(int nameID)
		{
			return this.GetVectorImpl(nameID);
		}

		public Color GetColor(string name)
		{
			return this.GetColorImpl(Shader.PropertyToID(name));
		}

		public Color GetColor(int nameID)
		{
			return this.GetColorImpl(nameID);
		}

		public Matrix4x4 GetMatrix(string name)
		{
			return this.GetMatrixImpl(Shader.PropertyToID(name));
		}

		public Matrix4x4 GetMatrix(int nameID)
		{
			return this.GetMatrixImpl(nameID);
		}

		public Texture GetTexture(string name)
		{
			return this.GetTextureImpl(Shader.PropertyToID(name));
		}

		public Texture GetTexture(int nameID)
		{
			return this.GetTextureImpl(nameID);
		}

		public float[] GetFloatArray(string name)
		{
			return this.GetFloatArray(Shader.PropertyToID(name));
		}

		public float[] GetFloatArray(int nameID)
		{
			return (this.GetFloatArrayCountImpl(nameID) != 0) ? this.GetFloatArrayImpl(nameID) : null;
		}

		public Vector4[] GetVectorArray(string name)
		{
			return this.GetVectorArray(Shader.PropertyToID(name));
		}

		public Vector4[] GetVectorArray(int nameID)
		{
			return (this.GetVectorArrayCountImpl(nameID) != 0) ? this.GetVectorArrayImpl(nameID) : null;
		}

		public Matrix4x4[] GetMatrixArray(string name)
		{
			return this.GetMatrixArray(Shader.PropertyToID(name));
		}

		public Matrix4x4[] GetMatrixArray(int nameID)
		{
			return (this.GetMatrixArrayCountImpl(nameID) != 0) ? this.GetMatrixArrayImpl(nameID) : null;
		}

		public void GetFloatArray(string name, List<float> values)
		{
			this.ExtractFloatArray(Shader.PropertyToID(name), values);
		}

		public void GetFloatArray(int nameID, List<float> values)
		{
			this.ExtractFloatArray(nameID, values);
		}

		public void GetVectorArray(string name, List<Vector4> values)
		{
			this.ExtractVectorArray(Shader.PropertyToID(name), values);
		}

		public void GetVectorArray(int nameID, List<Vector4> values)
		{
			this.ExtractVectorArray(nameID, values);
		}

		public void GetMatrixArray(string name, List<Matrix4x4> values)
		{
			this.ExtractMatrixArray(Shader.PropertyToID(name), values);
		}

		public void GetMatrixArray(int nameID, List<Matrix4x4> values)
		{
			this.ExtractMatrixArray(nameID, values);
		}

		public void CopySHCoefficientArraysFrom(List<SphericalHarmonicsL2> lightProbes)
		{
			bool flag = lightProbes == null;
			if (flag)
			{
				throw new ArgumentNullException("lightProbes");
			}
			this.CopySHCoefficientArraysFrom(NoAllocHelpers.ExtractArrayFromList<SphericalHarmonicsL2>(lightProbes), 0, 0, lightProbes.Count);
		}

		public void CopySHCoefficientArraysFrom(SphericalHarmonicsL2[] lightProbes)
		{
			bool flag = lightProbes == null;
			if (flag)
			{
				throw new ArgumentNullException("lightProbes");
			}
			this.CopySHCoefficientArraysFrom(lightProbes, 0, 0, lightProbes.Length);
		}

		public void CopySHCoefficientArraysFrom(List<SphericalHarmonicsL2> lightProbes, int sourceStart, int destStart, int count)
		{
			this.CopySHCoefficientArraysFrom(NoAllocHelpers.ExtractArrayFromList<SphericalHarmonicsL2>(lightProbes), sourceStart, destStart, count);
		}

		public void CopySHCoefficientArraysFrom(SphericalHarmonicsL2[] lightProbes, int sourceStart, int destStart, int count)
		{
			bool flag = lightProbes == null;
			if (flag)
			{
				throw new ArgumentNullException("lightProbes");
			}
			bool flag2 = sourceStart < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("sourceStart", "Argument sourceStart must not be negative.");
			}
			bool flag3 = destStart < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("sourceStart", "Argument destStart must not be negative.");
			}
			bool flag4 = count < 0;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Argument count must not be negative.");
			}
			bool flag5 = lightProbes.Length < sourceStart + count;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("The specified source start index or count is out of the range.");
			}
			MaterialPropertyBlock.Internal_CopySHCoefficientArraysFrom(this, lightProbes, sourceStart, destStart, count);
		}

		public void CopyProbeOcclusionArrayFrom(List<Vector4> occlusionProbes)
		{
			bool flag = occlusionProbes == null;
			if (flag)
			{
				throw new ArgumentNullException("occlusionProbes");
			}
			this.CopyProbeOcclusionArrayFrom(NoAllocHelpers.ExtractArrayFromList<Vector4>(occlusionProbes), 0, 0, occlusionProbes.Count);
		}

		public void CopyProbeOcclusionArrayFrom(Vector4[] occlusionProbes)
		{
			bool flag = occlusionProbes == null;
			if (flag)
			{
				throw new ArgumentNullException("occlusionProbes");
			}
			this.CopyProbeOcclusionArrayFrom(occlusionProbes, 0, 0, occlusionProbes.Length);
		}

		public void CopyProbeOcclusionArrayFrom(List<Vector4> occlusionProbes, int sourceStart, int destStart, int count)
		{
			this.CopyProbeOcclusionArrayFrom(NoAllocHelpers.ExtractArrayFromList<Vector4>(occlusionProbes), sourceStart, destStart, count);
		}

		public void CopyProbeOcclusionArrayFrom(Vector4[] occlusionProbes, int sourceStart, int destStart, int count)
		{
			bool flag = occlusionProbes == null;
			if (flag)
			{
				throw new ArgumentNullException("occlusionProbes");
			}
			bool flag2 = sourceStart < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("sourceStart", "Argument sourceStart must not be negative.");
			}
			bool flag3 = destStart < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("sourceStart", "Argument destStart must not be negative.");
			}
			bool flag4 = count < 0;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Argument count must not be negative.");
			}
			bool flag5 = occlusionProbes.Length < sourceStart + count;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("The specified source start index or count is out of the range.");
			}
			MaterialPropertyBlock.Internal_CopyProbeOcclusionArrayFrom(this, occlusionProbes, sourceStart, destStart, count);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVectorImpl_Injected(IntPtr _unity_self, int name, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetColorImpl_Injected(IntPtr _unity_self, int name, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMatrixImpl_Injected(IntPtr _unity_self, int name, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetTextureImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasPropertyImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasFloatImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasIntImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasTextureImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasMatrixImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasVectorImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasBufferImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasConstantBufferImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntImpl_Injected(IntPtr _unity_self, int name, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatImpl_Injected(IntPtr _unity_self, int name, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVectorImpl_Injected(IntPtr _unity_self, int name, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColorImpl_Injected(IntPtr _unity_self, int name, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMatrixImpl_Injected(IntPtr _unity_self, int name, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTextureImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderTextureImpl_Injected(IntPtr _unity_self, int name, IntPtr value, RenderTextureSubElement element);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGraphicsBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConstantBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConstantGraphicsBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVectorArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMatrixArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetFloatArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVectorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMatrixArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetFloatArrayCountImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVectorArrayCountImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMatrixArrayCountImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractFloatArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractVectorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractMatrixArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CopySHCoefficientArraysFrom_Injected(IntPtr properties, ref ManagedSpanWrapper lightProbes, int sourceStart, int destStart, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CopyProbeOcclusionArrayFrom_Injected(IntPtr properties, ref ManagedSpanWrapper occlusionProbes, int sourceStart, int destStart, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isEmpty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Clear_Injected(IntPtr _unity_self, bool keepMemory);

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(MaterialPropertyBlock materialPropertyBlock)
			{
				return materialPropertyBlock.m_Ptr;
			}
		}
	}
}
