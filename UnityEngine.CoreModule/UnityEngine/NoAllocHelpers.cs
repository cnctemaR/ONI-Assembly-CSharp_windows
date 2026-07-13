using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[VisibleToOtherModules]
	internal static class NoAllocHelpers
	{
		public unsafe static void EnsureListElemCount<T>(List<T> list, int count)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			bool flag2 = count < 0;
			if (flag2)
			{
				throw new ArgumentException("invalid size to resize.", "list");
			}
			list.Clear();
			bool flag3 = list.Capacity < count;
			if (flag3)
			{
				list.Capacity = count;
			}
			bool flag4 = count != list.Count;
			if (flag4)
			{
				NoAllocHelpers.ListPrivateFieldAccess<T> listPrivateFieldAccess = *UnsafeUtility.As<List<T>, NoAllocHelpers.ListPrivateFieldAccess<T>>(ref list);
				listPrivateFieldAccess._size = count;
				listPrivateFieldAccess._version++;
			}
		}

		public static int SafeLength(Array values)
		{
			return (values != null) ? values.Length : 0;
		}

		public static int SafeLength<T>(List<T> values)
		{
			return (values != null) ? values.Count : 0;
		}

		[Obsolete("Use ExtractArrayFromList", false)]
		public static T[] ExtractArrayFromListT<T>(List<T> list)
		{
			return NoAllocHelpers.ExtractArrayFromList<T>(list);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ExtractArrayFromList<T>(List<T> list)
		{
			bool flag = list == null;
			T[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				NoAllocHelpers.ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<NoAllocHelpers.ListPrivateFieldAccess<T>>(list);
				array = listPrivateFieldAccess._items;
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> CreateSpan<T>(List<T> list)
		{
			bool flag = list == null;
			Span<T> span;
			if (flag)
			{
				span = default(Span<T>);
			}
			else
			{
				NoAllocHelpers.ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<NoAllocHelpers.ListPrivateFieldAccess<T>>(list);
				span = new Span<T>(listPrivateFieldAccess._items, 0, listPrivateFieldAccess._size);
			}
			return span;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> CreateReadOnlySpan<T>(List<T> list)
		{
			bool flag = list == null;
			ReadOnlySpan<T> readOnlySpan;
			if (flag)
			{
				readOnlySpan = default(ReadOnlySpan<T>);
			}
			else
			{
				NoAllocHelpers.ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<NoAllocHelpers.ListPrivateFieldAccess<T>>(list);
				readOnlySpan = new ReadOnlySpan<T>(listPrivateFieldAccess._items, 0, listPrivateFieldAccess._size);
			}
			return readOnlySpan;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ResetListContents<T>(List<T> list, ReadOnlySpan<T> span)
		{
			NoAllocHelpers.ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<NoAllocHelpers.ListPrivateFieldAccess<T>>(list);
			listPrivateFieldAccess._items = span.ToArray();
			listPrivateFieldAccess._size = span.Length;
			listPrivateFieldAccess._version++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ResetListContents<T>(List<T> list, T[] array)
		{
			NoAllocHelpers.ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<NoAllocHelpers.ListPrivateFieldAccess<T>>(list);
			listPrivateFieldAccess._items = array;
			listPrivateFieldAccess._size = array.Length;
			listPrivateFieldAccess._version++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ResetListSize<T>(List<T> list, int size)
		{
			bool flag = list.Capacity < size;
			if (flag)
			{
				throw new ArgumentException(string.Format("Resetting to {0} which is bigger than capacity {1} is not allowed!", size, list.Capacity));
			}
			NoAllocHelpers.ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<NoAllocHelpers.ListPrivateFieldAccess<T>>(list);
			bool flag2 = RuntimeHelpers.IsReferenceOrContainsReferences<T>() && listPrivateFieldAccess._size > size;
			if (flag2)
			{
				Array.Clear(listPrivateFieldAccess._items, size, listPrivateFieldAccess._size - size);
			}
			listPrivateFieldAccess._size = size;
			listPrivateFieldAccess._version++;
		}

		[RequiredByNativeCode]
		private static Array PrepareListForNativeFill(object list, Type elementType, int newSize)
		{
			NoAllocHelpers.ListPrivateFieldAccess<byte> listPrivateFieldAccess = UnsafeUtility.As<NoAllocHelpers.ListPrivateFieldAccess<byte>>(list);
			ref byte[] ptr = ref listPrivateFieldAccess._items;
			int num = ptr.Length;
			int size = listPrivateFieldAccess._size;
			bool flag = num < newSize;
			if (flag)
			{
				ptr = UnsafeUtility.As<byte[]>(Array.CreateInstance(elementType, newSize));
			}
			else
			{
				bool flag2 = size > newSize;
				if (flag2)
				{
					Array.Clear(ptr, newSize, size - newSize);
				}
			}
			listPrivateFieldAccess._size = newSize;
			listPrivateFieldAccess._version++;
			return ptr;
		}

		private class ListPrivateFieldAccess<T>
		{
			internal T[] _items;

			internal int _size;

			internal int _version;
		}
	}
}
