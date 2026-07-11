using System;
using System.Xml.XPath;

namespace System.Xml
{
	internal sealed class DocumentXPathNodeIterator_Empty : XPathNodeIterator
	{
		internal DocumentXPathNodeIterator_Empty(DocumentXPathNavigator nav)
		{
			this.nav = nav.Clone();
		}

		internal DocumentXPathNodeIterator_Empty(DocumentXPathNodeIterator_Empty other)
		{
			this.nav = other.nav.Clone();
		}

		public override XPathNodeIterator Clone()
		{
			return new DocumentXPathNodeIterator_Empty(this);
		}

		public override bool MoveNext()
		{
			return false;
		}

		public override XPathNavigator Current
		{
			get
			{
				return this.nav;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return 0;
			}
		}

		public override int Count
		{
			get
			{
				return 0;
			}
		}

		private XPathNavigator nav;
	}
}
