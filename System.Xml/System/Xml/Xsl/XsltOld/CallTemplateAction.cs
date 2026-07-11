using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class CallTemplateAction : ContainerAction
	{
		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckRequiredAttribute(compiler, this.name, "name");
			this.CompileContent(compiler);
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.Name))
			{
				this.name = compiler.CreateXPathQName(value);
				return true;
			}
			return false;
		}

		private void CompileContent(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			if (compiler.Recurse())
			{
				for (;;)
				{
					XPathNodeType nodeType = input.NodeType;
					if (nodeType != XPathNodeType.Element)
					{
						if (nodeType - XPathNodeType.SignificantWhitespace > 3)
						{
							break;
						}
					}
					else
					{
						compiler.PushNamespaceScope();
						string namespaceURI = input.NamespaceURI;
						string localName = input.LocalName;
						if (!Ref.Equal(namespaceURI, input.Atoms.UriXsl) || !Ref.Equal(localName, input.Atoms.WithParam))
						{
							goto IL_0079;
						}
						WithParamAction withParamAction = compiler.CreateWithParamAction();
						base.CheckDuplicateParams(withParamAction.Name);
						base.AddAction(withParamAction);
						compiler.PopScope();
					}
					if (!compiler.Advance())
					{
						goto Block_5;
					}
				}
				throw XsltException.Create("The contents of '{0}' are invalid.", new string[] { "call-template" });
				IL_0079:
				throw compiler.UnexpectedKeyword();
				Block_5:
				compiler.ToParent();
			}
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			switch (frame.State)
			{
			case 0:
				processor.ResetParams();
				if (this.containedActions != null && this.containedActions.Count > 0)
				{
					processor.PushActionFrame(frame);
					frame.State = 2;
					return;
				}
				break;
			case 1:
				return;
			case 2:
				break;
			case 3:
				frame.Finished();
				return;
			default:
				return;
			}
			TemplateAction templateAction = processor.Stylesheet.FindTemplate(this.name);
			if (templateAction != null)
			{
				frame.State = 3;
				processor.PushActionFrame(templateAction, frame.NodeSet);
				return;
			}
			throw XsltException.Create("The named template '{0}' does not exist.", new string[] { this.name.ToString() });
		}

		private const int ProcessedChildren = 2;

		private const int ProcessedTemplate = 3;

		private XmlQualifiedName name;
	}
}
