using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	[NativeHeader("Editor/Src/Graphics/ShaderCompilerData.h")]
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
		private unsafe static bool IsKeywordNameEnabled(ShaderKeywordSet state, string name)
		{
			bool flag;
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
				flag = ShaderKeywordSet.IsKeywordNameEnabled_Injected(ref state, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction("keywords::EnableKeyword")]
		private static void EnableGlobalKeyword(ShaderKeywordSet state, uint index)
		{
			ShaderKeywordSet.EnableGlobalKeyword_Injected(ref state, index);
		}

		[FreeFunction("keywords::EnableKeyword")]
		private unsafe static void EnableKeywordName(ShaderKeywordSet state, string name)
		{
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
				ShaderKeywordSet.EnableKeywordName_Injected(ref state, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("keywords::DisableKeyword")]
		private static void DisableGlobalKeyword(ShaderKeywordSet state, uint index)
		{
			ShaderKeywordSet.DisableGlobalKeyword_Injected(ref state, index);
		}

		[FreeFunction("keywords::DisableKeyword")]
		private unsafe static void DisableKeywordName(ShaderKeywordSet state, string name)
		{
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
				ShaderKeywordSet.DisableKeywordName_Injected(ref state, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
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

		public override string ToString()
		{
			ShaderKeyword[] enabledKeywords = ShaderKeywordSet.GetEnabledKeywords(this);
			Array.Sort<ShaderKeyword>(enabledKeywords, new Comparison<ShaderKeyword>(ShaderKeywordSet.ShaderKeywordComparer));
			return string.Join<ShaderKeyword>(' ', enabledKeywords);
		}

		private static int ShaderKeywordComparer(ShaderKeyword kw1, ShaderKeyword kw2)
		{
			return kw1.m_Name.CompareTo(kw2.m_Name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsGlobalKeywordEnabled_Injected([In] ref ShaderKeywordSet state, uint index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsKeywordEnabled_Injected([In] ref ShaderKeywordSet state, [In] ref LocalKeywordSpace keywordSpace, uint index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsKeywordNameEnabled_Injected([In] ref ShaderKeywordSet state, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableGlobalKeyword_Injected([In] ref ShaderKeywordSet state, uint index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableKeywordName_Injected([In] ref ShaderKeywordSet state, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableGlobalKeyword_Injected([In] ref ShaderKeywordSet state, uint index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableKeywordName_Injected([In] ref ShaderKeywordSet state, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ShaderKeyword[] GetEnabledKeywords_Injected([In] ref ShaderKeywordSet state);

		private IntPtr m_KeywordState;

		private IntPtr m_Shader;

		private IntPtr m_ComputeShader;

		private ulong m_StateIndex;
	}
}
