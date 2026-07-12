using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ImGuiNET
{
	internal static class Util
	{
		public unsafe static string StringFromPtr(byte* ptr)
		{
			int num = 0;
			while (ptr[num] != 0)
			{
				num++;
			}
			return Encoding.UTF8.GetString(ptr, num);
		}

		internal unsafe static bool AreStringsEqual(byte* a, int aLength, byte* b)
		{
			for (int i = 0; i < aLength; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return b[aLength] == 0;
		}

		internal unsafe static byte* Allocate(int byteCount)
		{
			return (byte*)(void*)Marshal.AllocHGlobal(byteCount);
		}

		internal unsafe static void Free(byte* ptr)
		{
			Marshal.FreeHGlobal((IntPtr)((void*)ptr));
		}

		internal unsafe static int CalcSizeInUtf8(string s, int start, int length)
		{
			if (start < 0 || length < 0 || start + length > s.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			char* ptr = s;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return Encoding.UTF8.GetByteCount(ptr + start, length);
		}

		internal unsafe static int GetUtf8(string s, byte* utf8Bytes, int utf8ByteCount)
		{
			char* ptr = s;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return Encoding.UTF8.GetBytes(ptr, s.Length, utf8Bytes, utf8ByteCount);
		}

		internal unsafe static int GetUtf8(string s, int start, int length, byte* utf8Bytes, int utf8ByteCount)
		{
			if (start < 0 || length < 0 || start + length > s.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			char* ptr = s;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return Encoding.UTF8.GetBytes(ptr + start, length, utf8Bytes, utf8ByteCount);
		}

		internal const int StackAllocationSizeLimit = 2048;
	}
}
