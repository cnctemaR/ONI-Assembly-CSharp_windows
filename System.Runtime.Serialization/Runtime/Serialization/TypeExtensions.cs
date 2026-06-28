using System;

namespace System.Runtime.Serialization
{
	internal static class TypeExtensions
	{
		public static T GetCustomAttribute<T>(this Type type, bool inherit)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(T), inherit);
			return (customAttributes == null || customAttributes.Length != 1) ? default(T) : ((T)((object)customAttributes[0]));
		}
	}
}
