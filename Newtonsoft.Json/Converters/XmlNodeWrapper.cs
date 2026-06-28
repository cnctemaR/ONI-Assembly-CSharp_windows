using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	internal class XmlNodeWrapper : IXmlNode
	{
		public XmlNodeWrapper(XmlNode node)
		{
			this._node = node;
		}

		public object WrappedNode
		{
			get
			{
				return this._node;
			}
		}

		public XmlNodeType NodeType
		{
			get
			{
				return this._node.NodeType;
			}
		}

		public virtual string LocalName
		{
			get
			{
				return this._node.LocalName;
			}
		}

		public IList<IXmlNode> ChildNodes
		{
			get
			{
				if (this._childNodes == null)
				{
					this._childNodes = this._node.ChildNodes.Cast<XmlNode>().Select<XmlNode, IXmlNode>(new Func<XmlNode, IXmlNode>(XmlNodeWrapper.WrapNode)).ToList<IXmlNode>();
				}
				return this._childNodes;
			}
		}

		internal static IXmlNode WrapNode(XmlNode node)
		{
			XmlNodeType nodeType = node.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				return new XmlElementWrapper((XmlElement)node);
			}
			if (nodeType == XmlNodeType.DocumentType)
			{
				return new XmlDocumentTypeWrapper((XmlDocumentType)node);
			}
			if (nodeType != XmlNodeType.XmlDeclaration)
			{
				return new XmlNodeWrapper(node);
			}
			return new XmlDeclarationWrapper((XmlDeclaration)node);
		}

		public IList<IXmlNode> Attributes
		{
			get
			{
				if (this._node.Attributes == null)
				{
					return null;
				}
				return this._node.Attributes.Cast<XmlAttribute>().Select<XmlAttribute, IXmlNode>(new Func<XmlAttribute, IXmlNode>(XmlNodeWrapper.WrapNode)).ToList<IXmlNode>();
			}
		}

		public IXmlNode ParentNode
		{
			get
			{
				XmlNode xmlNode = ((this._node is XmlAttribute) ? ((XmlAttribute)this._node).OwnerElement : this._node.ParentNode);
				if (xmlNode == null)
				{
					return null;
				}
				return XmlNodeWrapper.WrapNode(xmlNode);
			}
		}

		public string Value
		{
			get
			{
				return this._node.Value;
			}
			set
			{
				this._node.Value = value;
			}
		}

		public IXmlNode AppendChild(IXmlNode newChild)
		{
			XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)newChild;
			this._node.AppendChild(xmlNodeWrapper._node);
			this._childNodes = null;
			return newChild;
		}

		public string NamespaceUri
		{
			get
			{
				return this._node.NamespaceURI;
			}
		}

		private readonly XmlNode _node;

		private IList<IXmlNode> _childNodes;
	}
}
