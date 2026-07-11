using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public struct ArrayRef<T>
{
	public ArrayRef(int initialCapacity)
	{
		this.capacityImpl = initialCapacity;
		this.elements = new T[initialCapacity];
		this.sizeImpl = 0;
	}

	public ArrayRef(T[] elements, int size)
	{
		this.elements = elements;
		this.sizeImpl = size;
		this.capacityImpl = elements.Length;
	}

	public T this[int i]
	{
		get
		{
			this.ValidateIndex(i);
			return this.elements[i];
		}
		set
		{
			this.ValidateIndex(i);
			this.elements[i] = value;
		}
	}

	public int size
	{
		get
		{
			return this.sizeImpl;
		}
	}

	public int Count
	{
		get
		{
			return this.size;
		}
	}

	public int capacity
	{
		get
		{
			return this.capacityImpl;
		}
	}

	public int Add(T item)
	{
		this.MaybeGrow(this.size);
		this.elements[this.size] = item;
		this.sizeImpl++;
		return this.size;
	}

	public bool RemoveFirst(Predicate<T> match)
	{
		int num = this.FindIndex(match);
		if (num != -1)
		{
			this.RemoveAt(num);
			return true;
		}
		return false;
	}

	public bool RemoveFirstSwap(Predicate<T> match)
	{
		int num = this.FindIndex(match);
		if (num != -1)
		{
			this.RemoveAtSwap(num);
			return true;
		}
		return false;
	}

	public void RemoveAt(int index)
	{
		this.ValidateIndex(index);
		for (int num = index; num != this.size - 1; num++)
		{
			this.elements[num] = this.elements[num + 1];
		}
		this.sizeImpl--;
		DebugUtil.Assert(this.sizeImpl >= 0);
	}

	public void RemoveAtSwap(int index)
	{
		this.ValidateIndex(index);
		this.elements[index] = this.elements[this.size - 1];
		this.sizeImpl--;
		DebugUtil.Assert(this.sizeImpl >= 0);
	}

	public void RemoveAll(Predicate<T> match)
	{
		for (int num = this.size - 1; num != -1; num--)
		{
			if (match(this.elements[num]))
			{
				this.RemoveAt(num);
			}
		}
	}

	public void RemoveAllSwap(Predicate<T> match)
	{
		int num = 0;
		while (num != this.size)
		{
			if (match(this.elements[num]))
			{
				this.elements[num] = this.elements[this.size - 1];
				this.sizeImpl--;
				DebugUtil.Assert(this.sizeImpl >= 0);
			}
			else
			{
				num++;
			}
		}
	}

	public void Clear()
	{
		this.sizeImpl = 0;
	}

	public int FindIndex(Predicate<T> match)
	{
		for (int num = 0; num != this.size; num++)
		{
			if (match(this.elements[num]))
			{
				return num;
			}
		}
		return -1;
	}

	public void ShrinkToFit()
	{
		if (this.size == this.capacity)
		{
			return;
		}
		this.Reallocate(this.size);
	}

	private void ValidateIndex(int index)
	{
	}

	private void MaybeGrow(int index)
	{
		DebugUtil.Assert(this.capacity == 0 || this.capacity == this.elements.Length);
		DebugUtil.Assert(index >= 0);
		if (index < this.capacity)
		{
			return;
		}
		this.Reallocate((this.capacity != 0) ? (this.capacity * 2) : 1);
		DebugUtil.Assert(this.capacity == 0 || this.capacity == this.elements.Length);
	}

	private void Reallocate(int newCapacity)
	{
		this.capacityImpl = newCapacity;
		T[] array = new T[this.capacity];
		for (int num = 0; num != this.size; num++)
		{
			array[num] = this.elements[num];
		}
		this.elements = array;
	}

	[Serialize]
	private T[] elements;

	[Serialize]
	private int sizeImpl;

	[Serialize]
	private int capacityImpl;
}
