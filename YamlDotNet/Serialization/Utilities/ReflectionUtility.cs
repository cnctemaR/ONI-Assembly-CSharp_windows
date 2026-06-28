using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YamlDotNet.Serialization.Utilities
{
	internal static class ReflectionUtility
	{
		public static Type GetImplementedGenericInterface(Type type, Type genericInterfaceType)
		{
			foreach (Type type2 in ReflectionUtility.GetImplementedInterfaces(type))
			{
				if (type2.IsGenericType() && type2.GetGenericTypeDefinition() == genericInterfaceType)
				{
					return type2;
				}
			}
			return null;
		}

		public static IEnumerable<Type> GetImplementedInterfaces(Type type)
		{
			if (type.IsInterface())
			{
				yield return type;
			}
			foreach (Type implementedInterface in type.GetInterfaces())
			{
				yield return implementedInterface;
			}
			yield break;
		}

		public static MethodInfo GetMethod(Expression<Action> methodAccess)
		{
			MethodInfo methodInfo = ((MethodCallExpression)methodAccess.Body).Method;
			if (methodInfo.IsGenericMethod)
			{
				methodInfo = methodInfo.GetGenericMethodDefinition();
			}
			return methodInfo;
		}

		public static MethodInfo GetMethod<T>(Expression<Action<T>> methodAccess)
		{
			MethodInfo methodInfo = ((MethodCallExpression)methodAccess.Body).Method;
			if (methodInfo.IsGenericMethod)
			{
				methodInfo = methodInfo.GetGenericMethodDefinition();
			}
			return methodInfo;
		}
	}
}
