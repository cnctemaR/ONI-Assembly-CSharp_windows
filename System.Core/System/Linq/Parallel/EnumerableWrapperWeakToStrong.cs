using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class EnumerableWrapperWeakToStrong : IEnumerable<object>, IEnumerable
	{
		internal EnumerableWrapperWeakToStrong(IEnumerable wrappedEnumerable)
		{
			this._wrappedEnumerable = wrappedEnumerable;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>)this).GetEnumerator();
		}

		public IEnumerator<object> GetEnumerator()
		{
			return new EnumerableWrapperWeakToStrong.WrapperEnumeratorWeakToStrong(this._wrappedEnumerable.GetEnumerator());
		}

		private readonly IEnumerable _wrappedEnumerable;

		private class WrapperEnumeratorWeakToStrong : IEnumerator<object>, IDisposable, IEnumerator
		{
			internal WrapperEnumeratorWeakToStrong(IEnumerator wrappedEnumerator)
			{
				this._wrappedEnumerator = wrappedEnumerator;
			}

			object IEnumerator.Current
			{
				get
				{
					return this._wrappedEnumerator.Current;
				}
			}

			object IEnumerator<object>.Current
			{
				get
				{
					return this._wrappedEnumerator.Current;
				}
			}

			void IDisposable.Dispose()
			{
				IDisposable disposable = this._wrappedEnumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}

			bool IEnumerator.MoveNext()
			{
				return this._wrappedEnumerator.MoveNext();
			}

			void IEnumerator.Reset()
			{
				this._wrappedEnumerator.Reset();
			}

			private IEnumerator _wrappedEnumerator;
		}
	}
}
