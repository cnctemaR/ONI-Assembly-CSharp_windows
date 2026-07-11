using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Parallel;

namespace System.Linq
{
	public class ParallelQuery<TSource> : ParallelQuery, IEnumerable<TSource>, IEnumerable
	{
		internal ParallelQuery(QuerySettings settings)
			: base(settings)
		{
		}

		internal sealed override ParallelQuery<TCastTo> Cast<TCastTo>()
		{
			return this.Select<TSource, TCastTo>((TSource elem) => (TCastTo)((object)elem));
		}

		internal sealed override ParallelQuery<TCastTo> OfType<TCastTo>()
		{
			return from elem in this
				where elem is TCastTo
				select (TCastTo)((object)elem);
		}

		internal override IEnumerator GetEnumeratorUntyped()
		{
			return ((IEnumerable<TSource>)this).GetEnumerator();
		}

		public virtual IEnumerator<TSource> GetEnumerator()
		{
			throw new NotSupportedException();
		}
	}
}
