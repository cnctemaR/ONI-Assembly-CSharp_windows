using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	public static class FixedListExtensions
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList32Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList32Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T, U>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length, comp);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList64Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList64Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T, U>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length, comp);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList128Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList128Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T, U>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length, comp);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList512Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList512Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T, U>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length, comp);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList4096Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList4096Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort<T, U>((T*)ptr + FixedList.PaddingBytes<T>() / sizeof(T), list.Length, comp);
			}
		}
	}
}
