using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[NativeHeader("Runtime/Shaders/GpuPrograms/ShaderVariantCollection.h")]
	[NativeHeader("Runtime/Shaders/Shader.h")]
	[NativeHeader("Runtime/Shaders/ShaderNameRegistry.h")]
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Misc/ResourceManager.h")]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	public sealed class Shader : Object
	{
		private Shader()
		{
		}

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

		[FreeFunction("GetScriptMapper().FindShader")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader Find(string name);

		[FreeFunction("GetBuiltinResource<Shader>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Shader FindBuiltin(string name);

		[NativeProperty("MaximumShaderLOD")]
		public extern int maximumLOD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("GlobalMaximumShaderLOD")]
		public static extern int globalMaximumLOD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isSupported
		{
			[NativeMethod("IsSupported")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string globalRenderPipeline
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("ShaderScripting::EnableKeyword")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EnableKeyword(string keyword);

		[FreeFunction("ShaderScripting::DisableKeyword")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableKeyword(string keyword);

		[FreeFunction("ShaderScripting::IsKeywordEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsKeywordEnabled(string keyword);

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

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WarmupAllShaders();

		[FreeFunction("ShaderScripting::TagToID")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int TagToID(string name);

		[FreeFunction("ShaderScripting::IDToTag")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string IDToTag(int name);

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

		public static void SetGlobalFloat(string name, float value)
		{
			Shader.SetGlobalFloatImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalFloat(int nameID, float value)
		{
			Shader.SetGlobalFloatImpl(nameID, value);
		}

		public static void SetGlobalInt(string name, int value)
		{
			Shader.SetGlobalFloatImpl(Shader.PropertyToID(name), (float)value);
		}

		public static void SetGlobalInt(int nameID, int value)
		{
			Shader.SetGlobalFloatImpl(nameID, (float)value);
		}

		public static void SetGlobalVector(string name, Vector4 value)
		{
			Shader.SetGlobalVectorImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalVector(int nameID, Vector4 value)
		{
			Shader.SetGlobalVectorImpl(nameID, value);
		}

		public static void SetGlobalColor(string name, Color value)
		{
			Shader.SetGlobalVectorImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalColor(int nameID, Color value)
		{
			Shader.SetGlobalVectorImpl(nameID, value);
		}

		public static void SetGlobalMatrix(string name, Matrix4x4 value)
		{
			Shader.SetGlobalMatrixImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalMatrix(int nameID, Matrix4x4 value)
		{
			Shader.SetGlobalMatrixImpl(nameID, value);
		}

		public static void SetGlobalTexture(string name, Texture value)
		{
			Shader.SetGlobalTextureImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalTexture(int nameID, Texture value)
		{
			Shader.SetGlobalTextureImpl(nameID, value);
		}

		public static void SetGlobalBuffer(string name, ComputeBuffer value)
		{
			Shader.SetGlobalBufferImpl(Shader.PropertyToID(name), value);
		}

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

		public static void SetGlobalFloatArray(string name, float[] values)
		{
			Shader.SetGlobalFloatArray(Shader.PropertyToID(name), values, values.Length);
		}

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

		public static void SetGlobalVectorArray(string name, Vector4[] values)
		{
			Shader.SetGlobalVectorArray(Shader.PropertyToID(name), values, values.Length);
		}

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

		public static void SetGlobalMatrixArray(string name, Matrix4x4[] values)
		{
			Shader.SetGlobalMatrixArray(Shader.PropertyToID(name), values, values.Length);
		}

		public static void SetGlobalMatrixArray(int nameID, Matrix4x4[] values)
		{
			Shader.SetGlobalMatrixArray(nameID, values, values.Length);
		}

		public static float GetGlobalFloat(string name)
		{
			return Shader.GetGlobalFloatImpl(Shader.PropertyToID(name));
		}

		public static float GetGlobalFloat(int nameID)
		{
			return Shader.GetGlobalFloatImpl(nameID);
		}

		public static int GetGlobalInt(string name)
		{
			return (int)Shader.GetGlobalFloatImpl(Shader.PropertyToID(name));
		}

		public static int GetGlobalInt(int nameID)
		{
			return (int)Shader.GetGlobalFloatImpl(nameID);
		}

		public static Vector4 GetGlobalVector(string name)
		{
			return Shader.GetGlobalVectorImpl(Shader.PropertyToID(name));
		}

		public static Vector4 GetGlobalVector(int nameID)
		{
			return Shader.GetGlobalVectorImpl(nameID);
		}

		public static Color GetGlobalColor(string name)
		{
			return Shader.GetGlobalVectorImpl(Shader.PropertyToID(name));
		}

		public static Color GetGlobalColor(int nameID)
		{
			return Shader.GetGlobalVectorImpl(nameID);
		}

		public static Matrix4x4 GetGlobalMatrix(string name)
		{
			return Shader.GetGlobalMatrixImpl(Shader.PropertyToID(name));
		}

		public static Matrix4x4 GetGlobalMatrix(int nameID)
		{
			return Shader.GetGlobalMatrixImpl(nameID);
		}

		public static Texture GetGlobalTexture(string name)
		{
			return Shader.GetGlobalTextureImpl(Shader.PropertyToID(name));
		}

		public static Texture GetGlobalTexture(int nameID)
		{
			return Shader.GetGlobalTextureImpl(nameID);
		}

		public static float[] GetGlobalFloatArray(string name)
		{
			return Shader.GetGlobalFloatArray(Shader.PropertyToID(name));
		}

		public static float[] GetGlobalFloatArray(int nameID)
		{
			return (Shader.GetGlobalFloatArrayCountImpl(nameID) == 0) ? null : Shader.GetGlobalFloatArrayImpl(nameID);
		}

		public static Vector4[] GetGlobalVectorArray(string name)
		{
			return Shader.GetGlobalVectorArray(Shader.PropertyToID(name));
		}

		public static Vector4[] GetGlobalVectorArray(int nameID)
		{
			return (Shader.GetGlobalVectorArrayCountImpl(nameID) == 0) ? null : Shader.GetGlobalVectorArrayImpl(nameID);
		}

		public static Matrix4x4[] GetGlobalMatrixArray(string name)
		{
			return Shader.GetGlobalMatrixArray(Shader.PropertyToID(name));
		}

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
