using System;
using System.Collections.Generic;

public class RingList<T>
{
	public RingList(int size)
		: this(size, default(T))
	{
	}

	public RingList(int size, T initial)
	{
		this.data = new List<T>(size);
		for (int i = 0; i < size; i++)
		{
			this.data.Add(initial);
		}
	}

	public int Count
	{
		get
		{
			return this.data.Count;
		}
	}

	public T this[int offset]
	{
		get
		{
			int num = (offset % this.data.Count + this.data.Count) % this.data.Count;
			return this.data[num];
		}
		set
		{
			int num = (offset % this.data.Count + this.data.Count) % this.data.Count;
			this.data[num] = value;
		}
	}

	private List<T> data;
}
