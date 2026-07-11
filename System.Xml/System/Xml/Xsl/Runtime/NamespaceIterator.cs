using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct NamespaceIterator
	{
		public void Create(XPathNavigator context)
		{
			this.navStack.Reset();
			if (context.MoveToFirstNamespace(XPathNamespaceScope.All))
			{
				do
				{
					if (context.LocalName.Length != 0 || context.Value.Length != 0)
					{
						this.navStack.Push(context.Clone());
					}
				}
				while (context.MoveToNextNamespace(XPathNamespaceScope.All));
				context.MoveToParent();
			}
		}

		public bool MoveNext()
		{
			if (this.navStack.IsEmpty)
			{
				return false;
			}
			this.navCurrent = this.navStack.Pop();
			return true;
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private XPathNavigator navCurrent;

		private XmlNavigatorStack navStack;
	}
}
