using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace Microsoft.Internal
{
	internal static class ReflectionServices
	{
		public static Assembly Assembly(this MemberInfo member)
		{
			Type type = member as Type;
			if (type != null)
			{
				return type.Assembly;
			}
			return member.DeclaringType.Assembly;
		}

		public static bool IsVisible(this ConstructorInfo constructor)
		{
			return constructor.DeclaringType.IsVisible && constructor.IsPublic;
		}

		public static bool IsVisible(this FieldInfo field)
		{
			return field.DeclaringType.IsVisible && field.IsPublic;
		}

		public static bool IsVisible(this MethodInfo method)
		{
			if (!method.DeclaringType.IsVisible)
			{
				return false;
			}
			if (!method.IsPublic)
			{
				return false;
			}
			if (method.IsGenericMethod)
			{
				Type[] genericArguments = method.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (!genericArguments[i].IsVisible)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static string GetDisplayName(Type declaringType, string name)
		{
			Assumes.NotNull<Type>(declaringType);
			return declaringType.GetDisplayName() + "." + name;
		}

		public static string GetDisplayName(this MemberInfo member)
		{
			Assumes.NotNull<MemberInfo>(member);
			MemberTypes memberType = member.MemberType;
			if (memberType == MemberTypes.TypeInfo || memberType == MemberTypes.NestedType)
			{
				return AttributedModelServices.GetTypeIdentity((Type)member);
			}
			return ReflectionServices.GetDisplayName(member.DeclaringType, member.Name);
		}

		internal static bool TryGetGenericInterfaceType(Type instanceType, Type targetOpenInterfaceType, out Type targetClosedInterfaceType)
		{
			Assumes.IsTrue(targetOpenInterfaceType.IsInterface);
			Assumes.IsTrue(targetOpenInterfaceType.IsGenericTypeDefinition);
			Assumes.IsTrue(!instanceType.IsGenericTypeDefinition);
			if (instanceType.IsInterface && instanceType.IsGenericType && instanceType.UnderlyingSystemType.GetGenericTypeDefinition() == targetOpenInterfaceType.UnderlyingSystemType)
			{
				targetClosedInterfaceType = instanceType;
				return true;
			}
			try
			{
				Type @interface = instanceType.GetInterface(targetOpenInterfaceType.Name, false);
				if (@interface != null && @interface.UnderlyingSystemType.GetGenericTypeDefinition() == targetOpenInterfaceType.UnderlyingSystemType)
				{
					targetClosedInterfaceType = @interface;
					return true;
				}
			}
			catch (AmbiguousMatchException)
			{
			}
			targetClosedInterfaceType = null;
			return false;
		}

		internal static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
		{
			return type.GetInterfaces().Concat<Type>(new Type[] { type }).SelectMany<Type, PropertyInfo>((Type itf) => itf.GetProperties());
		}

		internal static IEnumerable<MethodInfo> GetAllMethods(this Type type)
		{
			IEnumerable<MethodInfo> declaredMethods = type.GetDeclaredMethods();
			Type baseType = type.BaseType;
			if (baseType.UnderlyingSystemType != typeof(object))
			{
				return declaredMethods.Concat<MethodInfo>(baseType.GetAllMethods());
			}
			return declaredMethods;
		}

		private static IEnumerable<MethodInfo> GetDeclaredMethods(this Type type)
		{
			foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				yield return methodInfo;
			}
			MethodInfo[] array = null;
			yield break;
		}

		public static IEnumerable<FieldInfo> GetAllFields(this Type type)
		{
			IEnumerable<FieldInfo> declaredFields = type.GetDeclaredFields();
			Type baseType = type.BaseType;
			if (baseType.UnderlyingSystemType != typeof(object))
			{
				return declaredFields.Concat<FieldInfo>(baseType.GetAllFields());
			}
			return declaredFields;
		}

		private static IEnumerable<FieldInfo> GetDeclaredFields(this Type type)
		{
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				yield return fieldInfo;
			}
			FieldInfo[] array = null;
			yield break;
		}
	}
}
