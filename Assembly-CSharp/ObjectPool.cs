using System;
using System.Collections.Generic;

public class ObjectPool<T>
{
	public ObjectPool(Func<T> instantiator, int initial_count = 0)
	{
		this.instantiator = instantiator;
		this.unused = new Stack<T>(initial_count);
		for (int i = 0; i < initial_count; i++)
		{
			this.unused.Push(instantiator());
		}
	}

	public virtual T GetInstance()
	{
		T t = default(T);
		if (this.unused.Count > 0)
		{
			t = this.unused.Pop();
		}
		else
		{
			t = this.instantiator();
		}
		return t;
	}

	public void ReleaseInstance(T instance)
	{
		if (object.Equals(instance, null))
		{
			return;
		}
		this.unused.Push(instance);
	}

	protected Stack<T> unused;

	protected Func<T> instantiator;
}
