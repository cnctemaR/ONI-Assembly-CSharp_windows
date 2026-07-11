using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	internal interface IQueryableEnumerable<TElement> : IQueryableEnumerable, IEnumerable, IOrderedQueryable, IQueryable, IQueryable<TElement>, IEnumerable<TElement>, IOrderedQueryable<TElement>
	{
	}
}
