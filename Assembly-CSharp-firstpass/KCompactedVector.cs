using System;
using System.Collections;
using System.Collections.Generic;

public class KCompactedVector<T> : KCompactedVectorBase, ICollection, IEnumerable
{
	public KCompactedVector(int initial_count = 0)
		: base(initial_count)
	{
		this.data = new List<T>(initial_count);
	}

	public HandleVector<int>.Handle Allocate(T initial_data)
	{
		this.data.Add(initial_data);
		return base.Allocate(this.data.Count - 1);
	}

	public HandleVector<int>.Handle Free(HandleVector<int>.Handle handle)
	{
		int num = this.data.Count - 1;
		int num2;
		bool flag = base.Free(handle, num, out num2);
		if (flag)
		{
			if (num2 < num)
			{
				this.data[num2] = this.data[num];
			}
			this.data.RemoveAt(num);
		}
		if (!flag)
		{
			return handle;
		}
		return HandleVector<int>.InvalidHandle;
	}

	public T GetData(HandleVector<int>.Handle handle)
	{
		return this.data[base.ComputeIndex(handle)];
	}

	public void SetData(HandleVector<int>.Handle handle, T new_data)
	{
		this.data[base.ComputeIndex(handle)] = new_data;
	}

	public new virtual void Clear()
	{
		base.Clear();
		this.data.Clear();
	}

	public int Count
	{
		get
		{
			return this.data.Count;
		}
	}

	public List<T> GetDataList()
	{
		return this.data;
	}

	public bool IsSynchronized
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public object SyncRoot
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public void CopyTo(Array array, int index)
	{
		throw new NotImplementedException();
	}

	public IEnumerator GetEnumerator()
	{
		return this.data.GetEnumerator();
	}

	protected List<T> data;
}
