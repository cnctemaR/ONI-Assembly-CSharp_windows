using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class ParallelEnumerableWrapper : ParallelQuery<object>
	{
		internal ParallelEnumerableWrapper(IEnumerable source)
			: base(QuerySettings.Empty)
		{
			this._source = source;
		}

		internal override IEnumerator GetEnumeratorUntyped()
		{
			return this._source.GetEnumerator();
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new EnumerableWrapperWeakToStrong(this._source).GetEnumerator();
		}

		private readonly IEnumerable _source;
	}
}
