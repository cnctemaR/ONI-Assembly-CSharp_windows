using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Xml.Linq
{
	public static class Extensions
	{
		public static IEnumerable<XAttribute> Attributes(this IEnumerable<XElement> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetAttributes(source, null);
		}

		public static IEnumerable<XAttribute> Attributes(this IEnumerable<XElement> source, XName name)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XAttribute.EmptySequence;
			}
			return Extensions.GetAttributes(source, name);
		}

		public static IEnumerable<XElement> Ancestors<T>(this IEnumerable<T> source) where T : XNode
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetAncestors<T>(source, null, false);
		}

		public static IEnumerable<XElement> Ancestors<T>(this IEnumerable<T> source, XName name) where T : XNode
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetAncestors<T>(source, name, false);
		}

		public static IEnumerable<XElement> AncestorsAndSelf(this IEnumerable<XElement> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetAncestors<XElement>(source, null, true);
		}

		public static IEnumerable<XElement> AncestorsAndSelf(this IEnumerable<XElement> source, XName name)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetAncestors<XElement>(source, name, true);
		}

		public static IEnumerable<XNode> Nodes<T>(this IEnumerable<T> source) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			foreach (T t in source)
			{
				XContainer root = t;
				if (root != null)
				{
					XNode i = root.LastNode;
					if (i != null)
					{
						do
						{
							i = i.next;
							yield return i;
						}
						while (i.parent == root && i != root.content);
					}
					i = null;
				}
				root = null;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		public static IEnumerable<XNode> DescendantNodes<T>(this IEnumerable<T> source) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetDescendantNodes<T>(source, false);
		}

		public static IEnumerable<XElement> Descendants<T>(this IEnumerable<T> source) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetDescendants<T>(source, null, false);
		}

		public static IEnumerable<XElement> Descendants<T>(this IEnumerable<T> source, XName name) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetDescendants<T>(source, name, false);
		}

		public static IEnumerable<XNode> DescendantNodesAndSelf(this IEnumerable<XElement> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetDescendantNodes<XElement>(source, true);
		}

		public static IEnumerable<XElement> DescendantsAndSelf(this IEnumerable<XElement> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetDescendants<XElement>(source, null, true);
		}

		public static IEnumerable<XElement> DescendantsAndSelf(this IEnumerable<XElement> source, XName name)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetDescendants<XElement>(source, name, true);
		}

		public static IEnumerable<XElement> Elements<T>(this IEnumerable<T> source) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetElements<T>(source, null);
		}

		public static IEnumerable<XElement> Elements<T>(this IEnumerable<T> source, XName name) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetElements<T>(source, name);
		}

		public static IEnumerable<T> InDocumentOrder<T>(this IEnumerable<T> source) where T : XNode
		{
			return source.OrderBy<T, XNode>((T n) => n, XNode.DocumentOrderComparer);
		}

		public static void Remove(this IEnumerable<XAttribute> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			foreach (XAttribute xattribute in new List<XAttribute>(source))
			{
				if (xattribute != null)
				{
					xattribute.Remove();
				}
			}
		}

		public static void Remove<T>(this IEnumerable<T> source) where T : XNode
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			foreach (T t in new List<T>(source))
			{
				if (t != null)
				{
					t.Remove();
				}
			}
		}

		private static IEnumerable<XAttribute> GetAttributes(IEnumerable<XElement> source, XName name)
		{
			foreach (XElement e in source)
			{
				if (e != null)
				{
					XAttribute a = e.lastAttr;
					if (a != null)
					{
						do
						{
							a = a.next;
							if (name == null || a.name == name)
							{
								yield return a;
							}
						}
						while (a.parent == e && a != e.lastAttr);
					}
					a = null;
				}
				e = null;
			}
			IEnumerator<XElement> enumerator = null;
			yield break;
			yield break;
		}

		private static IEnumerable<XElement> GetAncestors<T>(IEnumerable<T> source, XName name, bool self) where T : XNode
		{
			foreach (T t in source)
			{
				XNode xnode = t;
				if (xnode != null)
				{
					XElement e;
					for (e = (self ? xnode : xnode.parent) as XElement; e != null; e = e.parent as XElement)
					{
						if (name == null || e.name == name)
						{
							yield return e;
						}
					}
					e = null;
				}
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		private static IEnumerable<XNode> GetDescendantNodes<T>(IEnumerable<T> source, bool self) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer root = t;
				if (root != null)
				{
					if (self)
					{
						yield return root;
					}
					XNode i = root;
					for (;;)
					{
						XContainer xcontainer = i as XContainer;
						XNode firstNode;
						if (xcontainer != null && (firstNode = xcontainer.FirstNode) != null)
						{
							i = firstNode;
						}
						else
						{
							while (i != null && i != root && i == i.parent.content)
							{
								i = i.parent;
							}
							if (i == null || i == root)
							{
								break;
							}
							i = i.next;
						}
						yield return i;
					}
					i = null;
				}
				root = null;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		private static IEnumerable<XElement> GetDescendants<T>(IEnumerable<T> source, XName name, bool self) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer root = t;
				if (root != null)
				{
					if (self)
					{
						XElement xelement = (XElement)root;
						if (name == null || xelement.name == name)
						{
							yield return xelement;
						}
					}
					XNode i = root;
					XContainer xcontainer = root;
					for (;;)
					{
						if (xcontainer != null && xcontainer.content is XNode)
						{
							i = ((XNode)xcontainer.content).next;
						}
						else
						{
							while (i != null && i != root && i == i.parent.content)
							{
								i = i.parent;
							}
							if (i == null || i == root)
							{
								break;
							}
							i = i.next;
						}
						XElement e = i as XElement;
						if (e != null && (name == null || e.name == name))
						{
							yield return e;
						}
						xcontainer = e;
						e = null;
					}
					i = null;
				}
				root = null;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		private static IEnumerable<XElement> GetElements<T>(IEnumerable<T> source, XName name) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer root = t;
				if (root != null)
				{
					XNode i = root.content as XNode;
					if (i != null)
					{
						do
						{
							i = i.next;
							XElement xelement = i as XElement;
							if (xelement != null && (name == null || xelement.name == name))
							{
								yield return xelement;
							}
						}
						while (i.parent == root && i != root.content);
					}
					i = null;
				}
				root = null;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}
	}
}
