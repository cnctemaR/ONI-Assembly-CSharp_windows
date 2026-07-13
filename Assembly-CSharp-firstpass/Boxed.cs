using System;
using UnityEngine.Pool;

public class Boxed<T> where T : struct
{
	public static PooledObject<Boxed<T>> Borrow(out Boxed<T> box, T value)
	{
		PooledObject<Boxed<T>> pooledObject = Boxed<T>.Pool.Get(out box);
		box.value = value;
		return pooledObject;
	}

	public static Boxed<T> Get(T value)
	{
		Boxed<T> boxed = Boxed<T>.Pool.Get();
		boxed.value = value;
		return boxed;
	}

	public static void Release(Boxed<T> box)
	{
		Boxed<T>.Pool.Release(box);
	}

	public void Release()
	{
		Boxed<T>.Release(this);
	}

	public static T Unbox(object obj)
	{
		return ((Boxed<T>)obj).value;
	}

	public static Boxed<bool> Get(bool value)
	{
		if (!value)
		{
			return BoxedBools.False;
		}
		return BoxedBools.True;
	}

	public static void Release(Boxed<bool> box)
	{
	}

	public static PooledObject<Boxed<bool>> Borrow(out Boxed<bool> value)
	{
		throw new Exception("Do not borrow a pooled bool, use Get/Release overrides to access the global True/False instances instead of pooling");
	}

	public T value;

	public static ObjectPool<Boxed<T>> Pool = new ObjectPool<Boxed<T>>(() => new Boxed<T>(), null, null, null, false, 1, 32);
}
