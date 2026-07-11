using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	/// <summary>
	///   <para>A block of material values to apply.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/ShaderPropertySheet.h")]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[NativeHeader("Runtime/Math/SphericalHarmonicsL2.h")]
	public sealed class MaterialPropertyBlock
	{
		public MaterialPropertyBlock()
		{
			this.m_Ptr = MaterialPropertyBlock.CreateImpl();
		}

		[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
		public void AddFloat(string name, float value)
		{
			this.SetFloat(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
		public void AddFloat(int nameID, float value)
		{
			this.SetFloat(nameID, value);
		}

		[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
		public void AddVector(string name, Vector4 value)
		{
			this.SetVector(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
		public void AddVector(int nameID, Vector4 value)
		{
			this.SetVector(nameID, value);
		}

		[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
		public void AddColor(string name, Color value)
		{
			this.SetColor(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
		public void AddColor(int nameID, Color value)
		{
			this.SetColor(nameID, value);
		}

		[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
		public void AddMatrix(string name, Matrix4x4 value)
		{
			this.SetMatrix(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
		public void AddMatrix(int nameID, Matrix4x4 value)
		{
			this.SetMatrix(nameID, value);
		}

		[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
		public void AddTexture(string name, Texture value)
		{
			this.SetTexture(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
		public void AddTexture(int nameID, Texture value)
		{
			this.SetTexture(nameID, value);
		}

		[NativeName("GetFloatFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFloatImpl(int name);

		[NativeName("GetVectorFromScript")]
		private Vector4 GetVectorImpl(int name)
		{
			Vector4 vector;
			this.GetVectorImpl_Injected(name, out vector);
			return vector;
		}

		[NativeName("GetColorFromScript")]
		private Color GetColorImpl(int name)
		{
			Color color;
			this.GetColorImpl_Injected(name, out color);
			return color;
		}

		[NativeName("GetMatrixFromScript")]
		private Matrix4x4 GetMatrixImpl(int name)
		{
			Matrix4x4 matrix4x;
			this.GetMatrixImpl_Injected(name, out matrix4x);
			return matrix4x;
		}

		[NativeName("GetTextureFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Texture GetTextureImpl(int name);

		[NativeName("SetFloatFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatImpl(int name, float value);

		[NativeName("SetVectorFromScript")]
		private void SetVectorImpl(int name, Vector4 value)
		{
			this.SetVectorImpl_Injected(name, ref value);
		}

		[NativeName("SetColorFromScript")]
		private void SetColorImpl(int name, Color value)
		{
			this.SetColorImpl_Injected(name, ref value);
		}

		[NativeName("SetMatrixFromScript")]
		private void SetMatrixImpl(int name, Matrix4x4 value)
		{
			this.SetMatrixImpl_Injected(name, ref value);
		}

		[NativeName("SetTextureFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTextureImpl(int name, [NotNull] Texture value);

		[NativeName("SetBufferFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBufferImpl(int name, ComputeBuffer value);

		[NativeName("SetFloatArrayFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatArrayImpl(int name, float[] values, int count);

		[NativeName("SetVectorArrayFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVectorArrayImpl(int name, Vector4[] values, int count);

		[NativeName("SetMatrixArrayFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMatrixArrayImpl(int name, Matrix4x4[] values, int count);

		[NativeName("GetFloatArrayFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float[] GetFloatArrayImpl(int name);

		[NativeName("GetVectorArrayFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Vector4[] GetVectorArrayImpl(int name);

		[NativeName("GetMatrixArrayFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Matrix4x4[] GetMatrixArrayImpl(int name);

		[NativeName("GetFloatArrayCountFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetFloatArrayCountImpl(int name);

		[NativeName("GetVectorArrayCountFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetVectorArrayCountImpl(int name);

		[NativeName("GetMatrixArrayCountFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetMatrixArrayCountImpl(int name);

		[NativeName("ExtractFloatArrayFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ExtractFloatArrayImpl(int name, [Out] float[] val);

		[NativeName("ExtractVectorArrayFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ExtractVectorArrayImpl(int name, [Out] Vector4[] val);

		[NativeName("ExtractMatrixArrayFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ExtractMatrixArrayImpl(int name, [Out] Matrix4x4[] val);

		[FreeFunction("ConvertAndCopySHCoefficientArraysToPropertySheetFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_CopySHCoefficientArraysFrom(MaterialPropertyBlock properties, SphericalHarmonicsL2[] lightProbes, int sourceStart, int destStart, int count);

		[FreeFunction("CopyProbeOcclusionArrayToPropertySheetFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_CopyProbeOcclusionArrayFrom(MaterialPropertyBlock properties, Vector4[] occlusionProbes, int sourceStart, int destStart, int count);

		[NativeMethod(Name = "MaterialPropertyBlockScripting::Create", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateImpl();

		[NativeMethod(Name = "MaterialPropertyBlockScripting::Destroy", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyImpl(IntPtr mpb);

		/// <summary>
		///   <para>Is the material property block empty? (Read Only)</para>
		/// </summary>
		public extern bool isEmpty
		{
			[NativeName("IsEmpty")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Clear(bool keepMemory);

		/// <summary>
		///   <para>Clear material property values.</para>
		/// </summary>
		public void Clear()
		{
			this.Clear(true);
		}

		private void SetFloatArray(int name, float[] values, int count)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			if (values.Length < count)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetFloatArrayImpl(name, values, count);
		}

		private void SetVectorArray(int name, Vector4[] values, int count)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			if (values.Length < count)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetVectorArrayImpl(name, values, count);
		}

		private void SetMatrixArray(int name, Matrix4x4[] values, int count)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			if (values.Length < count)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetMatrixArrayImpl(name, values, count);
		}

		private void ExtractFloatArray(int name, List<float> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int floatArrayCountImpl = this.GetFloatArrayCountImpl(name);
			if (floatArrayCountImpl > 0)
			{
				NoAllocHelpers.EnsureListElemCount<float>(values, floatArrayCountImpl);
				this.ExtractFloatArrayImpl(name, (float[])NoAllocHelpers.ExtractArrayFromList(values));
			}
		}

		private void ExtractVectorArray(int name, List<Vector4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int vectorArrayCountImpl = this.GetVectorArrayCountImpl(name);
			if (vectorArrayCountImpl > 0)
			{
				NoAllocHelpers.EnsureListElemCount<Vector4>(values, vectorArrayCountImpl);
				this.ExtractVectorArrayImpl(name, (Vector4[])NoAllocHelpers.ExtractArrayFromList(values));
			}
		}

		private void ExtractMatrixArray(int name, List<Matrix4x4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int matrixArrayCountImpl = this.GetMatrixArrayCountImpl(name);
			if (matrixArrayCountImpl > 0)
			{
				NoAllocHelpers.EnsureListElemCount<Matrix4x4>(values, matrixArrayCountImpl);
				this.ExtractMatrixArrayImpl(name, (Matrix4x4[])NoAllocHelpers.ExtractArrayFromList(values));
			}
		}

		~MaterialPropertyBlock()
		{
			this.Dispose();
		}

		private void Dispose()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				MaterialPropertyBlock.DestroyImpl(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   <para>Set a float property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The float value to set.</param>
		public void SetFloat(string name, float value)
		{
			this.SetFloatImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Set a float property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The float value to set.</param>
		public void SetFloat(int nameID, float value)
		{
			this.SetFloatImpl(nameID, value);
		}

		/// <summary>
		///   <para>Adds a property to the block. If an int property with the given name already exists, the old value is replaced.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The int value to set.</param>
		public void SetInt(string name, int value)
		{
			this.SetFloatImpl(Shader.PropertyToID(name), (float)value);
		}

		/// <summary>
		///   <para>Adds a property to the block. If an int property with the given name already exists, the old value is replaced.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The int value to set.</param>
		public void SetInt(int nameID, int value)
		{
			this.SetFloatImpl(nameID, (float)value);
		}

		/// <summary>
		///   <para>Set a vector property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The Vector4 value to set.</param>
		public void SetVector(string name, Vector4 value)
		{
			this.SetVectorImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Set a vector property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The Vector4 value to set.</param>
		public void SetVector(int nameID, Vector4 value)
		{
			this.SetVectorImpl(nameID, value);
		}

		/// <summary>
		///   <para>Set a color property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The Color value to set.</param>
		public void SetColor(string name, Color value)
		{
			this.SetColorImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Set a color property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The Color value to set.</param>
		public void SetColor(int nameID, Color value)
		{
			this.SetColorImpl(nameID, value);
		}

		/// <summary>
		///   <para>Set a matrix property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The matrix value to set.</param>
		public void SetMatrix(string name, Matrix4x4 value)
		{
			this.SetMatrixImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Set a matrix property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The matrix value to set.</param>
		public void SetMatrix(int nameID, Matrix4x4 value)
		{
			this.SetMatrixImpl(nameID, value);
		}

		/// <summary>
		///   <para>Set a ComputeBuffer property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The ComputeBuffer to set.</param>
		public void SetBuffer(string name, ComputeBuffer value)
		{
			this.SetBufferImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Set a ComputeBuffer property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The ComputeBuffer to set.</param>
		public void SetBuffer(int nameID, ComputeBuffer value)
		{
			this.SetBufferImpl(nameID, value);
		}

		/// <summary>
		///   <para>Set a texture property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The Texture to set.</param>
		public void SetTexture(string name, Texture value)
		{
			this.SetTextureImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Set a texture property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="value">The Texture to set.</param>
		public void SetTexture(int nameID, Texture value)
		{
			this.SetTextureImpl(nameID, value);
		}

		public void SetFloatArray(string name, List<float> values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT<float>(values), values.Count);
		}

		public void SetFloatArray(int nameID, List<float> values)
		{
			this.SetFloatArray(nameID, NoAllocHelpers.ExtractArrayFromListT<float>(values), values.Count);
		}

		/// <summary>
		///   <para>Set a float array property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="values">The array to set.</param>
		public void SetFloatArray(string name, float[] values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), values, values.Length);
		}

		/// <summary>
		///   <para>Set a float array property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="values">The array to set.</param>
		public void SetFloatArray(int nameID, float[] values)
		{
			this.SetFloatArray(nameID, values, values.Length);
		}

		public void SetVectorArray(string name, List<Vector4> values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT<Vector4>(values), values.Count);
		}

		public void SetVectorArray(int nameID, List<Vector4> values)
		{
			this.SetVectorArray(nameID, NoAllocHelpers.ExtractArrayFromListT<Vector4>(values), values.Count);
		}

		/// <summary>
		///   <para>Set a vector array property.</para>
		/// </summary>
		/// <param name="nameID">The name of the property.</param>
		/// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The array to set.</param>
		public void SetVectorArray(string name, Vector4[] values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), values, values.Length);
		}

		/// <summary>
		///   <para>Set a vector array property.</para>
		/// </summary>
		/// <param name="nameID">The name of the property.</param>
		/// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The array to set.</param>
		public void SetVectorArray(int nameID, Vector4[] values)
		{
			this.SetVectorArray(nameID, values, values.Length);
		}

		public void SetMatrixArray(string name, List<Matrix4x4> values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT<Matrix4x4>(values), values.Count);
		}

		public void SetMatrixArray(int nameID, List<Matrix4x4> values)
		{
			this.SetMatrixArray(nameID, NoAllocHelpers.ExtractArrayFromListT<Matrix4x4>(values), values.Count);
		}

		/// <summary>
		///   <para>Set a matrix array property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="nameID">The array to set.</param>
		public void SetMatrixArray(string name, Matrix4x4[] values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), values, values.Length);
		}

		/// <summary>
		///   <para>Set a matrix array property.</para>
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="nameID">The array to set.</param>
		public void SetMatrixArray(int nameID, Matrix4x4[] values)
		{
			this.SetMatrixArray(nameID, values, values.Length);
		}

		/// <summary>
		///   <para>Get a float from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public float GetFloat(string name)
		{
			return this.GetFloatImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Get a float from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public float GetFloat(int nameID)
		{
			return this.GetFloatImpl(nameID);
		}

		/// <summary>
		///   <para>Get an int from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public int GetInt(string name)
		{
			return (int)this.GetFloatImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Get an int from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public int GetInt(int nameID)
		{
			return (int)this.GetFloatImpl(nameID);
		}

		/// <summary>
		///   <para>Get a vector from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Vector4 GetVector(string name)
		{
			return this.GetVectorImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Get a vector from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Vector4 GetVector(int nameID)
		{
			return this.GetVectorImpl(nameID);
		}

		/// <summary>
		///   <para>Get a color from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Color GetColor(string name)
		{
			return this.GetColorImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Get a color from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Color GetColor(int nameID)
		{
			return this.GetColorImpl(nameID);
		}

		/// <summary>
		///   <para>Get a matrix from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Matrix4x4 GetMatrix(string name)
		{
			return this.GetMatrixImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Get a matrix from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Matrix4x4 GetMatrix(int nameID)
		{
			return this.GetMatrixImpl(nameID);
		}

		/// <summary>
		///   <para>Get a texture from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Texture GetTexture(string name)
		{
			return this.GetTextureImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Get a texture from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Texture GetTexture(int nameID)
		{
			return this.GetTextureImpl(nameID);
		}

		/// <summary>
		///   <para>Get a float array from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public float[] GetFloatArray(string name)
		{
			return this.GetFloatArray(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Get a float array from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public float[] GetFloatArray(int nameID)
		{
			return (this.GetFloatArrayCountImpl(nameID) == 0) ? null : this.GetFloatArrayImpl(nameID);
		}

		/// <summary>
		///   <para>Get a vector array from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Vector4[] GetVectorArray(string name)
		{
			return this.GetVectorArray(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Get a vector array from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Vector4[] GetVectorArray(int nameID)
		{
			return (this.GetVectorArrayCountImpl(nameID) == 0) ? null : this.GetVectorArrayImpl(nameID);
		}

		/// <summary>
		///   <para>Get a matrix array from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Matrix4x4[] GetMatrixArray(string name)
		{
			return this.GetMatrixArray(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Get a matrix array from the property block.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public Matrix4x4[] GetMatrixArray(int nameID)
		{
			return (this.GetMatrixArrayCountImpl(nameID) == 0) ? null : this.GetMatrixArrayImpl(nameID);
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
			if (lightProbes == null)
			{
				throw new ArgumentNullException("lightProbes");
			}
			this.CopySHCoefficientArraysFrom(NoAllocHelpers.ExtractArrayFromListT<SphericalHarmonicsL2>(lightProbes), 0, 0, lightProbes.Count);
		}

		/// <summary>
		///   <para>This function converts and copies the entire source array into 7 Vector4 property arrays named unity_SHAr, unity_SHAg, unity_SHAb, unity_SHBr, unity_SHBg, unity_SHBb and unity_SHC for use with instanced rendering.</para>
		/// </summary>
		/// <param name="lightProbes">The array of SH values to copy from.</param>
		public void CopySHCoefficientArraysFrom(SphericalHarmonicsL2[] lightProbes)
		{
			if (lightProbes == null)
			{
				throw new ArgumentNullException("lightProbes");
			}
			this.CopySHCoefficientArraysFrom(lightProbes, 0, 0, lightProbes.Length);
		}

		public void CopySHCoefficientArraysFrom(List<SphericalHarmonicsL2> lightProbes, int sourceStart, int destStart, int count)
		{
			this.CopySHCoefficientArraysFrom(NoAllocHelpers.ExtractArrayFromListT<SphericalHarmonicsL2>(lightProbes), sourceStart, destStart, count);
		}

		/// <summary>
		///   <para>This function converts and copies the source array into 7 Vector4 property arrays named unity_SHAr, unity_SHAg, unity_SHAb, unity_SHBr, unity_SHBg, unity_SHBb and unity_SHC with the specified source and destination range for use with instanced rendering.</para>
		/// </summary>
		/// <param name="lightProbes">The array of SH values to copy from.</param>
		/// <param name="sourceStart">The index of the first element in the source array to copy from.</param>
		/// <param name="destStart">The index of the first element in the destination MaterialPropertyBlock array to copy to.</param>
		/// <param name="count">The number of elements to copy.</param>
		public void CopySHCoefficientArraysFrom(SphericalHarmonicsL2[] lightProbes, int sourceStart, int destStart, int count)
		{
			if (lightProbes == null)
			{
				throw new ArgumentNullException("lightProbes");
			}
			if (sourceStart < 0)
			{
				throw new ArgumentOutOfRangeException("sourceStart", "Argument sourceStart must not be negative.");
			}
			if (destStart < 0)
			{
				throw new ArgumentOutOfRangeException("sourceStart", "Argument destStart must not be negative.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Argument count must not be negative.");
			}
			if (lightProbes.Length < sourceStart + count)
			{
				throw new ArgumentOutOfRangeException("The specified source start index or count is out of the range.");
			}
			MaterialPropertyBlock.Internal_CopySHCoefficientArraysFrom(this, lightProbes, sourceStart, destStart, count);
		}

		public void CopyProbeOcclusionArrayFrom(List<Vector4> occlusionProbes)
		{
			if (occlusionProbes == null)
			{
				throw new ArgumentNullException("occlusionProbes");
			}
			this.CopyProbeOcclusionArrayFrom(NoAllocHelpers.ExtractArrayFromListT<Vector4>(occlusionProbes), 0, 0, occlusionProbes.Count);
		}

		/// <summary>
		///   <para>This function copies the entire source array into a Vector4 property array named unity_ProbesOcclusion for use with instanced rendering.</para>
		/// </summary>
		/// <param name="occlusionProbes">The array of probe occlusion values to copy from.</param>
		public void CopyProbeOcclusionArrayFrom(Vector4[] occlusionProbes)
		{
			if (occlusionProbes == null)
			{
				throw new ArgumentNullException("occlusionProbes");
			}
			this.CopyProbeOcclusionArrayFrom(occlusionProbes, 0, 0, occlusionProbes.Length);
		}

		public void CopyProbeOcclusionArrayFrom(List<Vector4> occlusionProbes, int sourceStart, int destStart, int count)
		{
			this.CopyProbeOcclusionArrayFrom(NoAllocHelpers.ExtractArrayFromListT<Vector4>(occlusionProbes), sourceStart, destStart, count);
		}

		/// <summary>
		///   <para>This function copies the source array into a Vector4 property array named unity_ProbesOcclusion with the specified source and destination range for use with instanced rendering.</para>
		/// </summary>
		/// <param name="occlusionProbes">The array of probe occlusion values to copy from.</param>
		/// <param name="sourceStart">The index of the first element in the source array to copy from.</param>
		/// <param name="destStart">The index of the first element in the destination MaterialPropertyBlock array to copy to.</param>
		/// <param name="count">The number of elements to copy.</param>
		public void CopyProbeOcclusionArrayFrom(Vector4[] occlusionProbes, int sourceStart, int destStart, int count)
		{
			if (occlusionProbes == null)
			{
				throw new ArgumentNullException("occlusionProbes");
			}
			if (sourceStart < 0)
			{
				throw new ArgumentOutOfRangeException("sourceStart", "Argument sourceStart must not be negative.");
			}
			if (destStart < 0)
			{
				throw new ArgumentOutOfRangeException("sourceStart", "Argument destStart must not be negative.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Argument count must not be negative.");
			}
			if (occlusionProbes.Length < sourceStart + count)
			{
				throw new ArgumentOutOfRangeException("The specified source start index or count is out of the range.");
			}
			MaterialPropertyBlock.Internal_CopyProbeOcclusionArrayFrom(this, occlusionProbes, sourceStart, destStart, count);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetVectorImpl_Injected(int name, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetColorImpl_Injected(int name, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetMatrixImpl_Injected(int name, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVectorImpl_Injected(int name, ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetColorImpl_Injected(int name, ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMatrixImpl_Injected(int name, ref Matrix4x4 value);

		internal IntPtr m_Ptr;
	}
}
