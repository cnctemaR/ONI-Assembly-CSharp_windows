using System;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;

namespace System.Security.Authentication.ExtendedProtection.Configuration
{
	internal static class ConfigUtil
	{
		internal static T GetCustomAttribute<T>(MemberInfo m, bool inherit)
		{
			object[] customAttributes = m.GetCustomAttributes(typeof(T), false);
			if (customAttributes.Length == 0)
			{
				return default(T);
			}
			return (T)((object)customAttributes[0]);
		}

		internal static ConfigurationProperty BuildProperty(Type t, string name)
		{
			PropertyInfo property = t.GetProperty(name);
			ConfigurationPropertyAttribute customAttribute = ConfigUtil.GetCustomAttribute<ConfigurationPropertyAttribute>(property, false);
			TypeConverterAttribute customAttribute2 = ConfigUtil.GetCustomAttribute<TypeConverterAttribute>(property, false);
			ConfigurationValidatorAttribute customAttribute3 = ConfigUtil.GetCustomAttribute<ConfigurationValidatorAttribute>(property, false);
			return new ConfigurationProperty(customAttribute.Name, property.PropertyType, customAttribute.DefaultValue, (customAttribute2 != null) ? ((TypeConverter)Activator.CreateInstance(Type.GetType(customAttribute2.ConverterTypeName))) : null, (customAttribute3 != null) ? customAttribute3.ValidatorInstance : null, customAttribute.Options);
		}
	}
}
