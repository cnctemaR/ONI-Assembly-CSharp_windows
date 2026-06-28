using System;

public class HandleValueArray<T> : HandleValueArrayBase
{
	public HandleValueArray(int reserve_size)
	{
		this.Entries = new HandleValueArray<T>.Entry[reserve_size];
		this.Indices = new int[reserve_size];
		for (int i = 0; i < this.Entries.Length; i++)
		{
			this.Entries[i].Handle = i;
		}
	}

	public ValueArrayHandle Add(ref T value)
	{
		if (this.Count == this.Entries.Length)
		{
			HandleValueArray<T>.Entry[] array = new HandleValueArray<T>.Entry[this.Entries.Length * 2];
			int[] array2 = new int[this.Entries.Length * 2];
			for (int i = 0; i < this.Entries.Length; i++)
			{
				array[i] = this.Entries[i];
				array2[i] = this.Indices[i];
			}
			for (int j = this.Entries.Length; j < array.Length; j++)
			{
				array[j].Handle = j;
			}
			this.Entries = array;
			this.Indices = array2;
		}
		int handle = this.Entries[this.Count].Handle;
		int count = this.Count;
		this.Entries[count].Value = value;
		this.Indices[handle] = count;
		this.Count++;
		return new ValueArrayHandle(handle);
	}

	public int GetIndex(ref ValueArrayHandle handle)
	{
		return this.Indices[handle.handle];
	}

	public void Remove(ref ValueArrayHandle handle)
	{
		int num = this.Indices[handle.handle];
		this.Count--;
		int handle2 = this.Entries[this.Count].Handle;
		this.Entries[num] = this.Entries[this.Count];
		this.Entries[this.Count].Handle = handle.handle;
		this.Indices[handle2] = num;
	}

	public int Count;

	public HandleValueArray<T>.Entry[] Entries;

	private int[] Indices;

	public struct Entry
	{
		public T Value;

		public int Handle;
	}
}
