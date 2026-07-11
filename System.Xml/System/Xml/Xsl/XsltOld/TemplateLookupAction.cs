using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class TemplateLookupAction : Action
	{
		internal void Initialize(XmlQualifiedName mode, Stylesheet importsOf)
		{
			this.mode = mode;
			this.importsOf = importsOf;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			Action action;
			if (this.mode != null)
			{
				action = ((this.importsOf == null) ? processor.Stylesheet.FindTemplate(processor, frame.Node, this.mode) : this.importsOf.FindTemplateImports(processor, frame.Node, this.mode));
			}
			else
			{
				action = ((this.importsOf == null) ? processor.Stylesheet.FindTemplate(processor, frame.Node) : this.importsOf.FindTemplateImports(processor, frame.Node));
			}
			if (action == null)
			{
				action = this.BuiltInTemplate(frame.Node);
			}
			if (action != null)
			{
				frame.SetAction(action);
				return;
			}
			frame.Finished();
		}

		internal Action BuiltInTemplate(XPathNavigator node)
		{
			Action action = null;
			switch (node.NodeType)
			{
			case XPathNodeType.Root:
			case XPathNodeType.Element:
				action = ApplyTemplatesAction.BuiltInRule(this.mode);
				break;
			case XPathNodeType.Attribute:
			case XPathNodeType.Text:
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
				action = ValueOfAction.BuiltInRule();
				break;
			}
			return action;
		}

		protected XmlQualifiedName mode;

		protected Stylesheet importsOf;
	}
}
