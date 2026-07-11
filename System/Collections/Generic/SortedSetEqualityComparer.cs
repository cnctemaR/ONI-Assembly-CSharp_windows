using System;

namespace System.Collections.Generic
{
	internal sealed class SortedSetEqualityComparer<T> : IEqualityComparer<SortedSet<T>>
	{
		public SortedSetEqualityComparer(IEqualityComparer<T> memberEqualityComparer)
			: this(null, memberEqualityComparer)
		{
		}

		private SortedSetEqualityComparer(IComparer<T> comparer, IEqualityComparer<T> memberEqualityComparer)
		{
			this._comparer = comparer ?? Comparer<T>.Default;
			this._memberEqualityComparer = memberEqualityComparer ?? EqualityComparer<T>.Default;
		}

		public bool Equals(SortedSet<T> x, SortedSet<T> y)
		{
			return SortedSet<T>.SortedSetEquals(x, y, this._comparer);
		}

		public int GetHashCode(SortedSet<T> obj)
		{
			int num = 0;
			if (obj != null)
			{
				foreach (T t in obj)
				{
					num ^= this._memberEqualityComparer.GetHashCode(t) & int.MaxValue;
				}
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			SortedSetEqualityComparer<T> sortedSetEqualityComparer = obj as SortedSetEqualityComparer<T>;
			return sortedSetEqualityComparer != null && this._comparer == sortedSetEqualityComparer._comparer;
		}

		public override int GetHashCode()
		{
			return this._comparer.GetHashCode() ^ this._memberEqualityComparer.GetHashCode();
		}

		private readonly IComparer<T> _comparer;

		private readonly IEqualityComparer<T> _memberEqualityComparer;
	}
}
