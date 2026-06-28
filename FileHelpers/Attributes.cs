using System;
using System.Reflection;

namespace FileHelpers
{
	internal static class Attributes
	{
		public static T GetFirst<T>(MemberInfo type) where T : Attribute
		{
			return Attributes.GetFirstCore<T>(type, false);
		}

		public static T GetFirstInherited<T>(MemberInfo type) where T : Attribute
		{
			return Attributes.GetFirstCore<T>(type, true);
		}

		private static T GetFirstCore<T>(MemberInfo type, bool inherited) where T : Attribute
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(T), inherited);
			if (customAttributes.Length == 0)
			{
				return default(T);
			}
			return (T)((object)customAttributes[0]);
		}

		public static void WorkWithFirst<T>(MemberInfo type, Action<T> action) where T : Attribute
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(T), false);
			if (customAttributes.Length == 0)
			{
				return;
			}
			action((T)((object)customAttributes[0]));
		}
	}
}
