using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct AncestorIterator
	{
		public void Create(XPathNavigator context, XmlNavigatorFilter filter, bool orSelf)
		{
			this.filter = filter;
			this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
			this.haveCurrent = orSelf && !this.filter.IsFiltered(this.navCurrent);
		}

		public bool MoveNext()
		{
			if (this.haveCurrent)
			{
				this.haveCurrent = false;
				return true;
			}
			while (this.navCurrent.MoveToParent())
			{
				if (!this.filter.IsFiltered(this.navCurrent))
				{
					return true;
				}
			}
			return false;
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private XmlNavigatorFilter filter;

		private XPathNavigator navCurrent;

		private bool haveCurrent;
	}
}
