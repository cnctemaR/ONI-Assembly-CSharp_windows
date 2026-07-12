using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Collections
{
	internal class IndexedSet<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public void Add(T item)
		{
			this.Add(item, true);
		}

		public void Add(T item, bool isActive)
		{
			this.m_List.Add(item);
			this.m_Dictionary.Add(item, this.m_List.Count - 1);
			if (isActive)
			{
				this.EnableItem(item);
			}
		}

		public bool AddUnique(T item, bool isActive = true)
		{
			if (this.m_Dictionary.ContainsKey(item))
			{
				if (isActive)
				{
					this.EnableItem(item);
				}
				else
				{
					this.DisableItem(item);
				}
				return false;
			}
			this.Add(item, isActive);
			return true;
		}

		public bool EnableItem(T item)
		{
			int num;
			if (!this.m_Dictionary.TryGetValue(item, out num))
			{
				return false;
			}
			if (num < this.m_EnabledObjectCount)
			{
				return true;
			}
			if (num > this.m_EnabledObjectCount)
			{
				this.Swap(this.m_EnabledObjectCount, num);
			}
			this.m_EnabledObjectCount++;
			return true;
		}

		public bool DisableItem(T item)
		{
			int num;
			if (!this.m_Dictionary.TryGetValue(item, out num))
			{
				return false;
			}
			if (num >= this.m_EnabledObjectCount)
			{
				return true;
			}
			if (num < this.m_EnabledObjectCount - 1)
			{
				this.Swap(num, this.m_EnabledObjectCount - 1);
			}
			this.m_EnabledObjectCount--;
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Clear()
		{
			this.m_List.Clear();
			this.m_Dictionary.Clear();
			this.m_EnabledObjectCount = 0;
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
				return this.m_EnabledObjectCount;
			}
		}

		public int Capacity
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
			if (this.m_Dictionary.TryGetValue(item, out num))
			{
				return num;
			}
			return -1;
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException("Random Insertion is semantically invalid, since this structure does not guarantee ordering.");
		}

		public void RemoveAt(int index)
		{
			T t = this.m_List[index];
			if (index == this.m_List.Count - 1)
			{
				if (this.m_EnabledObjectCount == this.m_List.Count)
				{
					this.m_EnabledObjectCount--;
				}
				this.m_List.RemoveAt(index);
			}
			else
			{
				int num = this.m_List.Count - 1;
				if (index < this.m_EnabledObjectCount - 1)
				{
					int num2 = this.m_EnabledObjectCount - 1;
					this.m_EnabledObjectCount = num2;
					this.Swap(num2, index);
					index = this.m_EnabledObjectCount;
				}
				else if (index == this.m_EnabledObjectCount - 1)
				{
					this.m_EnabledObjectCount--;
				}
				this.Swap(num, index);
				this.m_List.RemoveAt(num);
			}
			this.m_Dictionary.Remove(t);
		}

		private void Swap(int index1, int index2)
		{
			if (index1 == index2)
			{
				return;
			}
			T t = this.m_List[index1];
			T t2 = this.m_List[index2];
			this.m_List[index1] = t2;
			this.m_List[index2] = t;
			this.m_Dictionary[t2] = index1;
			this.m_Dictionary[t] = index2;
		}

		public T this[int index]
		{
			get
			{
				if (index >= this.m_EnabledObjectCount)
				{
					throw new IndexOutOfRangeException();
				}
				return this.m_List[index];
			}
			set
			{
				T t = this.m_List[index];
				this.m_Dictionary.Remove(t);
				this.m_List[index] = value;
				this.m_Dictionary.Add(value, index);
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

		private int m_EnabledObjectCount;
	}
}
