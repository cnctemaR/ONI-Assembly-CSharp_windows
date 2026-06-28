using System;

namespace System.Collections.Generic
{
	[Serializable]
	public abstract class Comparer<T> : IComparer<T>, IComparer
	{
		static Comparer()
		{
			if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
			{
				Comparer<T>._default = (Comparer<T>)Activator.CreateInstance(typeof(GenericComparer<>).MakeGenericType(new Type[] { typeof(T) }));
			}
			else
			{
				Comparer<T>._default = new Comparer<T>.DefaultComparer();
			}
		}

		int IComparer.Compare(object x, object y)
		{
			if (x == null)
			{
				return (y != null) ? (-1) : 0;
			}
			if (y == null)
			{
				return 1;
			}
			if (x is T && y is T)
			{
				return this.Compare((T)((object)x), (T)((object)y));
			}
			throw new ArgumentException();
		}

		public abstract int Compare(T x, T y);

		public static Comparer<T> Default
		{
			get
			{
				return Comparer<T>._default;
			}
		}

		private static readonly Comparer<T> _default;

		private sealed class DefaultComparer : Comparer<T>
		{
			public override int Compare(T x, T y)
			{
				if (x == null)
				{
					return (y != null) ? (-1) : 0;
				}
				if (y == null)
				{
					return 1;
				}
				if (x is IComparable<T>)
				{
					return ((IComparable<T>)((object)x)).CompareTo(y);
				}
				if (x is IComparable)
				{
					return ((IComparable)((object)x)).CompareTo(y);
				}
				throw new ArgumentException("does not implement right interface");
			}
		}
	}
}
