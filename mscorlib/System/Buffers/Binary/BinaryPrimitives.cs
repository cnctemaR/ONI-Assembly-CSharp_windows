using System;
using System.Runtime.CompilerServices;

namespace System.Buffers.Binary
{
	public static class BinaryPrimitives
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte ReverseEndianness(sbyte value)
		{
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ReverseEndianness(short value)
		{
			return (short)(((int)(value & 255) << 8) | (((int)value & 65280) >> 8));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ReverseEndianness(int value)
		{
			return (int)BinaryPrimitives.ReverseEndianness((uint)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ReverseEndianness(long value)
		{
			return (long)BinaryPrimitives.ReverseEndianness((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReverseEndianness(byte value)
		{
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReverseEndianness(ushort value)
		{
			return (ushort)(((int)(value & 255) << 8) | (int)((uint)(value & 65280) >> 8));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReverseEndianness(uint value)
		{
			value = (value << 16) | (value >> 16);
			value = ((value & 16711935U) << 8) | ((value & 4278255360U) >> 8);
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReverseEndianness(ulong value)
		{
			value = (value << 32) | (value >> 32);
			value = ((value & 281470681808895UL) << 16) | ((value & 18446462603027742720UL) >> 16);
			value = ((value & 71777214294589695UL) << 8) | ((value & 18374966859414961920UL) >> 8);
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadMachineEndian<T>(ReadOnlySpan<byte> buffer) where T : struct
		{
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				throw new ArgumentException(SR.Format("Cannot use type '{0}'. Only value types without pointers or references are supported.", typeof(T)));
			}
			if (Unsafe.SizeOf<T>() > buffer.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			return Unsafe.ReadUnaligned<T>(buffer.DangerousGetPinnableReference());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadMachineEndian<T>(ReadOnlySpan<byte> buffer, out T value) where T : struct
		{
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				throw new ArgumentException(SR.Format("Cannot use type '{0}'. Only value types without pointers or references are supported.", typeof(T)));
			}
			if ((long)Unsafe.SizeOf<T>() > (long)((ulong)buffer.Length))
			{
				value = default(T);
				return false;
			}
			value = Unsafe.ReadUnaligned<T>(buffer.DangerousGetPinnableReference());
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ReadInt16BigEndian(ReadOnlySpan<byte> buffer)
		{
			short num = BinaryPrimitives.ReadMachineEndian<short>(buffer);
			if (BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ReadInt32BigEndian(ReadOnlySpan<byte> buffer)
		{
			int num = BinaryPrimitives.ReadMachineEndian<int>(buffer);
			if (BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ReadInt64BigEndian(ReadOnlySpan<byte> buffer)
		{
			long num = BinaryPrimitives.ReadMachineEndian<long>(buffer);
			if (BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReadUInt16BigEndian(ReadOnlySpan<byte> buffer)
		{
			ushort num = BinaryPrimitives.ReadMachineEndian<ushort>(buffer);
			if (BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt32BigEndian(ReadOnlySpan<byte> buffer)
		{
			uint num = BinaryPrimitives.ReadMachineEndian<uint>(buffer);
			if (BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReadUInt64BigEndian(ReadOnlySpan<byte> buffer)
		{
			ulong num = BinaryPrimitives.ReadMachineEndian<ulong>(buffer);
			if (BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadInt16BigEndian(ReadOnlySpan<byte> buffer, out short value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<short>(buffer, out value);
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadInt32BigEndian(ReadOnlySpan<byte> buffer, out int value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<int>(buffer, out value);
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadInt64BigEndian(ReadOnlySpan<byte> buffer, out long value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<long>(buffer, out value);
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadUInt16BigEndian(ReadOnlySpan<byte> buffer, out ushort value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<ushort>(buffer, out value);
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadUInt32BigEndian(ReadOnlySpan<byte> buffer, out uint value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<uint>(buffer, out value);
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadUInt64BigEndian(ReadOnlySpan<byte> buffer, out ulong value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<ulong>(buffer, out value);
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ReadInt16LittleEndian(ReadOnlySpan<byte> buffer)
		{
			short num = BinaryPrimitives.ReadMachineEndian<short>(buffer);
			if (!BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ReadInt32LittleEndian(ReadOnlySpan<byte> buffer)
		{
			int num = BinaryPrimitives.ReadMachineEndian<int>(buffer);
			if (!BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ReadInt64LittleEndian(ReadOnlySpan<byte> buffer)
		{
			long num = BinaryPrimitives.ReadMachineEndian<long>(buffer);
			if (!BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReadUInt16LittleEndian(ReadOnlySpan<byte> buffer)
		{
			ushort num = BinaryPrimitives.ReadMachineEndian<ushort>(buffer);
			if (!BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt32LittleEndian(ReadOnlySpan<byte> buffer)
		{
			uint num = BinaryPrimitives.ReadMachineEndian<uint>(buffer);
			if (!BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReadUInt64LittleEndian(ReadOnlySpan<byte> buffer)
		{
			ulong num = BinaryPrimitives.ReadMachineEndian<ulong>(buffer);
			if (!BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadInt16LittleEndian(ReadOnlySpan<byte> buffer, out short value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<short>(buffer, out value);
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadInt32LittleEndian(ReadOnlySpan<byte> buffer, out int value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<int>(buffer, out value);
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadInt64LittleEndian(ReadOnlySpan<byte> buffer, out long value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<long>(buffer, out value);
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadUInt16LittleEndian(ReadOnlySpan<byte> buffer, out ushort value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<ushort>(buffer, out value);
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadUInt32LittleEndian(ReadOnlySpan<byte> buffer, out uint value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<uint>(buffer, out value);
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryReadUInt64LittleEndian(ReadOnlySpan<byte> buffer, out ulong value)
		{
			bool flag = BinaryPrimitives.TryReadMachineEndian<ulong>(buffer, out value);
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteMachineEndian<T>(Span<byte> buffer, ref T value) where T : struct
		{
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				throw new ArgumentException(SR.Format("Cannot use type '{0}'. Only value types without pointers or references are supported.", typeof(T)));
			}
			if (Unsafe.SizeOf<T>() > buffer.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			Unsafe.WriteUnaligned<T>(buffer.DangerousGetPinnableReference(), value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteMachineEndian<T>(Span<byte> buffer, ref T value) where T : struct
		{
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				throw new ArgumentException(SR.Format("Cannot use type '{0}'. Only value types without pointers or references are supported.", typeof(T)));
			}
			if ((long)Unsafe.SizeOf<T>() > (long)((ulong)buffer.Length))
			{
				return false;
			}
			Unsafe.WriteUnaligned<T>(buffer.DangerousGetPinnableReference(), value);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteInt16BigEndian(Span<byte> buffer, short value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<short>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteInt32BigEndian(Span<byte> buffer, int value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<int>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteInt64BigEndian(Span<byte> buffer, long value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<long>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt16BigEndian(Span<byte> buffer, ushort value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<ushort>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt32BigEndian(Span<byte> buffer, uint value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<uint>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt64BigEndian(Span<byte> buffer, ulong value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<ulong>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteInt16BigEndian(Span<byte> buffer, short value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<short>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteInt32BigEndian(Span<byte> buffer, int value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<int>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteInt64BigEndian(Span<byte> buffer, long value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<long>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteUInt16BigEndian(Span<byte> buffer, ushort value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<ushort>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteUInt32BigEndian(Span<byte> buffer, uint value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<uint>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteUInt64BigEndian(Span<byte> buffer, ulong value)
		{
			if (BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<ulong>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteInt16LittleEndian(Span<byte> buffer, short value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<short>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteInt32LittleEndian(Span<byte> buffer, int value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<int>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteInt64LittleEndian(Span<byte> buffer, long value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<long>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt16LittleEndian(Span<byte> buffer, ushort value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<ushort>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt32LittleEndian(Span<byte> buffer, uint value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<uint>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt64LittleEndian(Span<byte> buffer, ulong value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			BinaryPrimitives.WriteMachineEndian<ulong>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteInt16LittleEndian(Span<byte> buffer, short value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<short>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteInt32LittleEndian(Span<byte> buffer, int value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<int>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteInt64LittleEndian(Span<byte> buffer, long value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<long>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteUInt16LittleEndian(Span<byte> buffer, ushort value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<ushort>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteUInt32LittleEndian(Span<byte> buffer, uint value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<uint>(buffer, ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteUInt64LittleEndian(Span<byte> buffer, ulong value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			return BinaryPrimitives.TryWriteMachineEndian<ulong>(buffer, ref value);
		}
	}
}
