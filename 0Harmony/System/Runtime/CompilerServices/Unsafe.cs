using System;
using MonoMod.Backports.ILHelpers;

namespace System.Runtime.CompilerServices
{
	[CLSCompliant(false)]
	internal static class Unsafe
	{
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T Read<T>(void* source)
		{
			return UnsafeRaw.Read<T>(source);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T ReadUnaligned<T>(void* source)
		{
			return UnsafeRaw.ReadUnaligned<T>(source);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadUnaligned<T>(ref byte source)
		{
			return UnsafeRaw.ReadUnaligned<T>(ref source);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Write<T>(void* destination, T value)
		{
			UnsafeRaw.Write<T>(destination, value);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUnaligned<T>(void* destination, T value)
		{
			UnsafeRaw.WriteUnaligned<T>(destination, value);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUnaligned<T>(ref byte destination, T value)
		{
			UnsafeRaw.WriteUnaligned<T>(ref destination, value);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Copy<T>(void* destination, ref T source)
		{
			UnsafeRaw.Copy<T>(destination, ref source);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Copy<T>(ref T destination, void* source)
		{
			UnsafeRaw.Copy<T>(ref destination, source);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* AsPointer<T>(ref T value)
		{
			return UnsafeRaw.AsPointer<T>(ref value);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SkipInit<T>(out T value)
		{
			UnsafeRaw.SkipInit<T>(out value);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyBlock(void* destination, void* source, uint byteCount)
		{
			UnsafeRaw.CopyBlock(destination, source, byteCount);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlock(ref byte destination, ref byte source, uint byteCount)
		{
			UnsafeRaw.CopyBlock(ref destination, ref source, byteCount);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyBlockUnaligned(void* destination, void* source, uint byteCount)
		{
			UnsafeRaw.CopyBlockUnaligned(destination, source, byteCount);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlockUnaligned(ref byte destination, ref byte source, uint byteCount)
		{
			UnsafeRaw.CopyBlockUnaligned(ref destination, ref source, byteCount);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void InitBlock(void* startAddress, byte value, uint byteCount)
		{
			UnsafeRaw.InitBlock(startAddress, value, byteCount);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlock(ref byte startAddress, byte value, uint byteCount)
		{
			UnsafeRaw.InitBlock(ref startAddress, value, byteCount);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void InitBlockUnaligned(void* startAddress, byte value, uint byteCount)
		{
			UnsafeRaw.InitBlockUnaligned(startAddress, value, byteCount);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
		{
			UnsafeRaw.InitBlockUnaligned(ref startAddress, value, byteCount);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T As<T>(object o) where T : class
		{
			return UnsafeRaw.As<T>(o);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ref T AsRef<T>(void* source)
		{
			return UnsafeRaw.AsRef<T>(source);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AsRef<T>(in T source)
		{
			return UnsafeRaw.AsRef<T>(in source);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref TTo As<TFrom, TTo>(ref TFrom source)
		{
			return UnsafeRaw.As<TFrom, TTo>(ref source);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Unbox<T>(object box) where T : struct
		{
			return UnsafeRaw.Unbox<T>(box);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AddByteOffset<T>(ref T source, [NativeInteger] IntPtr byteOffset)
		{
			return UnsafeRaw.AddByteOffset<T>(ref source, byteOffset);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AddByteOffset<T>(ref T source, [NativeInteger] UIntPtr byteOffset)
		{
			return UnsafeRaw.AddByteOffset<T>(ref source, byteOffset);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T SubtractByteOffset<T>(ref T source, [NativeInteger] IntPtr byteOffset)
		{
			return UnsafeRaw.SubtractByteOffset<T>(ref source, byteOffset);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T SubtractByteOffset<T>(ref T source, [NativeInteger] UIntPtr byteOffset)
		{
			return UnsafeRaw.SubtractByteOffset<T>(ref source, byteOffset);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: NativeInteger]
		public static IntPtr ByteOffset<T>(ref T origin, ref T target)
		{
			return UnsafeRaw.ByteOffset<T>(ref origin, ref target);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AreSame<T>(ref T left, ref T right)
		{
			return UnsafeRaw.AreSame<T>(ref left, ref right);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressGreaterThan<T>(ref T left, ref T right)
		{
			return UnsafeRaw.IsAddressGreaterThan<T>(ref left, ref right);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressLessThan<T>(ref T left, ref T right)
		{
			return UnsafeRaw.IsAddressLessThan<T>(ref left, ref right);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullRef<T>(ref T source)
		{
			return UnsafeRaw.IsNullRef<T>(ref source);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T NullRef<T>()
		{
			return UnsafeRaw.NullRef<T>();
		}

		[NullableContext(2)]
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SizeOf<T>()
		{
			return (int)Unsafe.PerTypeValues<T>.TypeSize;
		}

		[NullableContext(1)]
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<[Nullable(2)] T>(ref T source, int elementOffset)
		{
			return UnsafeRaw.AddByteOffset<T>(ref source, (IntPtr)elementOffset * Unsafe.PerTypeValues<T>.TypeSize);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* Add<[Nullable(2)] T>(void* source, int elementOffset)
		{
			return (void*)((byte*)source + (long)((IntPtr)elementOffset * Unsafe.PerTypeValues<T>.TypeSize));
		}

		[NullableContext(1)]
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<[Nullable(2)] T>(ref T source, [NativeInteger] IntPtr elementOffset)
		{
			return UnsafeRaw.AddByteOffset<T>(ref source, elementOffset * Unsafe.PerTypeValues<T>.TypeSize);
		}

		[NullableContext(1)]
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<[Nullable(2)] T>(ref T source, [NativeInteger] UIntPtr elementOffset)
		{
			return UnsafeRaw.AddByteOffset<T>(ref source, elementOffset * (UIntPtr)Unsafe.PerTypeValues<T>.TypeSize);
		}

		[NullableContext(1)]
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<[Nullable(2)] T>(ref T source, int elementOffset)
		{
			return UnsafeRaw.SubtractByteOffset<T>(ref source, (IntPtr)elementOffset * Unsafe.PerTypeValues<T>.TypeSize);
		}

		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* Subtract<[Nullable(2)] T>(void* source, int elementOffset)
		{
			return (void*)((byte*)source - (long)((IntPtr)elementOffset * Unsafe.PerTypeValues<T>.TypeSize));
		}

		[NullableContext(1)]
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<[Nullable(2)] T>(ref T source, [NativeInteger] IntPtr elementOffset)
		{
			return UnsafeRaw.SubtractByteOffset<T>(ref source, elementOffset * Unsafe.PerTypeValues<T>.TypeSize);
		}

		[NullableContext(1)]
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<[Nullable(2)] T>(ref T source, [NativeInteger] UIntPtr elementOffset)
		{
			return UnsafeRaw.SubtractByteOffset<T>(ref source, elementOffset * (UIntPtr)Unsafe.PerTypeValues<T>.TypeSize);
		}

		private static class PerTypeValues<[Nullable(2)] T>
		{
			[return: NativeInteger]
			private static IntPtr ComputeTypeSize()
			{
				T[] array = new T[2];
				return UnsafeRaw.ByteOffset<T>(ref array[0], ref array[1]);
			}

			[NativeInteger]
			public static readonly IntPtr TypeSize = Unsafe.PerTypeValues<T>.ComputeTypeSize();
		}
	}
}
