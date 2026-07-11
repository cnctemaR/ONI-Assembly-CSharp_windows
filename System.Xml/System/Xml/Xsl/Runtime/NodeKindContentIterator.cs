using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct NodeKindContentIterator
	{
		public void Create(XPathNavigator context, XPathNodeType nodeType)
		{
			this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
			this.nodeType = nodeType;
			this.needFirst = true;
		}

		public bool MoveNext()
		{
			if (this.needFirst)
			{
				this.needFirst = !this.navCurrent.MoveToChild(this.nodeType);
				return !this.needFirst;
			}
			return this.navCurrent.MoveToNext(this.nodeType);
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private XPathNodeType nodeType;

		private XPathNavigator navCurrent;

		private bool needFirst;
	}
}
