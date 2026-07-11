using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct PrecedingSiblingDocOrderIterator
	{
		public void Create(XPathNavigator context, XmlNavigatorFilter filter)
		{
			this.filter = filter;
			this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
			this.navEnd = XmlQueryRuntime.SyncToNavigator(this.navEnd, context);
			this.needFirst = true;
			this.useCompPos = this.filter.IsFiltered(context);
		}

		public bool MoveNext()
		{
			if (this.needFirst)
			{
				if (!this.navCurrent.MoveToParent())
				{
					return false;
				}
				if (!this.filter.MoveToContent(this.navCurrent))
				{
					return false;
				}
				this.needFirst = false;
			}
			else if (!this.filter.MoveToFollowingSibling(this.navCurrent))
			{
				return false;
			}
			if (this.useCompPos)
			{
				return this.navCurrent.ComparePosition(this.navEnd) == XmlNodeOrder.Before;
			}
			if (this.navCurrent.IsSamePosition(this.navEnd))
			{
				this.useCompPos = true;
				return false;
			}
			return true;
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

		private XPathNavigator navEnd;

		private bool needFirst;

		private bool useCompPos;
	}
}
