using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Runtime
{
	internal static class TypeHelper
	{
		public static bool AreTypesCompatible(object source, Type destinationType)
		{
			if (source == null)
			{
				return !destinationType.IsValueType || TypeHelper.IsNullableType(destinationType);
			}
			return TypeHelper.AreTypesCompatible(source.GetType(), destinationType);
		}

		public static bool AreTypesCompatible(Type sourceType, Type destinationType)
		{
			return sourceType == destinationType || TypeHelper.IsImplicitNumericConversion(sourceType, destinationType) || TypeHelper.IsImplicitReferenceConversion(sourceType, destinationType) || TypeHelper.IsImplicitBoxingConversion(sourceType, destinationType) || TypeHelper.IsImplicitNullableConversion(sourceType, destinationType);
		}

		public static bool AreReferenceTypesCompatible(Type sourceType, Type destinationType)
		{
			return sourceType == destinationType || TypeHelper.IsImplicitReferenceConversion(sourceType, destinationType);
		}

		public static IEnumerable<Type> GetCompatibleTypes(IEnumerable<Type> enumerable, Type targetType)
		{
			foreach (Type type in enumerable)
			{
				if (TypeHelper.AreTypesCompatible(type, targetType))
				{
					yield return type;
				}
			}
			IEnumerator<Type> enumerator = null;
			yield break;
			yield break;
		}

		public static bool ContainsCompatibleType(IEnumerable<Type> enumerable, Type targetType)
		{
			using (IEnumerator<Type> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (TypeHelper.AreTypesCompatible(enumerator.Current, targetType))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static T Convert<T>(object source)
		{
			if (source is T)
			{
				return (T)((object)source);
			}
			if (source == null)
			{
				if (typeof(T).IsValueType && !TypeHelper.IsNullableType(typeof(T)))
				{
					throw Fx.Exception.AsError(new InvalidCastException(InternalSR.CannotConvertObject(source, typeof(T))));
				}
				return default(T);
			}
			else
			{
				T t;
				if (TypeHelper.TryNumericConversion<T>(source, out t))
				{
					return t;
				}
				throw Fx.Exception.AsError(new InvalidCastException(InternalSR.CannotConvertObject(source, typeof(T))));
			}
		}

		public static IEnumerable<Type> GetImplementedTypes(Type type)
		{
			Dictionary<Type, object> dictionary = new Dictionary<Type, object>();
			TypeHelper.GetImplementedTypesHelper(type, dictionary);
			return dictionary.Keys;
		}

		private static void GetImplementedTypesHelper(Type type, Dictionary<Type, object> typesEncountered)
		{
			if (typesEncountered.ContainsKey(type))
			{
				return;
			}
			typesEncountered.Add(type, type);
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				TypeHelper.GetImplementedTypesHelper(interfaces[i], typesEncountered);
			}
			Type type2 = type.BaseType;
			while (type2 != null && type2 != TypeHelper.ObjectType)
			{
				TypeHelper.GetImplementedTypesHelper(type2, typesEncountered);
				type2 = type2.BaseType;
			}
		}

		private static bool IsImplicitNumericConversion(Type source, Type destination)
		{
			TypeCode typeCode = Type.GetTypeCode(source);
			TypeCode typeCode2 = Type.GetTypeCode(destination);
			switch (typeCode)
			{
			case TypeCode.Char:
				return typeCode2 - TypeCode.UInt16 <= 7;
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
				return false;
			case TypeCode.Byte:
				return typeCode2 - TypeCode.Int16 <= 8;
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
				return false;
			case TypeCode.UInt16:
				return typeCode2 - TypeCode.Int32 <= 6;
			case TypeCode.Int32:
				return typeCode2 == TypeCode.Int64 || typeCode2 - TypeCode.Single <= 2;
			case TypeCode.UInt32:
				return typeCode2 - TypeCode.UInt32 <= 5;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return typeCode2 - TypeCode.Single <= 2;
			case TypeCode.Single:
				return typeCode2 == TypeCode.Double;
			default:
				return false;
			}
		}

		private static bool IsImplicitReferenceConversion(Type sourceType, Type destinationType)
		{
			return destinationType.IsAssignableFrom(sourceType);
		}

		private static bool IsImplicitBoxingConversion(Type sourceType, Type destinationType)
		{
			return (sourceType.IsValueType && (destinationType == TypeHelper.ObjectType || destinationType == typeof(ValueType))) || (sourceType.IsEnum && destinationType == typeof(Enum));
		}

		private static bool IsImplicitNullableConversion(Type sourceType, Type destinationType)
		{
			if (!TypeHelper.IsNullableType(destinationType))
			{
				return false;
			}
			destinationType = destinationType.GetGenericArguments()[0];
			if (TypeHelper.IsNullableType(sourceType))
			{
				sourceType = sourceType.GetGenericArguments()[0];
			}
			return TypeHelper.AreTypesCompatible(sourceType, destinationType);
		}

		private static bool IsNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == TypeHelper.NullableType;
		}

		private static bool TryNumericConversion<T>(object source, out T result)
		{
			TypeCode typeCode = Type.GetTypeCode(source.GetType());
			TypeCode typeCode2 = Type.GetTypeCode(typeof(T));
			switch (typeCode)
			{
			case TypeCode.Char:
			{
				char c = (char)source;
				switch (typeCode2)
				{
				case TypeCode.UInt16:
					result = (T)((object)((ushort)c));
					return true;
				case TypeCode.Int32:
					result = (T)((object)((int)c));
					return true;
				case TypeCode.UInt32:
					result = (T)((object)((uint)c));
					return true;
				case TypeCode.Int64:
					result = (T)((object)((long)((ulong)c)));
					return true;
				case TypeCode.UInt64:
					result = (T)((object)((ulong)c));
					return true;
				case TypeCode.Single:
					result = (T)((object)((float)c));
					return true;
				case TypeCode.Double:
					result = (T)((object)((double)c));
					return true;
				case TypeCode.Decimal:
					result = (T)((object)c);
					return true;
				}
				break;
			}
			case TypeCode.SByte:
			{
				sbyte b = (sbyte)source;
				switch (typeCode2)
				{
				case TypeCode.Int16:
					result = (T)((object)((short)b));
					return true;
				case TypeCode.Int32:
					result = (T)((object)((int)b));
					return true;
				case TypeCode.Int64:
					result = (T)((object)((long)b));
					return true;
				case TypeCode.Single:
					result = (T)((object)((float)b));
					return true;
				case TypeCode.Double:
					result = (T)((object)((double)b));
					return true;
				case TypeCode.Decimal:
					result = (T)((object)b);
					return true;
				}
				break;
			}
			case TypeCode.Byte:
			{
				byte b2 = (byte)source;
				switch (typeCode2)
				{
				case TypeCode.Int16:
					result = (T)((object)((short)b2));
					return true;
				case TypeCode.UInt16:
					result = (T)((object)((ushort)b2));
					return true;
				case TypeCode.Int32:
					result = (T)((object)((int)b2));
					return true;
				case TypeCode.UInt32:
					result = (T)((object)((uint)b2));
					return true;
				case TypeCode.Int64:
					result = (T)((object)((long)((ulong)b2)));
					return true;
				case TypeCode.UInt64:
					result = (T)((object)((ulong)b2));
					return true;
				case TypeCode.Single:
					result = (T)((object)((float)b2));
					return true;
				case TypeCode.Double:
					result = (T)((object)((double)b2));
					return true;
				case TypeCode.Decimal:
					result = (T)((object)b2);
					return true;
				}
				break;
			}
			case TypeCode.Int16:
			{
				short num = (short)source;
				switch (typeCode2)
				{
				case TypeCode.Int32:
					result = (T)((object)((int)num));
					return true;
				case TypeCode.Int64:
					result = (T)((object)((long)num));
					return true;
				case TypeCode.Single:
					result = (T)((object)((float)num));
					return true;
				case TypeCode.Double:
					result = (T)((object)((double)num));
					return true;
				case TypeCode.Decimal:
					result = (T)((object)num);
					return true;
				}
				break;
			}
			case TypeCode.UInt16:
			{
				ushort num2 = (ushort)source;
				switch (typeCode2)
				{
				case TypeCode.Int32:
					result = (T)((object)((int)num2));
					return true;
				case TypeCode.UInt32:
					result = (T)((object)((uint)num2));
					return true;
				case TypeCode.Int64:
					result = (T)((object)((long)((ulong)num2)));
					return true;
				case TypeCode.UInt64:
					result = (T)((object)((ulong)num2));
					return true;
				case TypeCode.Single:
					result = (T)((object)((float)num2));
					return true;
				case TypeCode.Double:
					result = (T)((object)((double)num2));
					return true;
				case TypeCode.Decimal:
					result = (T)((object)num2);
					return true;
				}
				break;
			}
			case TypeCode.Int32:
			{
				int num3 = (int)source;
				switch (typeCode2)
				{
				case TypeCode.Int64:
					result = (T)((object)((long)num3));
					return true;
				case TypeCode.Single:
					result = (T)((object)((float)num3));
					return true;
				case TypeCode.Double:
					result = (T)((object)((double)num3));
					return true;
				case TypeCode.Decimal:
					result = (T)((object)num3);
					return true;
				}
				break;
			}
			case TypeCode.UInt32:
			{
				uint num4 = (uint)source;
				switch (typeCode2)
				{
				case TypeCode.UInt32:
					result = (T)((object)num4);
					return true;
				case TypeCode.Int64:
					result = (T)((object)((long)((ulong)num4)));
					return true;
				case TypeCode.UInt64:
					result = (T)((object)((ulong)num4));
					return true;
				case TypeCode.Single:
					result = (T)((object)num4);
					return true;
				case TypeCode.Double:
					result = (T)((object)num4);
					return true;
				case TypeCode.Decimal:
					result = (T)((object)num4);
					return true;
				}
				break;
			}
			case TypeCode.Int64:
			{
				long num5 = (long)source;
				switch (typeCode2)
				{
				case TypeCode.Single:
					result = (T)((object)((float)num5));
					return true;
				case TypeCode.Double:
					result = (T)((object)((double)num5));
					return true;
				case TypeCode.Decimal:
					result = (T)((object)num5);
					return true;
				}
				break;
			}
			case TypeCode.UInt64:
			{
				ulong num6 = (ulong)source;
				switch (typeCode2)
				{
				case TypeCode.Single:
					result = (T)((object)num6);
					return true;
				case TypeCode.Double:
					result = (T)((object)num6);
					return true;
				case TypeCode.Decimal:
					result = (T)((object)num6);
					return true;
				}
				break;
			}
			case TypeCode.Single:
				if (typeCode2 == TypeCode.Double)
				{
					result = (T)((object)((double)((float)source)));
					return true;
				}
				break;
			}
			result = default(T);
			return false;
		}

		public static object GetDefaultValueForType(Type type)
		{
			if (!type.IsValueType)
			{
				return null;
			}
			if (type.IsEnum)
			{
				Array values = Enum.GetValues(type);
				if (values.Length > 0)
				{
					return values.GetValue(0);
				}
			}
			return Activator.CreateInstance(type);
		}

		public static bool IsNullableValueType(Type type)
		{
			return type.IsValueType && TypeHelper.IsNullableType(type);
		}

		public static bool IsNonNullableValueType(Type type)
		{
			return type.IsValueType && !type.IsGenericType && type != TypeHelper.StringType;
		}

		public static bool ShouldFilterProperty(PropertyDescriptor property, Attribute[] attributes)
		{
			if (attributes == null || attributes.Length == 0)
			{
				return false;
			}
			foreach (Attribute attribute in attributes)
			{
				Attribute attribute2 = property.Attributes[attribute.GetType()];
				if (attribute2 == null)
				{
					if (!attribute.IsDefaultAttribute())
					{
						return true;
					}
				}
				else if (!attribute.Match(attribute2))
				{
					return true;
				}
			}
			return false;
		}

		public static readonly Type ArrayType = typeof(Array);

		public static readonly Type BoolType = typeof(bool);

		public static readonly Type GenericCollectionType = typeof(ICollection<>);

		public static readonly Type ByteType = typeof(byte);

		public static readonly Type SByteType = typeof(sbyte);

		public static readonly Type CharType = typeof(char);

		public static readonly Type ShortType = typeof(short);

		public static readonly Type UShortType = typeof(ushort);

		public static readonly Type IntType = typeof(int);

		public static readonly Type UIntType = typeof(uint);

		public static readonly Type LongType = typeof(long);

		public static readonly Type ULongType = typeof(ulong);

		public static readonly Type FloatType = typeof(float);

		public static readonly Type DoubleType = typeof(double);

		public static readonly Type DecimalType = typeof(decimal);

		public static readonly Type ExceptionType = typeof(Exception);

		public static readonly Type NullableType = typeof(Nullable<>);

		public static readonly Type ObjectType = typeof(object);

		public static readonly Type StringType = typeof(string);

		public static readonly Type TypeType = typeof(Type);

		public static readonly Type VoidType = typeof(void);
	}
}
