using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace YamlDotNet.Serialization.Utilities
{
	public static class TypeConverter
	{
		public static T ChangeType<T>(object value)
		{
			return (T)((object)TypeConverter.ChangeType(value, typeof(T)));
		}

		public static T ChangeType<T>(object value, IFormatProvider provider)
		{
			return (T)((object)TypeConverter.ChangeType(value, typeof(T), provider));
		}

		public static T ChangeType<T>(object value, CultureInfo culture)
		{
			return (T)((object)TypeConverter.ChangeType(value, typeof(T), culture));
		}

		public static object ChangeType(object value, Type destinationType)
		{
			return TypeConverter.ChangeType(value, destinationType, CultureInfo.InvariantCulture);
		}

		public static object ChangeType(object value, Type destinationType, IFormatProvider provider)
		{
			return TypeConverter.ChangeType(value, destinationType, new CultureInfoAdapter(CultureInfo.CurrentCulture, provider));
		}

		public static object ChangeType(object value, Type destinationType, CultureInfo culture)
		{
			if (value == null || value is DBNull)
			{
				if (!destinationType.IsValueType())
				{
					return null;
				}
				return Activator.CreateInstance(destinationType);
			}
			else
			{
				Type type = value.GetType();
				if (destinationType.IsAssignableFrom(type))
				{
					return value;
				}
				if (destinationType.IsGenericType() && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					Type type2 = destinationType.GetGenericArguments()[0];
					object obj = TypeConverter.ChangeType(value, type2, culture);
					return Activator.CreateInstance(destinationType, new object[] { obj });
				}
				if (destinationType.IsEnum())
				{
					string text = value as string;
					if (text == null)
					{
						return value;
					}
					return Enum.Parse(destinationType, text, true);
				}
				else
				{
					if (destinationType == typeof(bool))
					{
						if ("0".Equals(value))
						{
							return false;
						}
						if ("1".Equals(value))
						{
							return true;
						}
					}
					TypeConverter converter = TypeDescriptor.GetConverter(value);
					if (converter != null && converter.CanConvertTo(destinationType))
					{
						return converter.ConvertTo(null, culture, value, destinationType);
					}
					TypeConverter converter2 = TypeDescriptor.GetConverter(destinationType);
					if (converter2 != null && converter2.CanConvertFrom(type))
					{
						return converter2.ConvertFrom(null, culture, value);
					}
					Type[] array = new Type[] { type, destinationType };
					for (int i = 0; i < array.Length; i++)
					{
						foreach (MethodInfo methodInfo in array[i].GetPublicStaticMethods())
						{
							if (methodInfo.IsSpecialName && (methodInfo.Name == "op_Implicit" || methodInfo.Name == "op_Explicit") && destinationType.IsAssignableFrom(methodInfo.ReturnParameter.ParameterType))
							{
								ParameterInfo[] parameters = methodInfo.GetParameters();
								if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(type))
								{
									try
									{
										return methodInfo.Invoke(null, new object[] { value });
									}
									catch (TargetInvocationException ex)
									{
										throw ex.Unwrap();
									}
								}
							}
						}
					}
					if (type == typeof(string))
					{
						try
						{
							MethodInfo methodInfo2 = destinationType.GetPublicStaticMethod("Parse", new Type[]
							{
								typeof(string),
								typeof(IFormatProvider)
							});
							if (methodInfo2 != null)
							{
								return methodInfo2.Invoke(null, new object[] { value, culture });
							}
							methodInfo2 = destinationType.GetPublicStaticMethod("Parse", new Type[] { typeof(string) });
							if (methodInfo2 != null)
							{
								return methodInfo2.Invoke(null, new object[] { value });
							}
						}
						catch (TargetInvocationException ex2)
						{
							throw ex2.Unwrap();
						}
					}
					if (destinationType == typeof(TimeSpan))
					{
						return TimeSpan.Parse((string)TypeConverter.ChangeType(value, typeof(string), CultureInfo.InvariantCulture));
					}
					return Convert.ChangeType(value, destinationType, CultureInfo.InvariantCulture);
				}
			}
		}
	}
}
