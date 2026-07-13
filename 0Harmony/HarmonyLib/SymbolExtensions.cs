using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib
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
			if (methodCallExpression == null)
			{
				UnaryExpression unaryExpression = expression.Body as UnaryExpression;
				if (unaryExpression != null)
				{
					MethodCallExpression methodCallExpression2 = unaryExpression.Operand as MethodCallExpression;
					if (methodCallExpression2 != null)
					{
						ConstantExpression constantExpression = methodCallExpression2.Object as ConstantExpression;
						if (constantExpression != null)
						{
							MethodInfo methodInfo = constantExpression.Value as MethodInfo;
							if (methodInfo != null)
							{
								return methodInfo;
							}
						}
					}
				}
				throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
			}
			MethodInfo method = methodCallExpression.Method;
			if (method == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Cannot find method for expression ");
				defaultInterpolatedStringHandler.AppendFormatted<LambdaExpression>(expression);
				throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return method;
		}
	}
}
