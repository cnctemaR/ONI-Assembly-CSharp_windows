using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct XPathPrecedingIterator
	{
		public void Create(XPathNavigator context, XmlNavigatorFilter filter)
		{
			XPathPrecedingDocOrderIterator xpathPrecedingDocOrderIterator = default(XPathPrecedingDocOrderIterator);
			xpathPrecedingDocOrderIterator.Create(context, filter);
			this.stack.Reset();
			while (xpathPrecedingDocOrderIterator.MoveNext())
			{
				XPathNavigator xpathNavigator = xpathPrecedingDocOrderIterator.Current;
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
