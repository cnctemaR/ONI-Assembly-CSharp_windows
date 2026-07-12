using System;
using System.Collections;
using System.Collections.Generic;

public class ListWithEvents<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public int Count
	{
		get
		{
			return this.internalList.Count;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return ((ICollection<T>)this.internalList).IsReadOnly;
		}
	}

	public event Action<T> onAdd;

	public event Action<T> onRemove;

	public T this[int index]
	{
		get
		{
			return this.internalList[index];
		}
		set
		{
			this.internalList[index] = value;
		}
	}

	public void Add(T item)
	{
		this.internalList.Add(item);
		if (this.onAdd != null)
		{
			this.onAdd(item);
		}
	}

	public void Insert(int index, T item)
	{
		this.internalList.Insert(index, item);
		if (this.onAdd != null)
		{
			this.onAdd(item);
		}
	}

	public void RemoveAt(int index)
	{
		T t = this.internalList[index];
		this.internalList.RemoveAt(index);
		if (this.onRemove != null)
		{
			this.onRemove(t);
		}
	}

	public bool Remove(T item)
	{
		bool flag = this.internalList.Remove(item);
		if (flag && this.onRemove != null)
		{
			this.onRemove(item);
		}
		return flag;
	}

	public void Clear()
	{
		while (this.Count > 0)
		{
			this.RemoveAt(0);
		}
	}

	public int IndexOf(T item)
	{
		return this.internalList.IndexOf(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		this.internalList.CopyTo(array, arrayIndex);
	}

	public bool Contains(T item)
	{
		return this.internalList.Contains(item);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return this.internalList.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.internalList.GetEnumerator();
	}

	private List<T> internalList = new List<T>();
}
