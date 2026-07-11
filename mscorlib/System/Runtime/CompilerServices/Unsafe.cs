using System;

namespace System.Runtime.CompilerServices
{
	internal static class Unsafe
	{
		public static ref T Add<T>(ref T source, int elementOffset)
		{
			return (ref source) + (IntPtr)elementOffset * (IntPtr)sizeof(T);
		}

		public static ref T Add<T>(ref T source, IntPtr elementOffset)
		{
			return (ref source) + elementOffset * (IntPtr)sizeof(T);
		}

		public unsafe static void* Add<T>(void* source, int elementOffset)
		{
			return (void*)((byte*)source + (IntPtr)elementOffset * (IntPtr)sizeof(T));
		}

		public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
		{
			return (ref source) + byteOffset;
		}

		public static bool AreSame<T>(ref T left, ref T right)
		{
			return (ref left) == (ref right);
		}

		public static T As<T>(object o) where T : class
		{
			return o;
		}

		public static ref TTo As<TFrom, TTo>(ref TFrom source)
		{
			return ref source;
		}

		public unsafe static ref T AsRef<T>(void* source)
		{
			return ref *(T*)source;
		}

		public static IntPtr ByteOffset<T>(ref T origin, ref T target)
		{
			return (ref target) - (ref origin);
		}

		public static void CopyBlock(ref byte destination, ref byte source, uint byteCount)
		{
			cpblk(ref destination, ref source, byteCount);
		}

		public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
		{
			initblk(ref startAddress, value, byteCount);
		}

		public unsafe static void InitBlockUnaligned(void* startAddress, byte value, uint byteCount)
		{
			initblk(startAddress, value, byteCount);
		}

		public unsafe static T Read<T>(void* source)
		{
			return *(T*)source;
		}

		public static T ReadUnaligned<T>(ref byte source)
		{
			return source;
		}

		public static int SizeOf<T>()
		{
			return sizeof(T);
		}

		public static ref T Subtract<T>(ref T source, int elementOffset)
		{
			return (ref source) - (IntPtr)elementOffset * (IntPtr)sizeof(T);
		}

		public static void WriteUnaligned<T>(ref byte destination, T value)
		{
			destination = value;
		}
	}
}
