using System;
using System.Xml.XPath;

namespace System.Xml
{
	internal abstract class DocumentXPathNodeIterator_ElemDescendants : XPathNodeIterator
	{
		internal DocumentXPathNodeIterator_ElemDescendants(DocumentXPathNavigator nav)
		{
			this.nav = (DocumentXPathNavigator)nav.Clone();
			this.level = 0;
			this.position = 0;
		}

		internal DocumentXPathNodeIterator_ElemDescendants(DocumentXPathNodeIterator_ElemDescendants other)
		{
			this.nav = (DocumentXPathNavigator)other.nav.Clone();
			this.level = other.level;
			this.position = other.position;
		}

		protected abstract bool Match(XmlNode node);

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
				return this.position;
			}
		}

		protected void SetPosition(int pos)
		{
			this.position = pos;
		}

		public override bool MoveNext()
		{
			for (;;)
			{
				if (this.nav.MoveToFirstChild())
				{
					this.level++;
				}
				else
				{
					if (this.level == 0)
					{
						break;
					}
					while (!this.nav.MoveToNext())
					{
						this.level--;
						if (this.level == 0)
						{
							return false;
						}
						if (!this.nav.MoveToParent())
						{
							return false;
						}
					}
				}
				XmlNode xmlNode = (XmlNode)this.nav.UnderlyingObject;
				if (xmlNode.NodeType == XmlNodeType.Element && this.Match(xmlNode))
				{
					goto Block_5;
				}
			}
			return false;
			Block_5:
			this.position++;
			return true;
		}

		private DocumentXPathNavigator nav;

		private int level;

		private int position;
	}
}
