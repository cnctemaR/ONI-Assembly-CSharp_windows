using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine.Bindings;

namespace Unity.Collections
{
	[VisibleToOtherModules]
	internal static class CollectionExtensions
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void AddToArray<T>(ref T[] array, T item)
		{
			Array.Resize<T>(ref array, array.Length + 1);
			T[] array2 = array;
			array2[array2.Length - 1] = item;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void InsertIntoArray<T>(ref T[] array, int index, T item)
		{
			bool flag = index < 0 || index > array.Length;
			if (flag)
			{
				throw new IndexOutOfRangeException("Trying to insert into an array out of bounds.");
			}
			T[] array2 = array;
			array = new T[array2.Length + 1];
			Array.Copy(array2, array, index);
			Array.Copy(array2, index, array, index + 1, array2.Length - index);
			array[index] = item;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEditor.UIBuilderModule" })]
		internal static bool RemoveFromArray<T>(ref T[] array, T item)
		{
			int num = Array.IndexOf<T>(array, item);
			bool flag = num == -1;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				CollectionExtensions.RemoveFromArray<T>(ref array, num);
				flag2 = true;
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEditor.UIBuilderModule" })]
		internal static void RemoveFromArray<T>(ref T[] array, int index)
		{
			bool flag = index < 0 || index >= array.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int i = 0;
			int num = 0;
			while (i < array.Length)
			{
				bool flag2 = i != index;
				if (flag2)
				{
					array[num++] = array[i];
				}
				i++;
			}
			Array.Resize<T>(ref array, array.Length - 1);
		}

		internal static void AddSorted<T>([DisallowNull] this List<T> list, T item, IComparer<T> comparer = null)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list must not be null.");
			}
			if (comparer == null)
			{
				comparer = Comparer<T>.Default;
			}
			bool flag2 = list.Count == 0;
			if (flag2)
			{
				list.Add(item);
			}
			else
			{
				bool flag3 = comparer.Compare(list[list.Count - 1], item) <= 0;
				if (flag3)
				{
					list.Add(item);
				}
				else
				{
					bool flag4 = comparer.Compare(list[0], item) >= 0;
					if (flag4)
					{
						list.Insert(0, item);
					}
					else
					{
						int num = list.BinarySearch(item, comparer);
						bool flag5 = num < 0;
						if (flag5)
						{
							num = ~num;
						}
						list.Insert(num, item);
					}
				}
			}
		}

		public static void Fill<T>([DisallowNull] this List<T> dest, T value, int count)
		{
			bool flag = dest == null;
			if (flag)
			{
				throw new ArgumentNullException("dest");
			}
			dest.Capacity = Math.Max(dest.Capacity, dest.Count + count);
			while (count-- > 0)
			{
				dest.Add(value);
			}
		}

		public static T FirstOrDefaultSorted<T>(this IEnumerable<T> collection, IComparer<T> comparer = null)
		{
			bool flag = collection == null;
			if (flag)
			{
				throw new ArgumentNullException("collection must not be null.");
			}
			if (comparer == null)
			{
				comparer = Comparer<T>.Default;
			}
			bool flag2 = false;
			T t = default(T);
			foreach (T t2 in collection)
			{
				bool flag3 = !flag2;
				if (flag3)
				{
					t = t2;
					flag2 = true;
				}
				bool flag4 = comparer.Compare(t2, t) < 0;
				if (flag4)
				{
					t = t2;
				}
			}
			return t;
		}

		internal static string SerializedView<T>([DisallowNull] this IEnumerable<T> collection, [DisallowNull] Func<T, string> serializeElement)
		{
			bool flag = collection == null;
			if (flag)
			{
				throw new ArgumentNullException("collection must not be null.");
			}
			bool flag2 = serializeElement == null;
			if (flag2)
			{
				throw new ArgumentNullException("Argument serializeElement must not be null.");
			}
			return "[" + string.Join(",", collection.Select<T, string>((T t) => (t == null) ? "null" : serializeElement(t))) + "]";
		}

		internal static bool ContainsByEquals<T>([DisallowNull] this IEnumerable<T> collection, T element)
		{
			bool flag = collection == null;
			if (flag)
			{
				throw new ArgumentNullException("collection must not be null.");
			}
			foreach (T t in collection)
			{
				bool flag2 = t.Equals(element);
				if (flag2)
				{
					return true;
				}
			}
			return false;
		}
	}
}
