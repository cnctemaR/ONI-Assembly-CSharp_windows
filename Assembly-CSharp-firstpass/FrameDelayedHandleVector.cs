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
		List<HandleVector<T>.Handle>[] array = this.frameDelayedFreeHandles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
	}

	public override T Release(HandleVector<T>.Handle handle)
	{
		this.frameDelayedFreeHandles[this.curFrame].Add(handle);
		return base.GetItem(handle);
	}

	public void NextFrame()
	{
		int num = (this.curFrame + 1) % this.frameDelayedFreeHandles.Length;
		List<HandleVector<T>.Handle> list = this.frameDelayedFreeHandles[num];
		foreach (HandleVector<T>.Handle handle in list)
		{
			base.Release(handle);
		}
		list.Clear();
		this.curFrame = num;
	}

	private List<HandleVector<T>.Handle>[] frameDelayedFreeHandles = new List<HandleVector<T>.Handle>[3];

	private int curFrame;
}
