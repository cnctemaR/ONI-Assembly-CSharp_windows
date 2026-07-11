using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Harmony
{
	public static class SymbolExtensions
	{
		public static MethodInfo GetMethodInfo(Expression<Action> expression)
		{
			return SymbolExtensions.GetMethodInfo(expression);
		}

		public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
		{
			return SymbolExtensions.GetMethodInfo(expression);
		}

		public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			return SymbolExtensions.GetMethodInfo(expression);
		}

		public static MethodInfo GetMethodInfo(LambdaExpression expression)
		{
			MethodCallExpression methodCallExpression = expression.Body as MethodCallExpression;
			bool flag = methodCallExpression == null;
			if (flag)
			{
				throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
			}
			MethodInfo method = methodCallExpression.Method;
			bool flag2 = method == null;
			if (flag2)
			{
				throw new Exception("Cannot find method for expression " + expression);
			}
			return method;
		}
	}
}
