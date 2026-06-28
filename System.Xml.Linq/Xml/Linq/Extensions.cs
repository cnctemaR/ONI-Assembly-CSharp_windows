using System;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	public static class Extensions
	{
		public static IEnumerable<XElement> Ancestors<T>(this IEnumerable<T> source) where T : XNode
		{
			foreach (T item in source)
			{
				for (XElement i = item.Parent; i != null; i = i.Parent)
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<XElement> Ancestors<T>(this IEnumerable<T> source, XName name) where T : XNode
		{
			foreach (T item in source)
			{
				for (XElement i = item.Parent; i != null; i = i.Parent)
				{
					if (i.Name == name)
					{
						yield return i;
					}
				}
			}
			yield break;
		}

		public static IEnumerable<XElement> AncestorsAndSelf(this IEnumerable<XElement> source)
		{
			foreach (XElement item in source)
			{
				for (XElement i = item; i != null; i = i.Parent)
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<XElement> AncestorsAndSelf(this IEnumerable<XElement> source, XName name)
		{
			foreach (XElement item in source)
			{
				for (XElement i = item; i != null; i = i.Parent)
				{
					if (i.Name == name)
					{
						yield return i;
					}
				}
			}
			yield break;
		}

		public static IEnumerable<XAttribute> Attributes(this IEnumerable<XElement> source)
		{
			foreach (XElement item in source)
			{
				foreach (XAttribute attr in item.Attributes())
				{
					yield return attr;
				}
			}
			yield break;
		}

		public static IEnumerable<XAttribute> Attributes(this IEnumerable<XElement> source, XName name)
		{
			foreach (XElement item in source)
			{
				foreach (XAttribute attr in item.Attributes(name))
				{
					yield return attr;
				}
			}
			yield break;
		}

		public static IEnumerable<XNode> DescendantNodes<T>(this IEnumerable<T> source) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer item = t;
				foreach (XNode i in item.DescendantNodes())
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<XNode> DescendantNodesAndSelf(this IEnumerable<XElement> source)
		{
			foreach (XElement item in source)
			{
				foreach (XNode i in item.DescendantNodesAndSelf())
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<XElement> Descendants<T>(this IEnumerable<T> source) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer item = t;
				foreach (XElement i in item.Descendants())
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<XElement> Descendants<T>(this IEnumerable<T> source, XName name) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer item = t;
				foreach (XElement i in item.Descendants(name))
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<XElement> DescendantsAndSelf(this IEnumerable<XElement> source)
		{
			foreach (XElement item in source)
			{
				foreach (XElement i in item.DescendantsAndSelf())
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<XElement> DescendantsAndSelf(this IEnumerable<XElement> source, XName name)
		{
			foreach (XElement item in source)
			{
				foreach (XElement i in item.DescendantsAndSelf(name))
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<XElement> Elements<T>(this IEnumerable<T> source) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer item = t;
				foreach (XElement i in item.Elements())
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<XElement> Elements<T>(this IEnumerable<T> source, XName name) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer item = t;
				foreach (XElement i in item.Elements(name))
				{
					yield return i;
				}
			}
			yield break;
		}

		public static IEnumerable<T> InDocumentOrder<T>(this IEnumerable<T> source) where T : XNode
		{
			List<XNode> list = new List<XNode>();
			foreach (T t in source)
			{
				XNode i = t;
				list.Add(i);
			}
			list.Sort(XNode.DocumentOrderComparer);
			foreach (XNode xnode in list)
			{
				T j = (T)((object)xnode);
				yield return j;
			}
			yield break;
		}

		public static IEnumerable<XNode> Nodes<T>(this IEnumerable<T> source) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer item = t;
				foreach (XNode i in item.Nodes())
				{
					yield return i;
				}
			}
			yield break;
		}

		public static void Remove(this IEnumerable<XAttribute> source)
		{
			foreach (XAttribute xattribute in source)
			{
				xattribute.Remove();
			}
		}

		public static void Remove<T>(this IEnumerable<T> source) where T : XNode
		{
			foreach (T t in source)
			{
				t.Remove();
			}
		}
	}
}
