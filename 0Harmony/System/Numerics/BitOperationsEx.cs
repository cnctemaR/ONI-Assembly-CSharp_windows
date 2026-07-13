using System;
using System.Runtime.CompilerServices;

namespace System.Numerics
{
	internal static class BitOperationsEx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPow2(int value)
		{
			return (value & (value - 1)) == 0 && value > 0;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPow2(uint value)
		{
			return (value & (value - 1U)) == 0U && value > 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPow2(long value)
		{
			return (value & (value - 1L)) == 0L && value > 0L;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPow2(ulong value)
		{
			return (value & (value - 1UL)) == 0UL && value > 0UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPow2([NativeInteger] IntPtr value)
		{
			return (value & (value - (IntPtr)1)) == 0 && value > (IntPtr)0;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPow2([NativeInteger] UIntPtr value)
		{
			return (value & (value - (UIntPtr)((IntPtr)1))) == 0 && value > (UIntPtr)((IntPtr)0);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RoundUpToPowerOf2(uint value)
		{
			value -= 1U;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			return value + 1U;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RoundUpToPowerOf2(ulong value)
		{
			value -= 1UL;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value |= value >> 32;
			return value + 1UL;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: NativeInteger]
		public static UIntPtr RoundUpToPowerOf2([NativeInteger] UIntPtr value)
		{
			if (IntPtr.Size == 8)
			{
				return (UIntPtr)BitOperationsEx.RoundUpToPowerOf2((ulong)value);
			}
			return (UIntPtr)BitOperationsEx.RoundUpToPowerOf2((uint)value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(uint value)
		{
			return BitOperations.LeadingZeroCount(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(ulong value)
		{
			return BitOperations.LeadingZeroCount(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount([NativeInteger] UIntPtr value)
		{
			if (IntPtr.Size == 8)
			{
				return BitOperationsEx.LeadingZeroCount((ulong)value);
			}
			return BitOperationsEx.LeadingZeroCount((uint)value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Log2(uint value)
		{
			return BitOperations.Log2(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Log2(ulong value)
		{
			return BitOperations.Log2(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Log2([NativeInteger] UIntPtr value)
		{
			if (IntPtr.Size == 8)
			{
				return BitOperationsEx.Log2((ulong)value);
			}
			return BitOperationsEx.Log2((uint)value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(uint value)
		{
			return BitOperations.PopCount(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(ulong value)
		{
			return BitOperations.PopCount(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount([NativeInteger] UIntPtr value)
		{
			if (IntPtr.Size == 8)
			{
				return BitOperationsEx.PopCount((ulong)value);
			}
			return BitOperationsEx.PopCount((uint)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(int value)
		{
			return BitOperations.TrailingZeroCount(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(uint value)
		{
			return BitOperations.TrailingZeroCount(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(long value)
		{
			return BitOperations.TrailingZeroCount(value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(ulong value)
		{
			return BitOperations.TrailingZeroCount(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount([NativeInteger] IntPtr value)
		{
			if (IntPtr.Size == 8)
			{
				return BitOperationsEx.TrailingZeroCount((long)value);
			}
			return BitOperationsEx.TrailingZeroCount((int)value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount([NativeInteger] UIntPtr value)
		{
			if (IntPtr.Size == 8)
			{
				return BitOperationsEx.TrailingZeroCount((ulong)value);
			}
			return BitOperationsEx.TrailingZeroCount((uint)value);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RotateLeft(uint value, int offset)
		{
			return BitOperations.RotateLeft(value, offset);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RotateLeft(ulong value, int offset)
		{
			return BitOperations.RotateLeft(value, offset);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: NativeInteger]
		public static UIntPtr RotateLeft([NativeInteger] UIntPtr value, int offset)
		{
			if (IntPtr.Size == 8)
			{
				return (UIntPtr)BitOperationsEx.RotateLeft((ulong)value, offset);
			}
			return (UIntPtr)BitOperationsEx.RotateLeft((uint)value, offset);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RotateRight(uint value, int offset)
		{
			return BitOperations.RotateRight(value, offset);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RotateRight(ulong value, int offset)
		{
			return BitOperations.RotateRight(value, offset);
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: NativeInteger]
		public static UIntPtr RotateRight([NativeInteger] UIntPtr value, int offset)
		{
			if (IntPtr.Size == 8)
			{
				return (UIntPtr)BitOperationsEx.RotateRight((ulong)value, offset);
			}
			return (UIntPtr)BitOperationsEx.RotateRight((uint)value, offset);
		}
	}
}
