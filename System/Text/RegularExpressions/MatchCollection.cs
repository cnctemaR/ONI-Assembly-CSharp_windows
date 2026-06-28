using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class MatchCollection : ICollection, IEnumerable
	{
		internal MatchCollection(Match start)
		{
			this.current = start;
			this.list = new ArrayList();
		}

		public int Count
		{
			get
			{
				return this.FullList.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual Match this[int i]
		{
			get
			{
				if (i < 0 || !this.TryToGet(i))
				{
					throw new ArgumentOutOfRangeException("i");
				}
				return (i >= this.list.Count) ? this.current : ((Match)this.list[i]);
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.list;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this.FullList.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			IEnumerator enumerator2;
			if (this.current.Success)
			{
				IEnumerator enumerator = new MatchCollection.Enumerator(this);
				enumerator2 = enumerator;
			}
			else
			{
				enumerator2 = this.list.GetEnumerator();
			}
			return enumerator2;
		}

		private bool TryToGet(int i)
		{
			while (i > this.list.Count && this.current.Success)
			{
				this.list.Add(this.current);
				this.current = this.current.NextMatch();
			}
			return i < this.list.Count || this.current.Success;
		}

		private ICollection FullList
		{
			get
			{
				if (this.TryToGet(2147483647))
				{
					throw new SystemException("too many matches");
				}
				return this.list;
			}
		}

		private Match current;

		private ArrayList list;

		private class Enumerator : IEnumerator
		{
			internal Enumerator(MatchCollection coll)
			{
				this.coll = coll;
				this.index = -1;
			}

			void IEnumerator.Reset()
			{
				this.index = -1;
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.index < 0)
					{
						throw new InvalidOperationException("'Current' called before 'MoveNext()'");
					}
					if (this.index > this.coll.list.Count)
					{
						throw new SystemException("MatchCollection in invalid state");
					}
					if (this.index == this.coll.list.Count && !this.coll.current.Success)
					{
						throw new InvalidOperationException("'Current' called after 'MoveNext()' returned false");
					}
					return (this.index >= this.coll.list.Count) ? this.coll.current : this.coll.list[this.index];
				}
			}

			bool IEnumerator.MoveNext()
			{
				if (this.index > this.coll.list.Count)
				{
					throw new SystemException("MatchCollection in invalid state");
				}
				return (this.index != this.coll.list.Count || this.coll.current.Success) && this.coll.TryToGet(++this.index);
			}

			private int index;

			private MatchCollection coll;
		}
	}
}
