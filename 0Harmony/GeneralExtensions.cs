using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Harmony
{
	public static class GeneralExtensions
	{
		public static string Join<T>(this IEnumerable<T> enumeration, Func<T, string> converter = null, string delimiter = ", ")
		{
			bool flag = converter == null;
			if (flag)
			{
				converter = (T t) => t.ToString();
			}
			return enumeration.Aggregate("", (string prev, T curr) => prev + ((prev != "") ? delimiter : "") + converter(curr));
		}

		public static string Description(this Type[] parameters)
		{
			bool flag = parameters == null;
			string text;
			if (flag)
			{
				text = "NULL";
			}
			else
			{
				string pattern = ", \\w+, Version=[0-9.]+, Culture=neutral, PublicKeyToken=[0-9a-f]+";
				text = "(" + parameters.Join<Type>((Type p) => (p == null || p.FullName == null) ? "null" : Regex.Replace(p.FullName, pattern, ""), ", ") + ")";
			}
			return text;
		}

		public static string FullDescription(this MethodBase method)
		{
			Type[] array = (from p in method.GetParameters()
				select p.ParameterType).ToArray<Type>();
			return method.DeclaringType.FullName + "." + method.Name + array.Description();
		}

		public static Type[] Types(this ParameterInfo[] pinfo)
		{
			return pinfo.Select<ParameterInfo, Type>((ParameterInfo pi) => pi.ParameterType).ToArray<Type>();
		}

		public static T GetValueSafe<S, T>(this Dictionary<S, T> dictionary, S key)
		{
			T t;
			bool flag = dictionary.TryGetValue(key, out t);
			T t2;
			if (flag)
			{
				t2 = t;
			}
			else
			{
				t2 = default(T);
			}
			return t2;
		}

		public static T GetTypedValue<T>(this Dictionary<string, object> dictionary, string key)
		{
			object obj;
			bool flag = dictionary.TryGetValue(key, out obj);
			if (flag)
			{
				bool flag2 = obj is T;
				if (flag2)
				{
					return (T)((object)obj);
				}
			}
			return default(T);
		}
	}
}
