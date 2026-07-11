using System;

namespace System.Collections.Generic
{
	[Serializable]
	internal sealed class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
	{
		public HashSetEqualityComparer()
		{
			this._comparer = EqualityComparer<T>.Default;
		}

		public bool Equals(HashSet<T> x, HashSet<T> y)
		{
			return HashSet<T>.HashSetEquals(x, y, this._comparer);
		}

		public int GetHashCode(HashSet<T> obj)
		{
			int num = 0;
			if (obj != null)
			{
				foreach (T t in obj)
				{
					num ^= this._comparer.GetHashCode(t) & int.MaxValue;
				}
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			HashSetEqualityComparer<T> hashSetEqualityComparer = obj as HashSetEqualityComparer<T>;
			return hashSetEqualityComparer != null && this._comparer == hashSetEqualityComparer._comparer;
		}

		public override int GetHashCode()
		{
			return this._comparer.GetHashCode();
		}

		private readonly IEqualityComparer<T> _comparer;
	}
}
