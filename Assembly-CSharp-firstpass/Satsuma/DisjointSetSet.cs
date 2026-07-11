using System;

namespace Satsuma
{
	public struct DisjointSetSet<T> : IEquatable<DisjointSetSet<T>>
	{
		public T Representative { get; private set; }

		public DisjointSetSet(T representative)
		{
			this = default(DisjointSetSet<T>);
			this.Representative = representative;
		}

		public bool Equals(DisjointSetSet<T> other)
		{
			T representative = this.Representative;
			return representative.Equals(other.Representative);
		}

		public override bool Equals(object obj)
		{
			return obj is DisjointSetSet<T> && this.Equals((DisjointSetSet<T>)obj);
		}

		public static bool operator ==(DisjointSetSet<T> a, DisjointSetSet<T> b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(DisjointSetSet<T> a, DisjointSetSet<T> b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			T representative = this.Representative;
			return representative.GetHashCode();
		}

		public override string ToString()
		{
			return "[DisjointSetSet:" + this.Representative + "]";
		}
	}
}
