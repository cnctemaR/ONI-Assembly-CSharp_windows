using System;
using System.Reflection;

namespace System.Xml.Linq
{
	internal static class XHelper
	{
		internal static bool IsInstanceOfType(object o, Type type)
		{
			return o != null && type.GetTypeInfo().IsAssignableFrom(o.GetType().GetTypeInfo());
		}
	}
}
