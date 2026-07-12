using System;
using System.Collections.Generic;
using System.Reflection;

namespace HarmonyLib
{
	internal class AccessCache
	{
		private static T Get<T>(Dictionary<Type, Dictionary<string, T>> dict, Type type, string name, Func<T> fetcher)
		{
			T t2;
			lock (dict)
			{
				Dictionary<string, T> dictionary;
				if (!dict.TryGetValue(type, out dictionary))
				{
					dictionary = new Dictionary<string, T>();
					dict[type] = dictionary;
				}
				T t;
				if (!dictionary.TryGetValue(name, out t))
				{
					t = fetcher();
					dictionary[name] = t;
				}
				t2 = t;
			}
			return t2;
		}

		private static T Get<T>(Dictionary<Type, Dictionary<string, Dictionary<int, T>>> dict, Type type, string name, Type[] arguments, Func<T> fetcher)
		{
			T t2;
			lock (dict)
			{
				Dictionary<string, Dictionary<int, T>> dictionary;
				if (!dict.TryGetValue(type, out dictionary))
				{
					dictionary = new Dictionary<string, Dictionary<int, T>>();
					dict[type] = dictionary;
				}
				Dictionary<int, T> dictionary2;
				if (!dictionary.TryGetValue(name, out dictionary2))
				{
					dictionary2 = new Dictionary<int, T>();
					dictionary[name] = dictionary2;
				}
				int num = AccessTools.CombinedHashCode(arguments);
				T t;
				if (!dictionary2.TryGetValue(num, out t))
				{
					t = fetcher();
					dictionary2[num] = t;
				}
				t2 = t;
			}
			return t2;
		}

		internal FieldInfo GetFieldInfo(Type type, string name, AccessCache.MemberType memberType = AccessCache.MemberType.Any, bool declaredOnly = false)
		{
			FieldInfo fieldInfo = AccessCache.Get<FieldInfo>(this.declaredFields, type, name, () => type.GetField(name, AccessCache.declaredOnlyBindingFlags[memberType]));
			if (fieldInfo == null && !declaredOnly)
			{
				Func<Type, FieldInfo> <>9__2;
				fieldInfo = AccessCache.Get<FieldInfo>(this.inheritedFields, type, name, delegate
				{
					Type type2 = type;
					Func<Type, FieldInfo> func;
					if ((func = <>9__2) == null)
					{
						func = (<>9__2 = (Type t) => t.GetField(name, AccessTools.all));
					}
					return AccessTools.FindIncludingBaseTypes<FieldInfo>(type2, func);
				});
			}
			return fieldInfo;
		}

		internal PropertyInfo GetPropertyInfo(Type type, string name, AccessCache.MemberType memberType = AccessCache.MemberType.Any, bool declaredOnly = false)
		{
			PropertyInfo propertyInfo = AccessCache.Get<PropertyInfo>(this.declaredProperties, type, name, () => type.GetProperty(name, AccessCache.declaredOnlyBindingFlags[memberType]));
			if (propertyInfo == null && !declaredOnly)
			{
				Func<Type, PropertyInfo> <>9__2;
				propertyInfo = AccessCache.Get<PropertyInfo>(this.inheritedProperties, type, name, delegate
				{
					Type type2 = type;
					Func<Type, PropertyInfo> func;
					if ((func = <>9__2) == null)
					{
						func = (<>9__2 = (Type t) => t.GetProperty(name, AccessTools.all));
					}
					return AccessTools.FindIncludingBaseTypes<PropertyInfo>(type2, func);
				});
			}
			return propertyInfo;
		}

		internal MethodBase GetMethodInfo(Type type, string name, Type[] arguments, AccessCache.MemberType memberType = AccessCache.MemberType.Any, bool declaredOnly = false)
		{
			MethodBase methodBase = AccessCache.Get<MethodBase>(this.declaredMethods, type, name, arguments, () => type.GetMethod(name, AccessCache.declaredOnlyBindingFlags[memberType], null, arguments, null));
			if (methodBase == null && !declaredOnly)
			{
				methodBase = AccessCache.Get<MethodBase>(this.inheritedMethods, type, name, arguments, () => AccessTools.Method(type, name, arguments, null));
			}
			return methodBase;
		}

		private const BindingFlags BasicFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;

		private static readonly Dictionary<AccessCache.MemberType, BindingFlags> declaredOnlyBindingFlags = new Dictionary<AccessCache.MemberType, BindingFlags>
		{
			{
				AccessCache.MemberType.Any,
				BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty
			},
			{
				AccessCache.MemberType.Instance,
				BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty
			},
			{
				AccessCache.MemberType.Static,
				BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty
			}
		};

		private readonly Dictionary<Type, Dictionary<string, FieldInfo>> declaredFields = new Dictionary<Type, Dictionary<string, FieldInfo>>();

		private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> declaredProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

		private readonly Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>> declaredMethods = new Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>>();

		private readonly Dictionary<Type, Dictionary<string, FieldInfo>> inheritedFields = new Dictionary<Type, Dictionary<string, FieldInfo>>();

		private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> inheritedProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

		private readonly Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>> inheritedMethods = new Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>>();

		internal enum MemberType
		{
			Any,
			Static,
			Instance
		}
	}
}
