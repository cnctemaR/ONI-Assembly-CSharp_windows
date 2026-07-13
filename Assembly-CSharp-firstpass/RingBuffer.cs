using System;

public class RingBuffer<T>
{
	public RingBuffer(int capacity)
		: this(capacity, default(T))
	{
	}

	public RingBuffer(int capacity, T initial)
	{
		this.data = new RingList<T>(capacity, initial);
		this.index = -1;
	}

	public int Count
	{
		get
		{
			return this.data.Count;
		}
	}

	public void Add(T value)
	{
		this.index = (this.index + 1) % this.data.Count;
		this.data[this.index] = value;
	}

	public T this[int offset]
	{
		get
		{
			return this.data[offset + this.index];
		}
	}

	private RingList<T> data;

	private int index;
}
