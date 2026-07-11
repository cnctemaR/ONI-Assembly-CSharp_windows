using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Utilities/Hash128.h")]
	[Serializable]
	public struct Hash128 : IComparable, IComparable<Hash128>, IEquatable<Hash128>
	{
		public Hash128(uint u32_0, uint u32_1, uint u32_2, uint u32_3)
		{
			this.m_u32_0 = u32_0;
			this.m_u32_1 = u32_1;
			this.m_u32_2 = u32_2;
			this.m_u32_3 = u32_3;
		}

		public unsafe Hash128(ulong u64_0, ulong u64_1)
		{
			this.m_u32_0 = (uint)u64_0;
			this.m_u32_1 = *((ref u64_0) + 4);
			this.m_u32_2 = (uint)u64_1;
			this.m_u32_3 = *((ref u64_1) + 4);
		}

		internal unsafe ulong u64_0
		{
			get
			{
				fixed (uint* ptr = &this.m_u32_0)
				{
					return (ulong)(*(long*)ptr);
				}
			}
		}

		internal unsafe ulong u64_1
		{
			get
			{
				fixed (uint* ptr = &this.m_u32_1)
				{
					return (ulong)(*(long*)ptr);
				}
			}
		}

		public bool isValid
		{
			get
			{
				return this.m_u32_0 != 0U || this.m_u32_1 != 0U || this.m_u32_2 != 0U || this.m_u32_3 != 0U;
			}
		}

		public int CompareTo(Hash128 rhs)
		{
			int num;
			if (this < rhs)
			{
				num = -1;
			}
			else if (this > rhs)
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public override string ToString()
		{
			return Hash128.Internal_Hash128ToString(this);
		}

		[FreeFunction("StringToHash128")]
		public static Hash128 Parse(string hashString)
		{
			Hash128 hash;
			Hash128.Parse_Injected(hashString, out hash);
			return hash;
		}

		[FreeFunction("Hash128ToString")]
		internal static string Internal_Hash128ToString(Hash128 hash128)
		{
			return Hash128.Internal_Hash128ToString_Injected(ref hash128);
		}

		[FreeFunction("ComputeHash128FromString")]
		public static Hash128 Compute(string hashString)
		{
			Hash128 hash;
			Hash128.Compute_Injected(hashString, out hash);
			return hash;
		}

		public override bool Equals(object obj)
		{
			return obj is Hash128 && this == (Hash128)obj;
		}

		public bool Equals(Hash128 obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return this.m_u32_0.GetHashCode() ^ this.m_u32_1.GetHashCode() ^ this.m_u32_2.GetHashCode() ^ this.m_u32_3.GetHashCode();
		}

		public int CompareTo(object obj)
		{
			int num;
			if (obj == null || !(obj is Hash128))
			{
				num = 1;
			}
			else
			{
				Hash128 hash = (Hash128)obj;
				num = this.CompareTo(hash);
			}
			return num;
		}

		public static bool operator ==(Hash128 hash1, Hash128 hash2)
		{
			return hash1.m_u32_0 == hash2.m_u32_0 && hash1.m_u32_1 == hash2.m_u32_1 && hash1.m_u32_2 == hash2.m_u32_2 && hash1.m_u32_3 == hash2.m_u32_3;
		}

		public static bool operator !=(Hash128 hash1, Hash128 hash2)
		{
			return !(hash1 == hash2);
		}

		public static bool operator <(Hash128 x, Hash128 y)
		{
			bool flag;
			if (x.m_u32_0 != y.m_u32_0)
			{
				flag = x.m_u32_0 < y.m_u32_0;
			}
			else if (x.m_u32_1 != y.m_u32_1)
			{
				flag = x.m_u32_1 < y.m_u32_1;
			}
			else if (x.m_u32_2 != y.m_u32_2)
			{
				flag = x.m_u32_2 < y.m_u32_2;
			}
			else
			{
				flag = x.m_u32_3 < y.m_u32_3;
			}
			return flag;
		}

		public static bool operator >(Hash128 x, Hash128 y)
		{
			return !(x < y) && !(x == y);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Parse_Injected(string hashString, out Hash128 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_Hash128ToString_Injected(ref Hash128 hash128);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Compute_Injected(string hashString, out Hash128 ret);

		private uint m_u32_0;

		private uint m_u32_1;

		private uint m_u32_2;

		private uint m_u32_3;
	}
}
