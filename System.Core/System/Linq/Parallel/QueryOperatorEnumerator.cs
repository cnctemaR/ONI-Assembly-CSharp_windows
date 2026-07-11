using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal abstract class QueryOperatorEnumerator<TElement, TKey>
	{
		internal abstract bool MoveNext(ref TElement currentElement, ref TKey currentKey);

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		internal virtual void Reset()
		{
		}

		internal IEnumerator<TElement> AsClassicEnumerator()
		{
			return new QueryOperatorEnumerator<TElement, TKey>.QueryOperatorClassicEnumerator(this);
		}

		private class QueryOperatorClassicEnumerator : IEnumerator<TElement>, IDisposable, IEnumerator
		{
			internal QueryOperatorClassicEnumerator(QueryOperatorEnumerator<TElement, TKey> operatorEnumerator)
			{
				this._operatorEnumerator = operatorEnumerator;
			}

			public bool MoveNext()
			{
				TKey tkey = default(TKey);
				return this._operatorEnumerator.MoveNext(ref this._current, ref tkey);
			}

			public TElement Current
			{
				get
				{
					return this._current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this._current;
				}
			}

			public void Dispose()
			{
				this._operatorEnumerator.Dispose();
				this._operatorEnumerator = null;
			}

			public void Reset()
			{
				this._operatorEnumerator.Reset();
			}

			private QueryOperatorEnumerator<TElement, TKey> _operatorEnumerator;

			private TElement _current;
		}
	}
}
