using System;
using System.Reflection;

namespace System.Runtime.Serialization
{
	internal static class CodeInterpreter
	{
		internal static object ConvertValue(object arg, Type source, Type target)
		{
			return CodeInterpreter.InternalConvert(arg, source, target, false);
		}

		private static bool CanConvert(TypeCode typeCode)
		{
			return typeCode - TypeCode.Boolean <= 11;
		}

		private static object InternalConvert(object arg, Type source, Type target, bool isAddress)
		{
			if (target == source)
			{
				return arg;
			}
			if (target.IsValueType)
			{
				if (source.IsValueType)
				{
					if (!CodeInterpreter.CanConvert(Type.GetTypeCode(target)))
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("An internal error has occurred. No conversion is possible to '{0}' - error generating code for serialization.", new object[] { DataContract.GetClrTypeFullName(target) })));
					}
					return target;
				}
				else
				{
					if (source.IsAssignableFrom(target))
					{
						return arg;
					}
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("An internal error has occurred. '{0}' is not assignable from '{1}' - error generating code for serialization.", new object[]
					{
						DataContract.GetClrTypeFullName(target),
						DataContract.GetClrTypeFullName(source)
					})));
				}
			}
			else
			{
				if (target.IsAssignableFrom(source))
				{
					return arg;
				}
				if (source.IsAssignableFrom(target))
				{
					return arg;
				}
				if (target.IsInterface || source.IsInterface)
				{
					return arg;
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("An internal error has occurred. '{0}' is not assignable from '{1}' - error generating code for serialization.", new object[]
				{
					DataContract.GetClrTypeFullName(target),
					DataContract.GetClrTypeFullName(source)
				})));
			}
		}

		public static object GetMember(MemberInfo memberInfo, object instance)
		{
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				return propertyInfo.GetValue(instance);
			}
			return ((FieldInfo)memberInfo).GetValue(instance);
		}

		public static void SetMember(MemberInfo memberInfo, object instance, object value)
		{
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				propertyInfo.SetValue(instance, value);
				return;
			}
			((FieldInfo)memberInfo).SetValue(instance, value);
		}
	}
}
