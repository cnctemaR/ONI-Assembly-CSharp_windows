using System;
using System.Collections.Generic;

public abstract class KCompactedVectorBase
{
	protected KCompactedVectorBase(int initial_count)
	{
		this.handles = new HandleVector<int>(initial_count);
	}

	protected HandleVector<int>.Handle Allocate(int item)
	{
		HandleVector<int>.Handle handle = this.handles.Add(item);
		byte b;
		int num;
		this.handles.UnpackHandle(handle, out b, out num);
		this.dataHandleIndices.Add(num);
		return handle;
	}

	protected bool Free(HandleVector<int>.Handle handle, int last_idx, out int free_component_idx)
	{
		free_component_idx = -1;
		if (!handle.IsValid())
		{
			return false;
		}
		free_component_idx = this.handles.Release(handle);
		if (free_component_idx < last_idx)
		{
			int num = this.dataHandleIndices[last_idx];
			if (this.handles.Items[num] != last_idx)
			{
				Output.LogError(new object[] { "KCompactedVector: Bad state after attempting to free handle", handle.index });
			}
			this.handles.Items[num] = free_component_idx;
			this.dataHandleIndices[free_component_idx] = num;
		}
		this.dataHandleIndices.RemoveAt(last_idx);
		return true;
	}

	public bool IsValid(HandleVector<int>.Handle handle)
	{
		return this.handles.IsValid(handle);
	}

	public bool IsVersionValid(HandleVector<int>.Handle handle)
	{
		return this.handles.IsVersionValid(handle);
	}

	protected int ComputeIndex(HandleVector<int>.Handle handle)
	{
		byte b;
		int num;
		this.handles.UnpackHandle(handle, out b, out num);
		return this.handles.Items[num];
	}

	protected void Clear()
	{
		this.dataHandleIndices.Clear();
		this.handles.Clear();
	}

	protected List<int> dataHandleIndices = new List<int>();

	protected HandleVector<int> handles;
}
