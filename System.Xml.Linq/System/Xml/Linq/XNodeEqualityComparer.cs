using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	public sealed class XNodeEqualityComparer : IEqualityComparer, IEqualityComparer<XNode>
	{
		public bool Equals(XNode x, XNode y)
		{
			return XNode.DeepEquals(x, y);
		}

		public int GetHashCode(XNode obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.GetDeepHashCode();
		}

		bool IEqualityComparer.Equals(object x, object y)
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
			return this.Equals(xnode, xnode2);
		}

		int IEqualityComparer.GetHashCode(object obj)
		{
			XNode xnode = obj as XNode;
			if (xnode == null && obj != null)
			{
				throw new ArgumentException(global::SR.Format("The argument must be derived from {0}.", typeof(XNode)), "obj");
			}
			return this.GetHashCode(xnode);
		}
	}
}
