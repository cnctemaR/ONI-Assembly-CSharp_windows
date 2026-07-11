using System;
using System.Collections;
using System.Collections.Generic;

public class BinaryHeap<T> : IEnumerable<T>, IEnumerable
{
	public BinaryHeap()
		: this(Comparer<T>.Default)
	{
	}

	public BinaryHeap(IComparer<T> comp)
	{
		this.Comparer = comp;
	}

	public int Count
	{
		get
		{
			return this.Items.Count;
		}
	}

	public void Clear()
	{
		this.Items.Clear();
	}

	public void TrimExcess()
	{
		this.Items.TrimExcess();
	}

	public void Insert(T newItem)
	{
		int num = this.Count;
		this.Items.Add(newItem);
		while (num > 0 && this.Comparer.Compare(this.Items[(num - 1) / 2], newItem) > 0)
		{
			this.Items[num] = this.Items[(num - 1) / 2];
			num = (num - 1) / 2;
		}
		this.Items[num] = newItem;
	}

	public T Peek()
	{
		if (this.Items.Count == 0)
		{
			throw new InvalidOperationException("The heap is empty.");
		}
		return this.Items[0];
	}

	public T RemoveRoot()
	{
		if (this.Items.Count == 0)
		{
			throw new InvalidOperationException("The heap is empty.");
		}
		T t = this.Items[0];
		T t2 = this.Items[this.Items.Count - 1];
		this.Items.RemoveAt(this.Items.Count - 1);
		if (this.Items.Count > 0)
		{
			int i;
			int num;
			for (i = 0; i < this.Items.Count / 2; i = num)
			{
				num = 2 * i + 1;
				if (num < this.Items.Count - 1 && this.Comparer.Compare(this.Items[num], this.Items[num + 1]) > 0)
				{
					num++;
				}
				if (this.Comparer.Compare(this.Items[num], t2) >= 0)
				{
					break;
				}
				this.Items[i] = this.Items[num];
			}
			this.Items[i] = t2;
		}
		return t;
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		foreach (T t in this.Items)
		{
			yield return t;
		}
		List<T>.Enumerator enumerator = default(List<T>.Enumerator);
		yield break;
		yield break;
	}

	public IEnumerator GetEnumerator()
	{
		return this.GetEnumerator();
	}

	private IComparer<T> Comparer;

	private List<T> Items = new List<T>();
}
