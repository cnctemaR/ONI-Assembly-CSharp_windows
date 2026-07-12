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

	public static List<Type> CollectTypesInNamespace(string namespaceName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
	{
		List<Type> list = new List<Type>();
		string[] array = namespaceName.Split(new char[] { '.' });
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			foreach (Type type in assemblies[i].GetTypes())
			{
				if (!string.IsNullOrWhiteSpace(type.Namespace))
				{
					string[] array2 = type.Namespace.Split(new char[] { '.' });
					if (array2.Length >= array.Length)
					{
						bool flag = true;
						for (int k = 0; k < array.Length; k++)
						{
							if (array[k] != array2[k])
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							list.Add(type);
						}
					}
				}
			}
		}
		return list;
	}

	public static T[] GetEnumValues<T>() where T : struct, Enum
	{
		return (T[])Enum.GetValues(typeof(T));
	}

	public static bool HasDefaultConstructor(Type type)
	{
		return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
	}

	public static ReflectionUtil.ForObject<T> For<T>(T sourceObject)
	{
		return new ReflectionUtil.ForObject<T>(sourceObject);
	}

	public const BindingFlags BINDING_FLAGS_MOST = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	public const BindingFlags BINDING_FLAGS_DECLARED_ONLY = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	public readonly ref struct ForObject<TSourceObject>
	{
		public ForObject(TSourceObject sourceObject)
		{
			this.sourceObject = sourceObject;
			this.sourceObjectType = sourceObject.GetType();
		}

		public List<FieldInfo> CollectFieldsThatInheritOrImplement<TFieldType>(BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
		{
			Type typeFromHandle = typeof(TFieldType);
			List<FieldInfo> list = new List<FieldInfo>();
			foreach (FieldInfo fieldInfo in this.sourceObjectType.GetFields(bindingFlags))
			{
				if (typeFromHandle.IsAssignableFrom(fieldInfo.FieldType))
				{
					list.Add(fieldInfo);
				}
			}
			return list;
		}

		public List<TFieldType> CollectValuesForFieldsThatInheritOrImplement<TFieldType>(BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
		{
			Type typeFromHandle = typeof(TFieldType);
			List<TFieldType> list = new List<TFieldType>();
			foreach (FieldInfo fieldInfo in this.sourceObjectType.GetFields(bindingFlags))
			{
				if (typeFromHandle.IsAssignableFrom(fieldInfo.FieldType))
				{
					list.Add((TFieldType)((object)fieldInfo.GetValue(this.sourceObject)));
				}
			}
			return list;
		}

		public readonly TSourceObject sourceObject;

		public readonly Type sourceObjectType;
	}
}
