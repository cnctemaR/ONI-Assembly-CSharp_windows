using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/NoAllocHelpers.bindings.h")]
	internal sealed class NoAllocHelpers
	{
		public static void ResizeList<T>(List<T> list, int size)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (size < 0 || size > list.Capacity)
			{
				throw new ArgumentException("invalid size to resize.", "list");
			}
			if (size != list.Count)
			{
				NoAllocHelpers.Internal_ResizeList(list, size);
			}
		}

		public static void EnsureListElemCount<T>(List<T> list, int count)
		{
			list.Clear();
			if (list.Capacity < count)
			{
				list.Capacity = count;
			}
			NoAllocHelpers.ResizeList<T>(list, count);
		}

		public static int SafeLength(Array values)
		{
			return (values == null) ? 0 : values.Length;
		}

		public static int SafeLength<T>(List<T> values)
		{
			return (values == null) ? 0 : values.Count;
		}

		public static T[] ExtractArrayFromListT<T>(List<T> list)
		{
			return (T[])NoAllocHelpers.ExtractArrayFromList(list);
		}

		[FreeFunction("NoAllocHelpers_Bindings::Internal_ResizeList")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_ResizeList(object list, int size);

		[FreeFunction("NoAllocHelpers_Bindings::ExtractArrayFromList")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Array ExtractArrayFromList(object list);
	}
}
