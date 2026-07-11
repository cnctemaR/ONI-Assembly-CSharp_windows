using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct ContentIterator
	{
		public void Create(XPathNavigator context)
		{
			this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
			this.needFirst = true;
		}

		public bool MoveNext()
		{
			if (this.needFirst)
			{
				this.needFirst = !this.navCurrent.MoveToFirstChild();
				return !this.needFirst;
			}
			return this.navCurrent.MoveToNext();
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private XPathNavigator navCurrent;

		private bool needFirst;
	}
}
