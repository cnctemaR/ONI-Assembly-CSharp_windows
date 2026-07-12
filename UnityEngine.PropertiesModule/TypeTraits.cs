using System;

namespace Unity.Properties
{
	public static class TypeTraits
	{
		public static bool IsContainer(Type type)
		{
			bool flag = null == type;
			if (flag)
			{
				throw new ArgumentNullException("type");
			}
			return !type.IsPrimitive && !type.IsPointer && !type.IsEnum && !(type == typeof(string));
		}
	}
}
