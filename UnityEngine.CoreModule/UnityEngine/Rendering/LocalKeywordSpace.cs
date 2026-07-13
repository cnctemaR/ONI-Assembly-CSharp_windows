using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
	public readonly struct LocalKeywordSpace : IEquatable<LocalKeywordSpace>
	{
		[FreeFunction("keywords::GetKeywords", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern LocalKeyword[] GetKeywords();

		[FreeFunction("keywords::GetKeywordNames", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string[] GetKeywordNames();

		[FreeFunction("keywords::GetKeywordCount", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern uint GetKeywordCount();

		[FreeFunction("keywords::GetKeyword", HasExplicitThis = true)]
		private unsafe LocalKeyword GetKeyword(string name)
		{
			LocalKeyword localKeyword2;
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
				LocalKeyword localKeyword;
				LocalKeywordSpace.GetKeyword_Injected(ref this, ref managedSpanWrapper, out localKeyword);
			}
			finally
			{
				char* ptr = null;
				LocalKeyword localKeyword;
				localKeyword2 = localKeyword;
			}
			return localKeyword2;
		}

		public LocalKeyword[] keywords
		{
			get
			{
				return this.GetKeywords();
			}
		}

		public string[] keywordNames
		{
			get
			{
				return this.GetKeywordNames();
			}
		}

		public uint keywordCount
		{
			get
			{
				return this.GetKeywordCount();
			}
		}

		public LocalKeyword FindKeyword(string name)
		{
			return this.GetKeyword(name);
		}

		public override bool Equals(object o)
		{
			bool flag;
			if (o is LocalKeywordSpace)
			{
				LocalKeywordSpace localKeywordSpace = (LocalKeywordSpace)o;
				flag = this.Equals(localKeywordSpace);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(LocalKeywordSpace rhs)
		{
			return this.m_KeywordSpace == rhs.m_KeywordSpace;
		}

		public static bool operator ==(LocalKeywordSpace lhs, LocalKeywordSpace rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(LocalKeywordSpace lhs, LocalKeywordSpace rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return this.m_KeywordSpace.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetKeyword_Injected(ref LocalKeywordSpace _unity_self, ref ManagedSpanWrapper name, out LocalKeyword ret);

		private readonly IntPtr m_KeywordSpace;
	}
}
