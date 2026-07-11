using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct IdIterator
	{
		public void Create(XPathNavigator context, string value)
		{
			this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
			this.idrefs = XmlConvert.SplitString(value);
			this.idx = -1;
		}

		public bool MoveNext()
		{
			for (;;)
			{
				this.idx++;
				if (this.idx >= this.idrefs.Length)
				{
					break;
				}
				if (this.navCurrent.MoveToId(this.idrefs[this.idx]))
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

		private XPathNavigator navCurrent;

		private string[] idrefs;

		private int idx;
	}
}
