using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics
{
	internal static class BitOperations
	{
		private unsafe static global::System.ReadOnlySpan<byte> TrailingZeroCountDeBruijn
		{
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<1c2fb156-e9ba-45cc-af54-d5335bdb59af><PrivateImplementationDetails>.3BF63951626584EB1653F9B8DBB590A5EE1EAE1135A904B9317C3773896DF076), 32);
			}
		}

		private unsafe static global::System.ReadOnlySpan<byte> Log2DeBruijn
		{
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<1c2fb156-e9ba-45cc-af54-d5335bdb59af><PrivateImplementationDetails>.4BCD43D478B9229AB7A13406353712C7944B60348C36B4D0E6B789D10F697652), 32);
			}
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(uint value)
		{
			if (value == 0U)
			{
				return 32;
			}
			return 31 ^ BitOperations.Log2SoftwareFallback(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(ulong value)
		{
			uint num = (uint)(value >> 32);
			if (num == 0U)
			{
				return 32 + BitOperations.LeadingZeroCount((uint)value);
			}
			return BitOperations.LeadingZeroCount(num);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Log2(uint value)
		{
			value |= 1U;
			return BitOperations.Log2SoftwareFallback(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Log2(ulong value)
		{
			value |= 1UL;
			uint num = (uint)(value >> 32);
			if (num == 0U)
			{
				return BitOperations.Log2((uint)value);
			}
			return 32 + BitOperations.Log2(num);
		}

		private unsafe static int Log2SoftwareFallback(uint value)
		{
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			return (int)(*Unsafe.AddByteOffset<byte>(global::System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(BitOperations.Log2DeBruijn), (IntPtr)((int)(value * 130329821U >> 27))));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int Log2Ceiling(uint value)
		{
			int num = BitOperations.Log2(value);
			if (BitOperations.PopCount(value) != 1)
			{
				num++;
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int Log2Ceiling(ulong value)
		{
			int num = BitOperations.Log2(value);
			if (BitOperations.PopCount(value) != 1)
			{
				num++;
			}
			return num;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(uint value)
		{
			return BitOperations.<PopCount>g__SoftwareFallback|11_0(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(ulong value)
		{
			if (IntPtr.Size == 8)
			{
				return BitOperations.PopCount((uint)value) + BitOperations.PopCount((uint)(value >> 32));
			}
			return BitOperations.<PopCount>g__SoftwareFallback|12_0(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(int value)
		{
			return BitOperations.TrailingZeroCount((uint)value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int TrailingZeroCount(uint value)
		{
			if (value == 0U)
			{
				return 32;
			}
			return (int)(*Unsafe.AddByteOffset<byte>(global::System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(BitOperations.TrailingZeroCountDeBruijn), (IntPtr)((int)((value & -value) * 125613361U >> 27))));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(long value)
		{
			return BitOperations.TrailingZeroCount((ulong)value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(ulong value)
		{
			uint num = (uint)value;
			if (num == 0U)
			{
				return 32 + BitOperations.TrailingZeroCount((uint)(value >> 32));
			}
			return BitOperations.TrailingZeroCount(num);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RotateLeft(uint value, int offset)
		{
			return (value << offset) | (value >> 32 - offset);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RotateLeft(ulong value, int offset)
		{
			return (value << offset) | (value >> 64 - offset);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RotateRight(uint value, int offset)
		{
			return (value >> offset) | (value << 32 - offset);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RotateRight(ulong value, int offset)
		{
			return (value >> offset) | (value << 64 - offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint ResetLowestSetBit(uint value)
		{
			return value & (value - 1U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint ResetBit(uint value, int bitPos)
		{
			return value & ~(1U << bitPos);
		}

		[CompilerGenerated]
		internal static int <PopCount>g__SoftwareFallback|11_0(uint value)
		{
			value -= (value >> 1) & 1431655765U;
			value = (value & 858993459U) + ((value >> 2) & 858993459U);
			value = ((value + (value >> 4)) & 252645135U) * 16843009U >> 24;
			return (int)value;
		}

		[CompilerGenerated]
		internal static int <PopCount>g__SoftwareFallback|12_0(ulong value)
		{
			value -= (value >> 1) & 6148914691236517205UL;
			value = (value & 3689348814741910323UL) + ((value >> 2) & 3689348814741910323UL);
			value = ((value + (value >> 4)) & 1085102592571150095UL) * 72340172838076673UL >> 56;
			return (int)value;
		}
	}
}
