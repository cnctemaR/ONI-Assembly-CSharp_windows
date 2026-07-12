using System;
using System.Collections;

public class Promise : IEnumerator
{
	public bool IsResolved
	{
		get
		{
			return this.m_is_resolved;
		}
	}

	public Promise(Action<global::System.Action> fn)
	{
		fn(delegate
		{
			this.Resolve();
		});
	}

	public Promise()
	{
	}

	public void EnsureResolved()
	{
		if (this.IsResolved)
		{
			return;
		}
		this.Resolve();
	}

	public void Resolve()
	{
		DebugUtil.Assert(!this.m_is_resolved, "Can only resolve a promise once");
		this.m_is_resolved = true;
		if (this.on_complete != null)
		{
			this.on_complete();
			this.on_complete = null;
		}
	}

	public Promise Then(global::System.Action callback)
	{
		if (this.m_is_resolved)
		{
			callback();
		}
		else
		{
			this.on_complete = (global::System.Action)Delegate.Combine(this.on_complete, callback);
		}
		return this;
	}

	public Promise ThenWait(Func<Promise> callback)
	{
		if (this.m_is_resolved)
		{
			return callback();
		}
		return new Promise(delegate(global::System.Action resolve)
		{
			this.on_complete = (global::System.Action)Delegate.Combine(this.on_complete, new global::System.Action(delegate
			{
				callback().Then(resolve);
			}));
		});
	}

	public Promise<T> ThenWait<T>(Func<Promise<T>> callback)
	{
		if (this.m_is_resolved)
		{
			return callback();
		}
		return new Promise<T>(delegate(Action<T> resolve)
		{
			this.on_complete = (global::System.Action)Delegate.Combine(this.on_complete, new global::System.Action(delegate
			{
				callback().Then(resolve);
			}));
		});
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
		return !this.IsResolved;
	}

	void IEnumerator.Reset()
	{
	}

	static Promise()
	{
		Promise.m_instant.Resolve();
	}

	public static Promise Instant
	{
		get
		{
			return Promise.m_instant;
		}
	}

	public static Promise Fail
	{
		get
		{
			return new Promise();
		}
	}

	public static Promise All(params Promise[] promises)
	{
		Promise.<>c__DisplayClass21_0 CS$<>8__locals1 = new Promise.<>c__DisplayClass21_0();
		CS$<>8__locals1.promises = promises;
		if (CS$<>8__locals1.promises == null || CS$<>8__locals1.promises.Length == 0)
		{
			return Promise.Instant;
		}
		CS$<>8__locals1.all_resolved_promise = new Promise();
		Promise[] promises2 = CS$<>8__locals1.promises;
		for (int i = 0; i < promises2.Length; i++)
		{
			promises2[i].Then(new global::System.Action(CS$<>8__locals1.<All>g__TryResolve|0));
		}
		return CS$<>8__locals1.all_resolved_promise;
	}

	public static Promise Chain(params Func<Promise>[] make_promise_fns)
	{
		Promise.<>c__DisplayClass22_0 CS$<>8__locals1 = new Promise.<>c__DisplayClass22_0();
		CS$<>8__locals1.make_promise_fns = make_promise_fns;
		CS$<>8__locals1.all_resolve_promise = new Promise();
		CS$<>8__locals1.current_promise_fn_index = 0;
		CS$<>8__locals1.<Chain>g__TryNext|0();
		return CS$<>8__locals1.all_resolve_promise;
	}

	private global::System.Action on_complete;

	private bool m_is_resolved;

	private static Promise m_instant = new Promise();
}
