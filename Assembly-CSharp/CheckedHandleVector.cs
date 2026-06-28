using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckedHandleVector<T> where T : class
{
	public CheckedHandleVector(int initial_size)
	{
		this.handleVector = new HandleVector<T>(initial_size);
		this.isFree = new List<bool>(initial_size);
		for (int i = 0; i < initial_size; i++)
		{
			this.isFree.Add(true);
		}
	}

	public HandleVector<T>.Handle Add(T item, string debug_info)
	{
		Debug.Assert(item != null);
		HandleVector<T>.Handle handle = this.handleVector.Add(item);
		if (handle.index >= this.isFree.Count)
		{
			this.isFree.Add(false);
		}
		else
		{
			this.isFree[handle.index] = false;
		}
		int i = this.handleVector.Items.Count;
		while (i > this.debugInfo.Count)
		{
			this.debugInfo.Add(null);
		}
		this.debugInfo[handle.index] = debug_info;
		return handle;
	}

	public T Release(HandleVector<T>.Handle handle)
	{
		if (this.isFree[handle.index])
		{
			Output.LogError(new object[]
			{
				"Tried to double free checked handle, debug info:",
				this.debugInfo[handle.index]
			});
		}
		this.isFree[handle.index] = true;
		return this.handleVector.Release(handle);
	}

	private HandleVector<T> handleVector;

	private List<string> debugInfo = new List<string>();

	private List<bool> isFree;
}
