using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;

namespace System.Resources
{
	internal sealed class FastResourceComparer : IComparer, IEqualityComparer, IComparer<string>, IEqualityComparer<string>
	{
		public int GetHashCode(object key)
		{
			return FastResourceComparer.HashFunction((string)key);
		}

		public int GetHashCode(string key)
		{
			return FastResourceComparer.HashFunction(key);
		}

		internal static int HashFunction(string key)
		{
			uint num = 5381U;
			for (int i = 0; i < key.Length; i++)
			{
				num = ((num << 5) + num) ^ (uint)key[i];
			}
			return (int)num;
		}

		public int Compare(object a, object b)
		{
			if (a == b)
			{
				return 0;
			}
			string text = (string)a;
			string text2 = (string)b;
			return string.CompareOrdinal(text, text2);
		}

		public int Compare(string a, string b)
		{
			return string.CompareOrdinal(a, b);
		}

		public bool Equals(string a, string b)
		{
			return string.Equals(a, b);
		}

		public bool Equals(object a, object b)
		{
			if (a == b)
			{
				return true;
			}
			string text = (string)a;
			string text2 = (string)b;
			return string.Equals(text, text2);
		}

		[SecurityCritical]
		public unsafe static int CompareOrdinal(string a, byte[] bytes, int bCharLength)
		{
			int num = 0;
			int num2 = 0;
			int num3 = a.Length;
			if (num3 > bCharLength)
			{
				num3 = bCharLength;
			}
			if (bCharLength == 0)
			{
				if (a.Length != 0)
				{
					return -1;
				}
				return 0;
			}
			else
			{
				fixed (byte[] array = bytes)
				{
					byte* ptr;
					if (bytes == null || array.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array[0];
					}
					byte* ptr2 = ptr;
					while (num < num3 && num2 == 0)
					{
						int num4 = (int)(*ptr2) | ((int)ptr2[1] << 8);
						num2 = (int)a[num++] - num4;
						ptr2 += 2;
					}
				}
				if (num2 != 0)
				{
					return num2;
				}
				return a.Length - bCharLength;
			}
		}

		[SecurityCritical]
		public static int CompareOrdinal(byte[] bytes, int aCharLength, string b)
		{
			return -FastResourceComparer.CompareOrdinal(b, bytes, aCharLength);
		}

		[SecurityCritical]
		internal unsafe static int CompareOrdinal(byte* a, int byteLen, string b)
		{
			int num = 0;
			int num2 = 0;
			int num3 = byteLen >> 1;
			if (num3 > b.Length)
			{
				num3 = b.Length;
			}
			while (num2 < num3 && num == 0)
			{
				num = (int)((char)((int)(*(a++)) | ((int)(*(a++)) << 8)) - b[num2++]);
			}
			if (num != 0)
			{
				return num;
			}
			return byteLen - b.Length * 2;
		}

		internal static readonly FastResourceComparer Default = new FastResourceComparer();
	}
}
