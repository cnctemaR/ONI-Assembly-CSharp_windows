using System;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Editor/Src/Graphics/ShaderCompilerData.h")]
	[UsedByNativeCode]
	public struct ShaderKeywordSet
	{
		[FreeFunction("keywords::IsKeywordEnabled")]
		private static bool IsGlobalKeywordEnabled(ShaderKeywordSet state, uint index)
		{
			return ShaderKeywordSet.IsGlobalKeywordEnabled_Injected(ref state, index);
		}

		[FreeFunction("keywords::IsKeywordEnabled")]
		private static bool IsKeywordEnabled(ShaderKeywordSet state, LocalKeywordSpace keywordSpace, uint index)
		{
			return ShaderKeywordSet.IsKeywordEnabled_Injected(ref state, ref keywordSpace, index);
		}

		[FreeFunction("keywords::IsKeywordEnabled")]
		private static bool IsKeywordNameEnabled(ShaderKeywordSet state, string name)
		{
			return ShaderKeywordSet.IsKeywordNameEnabled_Injected(ref state, name);
		}

		[FreeFunction("keywords::EnableKeyword")]
		private static void EnableGlobalKeyword(ShaderKeywordSet state, uint index)
		{
			ShaderKeywordSet.EnableGlobalKeyword_Injected(ref state, index);
		}

		[FreeFunction("keywords::EnableKeyword")]
		private static void EnableKeywordName(ShaderKeywordSet state, string name)
		{
			ShaderKeywordSet.EnableKeywordName_Injected(ref state, name);
		}

		[FreeFunction("keywords::DisableKeyword")]
		private static void DisableGlobalKeyword(ShaderKeywordSet state, uint index)
		{
			ShaderKeywordSet.DisableGlobalKeyword_Injected(ref state, index);
		}

		[FreeFunction("keywords::DisableKeyword")]
		private static void DisableKeywordName(ShaderKeywordSet state, string name)
		{
			ShaderKeywordSet.DisableKeywordName_Injected(ref state, name);
		}

		[FreeFunction("keywords::GetEnabledKeywords")]
		private static ShaderKeyword[] GetEnabledKeywords(ShaderKeywordSet state)
		{
			return ShaderKeywordSet.GetEnabledKeywords_Injected(ref state);
		}

		private void CheckKeywordCompatible(ShaderKeyword keyword)
		{
			bool isLocal = keyword.m_IsLocal;
			if (isLocal)
			{
				bool flag = this.m_Shader != IntPtr.Zero;
				if (flag)
				{
					Assert.IsTrue(!keyword.m_IsCompute, "Trying to use a keyword that comes from a different shader.");
				}
				else
				{
					Assert.IsTrue(keyword.m_IsCompute, "Trying to use a keyword that comes from a different shader.");
				}
			}
		}

		public bool IsEnabled(ShaderKeyword keyword)
		{
			this.CheckKeywordCompatible(keyword);
			return ShaderKeywordSet.IsKeywordNameEnabled(this, keyword.m_Name);
		}

		public bool IsEnabled(GlobalKeyword keyword)
		{
			return ShaderKeywordSet.IsGlobalKeywordEnabled(this, keyword.m_Index);
		}

		public bool IsEnabled(LocalKeyword keyword)
		{
			return ShaderKeywordSet.IsKeywordEnabled(this, keyword.m_SpaceInfo, keyword.m_Index);
		}

		public void Enable(ShaderKeyword keyword)
		{
			this.CheckKeywordCompatible(keyword);
			bool flag = keyword.m_IsLocal || !keyword.IsValid();
			if (flag)
			{
				ShaderKeywordSet.EnableKeywordName(this, keyword.m_Name);
			}
			else
			{
				ShaderKeywordSet.EnableGlobalKeyword(this, keyword.m_Index);
			}
		}

		public void Disable(ShaderKeyword keyword)
		{
			bool flag = keyword.m_IsLocal || !keyword.IsValid();
			if (flag)
			{
				ShaderKeywordSet.DisableKeywordName(this, keyword.m_Name);
			}
			else
			{
				ShaderKeywordSet.DisableGlobalKeyword(this, keyword.m_Index);
			}
		}

		public ShaderKeyword[] GetShaderKeywords()
		{
			return ShaderKeywordSet.GetEnabledKeywords(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsGlobalKeywordEnabled_Injected(ref ShaderKeywordSet state, uint index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsKeywordEnabled_Injected(ref ShaderKeywordSet state, ref LocalKeywordSpace keywordSpace, uint index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsKeywordNameEnabled_Injected(ref ShaderKeywordSet state, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableGlobalKeyword_Injected(ref ShaderKeywordSet state, uint index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableKeywordName_Injected(ref ShaderKeywordSet state, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableGlobalKeyword_Injected(ref ShaderKeywordSet state, uint index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableKeywordName_Injected(ref ShaderKeywordSet state, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ShaderKeyword[] GetEnabledKeywords_Injected(ref ShaderKeywordSet state);

		private IntPtr m_KeywordState;

		private IntPtr m_Shader;

		private IntPtr m_ComputeShader;

		private ulong m_StateIndex;
	}
}
