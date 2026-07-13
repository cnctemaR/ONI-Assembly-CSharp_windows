using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
	public readonly struct GlobalKeyword
	{
		[FreeFunction("ShaderScripting::GetGlobalKeywordCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetGlobalKeywordCount();

		[FreeFunction("ShaderScripting::GetGlobalKeywordIndex")]
		private unsafe static uint GetGlobalKeywordIndex(string keyword)
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
				globalKeywordIndex_Injected = GlobalKeyword.GetGlobalKeywordIndex_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return globalKeywordIndex_Injected;
		}

		[FreeFunction("ShaderScripting::CreateGlobalKeyword")]
		private unsafe static void CreateGlobalKeyword(string keyword)
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
				GlobalKeyword.CreateGlobalKeyword_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("ShaderScripting::GetGlobalKeywordName")]
		private static string GetGlobalKeywordName(uint keywordIndex)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				GlobalKeyword.GetGlobalKeywordName_Injected(keywordIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static GlobalKeyword Create(string name)
		{
			GlobalKeyword.CreateGlobalKeyword(name);
			return new GlobalKeyword(name);
		}

		public GlobalKeyword(string name)
		{
			this.m_Index = GlobalKeyword.GetGlobalKeywordIndex(name);
			bool flag = this.m_Index >= GlobalKeyword.GetGlobalKeywordCount();
			if (flag)
			{
				Debug.LogErrorFormat("Global keyword {0} doesn't exist.", new object[] { name });
			}
		}

		public string name
		{
			get
			{
				return GlobalKeyword.GetGlobalKeywordName(this.m_Index);
			}
		}

		public override string ToString()
		{
			return this.name;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetGlobalKeywordIndex_Injected(ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateGlobalKeyword_Injected(ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalKeywordName_Injected(uint keywordIndex, out ManagedSpanWrapper ret);

		internal readonly uint m_Index;
	}
}
