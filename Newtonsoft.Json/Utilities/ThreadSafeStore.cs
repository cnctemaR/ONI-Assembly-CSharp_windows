using System;
using System.Collections.Generic;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
	internal class ThreadSafeStore<TKey, TValue>
	{
		public ThreadSafeStore(Func<TKey, TValue> creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			this._creator = creator;
			this._store = new Dictionary<TKey, TValue>();
		}

		public TValue Get(TKey key)
		{
			TValue tvalue;
			if (!this._store.TryGetValue(key, out tvalue))
			{
				return this.AddValue(key);
			}
			return tvalue;
		}

		private TValue AddValue(TKey key)
		{
			TValue tvalue = this._creator(key);
			TValue tvalue3;
			lock (this._lock)
			{
				if (this._store == null)
				{
					this._store = new Dictionary<TKey, TValue>();
					this._store[key] = tvalue;
				}
				else
				{
					TValue tvalue2;
					if (this._store.TryGetValue(key, out tvalue2))
					{
						return tvalue2;
					}
					Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._store);
					dictionary[key] = tvalue;
					Thread.MemoryBarrier();
					this._store = dictionary;
				}
				tvalue3 = tvalue;
			}
			return tvalue3;
		}

		private readonly object _lock = new object();

		private Dictionary<TKey, TValue> _store;

		private readonly Func<TKey, TValue> _creator;
	}
}
