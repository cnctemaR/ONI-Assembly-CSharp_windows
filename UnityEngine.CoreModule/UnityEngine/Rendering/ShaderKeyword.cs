using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Shaders/ShaderKeywords.h")]
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class ShaderKeyword
	{
		internal ShaderKeyword(int keywordIndex)
		{
			this.m_KeywordIndex = keywordIndex;
		}

		public ShaderKeyword(string keywordName)
		{
			this.m_KeywordIndex = ShaderKeyword.GetShaderKeywordIndex(keywordName);
		}

		[NativeMethod("keywords::Find", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetShaderKeywordIndex(string keywordName);

		[NativeMethod("keywords::GetKeywordName", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetShaderKeywordName(int keywordIndex);

		[NativeMethod("keywords::GetKeywordType", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ShaderKeywordType GetShaderKeywordType(int keywordIndex);

		public bool IsValid()
		{
			return this.m_KeywordIndex >= 0 && this.m_KeywordIndex < 256 && this.m_KeywordIndex != -1;
		}

		public ShaderKeywordType GetKeywordType()
		{
			return ShaderKeyword.GetShaderKeywordType(this.m_KeywordIndex);
		}

		public string GetKeywordName()
		{
			return ShaderKeyword.GetShaderKeywordName(this.m_KeywordIndex);
		}

		internal int GetKeywordIndex()
		{
			return this.m_KeywordIndex;
		}

		[Obsolete("GetName() has been deprecated. Use GetKeywordName() instead (UnityUpgradable) -> GetKeywordName()")]
		public string GetName()
		{
			return this.GetKeywordName();
		}

		internal const int k_MaxShaderKeywords = 256;

		private const int k_InvalidKeyword = -1;

		internal int m_KeywordIndex;
	}
}
