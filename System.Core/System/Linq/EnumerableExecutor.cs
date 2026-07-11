using System;
using System.Linq.Expressions;

namespace System.Linq
{
	public abstract class EnumerableExecutor
	{
		internal abstract object ExecuteBoxed();

		internal static EnumerableExecutor Create(Expression expression)
		{
			return (EnumerableExecutor)Activator.CreateInstance(typeof(EnumerableExecutor<>).MakeGenericType(new Type[] { expression.Type }), new object[] { expression });
		}
	}
}
