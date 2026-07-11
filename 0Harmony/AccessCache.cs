using System;
using System.Collections.Generic;
using System.Reflection;

namespace Harmony
{
	public class AccessCache
	{
		[UpgradeToLatestVersion(1)]
		public FieldInfo GetFieldInfo(Type type, string name)
		{
			Dictionary<string, FieldInfo> dictionary = null;
			bool flag = !this.fields.TryGetValue(type, out dictionary);
			if (flag)
			{
				dictionary = new Dictionary<string, FieldInfo>();
				this.fields.Add(type, dictionary);
			}
			FieldInfo fieldInfo = null;
			bool flag2 = !dictionary.TryGetValue(name, out fieldInfo);
			if (flag2)
			{
				fieldInfo = AccessTools.Field(type, name);
				dictionary.Add(name, fieldInfo);
			}
			return fieldInfo;
		}

		public PropertyInfo GetPropertyInfo(Type type, string name)
		{
			Dictionary<string, PropertyInfo> dictionary = null;
			bool flag = !this.properties.TryGetValue(type, out dictionary);
			if (flag)
			{
				dictionary = new Dictionary<string, PropertyInfo>();
				this.properties.Add(type, dictionary);
			}
			PropertyInfo propertyInfo = null;
			bool flag2 = !dictionary.TryGetValue(name, out propertyInfo);
			if (flag2)
			{
				propertyInfo = AccessTools.Property(type, name);
				dictionary.Add(name, propertyInfo);
			}
			return propertyInfo;
		}

		private static int CombinedHashCode(IEnumerable<object> objects)
		{
			int num = 352654597;
			int num2 = num;
			int num3 = 0;
			foreach (object obj in objects)
			{
				bool flag = num3 % 2 == 0;
				if (flag)
				{
					num = ((num << 5) + num + (num >> 27)) ^ obj.GetHashCode();
				}
				else
				{
					num2 = ((num2 << 5) + num2 + (num2 >> 27)) ^ obj.GetHashCode();
				}
				num3++;
			}
			return num + num2 * 1566083941;
		}

		public MethodBase GetMethodInfo(Type type, string name, Type[] arguments)
		{
			Dictionary<string, Dictionary<int, MethodBase>> dictionary = null;
			this.methods.TryGetValue(type, out dictionary);
			bool flag = dictionary == null;
			if (flag)
			{
				dictionary = new Dictionary<string, Dictionary<int, MethodBase>>();
				this.methods.Add(type, dictionary);
			}
			Dictionary<int, MethodBase> dictionary2 = null;
			dictionary.TryGetValue(name, out dictionary2);
			bool flag2 = dictionary2 == null;
			if (flag2)
			{
				dictionary2 = new Dictionary<int, MethodBase>();
				dictionary.Add(name, dictionary2);
			}
			MethodBase methodBase = null;
			int num = AccessCache.CombinedHashCode(arguments);
			dictionary2.TryGetValue(num, out methodBase);
			bool flag3 = methodBase == null;
			if (flag3)
			{
				methodBase = AccessTools.Method(type, name, arguments, null);
				dictionary2.Add(num, methodBase);
			}
			return methodBase;
		}

		private Dictionary<Type, Dictionary<string, FieldInfo>> fields = new Dictionary<Type, Dictionary<string, FieldInfo>>();

		private Dictionary<Type, Dictionary<string, PropertyInfo>> properties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

		private readonly Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>> methods = new Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>>();
	}
}
