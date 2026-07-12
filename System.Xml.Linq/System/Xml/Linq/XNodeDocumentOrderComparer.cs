using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	public sealed class XNodeDocumentOrderComparer : IComparer, IComparer<XNode>
	{
		public int Compare(XNode x, XNode y)
		{
			return XNode.CompareDocumentOrder(x, y);
		}

		int IComparer.Compare(object x, object y)
		{
			XNode xnode = x as XNode;
			if (xnode == null && x != null)
			{
				throw new ArgumentException(global::SR.Format("The argument must be derived from {0}.", typeof(XNode)), "x");
			}
			XNode xnode2 = y as XNode;
			if (xnode2 == null && y != null)
			{
				throw new ArgumentException(global::SR.Format("The argument must be derived from {0}.", typeof(XNode)), "y");
			}
			return this.Compare(xnode, xnode2);
		}
	}
}
