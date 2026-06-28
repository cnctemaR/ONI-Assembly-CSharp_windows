using System;

public class ValueArray<T>
{
	public ValueArray(int reserve_size)
	{
		this.Values = new T[reserve_size];
	}

	public T this[int idx]
	{
		get
		{
			return this.Values[idx];
		}
	}

	public void Add(ref T value)
	{
		if (this.Count == this.Values.Length)
		{
			this.Resize(this.Values.Length * 2);
		}
		this.Values[this.Count] = value;
		this.Count++;
	}

	public void Resize(int new_size)
	{
		T[] array = new T[new_size];
		for (int i = 0; i < this.Values.Length; i++)
		{
			array[i] = this.Values[i];
		}
		this.Values = array;
	}

	public void Remove(int index)
	{
		if (this.Count > 0)
		{
			this.Values[index] = this.Values[this.Count - 1];
		}
		this.Count--;
	}

	public void Clear()
	{
		this.Count = 0;
	}

	public bool IsEqual(ValueArray<T> array)
	{
		bool flag;
		if (this.Count != array.Count)
		{
			flag = false;
		}
		else
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (!this.Values[i].Equals(array.Values[i]))
				{
					return false;
				}
			}
			flag = true;
		}
		return flag;
	}

	public void CopyFrom(ValueArray<T> array)
	{
		this.Clear();
		for (int i = 0; i < array.Count; i++)
		{
			T t = array[i];
			this.Add(ref t);
		}
	}

	public int Count;

	public T[] Values;
}
