using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TiledSharp
{
	public class TmxList<T> : KeyedCollection<string, T> where T : ITmxElement
	{
		public new void Add(T t)
		{
			Tuple<TmxList<T>, string> tuple = Tuple.Create<TmxList<T>, string>(this, t.Name);
			if (base.Contains(t.Name))
			{
				Dictionary<Tuple<TmxList<T>, string>, int> dictionary;
				Tuple<TmxList<T>, string> tuple2;
				(dictionary = TmxList<T>.nameCount)[tuple2 = tuple] = dictionary[tuple2] + 1;
			}
			else
			{
				TmxList<T>.nameCount.Add(tuple, 0);
			}
			base.Add(t);
		}

		protected override string GetKeyForItem(T t)
		{
			Tuple<TmxList<T>, string> tuple = Tuple.Create<TmxList<T>, string>(this, t.Name);
			int num = TmxList<T>.nameCount[tuple];
			int num2 = 0;
			string text = t.Name;
			while (base.Contains(text))
			{
				text = t.Name + string.Concat(Enumerable.Repeat<string>("_", num2)) + num;
				num2++;
			}
			return text;
		}

		public static Dictionary<Tuple<TmxList<T>, string>, int> nameCount = new Dictionary<Tuple<TmxList<T>, string>, int>();
	}
}
