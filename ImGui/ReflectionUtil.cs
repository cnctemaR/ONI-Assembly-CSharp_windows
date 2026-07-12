using System;
using System.Collections.Generic;
using System.Reflection;

public static class ReflectionUtil
{
	public static List<Type> CollectTypesThatInheritOrImplement<T>(BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
	{
		Type typeFromHandle = typeof(T);
		List<Type> list = new List<Type>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			foreach (Type type in assemblies[i].GetTypes())
			{
				if (typeFromHandle.IsAssignableFrom(type))
				{
					list.Add(type);
				}
			}
		}
		return list;
	}

	public const BindingFlags BINDING_FLAGS_MOST = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
}
