using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Internal.Collections
{
	internal static class CollectionServices
	{
		public static ICollection<object> GetCollectionWrapper(Type itemType, object collectionObject)
		{
			Assumes.NotNull<Type, object>(itemType, collectionObject);
			Type underlyingSystemType = itemType.UnderlyingSystemType;
			if (underlyingSystemType == typeof(object))
			{
				return (ICollection<object>)collectionObject;
			}
			if (typeof(IList).IsAssignableFrom(collectionObject.GetType()))
			{
				return new CollectionServices.CollectionOfObjectList((IList)collectionObject);
			}
			return (ICollection<object>)Activator.CreateInstance(typeof(CollectionServices.CollectionOfObject<>).MakeGenericType(new Type[] { underlyingSystemType }), new object[] { collectionObject });
		}

		public static bool IsEnumerableOfT(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition().UnderlyingSystemType == CollectionServices.IEnumerableOfTType;
		}

		public static Type GetEnumerableElementType(Type type)
		{
			if (type.UnderlyingSystemType == CollectionServices.StringType || !CollectionServices.IEnumerableType.IsAssignableFrom(type))
			{
				return null;
			}
			Type type2;
			if (ReflectionServices.TryGetGenericInterfaceType(type, CollectionServices.IEnumerableOfTType, out type2))
			{
				return type2.GetGenericArguments()[0];
			}
			return null;
		}

		public static Type GetCollectionElementType(Type type)
		{
			Type type2;
			if (ReflectionServices.TryGetGenericInterfaceType(type, CollectionServices.ICollectionOfTType, out type2))
			{
				return type2.GetGenericArguments()[0];
			}
			return null;
		}

		public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
		{
			Assumes.NotNull<IEnumerable<T>>(source);
			return new ReadOnlyCollection<T>(source.AsArray<T>());
		}

		public static IEnumerable<T> ConcatAllowingNull<T>(this IEnumerable<T> source, IEnumerable<T> second)
		{
			if (second == null || !second.FastAny<T>())
			{
				return source;
			}
			if (source == null || !source.FastAny<T>())
			{
				return second;
			}
			return source.Concat<T>(second);
		}

		public static ICollection<T> ConcatAllowingNull<T>(this ICollection<T> source, ICollection<T> second)
		{
			if (second == null || second.Count == 0)
			{
				return source;
			}
			if (source == null || source.Count == 0)
			{
				return second;
			}
			List<T> list = new List<T>(source);
			list.AddRange(second);
			return list;
		}

		public static List<T> FastAppendToListAllowNulls<T>(this List<T> source, IEnumerable<T> second)
		{
			if (second == null)
			{
				return source;
			}
			if (source == null || source.Count == 0)
			{
				return second.AsList<T>();
			}
			List<T> list = second as List<T>;
			if (list != null)
			{
				if (list.Count == 0)
				{
					return source;
				}
				if (list.Count == 1)
				{
					source.Add(list[0]);
					return source;
				}
			}
			source.AddRange(second);
			return source;
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T t in source)
			{
				action(t);
			}
		}

		public static EnumerableCardinality GetCardinality<T>(this IEnumerable<T> source)
		{
			Assumes.NotNull<IEnumerable<T>>(source);
			ICollection collection = source as ICollection;
			if (collection == null)
			{
				EnumerableCardinality enumerableCardinality;
				using (IEnumerator<T> enumerator = source.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						enumerableCardinality = EnumerableCardinality.Zero;
					}
					else if (!enumerator.MoveNext())
					{
						enumerableCardinality = EnumerableCardinality.One;
					}
					else
					{
						enumerableCardinality = EnumerableCardinality.TwoOrMore;
					}
				}
				return enumerableCardinality;
			}
			int count = collection.Count;
			if (count == 0)
			{
				return EnumerableCardinality.Zero;
			}
			if (count != 1)
			{
				return EnumerableCardinality.TwoOrMore;
			}
			return EnumerableCardinality.One;
		}

		public static bool FastAny<T>(this IEnumerable<T> source)
		{
			ICollection collection = source as ICollection;
			if (collection != null)
			{
				return collection.Count > 0;
			}
			return source.Any<T>();
		}

		public static Stack<T> Copy<T>(this Stack<T> stack)
		{
			Assumes.NotNull<Stack<T>>(stack);
			return new Stack<T>(stack.Reverse<T>());
		}

		public static T[] AsArray<T>(this IEnumerable<T> enumerable)
		{
			T[] array = enumerable as T[];
			if (array != null)
			{
				return array;
			}
			return enumerable.ToArray<T>();
		}

		public static List<T> AsList<T>(this IEnumerable<T> enumerable)
		{
			List<T> list = enumerable as List<T>;
			if (list != null)
			{
				return list;
			}
			return enumerable.ToList<T>();
		}

		public static bool IsArrayEqual<T>(this T[] thisArray, T[] thatArray)
		{
			if (thisArray.Length != thatArray.Length)
			{
				return false;
			}
			for (int i = 0; i < thisArray.Length; i++)
			{
				if (!thisArray[i].Equals(thatArray[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsCollectionEqual<T>(this IList<T> thisList, IList<T> thatList)
		{
			if (thisList.Count != thatList.Count)
			{
				return false;
			}
			for (int i = 0; i < thisList.Count; i++)
			{
				T t = thisList[i];
				if (!t.Equals(thatList[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static readonly Type StringType = typeof(string);

		private static readonly Type IEnumerableType = typeof(IEnumerable);

		private static readonly Type IEnumerableOfTType = typeof(IEnumerable<>);

		private static readonly Type ICollectionOfTType = typeof(ICollection<>);

		private class CollectionOfObjectList : ICollection<object>, IEnumerable<object>, IEnumerable
		{
			public CollectionOfObjectList(IList list)
			{
				this._list = list;
			}

			public void Add(object item)
			{
				this._list.Add(item);
			}

			public void Clear()
			{
				this._list.Clear();
			}

			public bool Contains(object item)
			{
				return Assumes.NotReachable<bool>();
			}

			public void CopyTo(object[] array, int arrayIndex)
			{
				Assumes.NotReachable<object>();
			}

			public int Count
			{
				get
				{
					return Assumes.NotReachable<int>();
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return this._list.IsReadOnly;
				}
			}

			public bool Remove(object item)
			{
				return Assumes.NotReachable<bool>();
			}

			public IEnumerator<object> GetEnumerator()
			{
				return Assumes.NotReachable<IEnumerator<object>>();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return Assumes.NotReachable<IEnumerator>();
			}

			private readonly IList _list;
		}

		private class CollectionOfObject<T> : ICollection<object>, IEnumerable<object>, IEnumerable
		{
			public CollectionOfObject(object collectionOfT)
			{
				this._collectionOfT = (ICollection<T>)collectionOfT;
			}

			public void Add(object item)
			{
				this._collectionOfT.Add((T)((object)item));
			}

			public void Clear()
			{
				this._collectionOfT.Clear();
			}

			public bool Contains(object item)
			{
				return Assumes.NotReachable<bool>();
			}

			public void CopyTo(object[] array, int arrayIndex)
			{
				Assumes.NotReachable<object>();
			}

			public int Count
			{
				get
				{
					return Assumes.NotReachable<int>();
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return this._collectionOfT.IsReadOnly;
				}
			}

			public bool Remove(object item)
			{
				return Assumes.NotReachable<bool>();
			}

			public IEnumerator<object> GetEnumerator()
			{
				return Assumes.NotReachable<IEnumerator<object>>();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return Assumes.NotReachable<IEnumerator>();
			}

			private readonly ICollection<T> _collectionOfT;
		}
	}
}
