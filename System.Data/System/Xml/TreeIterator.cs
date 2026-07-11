using System;

namespace System.Xml
{
	internal sealed class TreeIterator : BaseTreeIterator
	{
		internal TreeIterator(XmlNode nodeTop)
			: base(((XmlDataDocument)nodeTop.OwnerDocument).Mapper)
		{
			this._nodeTop = nodeTop;
			this._currentNode = nodeTop;
		}

		internal override XmlNode CurrentNode
		{
			get
			{
				return this._currentNode;
			}
		}

		internal override bool Next()
		{
			XmlNode firstChild = this._currentNode.FirstChild;
			if (firstChild != null)
			{
				this._currentNode = firstChild;
				return true;
			}
			return this.NextRight();
		}

		internal override bool NextRight()
		{
			if (this._currentNode == this._nodeTop)
			{
				this._currentNode = null;
				return false;
			}
			XmlNode xmlNode = this._currentNode.NextSibling;
			if (xmlNode != null)
			{
				this._currentNode = xmlNode;
				return true;
			}
			xmlNode = this._currentNode;
			while (xmlNode != this._nodeTop && xmlNode.NextSibling == null)
			{
				xmlNode = xmlNode.ParentNode;
			}
			if (xmlNode == this._nodeTop)
			{
				this._currentNode = null;
				return false;
			}
			this._currentNode = xmlNode.NextSibling;
			return true;
		}

		private readonly XmlNode _nodeTop;

		private XmlNode _currentNode;
	}
}
