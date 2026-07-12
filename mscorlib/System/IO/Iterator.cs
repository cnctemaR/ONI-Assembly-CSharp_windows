using System;
using System.Collections;
using System.Collections.Generic;

namespace System.IO
{
	internal abstract class Iterator<TSource> : IEnumerable<TSource>, IEnumerable, IEnumerator<TSource>, IDisposable, IEnumerator
	{
		public Iterator()
		{
			this._threadId = Environment.CurrentManagedThreadId;
		}

		public TSource Current
		{
			get
			{
				return this.current;
			}
		}

		protected abstract Iterator<TSource> Clone();

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.current = default(TSource);
			this.state = -1;
		}

		public IEnumerator<TSource> GetEnumerator()
		{
			if (this.state == 0 && this._threadId == Environment.CurrentManagedThreadId)
			{
				this.state = 1;
				return this;
			}
			Iterator<TSource> iterator = this.Clone();
			iterator.state = 1;
			return iterator;
		}

		public abstract bool MoveNext();

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		private int _threadId;

		internal int state;

		internal TSource current;
	}
}
