using System;
using System.Collections.Generic;
using System.Diagnostics;

public class HandleVector<T>
{
	public HandleVector(int initial_size)
	{
		this.freeHandles = new Stack<HandleVector<T>.Handle>(initial_size);
		this.items = new List<T>(initial_size);
		this.versions = new List<byte>(initial_size);
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
		this.versions.Clear();
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
			this.versions.Add(0);
		}
	}

	public virtual HandleVector<T>.Handle Add(T item)
	{
		HandleVector<T>.Handle handle;
		if (this.freeHandles.Count > 0)
		{
			handle = this.freeHandles.Pop();
			byte b;
			int num;
			this.UnpackHandle(handle, out b, out num);
			this.items[num] = item;
		}
		else
		{
			this.versions.Add(0);
			handle = this.PackHandle(this.items.Count);
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
		byte b;
		int num;
		this.UnpackHandle(handle, out b, out num);
		b += 1;
		this.versions[num] = b;
		handle = this.PackHandle(num);
		this.freeHandles.Push(handle);
		T t = this.items[num];
		this.items[num] = default(T);
		return t;
	}

	public T GetItem(HandleVector<T>.Handle handle)
	{
		byte b;
		int num;
		this.UnpackHandle(handle, out b, out num);
		return this.items[num];
	}

	private HandleVector<T>.Handle PackHandle(int index)
	{
		byte b = this.versions[index];
		this.versions[index] = b;
		HandleVector<T>.Handle handle;
		handle.index = ((int)b << 24) | index;
		return handle;
	}

	public void UnpackHandle(HandleVector<T>.Handle handle, out byte version, out int index)
	{
		version = (byte)(handle.index >> 24);
		index = handle.index & 16777215;
		if (this.versions[index] != version)
		{
			throw new ArgumentException("Accessing mismatched handle version. Expected version=" + this.versions[index].ToString() + " but got version=" + version.ToString());
		}
	}

	public void UnpackHandleUnchecked(HandleVector<T>.Handle handle, out byte version, out int index)
	{
		version = (byte)(handle.index >> 24);
		index = handle.index & 16777215;
	}

	public bool IsValid(HandleVector<T>.Handle handle)
	{
		return (handle.index & 16777215) != 16777215;
	}

	public bool IsVersionValid(HandleVector<T>.Handle handle)
	{
		byte b = (byte)(handle.index >> 24);
		int num = handle.index & 16777215;
		return b == this.versions[num];
	}

	public static readonly HandleVector<T>.Handle InvalidHandle = new HandleVector<T>.Handle
	{
		index = -1
	};

	protected Stack<HandleVector<T>.Handle> freeHandles;

	protected List<T> items;

	protected List<byte> versions;

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
