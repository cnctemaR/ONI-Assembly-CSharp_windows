using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Dynamic.Utils
{
	internal static class TypeUtils
	{
		public static Type GetNonNullableType(this Type type)
		{
			if (!type.IsNullableType())
			{
				return type;
			}
			return type.GetGenericArguments()[0];
		}

		public static Type GetNullableType(this Type type)
		{
			if (type.IsValueType && !type.IsNullableType())
			{
				return typeof(Nullable<>).MakeGenericType(new Type[] { type });
			}
			return type;
		}

		public static bool IsNullableType(this Type type)
		{
			return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static bool IsNullableOrReferenceType(this Type type)
		{
			return !type.IsValueType || type.IsNullableType();
		}

		public static bool IsBool(this Type type)
		{
			return type.GetNonNullableType() == typeof(bool);
		}

		public static bool IsNumeric(this Type type)
		{
			type = type.GetNonNullableType();
			if (!type.IsEnum)
			{
				TypeCode typeCode = type.GetTypeCode();
				if (typeCode - TypeCode.Char <= 10)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsInteger(this Type type)
		{
			type = type.GetNonNullableType();
			if (!type.IsEnum)
			{
				TypeCode typeCode = type.GetTypeCode();
				if (typeCode - TypeCode.SByte <= 7)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsInteger64(this Type type)
		{
			type = type.GetNonNullableType();
			if (!type.IsEnum)
			{
				TypeCode typeCode = type.GetTypeCode();
				if (typeCode - TypeCode.Int64 <= 1)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsArithmetic(this Type type)
		{
			type = type.GetNonNullableType();
			if (!type.IsEnum)
			{
				TypeCode typeCode = type.GetTypeCode();
				if (typeCode - TypeCode.Int16 <= 7)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsUnsignedInt(this Type type)
		{
			type = type.GetNonNullableType();
			if (!type.IsEnum)
			{
				switch (type.GetTypeCode())
				{
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
				}
			}
			return false;
		}

		public static bool IsIntegerOrBool(this Type type)
		{
			type = type.GetNonNullableType();
			if (!type.IsEnum)
			{
				TypeCode typeCode = type.GetTypeCode();
				if (typeCode == TypeCode.Boolean || typeCode - TypeCode.SByte <= 7)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNumericOrBool(this Type type)
		{
			return type.IsNumeric() || type.IsBool();
		}

		public static bool IsValidInstanceType(MemberInfo member, Type instanceType)
		{
			Type declaringType = member.DeclaringType;
			if (TypeUtils.AreReferenceAssignable(declaringType, instanceType))
			{
				return true;
			}
			if (declaringType == null)
			{
				return false;
			}
			if (instanceType.IsValueType)
			{
				if (TypeUtils.AreReferenceAssignable(declaringType, typeof(object)))
				{
					return true;
				}
				if (TypeUtils.AreReferenceAssignable(declaringType, typeof(ValueType)))
				{
					return true;
				}
				if (instanceType.IsEnum && TypeUtils.AreReferenceAssignable(declaringType, typeof(Enum)))
				{
					return true;
				}
				if (declaringType.IsInterface)
				{
					foreach (Type type in instanceType.GetTypeInfo().ImplementedInterfaces)
					{
						if (TypeUtils.AreReferenceAssignable(declaringType, type))
						{
							return true;
						}
					}
					return false;
				}
			}
			return false;
		}

		public static bool HasIdentityPrimitiveOrNullableConversionTo(this Type source, Type dest)
		{
			return TypeUtils.AreEquivalent(source, dest) || (source.IsNullableType() && TypeUtils.AreEquivalent(dest, source.GetNonNullableType())) || (dest.IsNullableType() && TypeUtils.AreEquivalent(source, dest.GetNonNullableType())) || (source.IsConvertible() && dest.IsConvertible() && (dest.GetNonNullableType() != typeof(bool) || (source.IsEnum && source.GetEnumUnderlyingType() == typeof(bool))));
		}

		public static bool HasReferenceConversionTo(this Type source, Type dest)
		{
			if (source == typeof(void) || dest == typeof(void))
			{
				return false;
			}
			Type nonNullableType = source.GetNonNullableType();
			Type nonNullableType2 = dest.GetNonNullableType();
			return nonNullableType.IsAssignableFrom(nonNullableType2) || nonNullableType2.IsAssignableFrom(nonNullableType) || (source.IsInterface || dest.IsInterface) || TypeUtils.IsLegalExplicitVariantDelegateConversion(source, dest) || ((source.IsArray || dest.IsArray) && source.StrictHasReferenceConversionTo(dest, true));
		}

		private static bool StrictHasReferenceConversionTo(this Type source, Type dest, bool skipNonArray)
		{
			for (;;)
			{
				if (!skipNonArray)
				{
					if (source.IsValueType | dest.IsValueType)
					{
						break;
					}
					if (source.IsAssignableFrom(dest) || dest.IsAssignableFrom(source))
					{
						return true;
					}
					if (source.IsInterface)
					{
						if (dest.IsInterface || (dest.IsClass && !dest.IsSealed))
						{
							return true;
						}
					}
					else if (dest.IsInterface && source.IsClass && !source.IsSealed)
					{
						return true;
					}
				}
				if (!source.IsArray)
				{
					goto IL_00B2;
				}
				if (!dest.IsArray)
				{
					goto IL_00AA;
				}
				if (source.GetArrayRank() != dest.GetArrayRank() || source.IsSZArray != dest.IsSZArray)
				{
					return false;
				}
				source = source.GetElementType();
				dest = dest.GetElementType();
				skipNonArray = false;
			}
			return false;
			IL_00AA:
			return TypeUtils.HasArrayToInterfaceConversion(source, dest);
			IL_00B2:
			if (dest.IsArray)
			{
				return TypeUtils.HasInterfaceToArrayConversion(source, dest) || TypeUtils.IsImplicitReferenceConversion(typeof(Array), source);
			}
			return TypeUtils.IsLegalExplicitVariantDelegateConversion(source, dest);
		}

		private static bool HasArrayToInterfaceConversion(Type source, Type dest)
		{
			if (!source.IsSZArray || !dest.IsInterface || !dest.IsGenericType)
			{
				return false;
			}
			Type[] genericArguments = dest.GetGenericArguments();
			if (genericArguments.Length != 1)
			{
				return false;
			}
			Type genericTypeDefinition = dest.GetGenericTypeDefinition();
			foreach (Type type in TypeUtils.s_arrayAssignableInterfaces)
			{
				if (TypeUtils.AreEquivalent(genericTypeDefinition, type))
				{
					return source.GetElementType().StrictHasReferenceConversionTo(genericArguments[0], false);
				}
			}
			return false;
		}

		private static bool HasInterfaceToArrayConversion(Type source, Type dest)
		{
			if (!dest.IsSZArray || !source.IsInterface || !source.IsGenericType)
			{
				return false;
			}
			Type[] genericArguments = source.GetGenericArguments();
			if (genericArguments.Length != 1)
			{
				return false;
			}
			Type genericTypeDefinition = source.GetGenericTypeDefinition();
			foreach (Type type in TypeUtils.s_arrayAssignableInterfaces)
			{
				if (TypeUtils.AreEquivalent(genericTypeDefinition, type))
				{
					return genericArguments[0].StrictHasReferenceConversionTo(dest.GetElementType(), false);
				}
			}
			return false;
		}

		private static bool IsCovariant(Type t)
		{
			return (t.GenericParameterAttributes & GenericParameterAttributes.Covariant) > GenericParameterAttributes.None;
		}

		private static bool IsContravariant(Type t)
		{
			return (t.GenericParameterAttributes & GenericParameterAttributes.Contravariant) > GenericParameterAttributes.None;
		}

		private static bool IsInvariant(Type t)
		{
			return (t.GenericParameterAttributes & GenericParameterAttributes.VarianceMask) == GenericParameterAttributes.None;
		}

		private static bool IsDelegate(Type t)
		{
			return t.IsSubclassOf(typeof(MulticastDelegate));
		}

		public static bool IsLegalExplicitVariantDelegateConversion(Type source, Type dest)
		{
			if (!TypeUtils.IsDelegate(source) || !TypeUtils.IsDelegate(dest) || !source.IsGenericType || !dest.IsGenericType)
			{
				return false;
			}
			Type genericTypeDefinition = source.GetGenericTypeDefinition();
			if (dest.GetGenericTypeDefinition() != genericTypeDefinition)
			{
				return false;
			}
			Type[] genericArguments = genericTypeDefinition.GetGenericArguments();
			Type[] genericArguments2 = source.GetGenericArguments();
			Type[] genericArguments3 = dest.GetGenericArguments();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				Type type = genericArguments2[i];
				Type type2 = genericArguments3[i];
				if (!TypeUtils.AreEquivalent(type, type2))
				{
					Type type3 = genericArguments[i];
					if (TypeUtils.IsInvariant(type3))
					{
						return false;
					}
					if (TypeUtils.IsCovariant(type3))
					{
						if (!type.HasReferenceConversionTo(type2))
						{
							return false;
						}
					}
					else if (TypeUtils.IsContravariant(type3) && (type.IsValueType || type2.IsValueType))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool IsConvertible(this Type type)
		{
			type = type.GetNonNullableType();
			if (type.IsEnum)
			{
				return true;
			}
			TypeCode typeCode = type.GetTypeCode();
			return typeCode - TypeCode.Boolean <= 11;
		}

		public static bool HasReferenceEquality(Type left, Type right)
		{
			return !left.IsValueType && !right.IsValueType && (left.IsInterface || right.IsInterface || TypeUtils.AreReferenceAssignable(left, right) || TypeUtils.AreReferenceAssignable(right, left));
		}

		public static bool HasBuiltInEqualityOperator(Type left, Type right)
		{
			if (left.IsInterface && !right.IsValueType)
			{
				return true;
			}
			if (right.IsInterface && !left.IsValueType)
			{
				return true;
			}
			if (!left.IsValueType && !right.IsValueType && (TypeUtils.AreReferenceAssignable(left, right) || TypeUtils.AreReferenceAssignable(right, left)))
			{
				return true;
			}
			if (!TypeUtils.AreEquivalent(left, right))
			{
				return false;
			}
			Type nonNullableType = left.GetNonNullableType();
			return nonNullableType == typeof(bool) || nonNullableType.IsNumeric() || nonNullableType.IsEnum;
		}

		public static bool IsImplicitlyConvertibleTo(this Type source, Type destination)
		{
			return TypeUtils.AreEquivalent(source, destination) || TypeUtils.IsImplicitNumericConversion(source, destination) || TypeUtils.IsImplicitReferenceConversion(source, destination) || TypeUtils.IsImplicitBoxingConversion(source, destination) || TypeUtils.IsImplicitNullableConversion(source, destination);
		}

		public static MethodInfo GetUserDefinedCoercionMethod(Type convertFrom, Type convertToType)
		{
			Type nonNullableType = convertFrom.GetNonNullableType();
			Type nonNullableType2 = convertToType.GetNonNullableType();
			MethodInfo[] methods = nonNullableType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo methodInfo = TypeUtils.FindConversionOperator(methods, convertFrom, convertToType);
			if (methodInfo != null)
			{
				return methodInfo;
			}
			MethodInfo[] methods2 = nonNullableType2.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			methodInfo = TypeUtils.FindConversionOperator(methods2, convertFrom, convertToType);
			if (methodInfo != null)
			{
				return methodInfo;
			}
			if (TypeUtils.AreEquivalent(nonNullableType, convertFrom) && TypeUtils.AreEquivalent(nonNullableType2, convertToType))
			{
				return null;
			}
			MethodInfo methodInfo2;
			if ((methodInfo2 = TypeUtils.FindConversionOperator(methods, nonNullableType, nonNullableType2)) == null && (methodInfo2 = TypeUtils.FindConversionOperator(methods2, nonNullableType, nonNullableType2)) == null)
			{
				methodInfo2 = TypeUtils.FindConversionOperator(methods, nonNullableType, convertToType) ?? TypeUtils.FindConversionOperator(methods2, nonNullableType, convertToType);
			}
			return methodInfo2;
		}

		private static MethodInfo FindConversionOperator(MethodInfo[] methods, Type typeFrom, Type typeTo)
		{
			foreach (MethodInfo methodInfo in methods)
			{
				if ((methodInfo.Name == "op_Implicit" || methodInfo.Name == "op_Explicit") && TypeUtils.AreEquivalent(methodInfo.ReturnType, typeTo))
				{
					ParameterInfo[] parametersCached = methodInfo.GetParametersCached();
					if (parametersCached.Length == 1 && TypeUtils.AreEquivalent(parametersCached[0].ParameterType, typeFrom))
					{
						return methodInfo;
					}
				}
			}
			return null;
		}

		private static bool IsImplicitNumericConversion(Type source, Type destination)
		{
			TypeCode typeCode = source.GetTypeCode();
			TypeCode typeCode2 = destination.GetTypeCode();
			switch (typeCode)
			{
			case TypeCode.Char:
				if (typeCode2 - TypeCode.UInt16 <= 7)
				{
					return true;
				}
				break;
			case TypeCode.SByte:
				switch (typeCode2)
				{
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.Byte:
				if (typeCode2 - TypeCode.Int16 <= 8)
				{
					return true;
				}
				break;
			case TypeCode.Int16:
				switch (typeCode2)
				{
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.UInt16:
				if (typeCode2 - TypeCode.Int32 <= 6)
				{
					return true;
				}
				break;
			case TypeCode.Int32:
				if (typeCode2 == TypeCode.Int64 || typeCode2 - TypeCode.Single <= 2)
				{
					return true;
				}
				break;
			case TypeCode.UInt32:
				if (typeCode2 - TypeCode.Int64 <= 4)
				{
					return true;
				}
				break;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				if (typeCode2 - TypeCode.Single <= 2)
				{
					return true;
				}
				break;
			case TypeCode.Single:
				return typeCode2 == TypeCode.Double;
			}
			return false;
		}

		private static bool IsImplicitReferenceConversion(Type source, Type destination)
		{
			return destination.IsAssignableFrom(source);
		}

		private static bool IsImplicitBoxingConversion(Type source, Type destination)
		{
			return (source.IsValueType && (destination == typeof(object) || destination == typeof(ValueType))) || (source.IsEnum && destination == typeof(Enum));
		}

		private static bool IsImplicitNullableConversion(Type source, Type destination)
		{
			return destination.IsNullableType() && source.GetNonNullableType().IsImplicitlyConvertibleTo(destination.GetNonNullableType());
		}

		public static Type FindGenericType(Type definition, Type type)
		{
			while (type != null && type != typeof(object))
			{
				if (type.IsConstructedGenericType && TypeUtils.AreEquivalent(type.GetGenericTypeDefinition(), definition))
				{
					return type;
				}
				if (definition.IsInterface)
				{
					foreach (Type type2 in type.GetTypeInfo().ImplementedInterfaces)
					{
						Type type3 = TypeUtils.FindGenericType(definition, type2);
						if (type3 != null)
						{
							return type3;
						}
					}
				}
				type = type.BaseType;
			}
			return null;
		}

		public static MethodInfo GetBooleanOperator(Type type, string name)
		{
			MethodInfo anyStaticMethodValidated;
			for (;;)
			{
				anyStaticMethodValidated = type.GetAnyStaticMethodValidated(name, new Type[] { type });
				if (anyStaticMethodValidated != null && anyStaticMethodValidated.IsSpecialName && !anyStaticMethodValidated.ContainsGenericParameters)
				{
					break;
				}
				type = type.BaseType;
				if (!(type != null))
				{
					goto Block_3;
				}
			}
			return anyStaticMethodValidated;
			Block_3:
			return null;
		}

		public static Type GetNonRefType(this Type type)
		{
			if (!type.IsByRef)
			{
				return type;
			}
			return type.GetElementType();
		}

		public static bool AreEquivalent(Type t1, Type t2)
		{
			return t1 != null && t1.IsEquivalentTo(t2);
		}

		public static bool AreReferenceAssignable(Type dest, Type src)
		{
			return TypeUtils.AreEquivalent(dest, src) || (!dest.IsValueType && !src.IsValueType && dest.IsAssignableFrom(src));
		}

		public static bool IsSameOrSubclass(Type type, Type subType)
		{
			return TypeUtils.AreEquivalent(type, subType) || subType.IsSubclassOf(type);
		}

		public static void ValidateType(Type type, string paramName)
		{
			TypeUtils.ValidateType(type, paramName, false, false);
		}

		public static void ValidateType(Type type, string paramName, bool allowByRef, bool allowPointer)
		{
			if (TypeUtils.ValidateType(type, paramName, -1))
			{
				if (!allowByRef && type.IsByRef)
				{
					throw global::System.Linq.Expressions.Error.TypeMustNotBeByRef(paramName);
				}
				if (!allowPointer && type.IsPointer)
				{
					throw global::System.Linq.Expressions.Error.TypeMustNotBePointer(paramName);
				}
			}
		}

		public static bool ValidateType(Type type, string paramName, int index)
		{
			if (type == typeof(void))
			{
				return false;
			}
			if (type.ContainsGenericParameters)
			{
				throw type.IsGenericTypeDefinition ? global::System.Linq.Expressions.Error.TypeIsGeneric(type, paramName, index) : global::System.Linq.Expressions.Error.TypeContainsGenericParameters(type, paramName, index);
			}
			return true;
		}

		public static MethodInfo GetInvokeMethod(this Type delegateType)
		{
			return delegateType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		internal static bool IsUnsigned(this Type type)
		{
			return type.GetNonNullableType().GetTypeCode().IsUnsigned();
		}

		internal static bool IsUnsigned(this TypeCode typeCode)
		{
			switch (typeCode)
			{
			case TypeCode.Char:
			case TypeCode.Byte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				return true;
			}
			return false;
		}

		internal static bool IsFloatingPoint(this Type type)
		{
			return type.GetNonNullableType().GetTypeCode().IsFloatingPoint();
		}

		internal static bool IsFloatingPoint(this TypeCode typeCode)
		{
			return typeCode - TypeCode.Single <= 1;
		}

		private static readonly Type[] s_arrayAssignableInterfaces = (from i in typeof(int[]).GetInterfaces()
			where i.IsGenericType
			select i.GetGenericTypeDefinition()).ToArray<Type>();
	}
}
