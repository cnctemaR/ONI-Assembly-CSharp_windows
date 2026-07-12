using System;
using System.Reflection;

namespace Unity.Properties.Internal
{
	internal static class ReflectionUtilities
	{
		public static string SanitizeMemberName(MemberInfo info)
		{
			return info.Name.Replace(".", "_").Replace("<", "_").Replace(">", "_")
				.Replace("+", "_");
		}
	}
}
