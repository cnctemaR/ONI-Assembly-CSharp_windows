using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class PreSiblingQuery : CacheAxisQuery
	{
		public PreSiblingQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest)
			: base(qyInput, name, prefix, typeTest)
		{
		}

		protected PreSiblingQuery(PreSiblingQuery other)
			: base(other)
		{
		}

		private bool NotVisited(XPathNavigator nav, List<XPathNavigator> parentStk)
		{
			XPathNavigator xpathNavigator = nav.Clone();
			xpathNavigator.MoveToParent();
			for (int i = 0; i < parentStk.Count; i++)
			{
				if (xpathNavigator.IsSamePosition(parentStk[i]))
				{
					return false;
				}
			}
			parentStk.Add(xpathNavigator);
			return true;
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			base.Evaluate(context);
			List<XPathNavigator> list = new List<XPathNavigator>();
			Stack<XPathNavigator> stack = new Stack<XPathNavigator>();
			while ((this.currentNode = this.qyInput.Advance()) != null)
			{
				stack.Push(this.currentNode.Clone());
			}
			while (stack.Count != 0)
			{
				XPathNavigator xpathNavigator = stack.Pop();
				if (xpathNavigator.NodeType != XPathNodeType.Attribute && xpathNavigator.NodeType != XPathNodeType.Namespace && this.NotVisited(xpathNavigator, list))
				{
					XPathNavigator xpathNavigator2 = xpathNavigator.Clone();
					if (xpathNavigator2.MoveToParent())
					{
						xpathNavigator2.MoveToFirstChild();
						while (!xpathNavigator2.IsSamePosition(xpathNavigator))
						{
							if (this.matches(xpathNavigator2))
							{
								base.Insert(this.outputBuffer, xpathNavigator2);
							}
							if (!xpathNavigator2.MoveToNext())
							{
								break;
							}
						}
					}
				}
			}
			return this;
		}

		public override XPathNodeIterator Clone()
		{
			return new PreSiblingQuery(this);
		}

		public override QueryProps Properties
		{
			get
			{
				return base.Properties | QueryProps.Reverse;
			}
		}
	}
}
