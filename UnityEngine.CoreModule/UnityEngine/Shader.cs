using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[NativeHeader("Runtime/Shaders/GpuPrograms/ShaderVariantCollection.h")]
	[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
	[NativeHeader("Runtime/Misc/ResourceManager.h")]
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/ShaderNameRegistry.h")]
	[NativeHeader("Runtime/Shaders/Shader.h")]
	public sealed class Shader : Object
	{
		[Obsolete("Use Graphics.activeTier instead (UnityUpgradable) -> UnityEngine.Graphics.activeTier", true)]
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

		public static Shader Find(string name)
		{
			return ResourcesAPI.ActiveAPI.FindShaderByName(name);
		}

		[FreeFunction("GetBuiltinResource<Shader>")]
		internal unsafe static Shader FindBuiltin(string name)
		{
			Shader shader;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = Shader.FindBuiltin_Injected(ref managedSpanWrapper);
			}
			finally
			{
				IntPtr intPtr;
				shader = Unmarshal.UnmarshalUnityObject<Shader>(intPtr);
				char* ptr = null;
			}
			return shader;
		}

		[FreeFunction("ShaderScripting::CreateFromCompiledData")]
		internal unsafe static Shader CreateFromCompiledData(byte[] compiledData, Shader[] dependencies)
		{
			Span<byte> span = new Span<byte>(compiledData);
			Shader shader;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				shader = Unmarshal.UnmarshalUnityObject<Shader>(Shader.CreateFromCompiledData_Injected(ref managedSpanWrapper, dependencies));
			}
			return shader;
		}

		[NativeProperty("MaxChunksRuntimeOverride")]
		public static extern int maximumChunksOverride
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("MaximumShaderLOD")]
		public int maximumLOD
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Shader.get_maximumLOD_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Shader.set_maximumLOD_Injected(intPtr, value);
			}
		}

		[NativeProperty("GlobalMaximumShaderLOD")]
		public static extern int globalMaximumLOD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public bool isSupported
		{
			[NativeMethod("IsSupported")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Shader.get_isSupported_Injected(intPtr);
			}
		}

		public unsafe static string globalRenderPipeline
		{
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Shader.get_globalRenderPipeline_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					Shader.set_globalRenderPipeline_Injected(ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public static GlobalKeyword[] enabledGlobalKeywords
		{
			get
			{
				return Shader.GetEnabledGlobalKeywords();
			}
		}

		public static GlobalKeyword[] globalKeywords
		{
			get
			{
				return Shader.GetAllGlobalKeywords();
			}
		}

		public LocalKeywordSpace keywordSpace
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LocalKeywordSpace localKeywordSpace;
				Shader.get_keywordSpace_Injected(intPtr, out localKeywordSpace);
				return localKeywordSpace;
			}
		}

		[FreeFunction("keywords::GetEnabledGlobalKeywords")]
		internal static GlobalKeyword[] GetEnabledGlobalKeywords()
		{
			GlobalKeyword[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Shader.GetEnabledGlobalKeywords_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				GlobalKeyword[] array;
				blittableArrayWrapper.Unmarshal<GlobalKeyword>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("keywords::GetAllGlobalKeywords")]
		internal static GlobalKeyword[] GetAllGlobalKeywords()
		{
			GlobalKeyword[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Shader.GetAllGlobalKeywords_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				GlobalKeyword[] array;
				blittableArrayWrapper.Unmarshal<GlobalKeyword>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("ShaderScripting::EnableKeyword")]
		public unsafe static void EnableKeyword(string keyword)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Shader.EnableKeyword_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("ShaderScripting::DisableKeyword")]
		public unsafe static void DisableKeyword(string keyword)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Shader.DisableKeyword_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("ShaderScripting::IsKeywordEnabled")]
		public unsafe static bool IsKeywordEnabled(string keyword)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Shader.IsKeywordEnabled_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction("ShaderScripting::EnableKeyword")]
		internal static void EnableKeywordFast(GlobalKeyword keyword)
		{
			Shader.EnableKeywordFast_Injected(ref keyword);
		}

		[FreeFunction("ShaderScripting::DisableKeyword")]
		internal static void DisableKeywordFast(GlobalKeyword keyword)
		{
			Shader.DisableKeywordFast_Injected(ref keyword);
		}

		[FreeFunction("ShaderScripting::SetKeyword")]
		internal static void SetKeywordFast(GlobalKeyword keyword, bool value)
		{
			Shader.SetKeywordFast_Injected(ref keyword, value);
		}

		[FreeFunction("ShaderScripting::IsKeywordEnabled")]
		internal static bool IsKeywordEnabledFast(GlobalKeyword keyword)
		{
			return Shader.IsKeywordEnabledFast_Injected(ref keyword);
		}

		public static void EnableKeyword(in GlobalKeyword keyword)
		{
			Shader.EnableKeywordFast(keyword);
		}

		public static void DisableKeyword(in GlobalKeyword keyword)
		{
			Shader.DisableKeywordFast(keyword);
		}

		public static void SetKeyword(in GlobalKeyword keyword, bool value)
		{
			Shader.SetKeywordFast(keyword, value);
		}

		public static bool IsKeywordEnabled(in GlobalKeyword keyword)
		{
			return Shader.IsKeywordEnabledFast(keyword);
		}

		[FreeFunction("ShaderScripting::GetGlobalPropertyCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetGlobalPropertyCount();

		[FreeFunction("ShaderScripting::GetGlobalPropertyCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGlobalPropertyCountImpl(int propertyType);

		[FreeFunction("ShaderScripting::ExtractGlobalPropertyNames")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractGlobalPropertyNamesImpl(int propertyType, [Out] string[] names);

		public int renderQueue
		{
			[FreeFunction("ShaderScripting::GetRenderQueue", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Shader.get_renderQueue_Injected(intPtr);
			}
		}

		internal DisableBatchingType disableBatching
		{
			[FreeFunction("ShaderScripting::GetDisableBatchingType", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Shader.get_disableBatching_Injected(intPtr);
			}
		}

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WarmupAllShaders();

		[FreeFunction("ShaderScripting::TagToID")]
		internal unsafe static int TagToID(string name)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = Shader.TagToID_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		[FreeFunction("ShaderScripting::IDToTag")]
		internal static string IDToTag(int name)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				Shader.IDToTag_Injected(name, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction(Name = "ShaderScripting::PropertyToID", IsThreadSafe = true)]
		public unsafe static int PropertyToID(string name)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = Shader.PropertyToID_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public unsafe Shader GetDependency(string name)
		{
			Shader shader;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
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
				IntPtr dependency_Injected = Shader.GetDependency_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				IntPtr dependency_Injected;
				shader = Unmarshal.UnmarshalUnityObject<Shader>(dependency_Injected);
				char* ptr = null;
			}
			return shader;
		}

		public int passCount
		{
			[FreeFunction(Name = "ShaderScripting::GetPassCount", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Shader.get_passCount_Injected(intPtr);
			}
		}

		public int subshaderCount
		{
			[FreeFunction(Name = "ShaderScripting::GetSubshaderCount", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Shader.get_subshaderCount_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "ShaderScripting::GetPassCountInSubshader", HasExplicitThis = true)]
		public int GetPassCountInSubshader(int subshaderIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Shader.GetPassCountInSubshader_Injected(intPtr, subshaderIndex);
		}

		public ShaderTagId FindPassTagValue(int passIndex, ShaderTagId tagName)
		{
			bool flag = passIndex < 0 || passIndex >= this.passCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("passIndex");
			}
			int num = this.Internal_FindPassTagValue(passIndex, tagName.id);
			return new ShaderTagId
			{
				id = num
			};
		}

		public ShaderTagId FindPassTagValue(int subshaderIndex, int passIndex, ShaderTagId tagName)
		{
			bool flag = subshaderIndex < 0 || subshaderIndex >= this.subshaderCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("subshaderIndex");
			}
			bool flag2 = passIndex < 0 || passIndex >= this.GetPassCountInSubshader(subshaderIndex);
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("passIndex");
			}
			int num = this.Internal_FindPassTagValueInSubShader(subshaderIndex, passIndex, tagName.id);
			return new ShaderTagId
			{
				id = num
			};
		}

		public ShaderTagId FindSubshaderTagValue(int subshaderIndex, ShaderTagId tagName)
		{
			bool flag = subshaderIndex < 0 || subshaderIndex >= this.subshaderCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid subshaderIndex {0}. Value must be in the range [0, {1})", subshaderIndex, this.subshaderCount));
			}
			int num = this.Internal_FindSubshaderTagValue(subshaderIndex, tagName.id);
			return new ShaderTagId
			{
				id = num
			};
		}

		[FreeFunction(Name = "ShaderScripting::FindPassTagValue", HasExplicitThis = true)]
		private int Internal_FindPassTagValue(int passIndex, int tagName)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Shader.Internal_FindPassTagValue_Injected(intPtr, passIndex, tagName);
		}

		[FreeFunction(Name = "ShaderScripting::FindPassTagValue", HasExplicitThis = true)]
		private int Internal_FindPassTagValueInSubShader(int subShaderIndex, int passIndex, int tagName)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Shader.Internal_FindPassTagValueInSubShader_Injected(intPtr, subShaderIndex, passIndex, tagName);
		}

		[FreeFunction(Name = "ShaderScripting::FindSubshaderTagValue", HasExplicitThis = true)]
		private int Internal_FindSubshaderTagValue(int subShaderIndex, int tagName)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Shader.Internal_FindSubshaderTagValue_Injected(intPtr, subShaderIndex, tagName);
		}

		[FreeFunction("ShaderScripting::SetGlobalInt")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalIntImpl(int name, int value);

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
		private static void SetGlobalTextureImpl(int name, Texture value)
		{
			Shader.SetGlobalTextureImpl_Injected(name, Object.MarshalledUnityObject.Marshal<Texture>(value));
		}

		[FreeFunction("ShaderScripting::SetGlobalRenderTexture")]
		private static void SetGlobalRenderTextureImpl(int name, RenderTexture value, RenderTextureSubElement element)
		{
			Shader.SetGlobalRenderTextureImpl_Injected(name, Object.MarshalledUnityObject.Marshal<RenderTexture>(value), element);
		}

		[FreeFunction("ShaderScripting::SetGlobalBuffer")]
		private static void SetGlobalBufferImpl(int name, ComputeBuffer value)
		{
			Shader.SetGlobalBufferImpl_Injected(name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value));
		}

		[FreeFunction("ShaderScripting::SetGlobalBuffer")]
		private static void SetGlobalGraphicsBufferImpl(int name, GraphicsBuffer value)
		{
			Shader.SetGlobalGraphicsBufferImpl_Injected(name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value));
		}

		[FreeFunction("ShaderScripting::SetGlobalConstantBuffer")]
		private static void SetGlobalConstantBufferImpl(int name, ComputeBuffer value, int offset, int size)
		{
			Shader.SetGlobalConstantBufferImpl_Injected(name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
		}

		[FreeFunction("ShaderScripting::SetGlobalConstantBuffer")]
		private static void SetGlobalConstantGraphicsBufferImpl(int name, GraphicsBuffer value, int offset, int size)
		{
			Shader.SetGlobalConstantGraphicsBufferImpl_Injected(name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
		}

		[FreeFunction("ShaderScripting::SetGlobalRayTracingAccelerationStructure")]
		private static void SetGlobalRayTracingAccelerationStructureImpl(int name, RayTracingAccelerationStructure accelerationStructure)
		{
			Shader.SetGlobalRayTracingAccelerationStructureImpl_Injected(name, (accelerationStructure == null) ? ((IntPtr)0) : RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelerationStructure));
		}

		[FreeFunction("ShaderScripting::GetGlobalInt")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGlobalIntImpl(int name);

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
		private static Texture GetGlobalTextureImpl(int name)
		{
			return Unmarshal.UnmarshalUnityObject<Texture>(Shader.GetGlobalTextureImpl_Injected(name));
		}

		[FreeFunction("ShaderScripting::SetGlobalFloatArray")]
		private unsafe static void SetGlobalFloatArrayImpl(int name, float[] values, int count)
		{
			Span<float> span = new Span<float>(values);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Shader.SetGlobalFloatArrayImpl_Injected(name, ref managedSpanWrapper, count);
			}
		}

		[FreeFunction("ShaderScripting::SetGlobalVectorArray")]
		private unsafe static void SetGlobalVectorArrayImpl(int name, Vector4[] values, int count)
		{
			Span<Vector4> span = new Span<Vector4>(values);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Shader.SetGlobalVectorArrayImpl_Injected(name, ref managedSpanWrapper, count);
			}
		}

		[FreeFunction("ShaderScripting::SetGlobalMatrixArray")]
		private unsafe static void SetGlobalMatrixArrayImpl(int name, Matrix4x4[] values, int count)
		{
			Span<Matrix4x4> span = new Span<Matrix4x4>(values);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Shader.SetGlobalMatrixArrayImpl_Injected(name, ref managedSpanWrapper, count);
			}
		}

		[FreeFunction("ShaderScripting::GetGlobalFloatArray")]
		private static float[] GetGlobalFloatArrayImpl(int name)
		{
			float[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Shader.GetGlobalFloatArrayImpl_Injected(name, out blittableArrayWrapper);
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

		[FreeFunction("ShaderScripting::GetGlobalVectorArray")]
		private static Vector4[] GetGlobalVectorArrayImpl(int name)
		{
			Vector4[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Shader.GetGlobalVectorArrayImpl_Injected(name, out blittableArrayWrapper);
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

		[FreeFunction("ShaderScripting::GetGlobalMatrixArray")]
		private static Matrix4x4[] GetGlobalMatrixArrayImpl(int name)
		{
			Matrix4x4[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Shader.GetGlobalMatrixArrayImpl_Injected(name, out blittableArrayWrapper);
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
		private unsafe static void ExtractGlobalFloatArrayImpl(int name, [Out] float[] val)
		{
			try
			{
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
				Shader.ExtractGlobalFloatArrayImpl_Injected(name, out blittableArrayWrapper);
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
			}
		}

		[FreeFunction("ShaderScripting::ExtractGlobalVectorArray")]
		private unsafe static void ExtractGlobalVectorArrayImpl(int name, [Out] Vector4[] val)
		{
			try
			{
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
				Shader.ExtractGlobalVectorArrayImpl_Injected(name, out blittableArrayWrapper);
			}
			finally
			{
				Vector4[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Vector4>(ref array);
			}
		}

		[FreeFunction("ShaderScripting::ExtractGlobalMatrixArray")]
		private unsafe static void ExtractGlobalMatrixArrayImpl(int name, [Out] Matrix4x4[] val)
		{
			try
			{
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
				Shader.ExtractGlobalMatrixArrayImpl_Injected(name, out blittableArrayWrapper);
			}
			finally
			{
				Matrix4x4[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Matrix4x4>(ref array);
			}
		}

		private static void SetGlobalFloatArray(int name, float[] values, int count)
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
			Shader.SetGlobalFloatArrayImpl(name, values, count);
		}

		private static void SetGlobalVectorArray(int name, Vector4[] values, int count)
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
			Shader.SetGlobalVectorArrayImpl(name, values, count);
		}

		private static void SetGlobalMatrixArray(int name, Matrix4x4[] values, int count)
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
			Shader.SetGlobalMatrixArrayImpl(name, values, count);
		}

		private static void ExtractGlobalFloatArray(int name, List<float> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int globalFloatArrayCountImpl = Shader.GetGlobalFloatArrayCountImpl(name);
			bool flag2 = globalFloatArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<float>(values, globalFloatArrayCountImpl);
				Shader.ExtractGlobalFloatArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<float>(values));
			}
		}

		private static void ExtractGlobalVectorArray(int name, List<Vector4> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int globalVectorArrayCountImpl = Shader.GetGlobalVectorArrayCountImpl(name);
			bool flag2 = globalVectorArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<Vector4>(values, globalVectorArrayCountImpl);
				Shader.ExtractGlobalVectorArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<Vector4>(values));
			}
		}

		private static void ExtractGlobalMatrixArray(int name, List<Matrix4x4> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int globalMatrixArrayCountImpl = Shader.GetGlobalMatrixArrayCountImpl(name);
			bool flag2 = globalMatrixArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<Matrix4x4>(values, globalMatrixArrayCountImpl);
				Shader.ExtractGlobalMatrixArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(values));
			}
		}

		private static void ExtractGlobalPropertyNames(MaterialPropertyType type, List<string> names)
		{
			bool flag = names == null;
			if (flag)
			{
				throw new ArgumentNullException("names");
			}
			names.Clear();
			int globalPropertyCountImpl = Shader.GetGlobalPropertyCountImpl((int)type);
			bool flag2 = globalPropertyCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<string>(names, globalPropertyCountImpl);
				Shader.ExtractGlobalPropertyNamesImpl((int)type, NoAllocHelpers.ExtractArrayFromList<string>(names));
			}
		}

		public static void SetGlobalInt(string name, int value)
		{
			Shader.SetGlobalFloatImpl(Shader.PropertyToID(name), (float)value);
		}

		public static void SetGlobalInt(int nameID, int value)
		{
			Shader.SetGlobalFloatImpl(nameID, (float)value);
		}

		public static void SetGlobalFloat(string name, float value)
		{
			Shader.SetGlobalFloatImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalFloat(int nameID, float value)
		{
			Shader.SetGlobalFloatImpl(nameID, value);
		}

		public static void SetGlobalInteger(string name, int value)
		{
			Shader.SetGlobalIntImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalInteger(int nameID, int value)
		{
			Shader.SetGlobalIntImpl(nameID, value);
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

		public static void SetGlobalTexture(string name, RenderTexture value, RenderTextureSubElement element)
		{
			Shader.SetGlobalRenderTextureImpl(Shader.PropertyToID(name), value, element);
		}

		public static void SetGlobalTexture(int nameID, RenderTexture value, RenderTextureSubElement element)
		{
			Shader.SetGlobalRenderTextureImpl(nameID, value, element);
		}

		public static void SetGlobalBuffer(string name, ComputeBuffer value)
		{
			Shader.SetGlobalBufferImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalBuffer(int nameID, ComputeBuffer value)
		{
			Shader.SetGlobalBufferImpl(nameID, value);
		}

		public static void SetGlobalBuffer(string name, GraphicsBuffer value)
		{
			Shader.SetGlobalGraphicsBufferImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalBuffer(int nameID, GraphicsBuffer value)
		{
			Shader.SetGlobalGraphicsBufferImpl(nameID, value);
		}

		public static void SetGlobalConstantBuffer(string name, ComputeBuffer value, int offset, int size)
		{
			Shader.SetGlobalConstantBufferImpl(Shader.PropertyToID(name), value, offset, size);
		}

		public static void SetGlobalConstantBuffer(int nameID, ComputeBuffer value, int offset, int size)
		{
			Shader.SetGlobalConstantBufferImpl(nameID, value, offset, size);
		}

		public static void SetGlobalConstantBuffer(string name, GraphicsBuffer value, int offset, int size)
		{
			Shader.SetGlobalConstantGraphicsBufferImpl(Shader.PropertyToID(name), value, offset, size);
		}

		public static void SetGlobalConstantBuffer(int nameID, GraphicsBuffer value, int offset, int size)
		{
			Shader.SetGlobalConstantGraphicsBufferImpl(nameID, value, offset, size);
		}

		public static void SetGlobalRayTracingAccelerationStructure(string name, RayTracingAccelerationStructure value)
		{
			Shader.SetGlobalRayTracingAccelerationStructureImpl(Shader.PropertyToID(name), value);
		}

		public static void SetGlobalRayTracingAccelerationStructure(int nameID, RayTracingAccelerationStructure value)
		{
			Shader.SetGlobalRayTracingAccelerationStructureImpl(nameID, value);
		}

		public static void SetGlobalFloatArray(string name, List<float> values)
		{
			Shader.SetGlobalFloatArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<float>(values), values.Count);
		}

		public static void SetGlobalFloatArray(int nameID, List<float> values)
		{
			Shader.SetGlobalFloatArray(nameID, NoAllocHelpers.ExtractArrayFromList<float>(values), values.Count);
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
			Shader.SetGlobalVectorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<Vector4>(values), values.Count);
		}

		public static void SetGlobalVectorArray(int nameID, List<Vector4> values)
		{
			Shader.SetGlobalVectorArray(nameID, NoAllocHelpers.ExtractArrayFromList<Vector4>(values), values.Count);
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
			Shader.SetGlobalMatrixArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(values), values.Count);
		}

		public static void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
		{
			Shader.SetGlobalMatrixArray(nameID, NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(values), values.Count);
		}

		public static void SetGlobalMatrixArray(string name, Matrix4x4[] values)
		{
			Shader.SetGlobalMatrixArray(Shader.PropertyToID(name), values, values.Length);
		}

		public static void SetGlobalMatrixArray(int nameID, Matrix4x4[] values)
		{
			Shader.SetGlobalMatrixArray(nameID, values, values.Length);
		}

		public static int GetGlobalInt(string name)
		{
			return (int)Shader.GetGlobalFloatImpl(Shader.PropertyToID(name));
		}

		public static int GetGlobalInt(int nameID)
		{
			return (int)Shader.GetGlobalFloatImpl(nameID);
		}

		public static float GetGlobalFloat(string name)
		{
			return Shader.GetGlobalFloatImpl(Shader.PropertyToID(name));
		}

		public static float GetGlobalFloat(int nameID)
		{
			return Shader.GetGlobalFloatImpl(nameID);
		}

		public static int GetGlobalInteger(string name)
		{
			return Shader.GetGlobalIntImpl(Shader.PropertyToID(name));
		}

		public static int GetGlobalInteger(int nameID)
		{
			return Shader.GetGlobalIntImpl(nameID);
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
			return (Shader.GetGlobalFloatArrayCountImpl(nameID) != 0) ? Shader.GetGlobalFloatArrayImpl(nameID) : null;
		}

		public static Vector4[] GetGlobalVectorArray(string name)
		{
			return Shader.GetGlobalVectorArray(Shader.PropertyToID(name));
		}

		public static Vector4[] GetGlobalVectorArray(int nameID)
		{
			return (Shader.GetGlobalVectorArrayCountImpl(nameID) != 0) ? Shader.GetGlobalVectorArrayImpl(nameID) : null;
		}

		public static Matrix4x4[] GetGlobalMatrixArray(string name)
		{
			return Shader.GetGlobalMatrixArray(Shader.PropertyToID(name));
		}

		public static Matrix4x4[] GetGlobalMatrixArray(int nameID)
		{
			return (Shader.GetGlobalMatrixArrayCountImpl(nameID) != 0) ? Shader.GetGlobalMatrixArrayImpl(nameID) : null;
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

		internal static void GetGlobalPropertyNames(MaterialPropertyType type, List<string> names)
		{
			Shader.ExtractGlobalPropertyNames(type, names);
		}

		private Shader()
		{
		}

		[FreeFunction("ShaderScripting::GetPropertyName")]
		private static string GetPropertyName([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(shader, "shader");
				}
				ManagedSpanWrapper managedSpanWrapper;
				Shader.GetPropertyName_Injected(intPtr, propertyIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction("ShaderScripting::GetPropertyNameId")]
		private static int GetPropertyNameId([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			return Shader.GetPropertyNameId_Injected(intPtr, propertyIndex);
		}

		[FreeFunction("ShaderScripting::GetPropertyType")]
		private static ShaderPropertyType GetPropertyType([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			return Shader.GetPropertyType_Injected(intPtr, propertyIndex);
		}

		[FreeFunction("ShaderScripting::GetPropertyDescription")]
		private static string GetPropertyDescription([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(shader, "shader");
				}
				ManagedSpanWrapper managedSpanWrapper;
				Shader.GetPropertyDescription_Injected(intPtr, propertyIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction("ShaderScripting::GetPropertyFlags")]
		private static ShaderPropertyFlags GetPropertyFlags([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			return Shader.GetPropertyFlags_Injected(intPtr, propertyIndex);
		}

		[FreeFunction("ShaderScripting::GetPropertyAttributes")]
		private static string[] GetPropertyAttributes([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			return Shader.GetPropertyAttributes_Injected(intPtr, propertyIndex);
		}

		[FreeFunction("ShaderScripting::GetPropertyDefaultIntValue")]
		private static int GetPropertyDefaultIntValue([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			return Shader.GetPropertyDefaultIntValue_Injected(intPtr, propertyIndex);
		}

		[FreeFunction("ShaderScripting::GetPropertyDefaultValue")]
		private static Vector4 GetPropertyDefaultValue([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			Vector4 vector;
			Shader.GetPropertyDefaultValue_Injected(intPtr, propertyIndex, out vector);
			return vector;
		}

		[FreeFunction("ShaderScripting::GetPropertyTextureDimension")]
		private static TextureDimension GetPropertyTextureDimension([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			return Shader.GetPropertyTextureDimension_Injected(intPtr, propertyIndex);
		}

		[FreeFunction("ShaderScripting::GetPropertyTextureDefaultName")]
		private static string GetPropertyTextureDefaultName([NotNull] Shader shader, int propertyIndex)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(shader, "shader");
				}
				ManagedSpanWrapper managedSpanWrapper;
				Shader.GetPropertyTextureDefaultName_Injected(intPtr, propertyIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction("ShaderScripting::FindTextureStack")]
		private static bool FindTextureStackImpl([NotNull] Shader s, int propertyIdx, out string stackName, out int layerIndex)
		{
			if (s == null)
			{
				ThrowHelper.ThrowArgumentNullException(s, "s");
			}
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(s);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(s, "s");
				}
				ManagedSpanWrapper managedSpanWrapper;
				flag = Shader.FindTextureStackImpl_Injected(intPtr, propertyIdx, out managedSpanWrapper, out layerIndex);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stackName = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return flag;
		}

		private static void CheckPropertyIndex(Shader s, int propertyIndex)
		{
			bool flag = propertyIndex < 0 || propertyIndex >= s.GetPropertyCount();
			if (flag)
			{
				throw new ArgumentOutOfRangeException("propertyIndex");
			}
		}

		public int GetPropertyCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Shader.GetPropertyCount_Injected(intPtr);
		}

		public unsafe int FindPropertyIndex(string propertyName)
		{
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = propertyName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = Shader.FindPropertyIndex_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public string GetPropertyName(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			return Shader.GetPropertyName(this, propertyIndex);
		}

		public int GetPropertyNameId(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			return Shader.GetPropertyNameId(this, propertyIndex);
		}

		public ShaderPropertyType GetPropertyType(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			return Shader.GetPropertyType(this, propertyIndex);
		}

		public string GetPropertyDescription(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			return Shader.GetPropertyDescription(this, propertyIndex);
		}

		public ShaderPropertyFlags GetPropertyFlags(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			return Shader.GetPropertyFlags(this, propertyIndex);
		}

		public string[] GetPropertyAttributes(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			return Shader.GetPropertyAttributes(this, propertyIndex);
		}

		public float GetPropertyDefaultFloatValue(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			ShaderPropertyType propertyType = this.GetPropertyType(propertyIndex);
			bool flag = propertyType != ShaderPropertyType.Float && propertyType != ShaderPropertyType.Range;
			if (flag)
			{
				throw new ArgumentException("Property type is not Float or Range.");
			}
			return Shader.GetPropertyDefaultValue(this, propertyIndex)[0];
		}

		public Vector4 GetPropertyDefaultVectorValue(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			ShaderPropertyType propertyType = this.GetPropertyType(propertyIndex);
			bool flag = propertyType != ShaderPropertyType.Color && propertyType != ShaderPropertyType.Vector;
			if (flag)
			{
				throw new ArgumentException("Property type is not Color or Vector.");
			}
			return Shader.GetPropertyDefaultValue(this, propertyIndex);
		}

		public Vector2 GetPropertyRangeLimits(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			bool flag = this.GetPropertyType(propertyIndex) != ShaderPropertyType.Range;
			if (flag)
			{
				throw new ArgumentException("Property type is not Range.");
			}
			Vector4 propertyDefaultValue = Shader.GetPropertyDefaultValue(this, propertyIndex);
			return new Vector2(propertyDefaultValue[1], propertyDefaultValue[2]);
		}

		public int GetPropertyDefaultIntValue(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			bool flag = this.GetPropertyType(propertyIndex) != ShaderPropertyType.Int;
			if (flag)
			{
				throw new ArgumentException("Property type is not Int.");
			}
			return Shader.GetPropertyDefaultIntValue(this, propertyIndex);
		}

		public TextureDimension GetPropertyTextureDimension(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			bool flag = this.GetPropertyType(propertyIndex) != ShaderPropertyType.Texture;
			if (flag)
			{
				throw new ArgumentException("Property type is not TexEnv.");
			}
			return Shader.GetPropertyTextureDimension(this, propertyIndex);
		}

		public string GetPropertyTextureDefaultName(int propertyIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			ShaderPropertyType propertyType = this.GetPropertyType(propertyIndex);
			bool flag = propertyType != ShaderPropertyType.Texture;
			if (flag)
			{
				throw new ArgumentException("Property type is not Texture.");
			}
			return Shader.GetPropertyTextureDefaultName(this, propertyIndex);
		}

		public bool FindTextureStack(int propertyIndex, out string stackName, out int layerIndex)
		{
			Shader.CheckPropertyIndex(this, propertyIndex);
			ShaderPropertyType propertyType = this.GetPropertyType(propertyIndex);
			bool flag = propertyType != ShaderPropertyType.Texture;
			if (flag)
			{
				throw new ArgumentException("Property type is not Texture.");
			}
			return Shader.FindTextureStackImpl(this, propertyIndex, out stackName, out layerIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FindBuiltin_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateFromCompiledData_Injected(ref ManagedSpanWrapper compiledData, Shader[] dependencies);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_maximumLOD_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maximumLOD_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isSupported_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_globalRenderPipeline_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_globalRenderPipeline_Injected(ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_keywordSpace_Injected(IntPtr _unity_self, out LocalKeywordSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetEnabledGlobalKeywords_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAllGlobalKeywords_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableKeyword_Injected(ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableKeyword_Injected(ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsKeywordEnabled_Injected(ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableKeywordFast_Injected([In] ref GlobalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableKeywordFast_Injected([In] ref GlobalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetKeywordFast_Injected([In] ref GlobalKeyword keyword, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsKeywordEnabledFast_Injected([In] ref GlobalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_renderQueue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DisableBatchingType get_disableBatching_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int TagToID_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IDToTag_Injected(int name, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PropertyToID_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDependency_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_passCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_subshaderCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPassCountInSubshader_Injected(IntPtr _unity_self, int subshaderIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_FindPassTagValue_Injected(IntPtr _unity_self, int passIndex, int tagName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_FindPassTagValueInSubShader_Injected(IntPtr _unity_self, int subShaderIndex, int passIndex, int tagName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_FindSubshaderTagValue_Injected(IntPtr _unity_self, int subShaderIndex, int tagName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVectorImpl_Injected(int name, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrixImpl_Injected(int name, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalTextureImpl_Injected(int name, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalRenderTextureImpl_Injected(int name, IntPtr value, RenderTextureSubElement element);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalBufferImpl_Injected(int name, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalGraphicsBufferImpl_Injected(int name, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalConstantBufferImpl_Injected(int name, IntPtr value, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalConstantGraphicsBufferImpl_Injected(int name, IntPtr value, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalRayTracingAccelerationStructureImpl_Injected(int name, IntPtr accelerationStructure);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalVectorImpl_Injected(int name, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalMatrixImpl_Injected(int name, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetGlobalTextureImpl_Injected(int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalFloatArrayImpl_Injected(int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVectorArrayImpl_Injected(int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrixArrayImpl_Injected(int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalFloatArrayImpl_Injected(int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalVectorArrayImpl_Injected(int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalMatrixArrayImpl_Injected(int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractGlobalFloatArrayImpl_Injected(int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractGlobalVectorArrayImpl_Injected(int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractGlobalMatrixArrayImpl_Injected(int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPropertyName_Injected(IntPtr shader, int propertyIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPropertyNameId_Injected(IntPtr shader, int propertyIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ShaderPropertyType GetPropertyType_Injected(IntPtr shader, int propertyIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPropertyDescription_Injected(IntPtr shader, int propertyIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ShaderPropertyFlags GetPropertyFlags_Injected(IntPtr shader, int propertyIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetPropertyAttributes_Injected(IntPtr shader, int propertyIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPropertyDefaultIntValue_Injected(IntPtr shader, int propertyIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPropertyDefaultValue_Injected(IntPtr shader, int propertyIndex, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureDimension GetPropertyTextureDimension_Injected(IntPtr shader, int propertyIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPropertyTextureDefaultName_Injected(IntPtr shader, int propertyIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool FindTextureStackImpl_Injected(IntPtr s, int propertyIdx, out ManagedSpanWrapper stackName, out int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPropertyCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int FindPropertyIndex_Injected(IntPtr _unity_self, ref ManagedSpanWrapper propertyName);
	}
}
