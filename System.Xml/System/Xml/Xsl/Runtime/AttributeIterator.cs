using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct AttributeIterator
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
				this.needFirst = !this.navCurrent.MoveToFirstAttribute();
				return !this.needFirst;
			}
			return this.navCurrent.MoveToNextAttribute();
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
