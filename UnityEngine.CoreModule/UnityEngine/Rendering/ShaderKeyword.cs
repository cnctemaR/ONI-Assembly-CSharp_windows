using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Identifier of a specific code path in a shader.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Runtime/Shaders/ShaderKeywords.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class ShaderKeyword
	{
		internal ShaderKeyword(int keywordIndex)
		{
			this.m_KeywordIndex = keywordIndex;
		}

		/// <summary>
		///   <para>Initializes a new instance of the ShaderKeyword class from a shader keyword name.</para>
		/// </summary>
		/// <param name="keywordName"></param>
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

		/// <summary>
		///   <para>Returns true if the keyword has been imported by Unity.</para>
		/// </summary>
		public bool IsValid()
		{
			return this.m_KeywordIndex >= 0 && this.m_KeywordIndex < 256 && this.m_KeywordIndex != -1;
		}

		/// <summary>
		///   <para>Returns the string name of the keyword.</para>
		/// </summary>
		public string GetName()
		{
			return ShaderKeyword.GetShaderKeywordName(this.m_KeywordIndex);
		}

		internal int GetIndex()
		{
			return this.m_KeywordIndex;
		}

		internal const int k_MaxShaderKeywords = 256;

		private const int k_InvalidKeyword = -1;

		internal int m_KeywordIndex;
	}
}
