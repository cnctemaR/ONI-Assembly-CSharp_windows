using System;
using System.Collections.Generic;
using System.Diagnostics;

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

	public virtual void Clear()
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

	public virtual HandleVector<T>.Handle Add(T item)
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

	public virtual T Release(HandleVector<T>.Handle handle)
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

	public static readonly HandleVector<T>.Handle InvalidHandle = new HandleVector<T>.Handle
	{
		index = -1
	};

	protected Stack<HandleVector<T>.Handle> freeHandles;

	protected List<T> items;

	[DebuggerDisplay("{index}")]
	public struct Handle : IComparable<HandleVector<T>.Handle>, IEquatable<HandleVector<T>.Handle>
	{
		public bool IsValid()
		{
			return this.index != -1;
		}

		public void Clear()
		{
			this.index = -1;
		}

		public int CompareTo(HandleVector<T>.Handle obj)
		{
			if (this.index < obj.index)
			{
				return -1;
			}
			if (this.index > obj.index)
			{
				return 1;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			HandleVector<T>.Handle handle = (HandleVector<T>.Handle)obj;
			return this.index == handle.index;
		}

		public bool Equals(HandleVector<T>.Handle other)
		{
			return this.index == other.index;
		}

		public override int GetHashCode()
		{
			return this.index;
		}

		public static bool operator ==(HandleVector<T>.Handle x, HandleVector<T>.Handle y)
		{
			return x.index == y.index;
		}

		public static bool operator !=(HandleVector<T>.Handle x, HandleVector<T>.Handle y)
		{
			return x.index != y.index;
		}

		public const int InvalidIndex = -1;

		public int index;
	}
}
