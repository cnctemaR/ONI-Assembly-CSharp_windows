using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckedHandleVector<T> where T : class
{
	public CheckedHandleVector(int initial_size)
	{
		this.handleVector = new HandleVector<T>(initial_size);
	}

	public HandleVector<T>.Handle Add(T item, string debug_info)
	{
		Debug.Assert(item != null);
		HandleVector<T>.Handle handle = this.handleVector.Add(item);
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
		if (this.handleVector.Handles.Contains(handle))
		{
			Output.LogError(new object[]
			{
				"Tried to double free checked handle, debug info:",
				this.debugInfo[handle.index]
			});
		}
		return this.handleVector.Release(handle);
	}

	private HandleVector<T> handleVector;

	private List<string> debugInfo = new List<string>();
}
