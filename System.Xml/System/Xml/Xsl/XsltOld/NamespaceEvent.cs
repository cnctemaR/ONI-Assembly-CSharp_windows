using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class NamespaceEvent : Event
	{
		public NamespaceEvent(NavigatorInput input)
		{
			this.namespaceUri = input.Value;
			this.name = input.LocalName;
		}

		public override void ReplaceNamespaceAlias(Compiler compiler)
		{
			if (this.namespaceUri.Length != 0)
			{
				NamespaceInfo namespaceInfo = compiler.FindNamespaceAlias(this.namespaceUri);
				if (namespaceInfo != null)
				{
					this.namespaceUri = namespaceInfo.nameSpace;
					if (namespaceInfo.prefix != null)
					{
						this.name = namespaceInfo.prefix;
					}
				}
			}
		}

		public override bool Output(Processor processor, ActionFrame frame)
		{
			processor.BeginEvent(XPathNodeType.Namespace, null, this.name, this.namespaceUri, false);
			processor.EndEvent(XPathNodeType.Namespace);
			return true;
		}

		private string namespaceUri;

		private string name;
	}
}
