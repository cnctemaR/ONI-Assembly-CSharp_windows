using System;
using System.Collections.Generic;

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
			foreach (Type type2 in type.GetInterfaces())
			{
				yield return type2;
			}
			Type[] array = null;
			yield break;
		}
	}
}
