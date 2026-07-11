using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	public interface IOrderedQueryable<T> : IEnumerable, IOrderedQueryable, IQueryable, IQueryable<T>, IEnumerable<T>
	{
	}
}
