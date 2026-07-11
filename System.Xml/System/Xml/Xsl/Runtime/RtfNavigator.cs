using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal abstract class RtfNavigator : XPathNavigator
	{
		public abstract void CopyToWriter(XmlWriter writer);

		public abstract XPathNavigator ToNavigator();

		public override XPathNodeType NodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		public override string LocalName
		{
			get
			{
				return string.Empty;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return string.Empty;
			}
		}

		public override string Name
		{
			get
			{
				return string.Empty;
			}
		}

		public override string Prefix
		{
			get
			{
				return string.Empty;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return false;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override bool MoveToFirstAttribute()
		{
			throw new NotSupportedException();
		}

		public override bool MoveToNextAttribute()
		{
			throw new NotSupportedException();
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
		{
			throw new NotSupportedException();
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
		{
			throw new NotSupportedException();
		}

		public override bool MoveToNext()
		{
			throw new NotSupportedException();
		}

		public override bool MoveToPrevious()
		{
			throw new NotSupportedException();
		}

		public override bool MoveToFirstChild()
		{
			throw new NotSupportedException();
		}

		public override bool MoveToParent()
		{
			throw new NotSupportedException();
		}

		public override bool MoveToId(string id)
		{
			throw new NotSupportedException();
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			throw new NotSupportedException();
		}
	}
}
