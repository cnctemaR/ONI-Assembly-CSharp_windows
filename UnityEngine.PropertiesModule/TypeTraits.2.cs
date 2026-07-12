using System;
using System.Reflection;
using UnityEngine;

namespace Unity.Properties
{
	public static class TypeTraits<T>
	{
		public static bool IsValueType { get; }

		public static bool IsPrimitive { get; }

		public static bool IsInterface { get; }

		public static bool IsAbstract { get; }

		public static bool IsArray { get; }

		public static bool IsMultidimensionalArray { get; }

		public static bool IsEnum { get; }

		public static bool IsEnumFlags { get; }

		public static bool IsNullable { get; }

		public static bool IsObject { get; }

		public static bool IsString { get; }

		public static bool IsContainer { get; }

		public static bool CanBeNull { get; }

		public static bool IsPrimitiveOrString { get; }

		public static bool IsAbstractOrInterface { get; }

		public static bool IsUnityObject { get; }

		public static bool IsLazyLoadReference { get; }

		static TypeTraits()
		{
			Type typeFromHandle = typeof(T);
			TypeTraits<T>.IsValueType = typeFromHandle.IsValueType;
			TypeTraits<T>.IsPrimitive = typeFromHandle.IsPrimitive;
			TypeTraits<T>.IsInterface = typeFromHandle.IsInterface;
			TypeTraits<T>.IsAbstract = typeFromHandle.IsAbstract;
			TypeTraits<T>.IsArray = typeFromHandle.IsArray;
			TypeTraits<T>.IsEnum = typeFromHandle.IsEnum;
			TypeTraits<T>.IsEnumFlags = TypeTraits<T>.IsEnum && typeFromHandle.GetCustomAttribute<FlagsAttribute>() != null;
			TypeTraits<T>.IsNullable = Nullable.GetUnderlyingType(typeof(T)) != null;
			TypeTraits<T>.IsMultidimensionalArray = TypeTraits<T>.IsArray && typeof(T).GetArrayRank() != 1;
			TypeTraits<T>.IsObject = typeFromHandle == typeof(object);
			TypeTraits<T>.IsString = typeFromHandle == typeof(string);
			TypeTraits<T>.IsContainer = TypeTraits.IsContainer(typeFromHandle);
			TypeTraits<T>.CanBeNull = !TypeTraits<T>.IsValueType;
			TypeTraits<T>.IsPrimitiveOrString = TypeTraits<T>.IsPrimitive || TypeTraits<T>.IsString;
			TypeTraits<T>.IsAbstractOrInterface = TypeTraits<T>.IsAbstract || TypeTraits<T>.IsInterface;
			TypeTraits<T>.CanBeNull |= TypeTraits<T>.IsNullable;
			TypeTraits<T>.IsLazyLoadReference = typeFromHandle.IsGenericType && typeFromHandle.GetGenericTypeDefinition() == typeof(LazyLoadReference<>);
			TypeTraits<T>.IsUnityObject = typeof(Object).IsAssignableFrom(typeFromHandle);
		}
	}
}
