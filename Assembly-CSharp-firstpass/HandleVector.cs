using System;
using System.Collections.Generic;

public class HandleVector<T>
{
	public HandleVector(int initial_size)
	{
		this.freeHandles = new Stack<HandleVector<T>.Handle>(initial_size);
		this.items = new List<T>(initial_size);
		this.Initialize(initial_size);
	}

	public List<T> Items
	{
		get
		{
			return this.items;
		}
	}

	public Stack<HandleVector<T>.Handle> Handles
	{
		get
		{
			return this.freeHandles;
		}
	}

	public void Clear()
	{
		this.items.Clear();
		this.freeHandles.Clear();
	}

	private void Initialize(int size)
	{
		for (int i = size - 1; i >= 0; i--)
		{
			this.freeHandles.Push(new HandleVector<T>.Handle
			{
				index = i
			});
			this.items.Add(default(T));
		}
	}

	public HandleVector<T>.Handle Add(T item)
	{
		HandleVector<T>.Handle handle;
		if (this.freeHandles.Count > 0)
		{
			handle = this.freeHandles.Pop();
			this.items[handle.index] = item;
		}
		else
		{
			handle = new HandleVector<T>.Handle
			{
				index = this.items.Count
			};
			this.items.Add(item);
		}
		return handle;
	}

	public T Release(HandleVector<T>.Handle handle)
	{
		if (!handle.IsValid())
		{
			return default(T);
		}
		this.freeHandles.Push(handle);
		T t = this.items[handle.index];
		this.items[handle.index] = default(T);
		return t;
	}

	public T GetItem(HandleVector<T>.Handle handle)
	{
		return this.items[handle.index];
	}

	public T GetItem(int handle)
	{
		return this.items[handle];
	}

	public static readonly HandleVector<T>.Handle InvalidHandle = new HandleVector<T>.Handle
	{
		index = -1
	};

	protected Stack<HandleVector<T>.Handle> freeHandles;

	protected List<T> items;

	public struct Handle
	{
		public bool IsValid()
		{
			return this.index != -1;
		}

		public void Clear()
		{
			this.index = -1;
		}

		public const int InvalidIndex = -1;

		public int index;
	}
}
