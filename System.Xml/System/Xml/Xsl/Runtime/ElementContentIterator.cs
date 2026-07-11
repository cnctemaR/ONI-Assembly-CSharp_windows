using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct ElementContentIterator
	{
		public void Create(XPathNavigator context, string localName, string ns)
		{
			this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
			this.localName = localName;
			this.ns = ns;
			this.needFirst = true;
		}

		public bool MoveNext()
		{
			if (this.needFirst)
			{
				this.needFirst = !this.navCurrent.MoveToChild(this.localName, this.ns);
				return !this.needFirst;
			}
			return this.navCurrent.MoveToNext(this.localName, this.ns);
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private string localName;

		private string ns;

		private XPathNavigator navCurrent;

		private bool needFirst;
	}
}
