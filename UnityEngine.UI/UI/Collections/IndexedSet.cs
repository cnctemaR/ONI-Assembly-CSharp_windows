using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Collections
{
	internal class IndexedSet<T> : IEnumerable, IList<T>, ICollection<T>, IEnumerable<T>
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(T item)
		{
			this.m_List.Add(item);
			this.m_Dictionary.Add(item, this.m_List.Count - 1);
		}

		public bool AddUnique(T item)
		{
			if (this.m_Dictionary.ContainsKey(item))
			{
				return false;
			}
			this.m_List.Add(item);
			this.m_Dictionary.Add(item, this.m_List.Count - 1);
			return true;
		}

		public bool Remove(T item)
		{
			int num = -1;
			if (!this.m_Dictionary.TryGetValue(item, out num))
			{
				return false;
			}
			this.RemoveAt(num);
			return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			this.m_List.Clear();
			this.m_Dictionary.Clear();
		}

		public bool Contains(T item)
		{
			return this.m_Dictionary.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.m_List.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public int IndexOf(T item)
		{
			int num = -1;
			this.m_Dictionary.TryGetValue(item, out num);
			return num;
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException("Random Insertion is semantically invalid, since this structure does not guarantee ordering.");
		}

		public void RemoveAt(int index)
		{
			T t = this.m_List[index];
			this.m_Dictionary.Remove(t);
			if (index == this.m_List.Count - 1)
			{
				this.m_List.RemoveAt(index);
			}
			else
			{
				int num = this.m_List.Count - 1;
				T t2 = this.m_List[num];
				this.m_List[index] = t2;
				this.m_Dictionary[t2] = index;
				this.m_List.RemoveAt(num);
			}
		}

		public T this[int index]
		{
			get
			{
				return this.m_List[index];
			}
			set
			{
				T t = this.m_List[index];
				this.m_Dictionary.Remove(t);
				this.m_List[index] = value;
				this.m_Dictionary.Add(t, index);
			}
		}

		public void RemoveAll(Predicate<T> match)
		{
			int i = 0;
			while (i < this.m_List.Count)
			{
				T t = this.m_List[i];
				if (match(t))
				{
					this.Remove(t);
				}
				else
				{
					i++;
				}
			}
		}

		public void Sort(Comparison<T> sortLayoutFunction)
		{
			this.m_List.Sort(sortLayoutFunction);
			for (int i = 0; i < this.m_List.Count; i++)
			{
				T t = this.m_List[i];
				this.m_Dictionary[t] = i;
			}
		}

		private readonly List<T> m_List = new List<T>();

		private Dictionary<T, int> m_Dictionary = new Dictionary<T, int>();
	}
}
