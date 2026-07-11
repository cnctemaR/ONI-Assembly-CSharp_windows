using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class BeginEvent : Event
	{
		public BeginEvent(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			this.nodeType = input.NodeType;
			this.namespaceUri = input.NamespaceURI;
			this.name = input.LocalName;
			this.prefix = input.Prefix;
			this.empty = input.IsEmptyTag;
			if (this.nodeType == XPathNodeType.Element)
			{
				this.htmlProps = HtmlElementProps.GetProps(this.name);
				return;
			}
			if (this.nodeType == XPathNodeType.Attribute)
			{
				this.htmlProps = HtmlAttributeProps.GetProps(this.name);
			}
		}

		public override void ReplaceNamespaceAlias(Compiler compiler)
		{
			if (this.nodeType == XPathNodeType.Attribute && this.namespaceUri.Length == 0)
			{
				return;
			}
			NamespaceInfo namespaceInfo = compiler.FindNamespaceAlias(this.namespaceUri);
			if (namespaceInfo != null)
			{
				this.namespaceUri = namespaceInfo.nameSpace;
				if (namespaceInfo.prefix != null)
				{
					this.prefix = namespaceInfo.prefix;
				}
			}
		}

		public override bool Output(Processor processor, ActionFrame frame)
		{
			return processor.BeginEvent(this.nodeType, this.prefix, this.name, this.namespaceUri, this.empty, this.htmlProps, false);
		}

		private XPathNodeType nodeType;

		private string namespaceUri;

		private string name;

		private string prefix;

		private bool empty;

		private object htmlProps;
	}
}
