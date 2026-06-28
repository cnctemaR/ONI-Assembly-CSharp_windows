using System;
using System.Linq.Expressions;
using System.Reflection;

namespace YamlDotNet.Serialization.Utilities
{
	public sealed class GenericStaticMethod
	{
		public GenericStaticMethod(Expression<Action> methodCall)
		{
			MethodCallExpression methodCallExpression = (MethodCallExpression)methodCall.Body;
			this.methodToCall = methodCallExpression.Method.GetGenericMethodDefinition();
		}

		public object Invoke(Type[] genericArguments, params object[] arguments)
		{
			object obj;
			try
			{
				obj = this.methodToCall.MakeGenericMethod(genericArguments).Invoke(null, arguments);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.Unwrap();
			}
			return obj;
		}

		private readonly MethodInfo methodToCall;
	}
}
