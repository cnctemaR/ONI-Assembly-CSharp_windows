using System;
using System.Collections.Generic;

namespace System.Data
{
	internal sealed class Listeners<TElem> where TElem : class
	{
		internal Listeners(int ObjectID, Listeners<TElem>.Func<TElem, bool> notifyFilter)
		{
			this._listeners = new List<TElem>();
			this._filter = notifyFilter;
			this._objectID = ObjectID;
			this._listenerReaderCount = 0;
		}

		internal bool HasListeners
		{
			get
			{
				return 0 < this._listeners.Count;
			}
		}

		internal void Add(TElem listener)
		{
			this._listeners.Add(listener);
		}

		internal int IndexOfReference(TElem listener)
		{
			return Index.IndexOfReference<TElem>(this._listeners, listener);
		}

		internal void Remove(TElem listener)
		{
			int num = this.IndexOfReference(listener);
			this._listeners[num] = default(TElem);
			if (this._listenerReaderCount == 0)
			{
				this._listeners.RemoveAt(num);
				this._listeners.TrimExcess();
			}
		}

		internal void Notify<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, Listeners<TElem>.Action<TElem, T1, T2, T3> action)
		{
			int count = this._listeners.Count;
			if (0 < count)
			{
				int num = -1;
				this._listenerReaderCount++;
				try
				{
					for (int i = 0; i < count; i++)
					{
						TElem telem = this._listeners[i];
						if (this._filter(telem))
						{
							action(telem, arg1, arg2, arg3);
						}
						else
						{
							this._listeners[i] = default(TElem);
							num = i;
						}
					}
				}
				finally
				{
					this._listenerReaderCount--;
				}
				if (this._listenerReaderCount == 0)
				{
					this.RemoveNullListeners(num);
				}
			}
		}

		private void RemoveNullListeners(int nullIndex)
		{
			int num = nullIndex;
			while (0 <= num)
			{
				if (this._listeners[num] == null)
				{
					this._listeners.RemoveAt(num);
				}
				num--;
			}
		}

		private readonly List<TElem> _listeners;

		private readonly Listeners<TElem>.Func<TElem, bool> _filter;

		private readonly int _objectID;

		private int _listenerReaderCount;

		internal delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

		internal delegate TResult Func<T1, TResult>(T1 arg1);
	}
}
