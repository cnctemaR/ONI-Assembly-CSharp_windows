using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct FollowingSiblingMergeIterator
	{
		public void Create(XmlNavigatorFilter filter)
		{
			this.wrapped.Create(filter);
		}

		public IteratorResult MoveNext(XPathNavigator navigator)
		{
			return this.wrapped.MoveNext(navigator, false);
		}

		public XPathNavigator Current
		{
			get
			{
				return this.wrapped.Current;
			}
		}

		private ContentMergeIterator wrapped;
	}
}
