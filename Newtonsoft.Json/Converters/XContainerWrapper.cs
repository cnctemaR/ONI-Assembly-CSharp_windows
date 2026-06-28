using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	internal class XContainerWrapper : XObjectWrapper
	{
		private XContainer Container
		{
			get
			{
				return (XContainer)base.WrappedNode;
			}
		}

		public XContainerWrapper(XContainer container)
			: base(container)
		{
		}

		public override IList<IXmlNode> ChildNodes
		{
			get
			{
				if (this._childNodes == null)
				{
					this._childNodes = this.Container.Nodes().Select<XNode, IXmlNode>(new Func<XNode, IXmlNode>(XContainerWrapper.WrapNode)).ToList<IXmlNode>();
				}
				return this._childNodes;
			}
		}

		public override IXmlNode ParentNode
		{
			get
			{
				if (this.Container.Parent == null)
				{
					return null;
				}
				return XContainerWrapper.WrapNode(this.Container.Parent);
			}
		}

		internal static IXmlNode WrapNode(XObject node)
		{
			if (node is XDocument)
			{
				return new XDocumentWrapper((XDocument)node);
			}
			if (node is XElement)
			{
				return new XElementWrapper((XElement)node);
			}
			if (node is XContainer)
			{
				return new XContainerWrapper((XContainer)node);
			}
			if (node is XProcessingInstruction)
			{
				return new XProcessingInstructionWrapper((XProcessingInstruction)node);
			}
			if (node is XText)
			{
				return new XTextWrapper((XText)node);
			}
			if (node is XComment)
			{
				return new XCommentWrapper((XComment)node);
			}
			if (node is XAttribute)
			{
				return new XAttributeWrapper((XAttribute)node);
			}
			if (node is XDocumentType)
			{
				return new XDocumentTypeWrapper((XDocumentType)node);
			}
			return new XObjectWrapper(node);
		}

		public override IXmlNode AppendChild(IXmlNode newChild)
		{
			this.Container.Add(newChild.WrappedNode);
			this._childNodes = null;
			return newChild;
		}

		private IList<IXmlNode> _childNodes;
	}
}
