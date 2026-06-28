using System;

namespace System.Collections.Generic
{
	[Serializable]
	public abstract class EqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
	{
		static EqualityComparer()
		{
			if (typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
			{
				EqualityComparer<T>._default = (EqualityComparer<T>)Activator.CreateInstance(typeof(GenericEqualityComparer<>).MakeGenericType(new Type[] { typeof(T) }));
			}
			else
			{
				EqualityComparer<T>._default = new EqualityComparer<T>.DefaultComparer();
			}
		}

		int IEqualityComparer.GetHashCode(object obj)
		{
			return this.GetHashCode((T)((object)obj));
		}

		bool IEqualityComparer.Equals(object x, object y)
		{
			return this.Equals((T)((object)x), (T)((object)y));
		}

		public abstract int GetHashCode(T obj);

		public abstract bool Equals(T x, T y);

		public static EqualityComparer<T> Default
		{
			get
			{
				return EqualityComparer<T>._default;
			}
		}

		private static readonly EqualityComparer<T> _default;

		[Serializable]
		private sealed class DefaultComparer : EqualityComparer<T>
		{
			public override int GetHashCode(T obj)
			{
				if (obj == null)
				{
					return 0;
				}
				return obj.GetHashCode();
			}

			public override bool Equals(T x, T y)
			{
				if (x == null)
				{
					return y == null;
				}
				return x.Equals(y);
			}
		}
	}
}
