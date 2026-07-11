using System;
using System.Collections.Generic;
using System.Linq.Parallel;

namespace System.Linq
{
	public class OrderedParallelQuery<TSource> : ParallelQuery<TSource>
	{
		internal OrderedParallelQuery(QueryOperator<TSource> sortOp)
			: base(sortOp.SpecifiedQuerySettings)
		{
			this._sortOp = sortOp;
		}

		internal QueryOperator<TSource> SortOperator
		{
			get
			{
				return this._sortOp;
			}
		}

		internal IOrderedEnumerable<TSource> OrderedEnumerable
		{
			get
			{
				return (IOrderedEnumerable<TSource>)this._sortOp;
			}
		}

		public override IEnumerator<TSource> GetEnumerator()
		{
			return this._sortOp.GetEnumerator();
		}

		private QueryOperator<TSource> _sortOp;
	}
}
