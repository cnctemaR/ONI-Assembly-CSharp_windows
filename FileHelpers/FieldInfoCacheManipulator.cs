using System;
using System.Reflection;

namespace FileHelpers
{
	internal class FieldInfoCacheManipulator
	{
		public static void ResetFieldInfoCache(Type type)
		{
			if (FieldInfoCacheManipulator.mCacheProperty == null)
			{
				FieldInfoCacheManipulator.mCacheProperty = type.GetType().GetProperty("Cache", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
			}
			if (FieldInfoCacheManipulator.mCacheProperty != null)
			{
				object value = FieldInfoCacheManipulator.mCacheProperty.GetValue(type, null);
				FieldInfo field = value.GetType().GetField("m_fieldInfoCache", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (field != null)
				{
					field.SetValue(value, null);
				}
			}
		}

		private static PropertyInfo mCacheProperty;
	}
}
