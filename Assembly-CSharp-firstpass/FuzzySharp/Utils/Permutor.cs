using System;
using System.Collections.Generic;
using System.Linq;

namespace FuzzySharp.Utils
{
	public class Permutor<T> where T : IComparable<T>
	{
		public Permutor(IEnumerable<T> set)
		{
			this._set = set.ToList<T>();
		}

		public List<T> PermutationAt(long i)
		{
			List<T> list = new List<T>(this._set.OrderBy<T, T>((T e) => e).ToList<T>());
			for (long num = 0L; num < i - 1L; num += 1L)
			{
				this.NextPermutation(list);
			}
			return list;
		}

		public List<T> NextPermutation()
		{
			this.NextPermutation(this._set);
			return this._set;
		}

		public bool NextPermutation(List<T> set)
		{
			int i;
			for (i = set.Count - 1; i > 0; i--)
			{
				T t = set[i - 1];
				if (t.CompareTo(set[i]) < 0)
				{
					break;
				}
			}
			if (i <= 0)
			{
				return false;
			}
			int num = set.Count - 1;
			for (;;)
			{
				T t = set[num];
				if (t.CompareTo(set[i - 1]) > 0)
				{
					break;
				}
				num--;
			}
			T t2 = set[i - 1];
			set[i - 1] = set[num];
			set[num] = t2;
			num = set.Count - 1;
			while (i < num)
			{
				t2 = set[i];
				set[i] = set[num];
				set[num] = t2;
				i++;
				num--;
			}
			return true;
		}

		private readonly List<T> _set;
	}
}
