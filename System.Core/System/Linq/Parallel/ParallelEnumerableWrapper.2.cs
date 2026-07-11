using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class ParallelEnumerableWrapper<T> : ParallelQuery<T>
	{
		internal ParallelEnumerableWrapper(IEnumerable<T> wrappedEnumerable)
			: base(QuerySettings.Empty)
		{
			this._wrappedEnumerable = wrappedEnumerable;
		}

		internal IEnumerable<T> WrappedEnumerable
		{
			get
			{
				return this._wrappedEnumerable;
			}
		}

		public override IEnumerator<T> GetEnumerator()
		{
			return this._wrappedEnumerable.GetEnumerator();
		}

		private readonly IEnumerable<T> _wrappedEnumerable;
	}
}
