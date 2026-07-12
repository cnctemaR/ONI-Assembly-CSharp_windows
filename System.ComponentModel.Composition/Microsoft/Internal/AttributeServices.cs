using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Internal
{
	internal static class AttributeServices
	{
		public static T[] GetAttributes<T>(this ICustomAttributeProvider attributeProvider) where T : class
		{
			return (T[])attributeProvider.GetCustomAttributes(typeof(T), false);
		}

		public static T[] GetAttributes<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class
		{
			return (T[])attributeProvider.GetCustomAttributes(typeof(T), inherit);
		}

		public static T GetFirstAttribute<T>(this ICustomAttributeProvider attributeProvider) where T : class
		{
			return attributeProvider.GetAttributes<T>().FirstOrDefault<T>();
		}

		public static T GetFirstAttribute<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class
		{
			return attributeProvider.GetAttributes<T>(inherit).FirstOrDefault<T>();
		}

		public static bool IsAttributeDefined<T>(this ICustomAttributeProvider attributeProvider) where T : class
		{
			return attributeProvider.IsDefined(typeof(T), false);
		}

		public static bool IsAttributeDefined<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class
		{
			return attributeProvider.IsDefined(typeof(T), inherit);
		}
	}
}
