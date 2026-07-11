using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	public interface IOrderedQueryable<out T> : IQueryable<T>, IEnumerable<T>, IEnumerable, IQueryable, IOrderedQueryable
	{
	}
}
