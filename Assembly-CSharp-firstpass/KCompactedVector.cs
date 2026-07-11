using System;
using System.Collections;
using System.Collections.Generic;

public class KCompactedVector<T> : ICollection, IEnumerable
{
	public KCompactedVector(int initial_count = 0)
	{
		this.handles = new HandleVector<int>(initial_count);
		this.data = new List<T>(initial_count);
	}

	public HandleVector<int>.Handle Allocate(T initial_data)
	{
		HandleVector<int>.Handle handle = this.handles.Add(this.data.Count);
		byte b;
		int num;
		this.handles.UnpackHandle(handle, out b, out num);
		this.dataHandleIndices.Add(num);
		this.data.Add(initial_data);
		return handle;
	}

	public HandleVector<int>.Handle Free(HandleVector<int>.Handle handle)
	{
		if (!handle.IsValid())
		{
			return handle;
		}
		int num = this.handles.Release(handle);
		int num2 = this.data.Count - 1;
		if (num < num2)
		{
			this.data[num] = this.data[num2];
			int num3 = this.dataHandleIndices[num2];
			if (this.handles.Items[num3] != num2)
			{
				Output.LogError(new object[] { "KCompactedVector: Bad state after attempting to free handle", handle.index });
			}
			this.handles.Items[num3] = num;
			this.dataHandleIndices[num] = num3;
		}
		this.data.RemoveAt(num2);
		this.dataHandleIndices.RemoveAt(num2);
		return HandleVector<int>.InvalidHandle;
	}

	public bool IsValid(HandleVector<int>.Handle handle)
	{
		return this.handles.IsValid(handle);
	}

	public bool IsVersionValid(HandleVector<int>.Handle handle)
	{
		return this.handles.IsVersionValid(handle);
	}

	public T GetData(HandleVector<int>.Handle handle)
	{
		byte b;
		int num;
		this.handles.UnpackHandle(handle, out b, out num);
		int num2 = this.handles.Items[num];
		return this.data[num2];
	}

	public void SetData(HandleVector<int>.Handle handle, T new_data)
	{
		byte b;
		int num;
		this.handles.UnpackHandle(handle, out b, out num);
		int num2 = this.handles.Items[num];
		this.data[num2] = new_data;
	}

	public virtual void Clear()
	{
		this.dataHandleIndices.Clear();
		this.handles.Clear();
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

	protected List<int> dataHandleIndices = new List<int>();

	protected HandleVector<int> handles;
}
