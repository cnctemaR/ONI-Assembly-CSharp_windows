using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct XPathPrecedingDocOrderIterator
	{
		public void Create(XPathNavigator input, XmlNavigatorFilter filter)
		{
			this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
			this.filter = filter;
			this.PushAncestors();
		}

		public bool MoveNext()
		{
			if (!this.navStack.IsEmpty)
			{
				while (!this.filter.MoveToFollowing(this.navCurrent, this.navStack.Peek()))
				{
					this.navCurrent.MoveTo(this.navStack.Pop());
					if (this.navStack.IsEmpty)
					{
						return false;
					}
				}
				return true;
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

		private void PushAncestors()
		{
			this.navStack.Reset();
			do
			{
				this.navStack.Push(this.navCurrent.Clone());
			}
			while (this.navCurrent.MoveToParent());
			this.navStack.Pop();
		}

		private XmlNavigatorFilter filter;

		private XPathNavigator navCurrent;

		private XmlNavigatorStack navStack;
	}
}
