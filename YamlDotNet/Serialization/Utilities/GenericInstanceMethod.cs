using System;
using System.Linq.Expressions;
using System.Reflection;

namespace YamlDotNet.Serialization.Utilities
{
	public sealed class GenericInstanceMethod<TInstance>
	{
		public GenericInstanceMethod(Expression<Action<TInstance>> methodCall)
		{
			MethodCallExpression methodCallExpression = (MethodCallExpression)methodCall.Body;
			this.methodToCall = methodCallExpression.Method.GetGenericMethodDefinition();
		}

		public object Invoke(Type[] genericArguments, TInstance instance, params object[] arguments)
		{
			object obj;
			try
			{
				obj = this.methodToCall.MakeGenericMethod(genericArguments).Invoke(instance, arguments);
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
