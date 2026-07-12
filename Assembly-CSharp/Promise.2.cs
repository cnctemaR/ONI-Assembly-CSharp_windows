using System;
using System.Collections;

public class Promise<T> : IEnumerator
{
	public bool IsResolved
	{
		get
		{
			return this.promise.IsResolved;
		}
	}

	public Promise(Action<Action<T>> fn)
	{
		fn(delegate(T value)
		{
			this.Resolve(value);
		});
	}

	public Promise()
	{
	}

	public void EnsureResolved(T value)
	{
		this.result = value;
		this.promise.EnsureResolved();
	}

	public void Resolve(T value)
	{
		this.result = value;
		this.promise.Resolve();
	}

	public Promise<T> Then(Action<T> fn)
	{
		this.promise.Then(delegate
		{
			fn(this.result);
		});
		return this;
	}

	public Promise ThenWait(Func<Promise> fn)
	{
		return this.promise.ThenWait(fn);
	}

	public Promise<T> ThenWait(Func<Promise<T>> fn)
	{
		return this.promise.ThenWait<T>(fn);
	}

	object IEnumerator.Current
	{
		get
		{
			return null;
		}
	}

	bool IEnumerator.MoveNext()
	{
		return !this.promise.IsResolved;
	}

	void IEnumerator.Reset()
	{
	}

	private Promise promise = new Promise();

	private T result;
}
