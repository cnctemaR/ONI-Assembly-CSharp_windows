using System;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal static class ReflectionExtensions
	{
		public static ReflectionMember ToReflectionMember(this LazyMemberInfo lazyMember)
		{
			MemberInfo[] accessors = lazyMember.GetAccessors();
			MemberTypes memberType = lazyMember.MemberType;
			if (memberType <= MemberTypes.Property)
			{
				if (memberType == MemberTypes.Field)
				{
					Assumes.IsTrue(accessors.Length == 1);
					return ((FieldInfo)accessors[0]).ToReflectionField();
				}
				if (memberType == MemberTypes.Property)
				{
					Assumes.IsTrue(accessors.Length == 2);
					return ReflectionExtensions.CreateReflectionProperty((MethodInfo)accessors[0], (MethodInfo)accessors[1]);
				}
			}
			else if (memberType == MemberTypes.TypeInfo || memberType == MemberTypes.NestedType)
			{
				return ((Type)accessors[0]).ToReflectionType();
			}
			Assumes.IsTrue(memberType == MemberTypes.Method);
			return ((MethodInfo)accessors[0]).ToReflectionMethod();
		}

		public static LazyMemberInfo ToLazyMember(this MemberInfo member)
		{
			Assumes.NotNull<MemberInfo>(member);
			if (member.MemberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = member as PropertyInfo;
				Assumes.NotNull<PropertyInfo>(propertyInfo);
				MemberInfo[] array = new MemberInfo[]
				{
					propertyInfo.GetGetMethod(true),
					propertyInfo.GetSetMethod(true)
				};
				return new LazyMemberInfo(MemberTypes.Property, array);
			}
			return new LazyMemberInfo(member);
		}

		public static ReflectionWritableMember ToReflectionWriteableMember(this LazyMemberInfo lazyMember)
		{
			Assumes.IsTrue(lazyMember.MemberType == MemberTypes.Field || lazyMember.MemberType == MemberTypes.Property);
			ReflectionWritableMember reflectionWritableMember = lazyMember.ToReflectionMember() as ReflectionWritableMember;
			Assumes.NotNull<ReflectionWritableMember>(reflectionWritableMember);
			return reflectionWritableMember;
		}

		public static ReflectionProperty ToReflectionProperty(this PropertyInfo property)
		{
			Assumes.NotNull<PropertyInfo>(property);
			return ReflectionExtensions.CreateReflectionProperty(property.GetGetMethod(true), property.GetSetMethod(true));
		}

		public static ReflectionProperty CreateReflectionProperty(MethodInfo getMethod, MethodInfo setMethod)
		{
			Assumes.IsTrue(getMethod != null || setMethod != null);
			return new ReflectionProperty(getMethod, setMethod);
		}

		public static ReflectionParameter ToReflectionParameter(this ParameterInfo parameter)
		{
			Assumes.NotNull<ParameterInfo>(parameter);
			return new ReflectionParameter(parameter);
		}

		public static ReflectionMethod ToReflectionMethod(this MethodInfo method)
		{
			Assumes.NotNull<MethodInfo>(method);
			return new ReflectionMethod(method);
		}

		public static ReflectionField ToReflectionField(this FieldInfo field)
		{
			Assumes.NotNull<FieldInfo>(field);
			return new ReflectionField(field);
		}

		public static ReflectionType ToReflectionType(this Type type)
		{
			Assumes.NotNull<Type>(type);
			return new ReflectionType(type);
		}

		public static ReflectionWritableMember ToReflectionWritableMember(this MemberInfo member)
		{
			Assumes.NotNull<MemberInfo>(member);
			if (member.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)member).ToReflectionProperty();
			}
			return ((FieldInfo)member).ToReflectionField();
		}
	}
}
