using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[UsedByNativeCode]
	public struct ShaderKeyword
	{
		[FreeFunction("ShaderScripting::GetGlobalKeywordCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetGlobalKeywordCount();

		[FreeFunction("ShaderScripting::GetGlobalKeywordIndex")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetGlobalKeywordIndex(string keyword);

		[FreeFunction("ShaderScripting::GetKeywordCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetKeywordCount(Shader shader);

		[FreeFunction("ShaderScripting::GetKeywordIndex")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetKeywordIndex(Shader shader, string keyword);

		[FreeFunction("ShaderScripting::GetKeywordCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetComputeShaderKeywordCount(ComputeShader shader);

		[FreeFunction("ShaderScripting::GetKeywordIndex")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetComputeShaderKeywordIndex(ComputeShader shader, string keyword);

		[FreeFunction("ShaderScripting::CreateGlobalKeyword")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CreateGlobalKeyword(string keyword);

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
			bool flag = index.IsValid();
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

		[Obsolete("GetGlobalKeywordName is deprecated. Use the ShaderKeyword.name property instead.")]
		public static string GetGlobalKeywordName(ShaderKeyword index)
		{
			return index.m_Name;
		}

		[Obsolete("GetKeywordName is deprecated. Use the ShaderKeyword.name property instead.")]
		public static string GetKeywordName(Shader shader, ShaderKeyword index)
		{
			return index.m_Name;
		}

		[Obsolete("GetKeywordName is deprecated. Use the ShaderKeyword.name property instead.")]
		public static string GetKeywordName(ComputeShader shader, ShaderKeyword index)
		{
			return index.m_Name;
		}

		[Obsolete("GetKeywordType is deprecated. Use ShaderKeyword.name instead.")]
		public ShaderKeywordType GetKeywordType()
		{
			return ShaderKeyword.GetGlobalKeywordType(this);
		}

		[Obsolete("GetKeywordName is deprecated. Use ShaderKeyword.name instead.")]
		public string GetKeywordName()
		{
			return ShaderKeyword.GetGlobalKeywordName(this);
		}

		[Obsolete("GetName() has been deprecated. Use ShaderKeyword.name instead.")]
		public string GetName()
		{
			return this.GetKeywordName();
		}

		internal string m_Name;

		internal uint m_Index;

		internal bool m_IsLocal;

		internal bool m_IsCompute;

		internal bool m_IsValid;
	}
}
