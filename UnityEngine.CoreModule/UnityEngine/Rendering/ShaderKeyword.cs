using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
	public struct ShaderKeyword
	{
		[FreeFunction("ShaderScripting::GetGlobalKeywordCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetGlobalKeywordCount();

		[FreeFunction("ShaderScripting::GetGlobalKeywordIndex")]
		internal unsafe static uint GetGlobalKeywordIndex(string keyword)
		{
			uint globalKeywordIndex_Injected;
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
				globalKeywordIndex_Injected = ShaderKeyword.GetGlobalKeywordIndex_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return globalKeywordIndex_Injected;
		}

		[FreeFunction("ShaderScripting::GetKeywordCount")]
		internal static uint GetKeywordCount(Shader shader)
		{
			return ShaderKeyword.GetKeywordCount_Injected(Object.MarshalledUnityObject.Marshal<Shader>(shader));
		}

		[FreeFunction("ShaderScripting::GetKeywordIndex")]
		internal unsafe static uint GetKeywordIndex(Shader shader, string keyword)
		{
			uint keywordIndex_Injected;
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
				keywordIndex_Injected = ShaderKeyword.GetKeywordIndex_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return keywordIndex_Injected;
		}

		[FreeFunction("ShaderScripting::GetKeywordCount")]
		internal static uint GetComputeShaderKeywordCount(ComputeShader shader)
		{
			return ShaderKeyword.GetComputeShaderKeywordCount_Injected(Object.MarshalledUnityObject.Marshal<ComputeShader>(shader));
		}

		[FreeFunction("ShaderScripting::GetKeywordIndex")]
		internal unsafe static uint GetComputeShaderKeywordIndex(ComputeShader shader, string keyword)
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
				computeShaderKeywordIndex_Injected = ShaderKeyword.GetComputeShaderKeywordIndex_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return computeShaderKeywordIndex_Injected;
		}

		[FreeFunction("ShaderScripting::CreateGlobalKeyword")]
		internal unsafe static void CreateGlobalKeyword(string keyword)
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
				ShaderKeyword.CreateGlobalKeyword_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("ShaderScripting::GetKeywordType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ShaderKeywordType GetGlobalShaderKeywordType(uint keyword);

		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public static ShaderKeywordType GetGlobalKeywordType(ShaderKeyword index)
		{
			bool flag = index.IsValid() && !index.m_IsLocal;
			ShaderKeywordType shaderKeywordType;
			if (flag)
			{
				shaderKeywordType = ShaderKeyword.GetGlobalShaderKeywordType(index.m_Index);
			}
			else
			{
				shaderKeywordType = ShaderKeywordType.UserDefined;
			}
			return shaderKeywordType;
		}

		public ShaderKeyword(string keywordName)
		{
			this.m_Name = keywordName;
			this.m_Index = ShaderKeyword.GetGlobalKeywordIndex(keywordName);
			bool flag = this.m_Index >= ShaderKeyword.GetGlobalKeywordCount();
			if (flag)
			{
				ShaderKeyword.CreateGlobalKeyword(keywordName);
				this.m_Index = ShaderKeyword.GetGlobalKeywordIndex(keywordName);
			}
			this.m_IsValid = true;
			this.m_IsLocal = false;
			this.m_IsCompute = false;
		}

		public ShaderKeyword(Shader shader, string keywordName)
		{
			this.m_Name = keywordName;
			this.m_Index = ShaderKeyword.GetKeywordIndex(shader, keywordName);
			this.m_IsValid = this.m_Index < ShaderKeyword.GetKeywordCount(shader);
			this.m_IsLocal = true;
			this.m_IsCompute = false;
		}

		public ShaderKeyword(ComputeShader shader, string keywordName)
		{
			this.m_Name = keywordName;
			this.m_Index = ShaderKeyword.GetComputeShaderKeywordIndex(shader, keywordName);
			this.m_IsValid = this.m_Index < ShaderKeyword.GetComputeShaderKeywordCount(shader);
			this.m_IsLocal = true;
			this.m_IsCompute = true;
		}

		public static bool IsKeywordLocal(ShaderKeyword keyword)
		{
			return keyword.m_IsLocal;
		}

		public bool IsValid()
		{
			return this.m_IsValid;
		}

		public bool IsValid(ComputeShader shader)
		{
			return this.m_IsValid;
		}

		public bool IsValid(Shader shader)
		{
			return this.m_IsValid;
		}

		public int index
		{
			get
			{
				return (int)this.m_Index;
			}
		}

		public override string ToString()
		{
			return this.m_Name;
		}

		[Obsolete("GetKeywordType is deprecated. Only global keywords can have a type. This method always returns ShaderKeywordType.UserDefined.")]
		public static ShaderKeywordType GetKeywordType(Shader shader, ShaderKeyword index)
		{
			return ShaderKeywordType.UserDefined;
		}

		[Obsolete("GetKeywordType is deprecated. Only global keywords can have a type. This method always returns ShaderKeywordType.UserDefined.")]
		public static ShaderKeywordType GetKeywordType(ComputeShader shader, ShaderKeyword index)
		{
			return ShaderKeywordType.UserDefined;
		}

		[Obsolete("GetGlobalKeywordName is deprecated. Use the ShaderKeyword.name property instead.", true)]
		public static string GetGlobalKeywordName(ShaderKeyword index)
		{
			return "";
		}

		[Obsolete("GetKeywordName is deprecated. Use the ShaderKeyword.name property instead.", true)]
		public static string GetKeywordName(Shader shader, ShaderKeyword index)
		{
			return "";
		}

		[Obsolete("GetKeywordName is deprecated. Use the ShaderKeyword.name property instead.", true)]
		public static string GetKeywordName(ComputeShader shader, ShaderKeyword index)
		{
			return "";
		}

		[Obsolete("GetKeywordType is deprecated. Use ShaderKeyword.GetGlobalKeywordType instead.", true)]
		public ShaderKeywordType GetKeywordType()
		{
			return ShaderKeywordType.None;
		}

		[Obsolete("GetKeywordName is deprecated. Use ShaderKeyword.name instead.", true)]
		public string GetKeywordName()
		{
			return "";
		}

		[Obsolete("GetName() has been deprecated. Use ShaderKeyword.name instead.", true)]
		public string GetName()
		{
			return "";
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetGlobalKeywordIndex_Injected(ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetKeywordCount_Injected(IntPtr shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetComputeShaderKeywordCount_Injected(IntPtr shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetComputeShaderKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateGlobalKeyword_Injected(ref ManagedSpanWrapper keyword);

		internal string m_Name;

		internal uint m_Index;

		internal bool m_IsLocal;

		internal bool m_IsCompute;

		internal bool m_IsValid;
	}
}
