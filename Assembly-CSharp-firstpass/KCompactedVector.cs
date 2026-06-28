using System;
using System.Collections.Generic;

public class KCompactedVector<T>
{
	public KCompactedVector(int initial_count = 0)
	{
		this.handles = new HandleVector<int>(initial_count);
		this.data = new List<T>(initial_count);
	}

	public HandleVector<int>.Handle Allocate(T initial_data)
	{
		this.Validate();
		HandleVector<int>.Handle handle = this.handles.Add(this.data.Count);
		this.dataHandleIndices.Add(handle.index);
		this.data.Add(initial_data);
		this.Validate();
		return handle;
	}

	public HandleVector<int>.Handle Free(HandleVector<int>.Handle handle)
	{
		if (!handle.IsValid())
		{
			return handle;
		}
		this.Validate();
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
		this.Validate();
		handle = HandleVector<int>.InvalidHandle;
		return handle;
	}

	public T GetData(HandleVector<int>.Handle handle)
	{
		int num = this.handles.Items[handle.index];
		return this.data[num];
	}

	public void SetData(HandleVector<int>.Handle handle, T new_data)
	{
		int num = this.handles.Items[handle.index];
		this.data[num] = new_data;
	}

	private void Validate()
	{
	}

	private bool IsFreeHandle(int index)
	{
		bool flag = false;
		foreach (HandleVector<int>.Handle handle in this.handles.Handles)
		{
			if (handle.index == index)
			{
				flag = true;
				break;
			}
		}
		return flag;
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

	protected List<T> data;

	protected List<int> dataHandleIndices = new List<int>();

	protected HandleVector<int> handles;
}
