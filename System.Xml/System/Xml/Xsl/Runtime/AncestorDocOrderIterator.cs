using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct AncestorDocOrderIterator
	{
		public void Create(XPathNavigator context, XmlNavigatorFilter filter, bool orSelf)
		{
			AncestorIterator ancestorIterator = default(AncestorIterator);
			ancestorIterator.Create(context, filter, orSelf);
			this.stack.Reset();
			while (ancestorIterator.MoveNext())
			{
				XPathNavigator xpathNavigator = ancestorIterator.Current;
				this.stack.Push(xpathNavigator.Clone());
			}
		}

		public bool MoveNext()
		{
			if (this.stack.IsEmpty)
			{
				return false;
			}
			this.navCurrent = this.stack.Pop();
			return true;
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private XmlNavigatorStack stack;

		private XPathNavigator navCurrent;
	}
}
