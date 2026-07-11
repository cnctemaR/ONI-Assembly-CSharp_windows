using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	public interface IQueryable<T> : IEnumerable, IQueryable, IEnumerable<T>
	{
	}
}
