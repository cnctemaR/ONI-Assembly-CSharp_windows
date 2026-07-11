using System;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class IDQuery : CacheOutputQuery
	{
		public IDQuery(Query arg)
			: base(arg)
		{
		}

		private IDQuery(IDQuery other)
			: base(other)
		{
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			object obj = base.Evaluate(context);
			XPathNavigator xpathNavigator = context.Current.Clone();
			switch (base.GetXPathType(obj))
			{
			case XPathResultType.Number:
				this.ProcessIds(xpathNavigator, StringFunctions.toString((double)obj));
				break;
			case XPathResultType.String:
				this.ProcessIds(xpathNavigator, (string)obj);
				break;
			case XPathResultType.Boolean:
				this.ProcessIds(xpathNavigator, StringFunctions.toString((bool)obj));
				break;
			case XPathResultType.NodeSet:
			{
				XPathNavigator xpathNavigator2;
				while ((xpathNavigator2 = this.input.Advance()) != null)
				{
					this.ProcessIds(xpathNavigator, xpathNavigator2.Value);
				}
				break;
			}
			case (XPathResultType)4:
				this.ProcessIds(xpathNavigator, ((XPathNavigator)obj).Value);
				break;
			}
			return this;
		}

		private void ProcessIds(XPathNavigator contextNode, string val)
		{
			string[] array = XmlConvert.SplitString(val);
			for (int i = 0; i < array.Length; i++)
			{
				if (contextNode.MoveToId(array[i]))
				{
					base.Insert(this.outputBuffer, contextNode);
				}
			}
		}

		public override XPathNavigator MatchNode(XPathNavigator context)
		{
			this.Evaluate(new XPathSingletonIterator(context, true));
			XPathNavigator xpathNavigator;
			while ((xpathNavigator = this.Advance()) != null)
			{
				if (xpathNavigator.IsSamePosition(context))
				{
					return context;
				}
			}
			return null;
		}

		public override XPathNodeIterator Clone()
		{
			return new IDQuery(this);
		}
	}
}
