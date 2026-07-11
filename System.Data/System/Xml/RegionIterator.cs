using System;
using System.Text;

namespace System.Xml
{
	internal sealed class RegionIterator : BaseRegionIterator
	{
		internal RegionIterator(XmlBoundElement rowElement)
			: base(((XmlDataDocument)rowElement.OwnerDocument).Mapper)
		{
			this._rowElement = rowElement;
			this._currentNode = rowElement;
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
			ElementState elementState = this._rowElement.ElementState;
			XmlNode firstChild = this._currentNode.FirstChild;
			if (firstChild != null)
			{
				this._currentNode = firstChild;
				this._rowElement.ElementState = elementState;
				return true;
			}
			return this.NextRight();
		}

		internal override bool NextRight()
		{
			if (this._currentNode == this._rowElement)
			{
				this._currentNode = null;
				return false;
			}
			ElementState elementState = this._rowElement.ElementState;
			XmlNode xmlNode = this._currentNode.NextSibling;
			if (xmlNode != null)
			{
				this._currentNode = xmlNode;
				this._rowElement.ElementState = elementState;
				return true;
			}
			xmlNode = this._currentNode;
			while (xmlNode != this._rowElement && xmlNode.NextSibling == null)
			{
				xmlNode = xmlNode.ParentNode;
			}
			if (xmlNode == this._rowElement)
			{
				this._currentNode = null;
				this._rowElement.ElementState = elementState;
				return false;
			}
			this._currentNode = xmlNode.NextSibling;
			this._rowElement.ElementState = elementState;
			return true;
		}

		internal bool NextInitialTextLikeNodes(out string value)
		{
			ElementState elementState = this._rowElement.ElementState;
			XmlNode firstChild = this.CurrentNode.FirstChild;
			value = RegionIterator.GetInitialTextFromNodes(ref firstChild);
			if (firstChild == null)
			{
				this._rowElement.ElementState = elementState;
				return this.NextRight();
			}
			this._currentNode = firstChild;
			this._rowElement.ElementState = elementState;
			return true;
		}

		private static string GetInitialTextFromNodes(ref XmlNode n)
		{
			string text = null;
			if (n != null)
			{
				while (n.NodeType == XmlNodeType.Whitespace)
				{
					n = n.NextSibling;
					if (n == null)
					{
						return string.Empty;
					}
				}
				if (XmlDataDocument.IsTextLikeNode(n) && (n.NextSibling == null || !XmlDataDocument.IsTextLikeNode(n.NextSibling)))
				{
					text = n.Value;
					n = n.NextSibling;
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (n != null && XmlDataDocument.IsTextLikeNode(n))
					{
						if (n.NodeType != XmlNodeType.Whitespace)
						{
							stringBuilder.Append(n.Value);
						}
						n = n.NextSibling;
					}
					text = stringBuilder.ToString();
				}
			}
			return text ?? string.Empty;
		}

		private XmlBoundElement _rowElement;

		private XmlNode _currentNode;
	}
}
