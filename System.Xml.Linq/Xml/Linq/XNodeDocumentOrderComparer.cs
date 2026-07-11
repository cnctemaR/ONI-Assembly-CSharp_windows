using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	public sealed class XNodeDocumentOrderComparer : IComparer, IComparer<XNode>
	{
		int IComparer.Compare(object n1, object n2)
		{
			return this.Compare((XNode)n1, (XNode)n2);
		}

		public int Compare(XNode n1, XNode n2)
		{
			switch (this.CompareCore(n1, n2))
			{
			case XNodeDocumentOrderComparer.CompareResult.Same:
				return 0;
			case XNodeDocumentOrderComparer.CompareResult.Random:
				return (DateTime.Now.Ticks % 2L != 1L) ? (-1) : 1;
			case XNodeDocumentOrderComparer.CompareResult.Parent:
			case XNodeDocumentOrderComparer.CompareResult.Ancestor:
			case XNodeDocumentOrderComparer.CompareResult.Preceding:
				return 1;
			}
			return -1;
		}

		private XNodeDocumentOrderComparer.CompareResult CompareCore(XNode n1, XNode n2)
		{
			if (n1 == n2)
			{
				return XNodeDocumentOrderComparer.CompareResult.Same;
			}
			if (n1.Owner != null)
			{
				if (n2.Owner == null)
				{
					XNodeDocumentOrderComparer.CompareResult compareResult = this.CompareCore(n2, n1);
					switch (compareResult)
					{
					case XNodeDocumentOrderComparer.CompareResult.Same:
					case XNodeDocumentOrderComparer.CompareResult.Random:
						return compareResult;
					case XNodeDocumentOrderComparer.CompareResult.Parent:
						return XNodeDocumentOrderComparer.CompareResult.Child;
					case XNodeDocumentOrderComparer.CompareResult.Child:
						return XNodeDocumentOrderComparer.CompareResult.Parent;
					case XNodeDocumentOrderComparer.CompareResult.Ancestor:
						return XNodeDocumentOrderComparer.CompareResult.Descendant;
					case XNodeDocumentOrderComparer.CompareResult.Descendant:
						return XNodeDocumentOrderComparer.CompareResult.Ancestor;
					case XNodeDocumentOrderComparer.CompareResult.Preceding:
						return XNodeDocumentOrderComparer.CompareResult.Following;
					case XNodeDocumentOrderComparer.CompareResult.Following:
						return XNodeDocumentOrderComparer.CompareResult.Preceding;
					}
				}
				XNodeDocumentOrderComparer.CompareResult compareResult2 = this.CompareCore(n1.Owner, n2.Owner);
				switch (compareResult2)
				{
				case XNodeDocumentOrderComparer.CompareResult.Same:
					return this.CompareSibling(n1, n2, XNodeDocumentOrderComparer.CompareResult.Same);
				case XNodeDocumentOrderComparer.CompareResult.Parent:
					return this.CompareSibling(n1.Owner, n2, XNodeDocumentOrderComparer.CompareResult.Parent);
				case XNodeDocumentOrderComparer.CompareResult.Child:
					return this.CompareSibling(n1, n2.Owner, XNodeDocumentOrderComparer.CompareResult.Child);
				case XNodeDocumentOrderComparer.CompareResult.Ancestor:
				{
					XNode xnode = n1;
					while (xnode.Owner != n2.Owner)
					{
						xnode = xnode.Owner;
					}
					return this.CompareSibling(xnode, n2, XNodeDocumentOrderComparer.CompareResult.Ancestor);
				}
				case XNodeDocumentOrderComparer.CompareResult.Descendant:
				{
					XNode xnode2 = n2;
					while (xnode2.Owner != n1.Owner)
					{
						xnode2 = xnode2.Owner;
					}
					return this.CompareSibling(n1, xnode2, XNodeDocumentOrderComparer.CompareResult.Descendant);
				}
				}
				return compareResult2;
			}
			if (n2.Owner == null)
			{
				return XNodeDocumentOrderComparer.CompareResult.Random;
			}
			XNodeDocumentOrderComparer.CompareResult compareResult3 = this.CompareCore(n1, n2.Owner);
			switch (compareResult3)
			{
			case XNodeDocumentOrderComparer.CompareResult.Same:
				return XNodeDocumentOrderComparer.CompareResult.Child;
			case XNodeDocumentOrderComparer.CompareResult.Parent:
			case XNodeDocumentOrderComparer.CompareResult.Ancestor:
				throw new Exception("INTERNAL ERROR: should not happen");
			case XNodeDocumentOrderComparer.CompareResult.Child:
			case XNodeDocumentOrderComparer.CompareResult.Descendant:
				return XNodeDocumentOrderComparer.CompareResult.Descendant;
			}
			return compareResult3;
		}

		private XNodeDocumentOrderComparer.CompareResult CompareSibling(XNode n1, XNode n2, XNodeDocumentOrderComparer.CompareResult forSameValue)
		{
			if (n1 == n2)
			{
				return forSameValue;
			}
			for (XNode xnode = n1.NextNode; xnode != null; xnode = xnode.NextNode)
			{
				if (xnode == n2)
				{
					return XNodeDocumentOrderComparer.CompareResult.Following;
				}
			}
			return XNodeDocumentOrderComparer.CompareResult.Preceding;
		}

		private enum CompareResult
		{
			Same,
			Random,
			Parent,
			Child,
			Ancestor,
			Descendant,
			Preceding,
			Following
		}
	}
}
