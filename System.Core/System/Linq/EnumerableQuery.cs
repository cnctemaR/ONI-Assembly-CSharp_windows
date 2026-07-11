using System;
using System.Collections;
using System.Linq.Expressions;

namespace System.Linq
{
	public abstract class EnumerableQuery
	{
		internal abstract Expression Expression { get; }

		internal abstract IEnumerable Enumerable { get; }

		internal static IQueryable Create(Type elementType, IEnumerable sequence)
		{
			return (IQueryable)Activator.CreateInstance(typeof(EnumerableQuery<>).MakeGenericType(new Type[] { elementType }), new object[] { sequence });
		}

		internal static IQueryable Create(Type elementType, Expression expression)
		{
			return (IQueryable)Activator.CreateInstance(typeof(EnumerableQuery<>).MakeGenericType(new Type[] { elementType }), new object[] { expression });
		}
	}
}
