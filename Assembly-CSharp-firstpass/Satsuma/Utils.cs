using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Satsuma
{
	internal static class Utils
	{
		public static double LargestPowerOfTwo(double d)
		{
			long num = BitConverter.DoubleToInt64Bits(d);
			num &= 9218868437227405312L;
			if (num == 9218868437227405312L)
			{
				num = 9214364837600034816L;
			}
			return BitConverter.Int64BitsToDouble(num);
		}

		public static V MakeEntry<K, V>(Dictionary<K, V> dict, K key) where V : new()
		{
			V v;
			if (dict.TryGetValue(key, out v))
			{
				return v;
			}
			return dict[key] = new V();
		}

		public static void RemoveAll<T>(HashSet<T> set, Func<T, bool> condition)
		{
			foreach (T t in set.Where<T>(condition).ToList<T>())
			{
				set.Remove(t);
			}
		}

		public static void RemoveAll<K, V>(Dictionary<K, V> dict, Func<K, bool> condition)
		{
			foreach (K k in dict.Keys.Where<K>(condition).ToList<K>())
			{
				dict.Remove(k);
			}
		}

		public static void RemoveLast<T>(List<T> list, T element) where T : IEquatable<T>
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (element.Equals(list[i]))
				{
					list.RemoveAt(i);
					return;
				}
			}
		}

		public static IEnumerable<XElement> ElementsLocal(XElement xParent, string localName)
		{
			return from x in xParent.Elements()
				where x.Name.LocalName == localName
				select x;
		}

		public static XElement ElementLocal(XElement xParent, string localName)
		{
			return Utils.ElementsLocal(xParent, localName).FirstOrDefault<XElement>();
		}
	}
}
