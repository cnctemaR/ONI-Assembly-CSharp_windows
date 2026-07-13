using System;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace MonoMod.Backports.ILHelpers
{
	internal static class UnsafeRaw
	{
		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T Read<T>(void* source)
		{
			return *(T*)source;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T ReadUnaligned<T>(void* source)
		{
			return *(T*)source;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadUnaligned<T>(ref byte source)
		{
			return source;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Write<T>(void* destination, T value)
		{
			*(T*)destination = value;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUnaligned<T>(void* destination, T value)
		{
			*(T*)destination = value;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUnaligned<T>(ref byte destination, T value)
		{
			destination = value;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Copy<T>(void* destination, ref T source)
		{
			*(T*)destination = source;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Copy<T>(ref T destination, void* source)
		{
			destination = *(T*)source;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* AsPointer<T>(ref T value)
		{
			return (void*)(&value);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SkipInit<T>(out T value)
		{
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SizeOf<T>()
		{
			return sizeof(T);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyBlock(void* destination, void* source, uint byteCount)
		{
			cpblk(destination, source, byteCount);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlock(ref byte destination, ref byte source, uint byteCount)
		{
			cpblk(ref destination, ref source, byteCount);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyBlockUnaligned(void* destination, void* source, uint byteCount)
		{
			cpblk(destination, source, byteCount);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlockUnaligned(ref byte destination, ref byte source, uint byteCount)
		{
			cpblk(ref destination, ref source, byteCount);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void InitBlock(void* startAddress, byte value, uint byteCount)
		{
			initblk(startAddress, value, byteCount);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlock(ref byte startAddress, byte value, uint byteCount)
		{
			initblk(ref startAddress, value, byteCount);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void InitBlockUnaligned(void* startAddress, byte value, uint byteCount)
		{
			initblk(startAddress, value, byteCount);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
		{
			initblk(ref startAddress, value, byteCount);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T As<T>(object o) where T : class
		{
			return o;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ref T AsRef<T>(void* source)
		{
			return ref *(T*)source;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AsRef<T>(in T source)
		{
			return ref source;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref TTo As<TFrom, TTo>(ref TFrom source)
		{
			return ref source;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Unbox<T>(object box) where T : struct
		{
			return ref (T)box;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, int elementOffset)
		{
			return (ref source) + (IntPtr)elementOffset * (IntPtr)sizeof(T);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* Add<T>(void* source, int elementOffset)
		{
			return (void*)((byte*)source + (IntPtr)elementOffset * (IntPtr)sizeof(T));
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, [NativeInteger] IntPtr elementOffset)
		{
			return (ref source) + elementOffset * (IntPtr)sizeof(T);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, [NativeInteger] UIntPtr elementOffset)
		{
			return (ref source) + elementOffset * (UIntPtr)sizeof(T);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AddByteOffset<T>(ref T source, [NativeInteger] IntPtr byteOffset)
		{
			return (ref source) + byteOffset;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AddByteOffset<T>(ref T source, [NativeInteger] UIntPtr byteOffset)
		{
			return (ref source) + byteOffset;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, int elementOffset)
		{
			return (ref source) - (IntPtr)elementOffset * (IntPtr)sizeof(T);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* Subtract<T>(void* source, int elementOffset)
		{
			return (void*)((byte*)source - (IntPtr)elementOffset * (IntPtr)sizeof(T));
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, [NativeInteger] IntPtr elementOffset)
		{
			return (ref source) - elementOffset * (IntPtr)sizeof(T);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, [NativeInteger] UIntPtr elementOffset)
		{
			return (ref source) - elementOffset * (UIntPtr)sizeof(T);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T SubtractByteOffset<T>(ref T source, [NativeInteger] IntPtr byteOffset)
		{
			return (ref source) - byteOffset;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T SubtractByteOffset<T>(ref T source, [NativeInteger] UIntPtr byteOffset)
		{
			return (ref source) - byteOffset;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: NativeInteger]
		public static IntPtr ByteOffset<T>(ref T origin, ref T target)
		{
			return (ref target) - (ref origin);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AreSame<T>(ref T left, ref T right)
		{
			return (ref left) == (ref right);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressGreaterThan<T>(ref T left, ref T right)
		{
			return (ref left) != (ref right);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressLessThan<T>(ref T left, ref T right)
		{
			return (ref left) < (ref right);
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullRef<T>(ref T source)
		{
			return (ref source) == (UIntPtr)0;
		}

		[global::System.Runtime.Versioning.NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T NullRef<T>()
		{
			return (UIntPtr)0;
		}
	}
}
