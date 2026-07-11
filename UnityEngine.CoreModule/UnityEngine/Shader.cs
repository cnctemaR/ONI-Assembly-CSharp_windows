using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	/// <summary>
	///   <para>Shader scripts used for all rendering.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/Shader.h")]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[NativeHeader("Runtime/Shaders/ShaderNameRegistry.h")]
	[NativeHeader("Runtime/Shaders/GpuPrograms/ShaderVariantCollection.h")]
	[NativeHeader("Runtime/Misc/ResourceManager.h")]
	public sealed class Shader : Object
	{
		private Shader()
		{
		}

		/// <summary>
		///   <para>Shader hardware tier classification for current device.</para>
		/// </summary>
		[Obsolete("Use Graphics.activeTier instead (UnityUpgradable) -> UnityEngine.Graphics.activeTier", false)]
		public static ShaderHardwareTier globalShaderHardwareTier
		{
			get
			{
				return (ShaderHardwareTier)Graphics.activeTier;
			}
			set
			{
				Graphics.activeTier = (GraphicsTier)value;
			}
		}

		/// <summary>
		///   <para>Finds a shader with the given name.</para>
		/// </summary>
		/// <param name="name"></param>
		[FreeFunction("GetScriptMapper().FindShader")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader Find(string name);

		[FreeFunction("GetBuiltinResource<Shader>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Shader FindBuiltin(string name);

		/// <summary>
		///   <para>Shader LOD level for this shader.</para>
		/// </summary>
		[NativeProperty("MaximumShaderLOD")]
		public extern int maximumLOD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Shader LOD level for all shaders.</para>
		/// </summary>
		[NativeProperty("GlobalMaximumShaderLOD")]
		public static extern int globalMaximumLOD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Can this shader run on the end-users graphics card? (Read Only)</para>
		/// </summary>
		public extern bool isSupported
		{
			[NativeMethod("IsSupported")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Render pipeline currently in use.</para>
		/// </summary>
		public static extern string globalRenderPipeline
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set a global shader keyword.</para>
		/// </summary>
		/// <param name="keyword"></param>
		[FreeFunction("ShaderScripting::EnableKeyword")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EnableKeyword(string keyword);

		/// <summary>
		///   <para>Unset a global shader keyword.</para>
		/// </summary>
		/// <param name="keyword"></param>
		[FreeFunction("ShaderScripting::DisableKeyword")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableKeyword(string keyword);

		/// <summary>
		///   <para>Is global shader keyword enabled?</para>
		/// </summary>
		/// <param name="keyword"></param>
		[FreeFunction("ShaderScripting::IsKeywordEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsKeywordEnabled(string keyword);

		/// <summary>
		///   <para>Render queue of this shader. (Read Only)</para>
		/// </summary>
		public extern int renderQueue
		{
			[FreeFunction("ShaderScripting::GetRenderQueue", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern DisableBatchingType disableBatching
		{
			[FreeFunction("ShaderScripting::GetDisableBatchingType", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Fully load all shaders to prevent future performance hiccups.</para>
		/// </summary>
		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WarmupAllShaders();

		[FreeFunction("ShaderScripting::TagToID")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int TagToID(string name);

		[FreeFunction("ShaderScripting::IDToTag")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string IDToTag(int name);

		/// <summary>
		///   <para>Gets unique identifier for a shader property name.</para>
		/// </summary>
		/// <param name="name">Shader property name.</param>
		/// <returns>
		///   <para>Unique integer for the name.</para>
		/// </returns>
		[FreeFunction(Name = "ShaderScripting::PropertyToID", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int PropertyToID(string name);

		[FreeFunction("ShaderScripting::SetGlobalFloat")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalFloatImpl(int name, float value);

		[FreeFunction("ShaderScripting::SetGlobalVector")]
		private static void SetGlobalVectorImpl(int name, Vector4 value)
		{
			Shader.SetGlobalVectorImpl_Injected(name, ref value);
		}

		[FreeFunction("ShaderScripting::SetGlobalMatrix")]
		private static void SetGlobalMatrixImpl(int name, Matrix4x4 value)
		{
			Shader.SetGlobalMatrixImpl_Injected(name, ref value);
		}

		[FreeFunction("ShaderScripting::SetGlobalTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalTextureImpl(int name, Texture value);

		[FreeFunction("ShaderScripting::SetGlobalBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalBufferImpl(int name, ComputeBuffer value);

		[FreeFunction("ShaderScripting::GetGlobalFloat")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetGlobalFloatImpl(int name);

		[FreeFunction("ShaderScripting::GetGlobalVector")]
		private static Vector4 GetGlobalVectorImpl(int name)
		{
			Vector4 vector;
			Shader.GetGlobalVectorImpl_Injected(name, out vector);
			return vector;
		}

		[FreeFunction("ShaderScripting::GetGlobalMatrix")]
		private static Matrix4x4 GetGlobalMatrixImpl(int name)
		{
			Matrix4x4 matrix4x;
			Shader.GetGlobalMatrixImpl_Injected(name, out matrix4x);
			return matrix4x;
		}

		[FreeFunction("ShaderScripting::GetGlobalTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture GetGlobalTextureImpl(int name);

		[FreeFunction("ShaderScripting::SetGlobalFloatArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalFloatArrayImpl(int name, float[] values, int count);

		[FreeFunction("ShaderScripting::SetGlobalVectorArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVectorArrayImpl(int name, Vector4[] values, int count);

		[FreeFunction("ShaderScripting::SetGlobalMatrixArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrixArrayImpl(int name, Matrix4x4[] values, int count);

		[FreeFunction("ShaderScripting::GetGlobalFloatArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float[] GetGlobalFloatArrayImpl(int name);

		[FreeFunction("ShaderScripting::GetGlobalVectorArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector4[] GetGlobalVectorArrayImpl(int name);

		[FreeFunction("ShaderScripting::GetGlobalMatrixArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Matrix4x4[] GetGlobalMatrixArrayImpl(int name);

		[FreeFunction("ShaderScripting::GetGlobalFloatArrayCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGlobalFloatArrayCountImpl(int name);

		[FreeFunction("ShaderScripting::GetGlobalVectorArrayCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGlobalVectorArrayCountImpl(int name);

		[FreeFunction("ShaderScripting::GetGlobalMatrixArrayCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGlobalMatrixArrayCountImpl(int name);

		[FreeFunction("ShaderScripting::ExtractGlobalFloatArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractGlobalFloatArrayImpl(int name, [Out] float[] val);

		[FreeFunction("ShaderScripting::ExtractGlobalVectorArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractGlobalVectorArrayImpl(int name, [Out] Vector4[] val);

		[FreeFunction("ShaderScripting::ExtractGlobalMatrixArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractGlobalMatrixArrayImpl(int name, [Out] Matrix4x4[] val);

		private static void SetGlobalFloatArray(int name, float[] values, int count)
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
			Shader.SetGlobalFloatArrayImpl(name, values, count);
		}

		private static void SetGlobalVectorArray(int name, Vector4[] values, int count)
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
			Shader.SetGlobalVectorArrayImpl(name, values, count);
		}

		private static void SetGlobalMatrixArray(int name, Matrix4x4[] values, int count)
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
			Shader.SetGlobalMatrixArrayImpl(name, values, count);
		}

		private static void ExtractGlobalFloatArray(int name, List<float> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int globalFloatArrayCountImpl = Shader.GetGlobalFloatArrayCountImpl(name);
			if (globalFloatArrayCountImpl > 0)
			{
				NoAllocHelpers.EnsureListElemCount<float>(values, globalFloatArrayCountImpl);
				Shader.ExtractGlobalFloatArrayImpl(name, (float[])NoAllocHelpers.ExtractArrayFromList(values));
			}
		}

		private static void ExtractGlobalVectorArray(int name, List<Vector4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int globalVectorArrayCountImpl = Shader.GetGlobalVectorArrayCountImpl(name);
			if (globalVectorArrayCountImpl > 0)
			{
				NoAllocHelpers.EnsureListElemCount<Vector4>(values, globalVectorArrayCountImpl);
				Shader.ExtractGlobalVectorArrayImpl(name, (Vector4[])NoAllocHelpers.ExtractArrayFromList(values));
			}
		}

		private static void ExtractGlobalMatrixArray(int name, List<Matrix4x4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int globalMatrixArrayCountImpl = Shader.GetGlobalMatrixArrayCountImpl(name);
			if (globalMatrixArrayCountImpl > 0)
			{
				NoAllocHelpers.EnsureListElemCount<Matrix4x4>(values, globalMatrixArrayCountImpl);
				Shader.ExtractGlobalMatrixArrayImpl(name, (Matrix4x4[])NoAllocHelpers.ExtractArrayFromList(values));
			}
		}

		/// <summary>
		///   <para>Sets a global float property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalFloat(string name, float value)
		{
			Shader.SetGlobalFloatImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Sets a global float property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalFloat(int nameID, float value)
		{
			Shader.SetGlobalFloatImpl(nameID, value);
		}

		/// <summary>
		///   <para>Sets a global int property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalInt(string name, int value)
		{
			Shader.SetGlobalFloatImpl(Shader.PropertyToID(name), (float)value);
		}

		/// <summary>
		///   <para>Sets a global int property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalInt(int nameID, int value)
		{
			Shader.SetGlobalFloatImpl(nameID, (float)value);
		}

		/// <summary>
		///   <para>Sets a global vector property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalVector(string name, Vector4 value)
		{
			Shader.SetGlobalVectorImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Sets a global vector property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalVector(int nameID, Vector4 value)
		{
			Shader.SetGlobalVectorImpl(nameID, value);
		}

		/// <summary>
		///   <para>Sets a global color property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalColor(string name, Color value)
		{
			Shader.SetGlobalVectorImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Sets a global color property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalColor(int nameID, Color value)
		{
			Shader.SetGlobalVectorImpl(nameID, value);
		}

		/// <summary>
		///   <para>Sets a global matrix property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalMatrix(string name, Matrix4x4 value)
		{
			Shader.SetGlobalMatrixImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Sets a global matrix property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalMatrix(int nameID, Matrix4x4 value)
		{
			Shader.SetGlobalMatrixImpl(nameID, value);
		}

		/// <summary>
		///   <para>Sets a global texture property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalTexture(string name, Texture value)
		{
			Shader.SetGlobalTextureImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Sets a global texture property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalTexture(int nameID, Texture value)
		{
			Shader.SetGlobalTextureImpl(nameID, value);
		}

		/// <summary>
		///   <para>Sets a global compute buffer property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalBuffer(string name, ComputeBuffer value)
		{
			Shader.SetGlobalBufferImpl(Shader.PropertyToID(name), value);
		}

		/// <summary>
		///   <para>Sets a global compute buffer property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="value"></param>
		public static void SetGlobalBuffer(int nameID, ComputeBuffer value)
		{
			Shader.SetGlobalBufferImpl(nameID, value);
		}

		public static void SetGlobalFloatArray(string name, List<float> values)
		{
			Shader.SetGlobalFloatArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT<float>(values), values.Count);
		}

		public static void SetGlobalFloatArray(int nameID, List<float> values)
		{
			Shader.SetGlobalFloatArray(nameID, NoAllocHelpers.ExtractArrayFromListT<float>(values), values.Count);
		}

		/// <summary>
		///   <para>Sets a global float array property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="values"></param>
		public static void SetGlobalFloatArray(string name, float[] values)
		{
			Shader.SetGlobalFloatArray(Shader.PropertyToID(name), values, values.Length);
		}

		/// <summary>
		///   <para>Sets a global float array property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="values"></param>
		public static void SetGlobalFloatArray(int nameID, float[] values)
		{
			Shader.SetGlobalFloatArray(nameID, values, values.Length);
		}

		public static void SetGlobalVectorArray(string name, List<Vector4> values)
		{
			Shader.SetGlobalVectorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT<Vector4>(values), values.Count);
		}

		public static void SetGlobalVectorArray(int nameID, List<Vector4> values)
		{
			Shader.SetGlobalVectorArray(nameID, NoAllocHelpers.ExtractArrayFromListT<Vector4>(values), values.Count);
		}

		/// <summary>
		///   <para>Sets a global vector array property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="values"></param>
		public static void SetGlobalVectorArray(string name, Vector4[] values)
		{
			Shader.SetGlobalVectorArray(Shader.PropertyToID(name), values, values.Length);
		}

		/// <summary>
		///   <para>Sets a global vector array property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="values"></param>
		public static void SetGlobalVectorArray(int nameID, Vector4[] values)
		{
			Shader.SetGlobalVectorArray(nameID, values, values.Length);
		}

		public static void SetGlobalMatrixArray(string name, List<Matrix4x4> values)
		{
			Shader.SetGlobalMatrixArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromListT<Matrix4x4>(values), values.Count);
		}

		public static void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
		{
			Shader.SetGlobalMatrixArray(nameID, NoAllocHelpers.ExtractArrayFromListT<Matrix4x4>(values), values.Count);
		}

		/// <summary>
		///   <para>Sets a global matrix array property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="values"></param>
		public static void SetGlobalMatrixArray(string name, Matrix4x4[] values)
		{
			Shader.SetGlobalMatrixArray(Shader.PropertyToID(name), values, values.Length);
		}

		/// <summary>
		///   <para>Sets a global matrix array property for all shaders.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="values"></param>
		public static void SetGlobalMatrixArray(int nameID, Matrix4x4[] values)
		{
			Shader.SetGlobalMatrixArray(nameID, values, values.Length);
		}

		/// <summary>
		///   <para>Gets a global float property for all shaders previously set using SetGlobalFloat.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static float GetGlobalFloat(string name)
		{
			return Shader.GetGlobalFloatImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Gets a global float property for all shaders previously set using SetGlobalFloat.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static float GetGlobalFloat(int nameID)
		{
			return Shader.GetGlobalFloatImpl(nameID);
		}

		/// <summary>
		///   <para>Gets a global int property for all shaders previously set using SetGlobalInt.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static int GetGlobalInt(string name)
		{
			return (int)Shader.GetGlobalFloatImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Gets a global int property for all shaders previously set using SetGlobalInt.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static int GetGlobalInt(int nameID)
		{
			return (int)Shader.GetGlobalFloatImpl(nameID);
		}

		/// <summary>
		///   <para>Gets a global vector property for all shaders previously set using SetGlobalVector.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Vector4 GetGlobalVector(string name)
		{
			return Shader.GetGlobalVectorImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Gets a global vector property for all shaders previously set using SetGlobalVector.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Vector4 GetGlobalVector(int nameID)
		{
			return Shader.GetGlobalVectorImpl(nameID);
		}

		/// <summary>
		///   <para>Gets a global color property for all shaders previously set using SetGlobalColor.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Color GetGlobalColor(string name)
		{
			return Shader.GetGlobalVectorImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Gets a global color property for all shaders previously set using SetGlobalColor.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Color GetGlobalColor(int nameID)
		{
			return Shader.GetGlobalVectorImpl(nameID);
		}

		/// <summary>
		///   <para>Gets a global matrix property for all shaders previously set using SetGlobalMatrix.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Matrix4x4 GetGlobalMatrix(string name)
		{
			return Shader.GetGlobalMatrixImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Gets a global matrix property for all shaders previously set using SetGlobalMatrix.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Matrix4x4 GetGlobalMatrix(int nameID)
		{
			return Shader.GetGlobalMatrixImpl(nameID);
		}

		/// <summary>
		///   <para>Gets a global texture property for all shaders previously set using SetGlobalTexture.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Texture GetGlobalTexture(string name)
		{
			return Shader.GetGlobalTextureImpl(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Gets a global texture property for all shaders previously set using SetGlobalTexture.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Texture GetGlobalTexture(int nameID)
		{
			return Shader.GetGlobalTextureImpl(nameID);
		}

		/// <summary>
		///   <para>Gets a global float array for all shaders previously set using SetGlobalFloatArray.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static float[] GetGlobalFloatArray(string name)
		{
			return Shader.GetGlobalFloatArray(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Gets a global float array for all shaders previously set using SetGlobalFloatArray.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static float[] GetGlobalFloatArray(int nameID)
		{
			return (Shader.GetGlobalFloatArrayCountImpl(nameID) == 0) ? null : Shader.GetGlobalFloatArrayImpl(nameID);
		}

		/// <summary>
		///   <para>Gets a global vector array for all shaders previously set using SetGlobalVectorArray.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Vector4[] GetGlobalVectorArray(string name)
		{
			return Shader.GetGlobalVectorArray(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Gets a global vector array for all shaders previously set using SetGlobalVectorArray.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Vector4[] GetGlobalVectorArray(int nameID)
		{
			return (Shader.GetGlobalVectorArrayCountImpl(nameID) == 0) ? null : Shader.GetGlobalVectorArrayImpl(nameID);
		}

		/// <summary>
		///   <para>Gets a global matrix array for all shaders previously set using SetGlobalMatrixArray.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Matrix4x4[] GetGlobalMatrixArray(string name)
		{
			return Shader.GetGlobalMatrixArray(Shader.PropertyToID(name));
		}

		/// <summary>
		///   <para>Gets a global matrix array for all shaders previously set using SetGlobalMatrixArray.</para>
		/// </summary>
		/// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
		/// <param name="name">The name of the property.</param>
		public static Matrix4x4[] GetGlobalMatrixArray(int nameID)
		{
			return (Shader.GetGlobalMatrixArrayCountImpl(nameID) == 0) ? null : Shader.GetGlobalMatrixArrayImpl(nameID);
		}

		public static void GetGlobalFloatArray(string name, List<float> values)
		{
			Shader.ExtractGlobalFloatArray(Shader.PropertyToID(name), values);
		}

		public static void GetGlobalFloatArray(int nameID, List<float> values)
		{
			Shader.ExtractGlobalFloatArray(nameID, values);
		}

		public static void GetGlobalVectorArray(string name, List<Vector4> values)
		{
			Shader.ExtractGlobalVectorArray(Shader.PropertyToID(name), values);
		}

		public static void GetGlobalVectorArray(int nameID, List<Vector4> values)
		{
			Shader.ExtractGlobalVectorArray(nameID, values);
		}

		public static void GetGlobalMatrixArray(string name, List<Matrix4x4> values)
		{
			Shader.ExtractGlobalMatrixArray(Shader.PropertyToID(name), values);
		}

		public static void GetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
		{
			Shader.ExtractGlobalMatrixArray(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVectorImpl_Injected(int name, ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrixImpl_Injected(int name, ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalVectorImpl_Injected(int name, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalMatrixImpl_Injected(int name, out Matrix4x4 ret);
	}
}
