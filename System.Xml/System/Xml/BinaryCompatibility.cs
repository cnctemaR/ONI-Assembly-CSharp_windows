using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace System.Xml
{
	internal static class BinaryCompatibility
	{
		internal static bool TargetsAtLeast_Desktop_V4_5_2
		{
			get
			{
				return BinaryCompatibility._targetsAtLeast_Desktop_V4_5_2;
			}
		}

		[SecuritySafeCritical]
		[ReflectionPermission(SecurityAction.Assert, Unrestricted = true)]
		private static bool RunningOnCheck(string propertyName)
		{
			Type type;
			try
			{
				type = typeof(object).GetTypeInfo().Assembly.GetType("System.Runtime.Versioning.BinaryCompatibility", false);
			}
			catch (TypeLoadException)
			{
				return false;
			}
			if (type == null)
			{
				return false;
			}
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			return !(property == null) && (bool)property.GetValue(null);
		}

		private static bool _targetsAtLeast_Desktop_V4_5_2 = BinaryCompatibility.RunningOnCheck("TargetsAtLeast_Desktop_V4_5_2");
	}
}
