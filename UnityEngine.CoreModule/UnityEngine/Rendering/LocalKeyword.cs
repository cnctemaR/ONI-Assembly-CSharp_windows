using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	public readonly struct LocalKeyword : IEquatable<LocalKeyword>
	{
		[FreeFunction("keywords::IsKeywordDynamic")]
		private static bool IsDynamic(LocalKeyword kw)
		{
			return LocalKeyword.IsDynamic_Injected(ref kw);
		}

		[FreeFunction("keywords::IsKeywordOverridable")]
		private static bool IsOverridable(LocalKeyword kw)
		{
			return LocalKeyword.IsOverridable_Injected(ref kw);
		}

		[FreeFunction("ShaderScripting::GetKeywordCount")]
		private static uint GetShaderKeywordCount(Shader shader)
		{
			return LocalKeyword.GetShaderKeywordCount_Injected(Object.MarshalledUnityObject.Marshal<Shader>(shader));
		}

		[FreeFunction("ShaderScripting::GetKeywordIndex")]
		private unsafe static uint GetShaderKeywordIndex(Shader shader, string keyword)
		{
			uint shaderKeywordIndex_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.Marshal<Shader>(shader);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				shaderKeywordIndex_Injected = LocalKeyword.GetShaderKeywordIndex_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return shaderKeywordIndex_Injected;
		}

		[FreeFunction("ShaderScripting::GetKeywordCount")]
		private static uint GetComputeShaderKeywordCount(ComputeShader shader)
		{
			return LocalKeyword.GetComputeShaderKeywordCount_Injected(Object.MarshalledUnityObject.Marshal<ComputeShader>(shader));
		}

		[FreeFunction("ShaderScripting::GetKeywordIndex")]
		private unsafe static uint GetComputeShaderKeywordIndex(ComputeShader shader, string keyword)
		{
			uint computeShaderKeywordIndex_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.Marshal<ComputeShader>(shader);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				computeShaderKeywordIndex_Injected = LocalKeyword.GetComputeShaderKeywordIndex_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return computeShaderKeywordIndex_Injected;
		}

		[FreeFunction("ShaderScripting::GetKeywordCount")]
		private static uint GetRayTracingShaderKeywordCount(RayTracingShader shader)
		{
			return LocalKeyword.GetRayTracingShaderKeywordCount_Injected(Object.MarshalledUnityObject.Marshal<RayTracingShader>(shader));
		}

		[FreeFunction("ShaderScripting::GetKeywordIndex")]
		private unsafe static uint GetRayTracingShaderKeywordIndex(RayTracingShader shader, string keyword)
		{
			uint rayTracingShaderKeywordIndex_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.Marshal<RayTracingShader>(shader);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				rayTracingShaderKeywordIndex_Injected = LocalKeyword.GetRayTracingShaderKeywordIndex_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return rayTracingShaderKeywordIndex_Injected;
		}

		[FreeFunction("keywords::GetKeywordType")]
		private static ShaderKeywordType GetKeywordType(LocalKeywordSpace spaceInfo, uint keyword)
		{
			return LocalKeyword.GetKeywordType_Injected(ref spaceInfo, keyword);
		}

		[FreeFunction("keywords::IsKeywordValid")]
		private static bool IsValid(LocalKeywordSpace spaceInfo, uint keyword)
		{
			return LocalKeyword.IsValid_Injected(ref spaceInfo, keyword);
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public bool isDynamic
		{
			get
			{
				return LocalKeyword.IsDynamic(this);
			}
		}

		public bool isOverridable
		{
			get
			{
				return LocalKeyword.IsOverridable(this);
			}
		}

		public bool isValid
		{
			get
			{
				return LocalKeyword.IsValid(this.m_SpaceInfo, this.m_Index);
			}
		}

		public ShaderKeywordType type
		{
			get
			{
				return LocalKeyword.GetKeywordType(this.m_SpaceInfo, this.m_Index);
			}
		}

		public LocalKeyword(Shader shader, string name)
		{
			bool flag = shader == null;
			if (flag)
			{
				Debug.LogError("Cannot initialize a LocalKeyword with a null Shader.");
			}
			this.m_SpaceInfo = shader.keywordSpace;
			this.m_Name = name;
			this.m_Index = LocalKeyword.GetShaderKeywordIndex(shader, name);
			bool flag2 = this.m_Index >= LocalKeyword.GetShaderKeywordCount(shader);
			if (flag2)
			{
				Debug.LogErrorFormat("Local keyword {0} doesn't exist in the shader.", new object[] { name });
			}
		}

		public LocalKeyword(ComputeShader shader, string name)
		{
			bool flag = shader == null;
			if (flag)
			{
				Debug.LogError("Cannot initialize a LocalKeyword with a null ComputeShader.");
			}
			this.m_SpaceInfo = shader.keywordSpace;
			this.m_Name = name;
			this.m_Index = LocalKeyword.GetComputeShaderKeywordIndex(shader, name);
			bool flag2 = this.m_Index >= LocalKeyword.GetComputeShaderKeywordCount(shader);
			if (flag2)
			{
				Debug.LogErrorFormat("Local keyword {0} doesn't exist in the compute shader.", new object[] { name });
			}
		}

		public LocalKeyword(RayTracingShader shader, string name)
		{
			bool flag = shader == null;
			if (flag)
			{
				Debug.LogError("Cannot initialize a LocalKeyword with a null RayTracingShader.");
			}
			this.m_SpaceInfo = shader.keywordSpace;
			this.m_Name = name;
			this.m_Index = LocalKeyword.GetRayTracingShaderKeywordIndex(shader, name);
			bool flag2 = this.m_Index >= LocalKeyword.GetRayTracingShaderKeywordCount(shader);
			if (flag2)
			{
				Debug.LogErrorFormat("Local keyword {0} doesn't exist in the ray tracing shader.", new object[] { name });
			}
		}

		public override string ToString()
		{
			return this.m_Name;
		}

		public override bool Equals(object o)
		{
			bool flag;
			if (o is LocalKeyword)
			{
				LocalKeyword localKeyword = (LocalKeyword)o;
				flag = this.Equals(localKeyword);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(LocalKeyword rhs)
		{
			return this.m_SpaceInfo == rhs.m_SpaceInfo && this.m_Index == rhs.m_Index;
		}

		public static bool operator ==(LocalKeyword lhs, LocalKeyword rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(LocalKeyword lhs, LocalKeyword rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return this.m_Index.GetHashCode() ^ this.m_SpaceInfo.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsDynamic_Injected([In] ref LocalKeyword kw);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsOverridable_Injected([In] ref LocalKeyword kw);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetShaderKeywordCount_Injected(IntPtr shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetShaderKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetComputeShaderKeywordCount_Injected(IntPtr shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetComputeShaderKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetRayTracingShaderKeywordCount_Injected(IntPtr shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetRayTracingShaderKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ShaderKeywordType GetKeywordType_Injected([In] ref LocalKeywordSpace spaceInfo, uint keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValid_Injected([In] ref LocalKeywordSpace spaceInfo, uint keyword);

		internal readonly LocalKeywordSpace m_SpaceInfo;

		internal readonly string m_Name;

		internal readonly uint m_Index;
	}
}
