using System;
using System.Collections.Generic;

public class FrameDelayedHandleVector<T> : HandleVector<T>
{
	public FrameDelayedHandleVector(int initial_size)
		: base(initial_size)
	{
		for (int i = 0; i < this.frameDelayedFreeHandles.Length; i++)
		{
			this.frameDelayedFreeHandles[i] = new List<HandleVector<T>.Handle>();
		}
	}

	public override void Clear()
	{
		this.freeHandles.Clear();
		this.items.Clear();
		foreach (List<HandleVector<T>.Handle> list in this.frameDelayedFreeHandles)
		{
			list.Clear();
		}
	}

	public override T Release(HandleVector<T>.Handle handle)
	{
		this.frameDelayedFreeHandles[this.curFrame].Add(handle);
		return this.items[handle.index];
	}

	public void NextFrame()
	{
		int num = (this.curFrame + 1) % this.frameDelayedFreeHandles.Length;
		foreach (HandleVector<T>.Handle handle in this.frameDelayedFreeHandles[num])
		{
			this.items[handle.index] = default(T);
			this.freeHandles.Push(handle);
		}
		this.frameDelayedFreeHandles[num].Clear();
		this.curFrame = num;
	}

	public virtual void Reset(HandleVector<T>.Handle handle)
	{
	}

	private List<HandleVector<T>.Handle>[] frameDelayedFreeHandles = new List<HandleVector<T>.Handle>[2];

	private int curFrame;
}
