using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class ApplyTemplatesAction : ContainerAction
	{
		internal static ApplyTemplatesAction BuiltInRule()
		{
			return ApplyTemplatesAction.s_BuiltInRule;
		}

		internal static ApplyTemplatesAction BuiltInRule(XmlQualifiedName mode)
		{
			if (!(mode == null) && !mode.IsEmpty)
			{
				return new ApplyTemplatesAction(mode);
			}
			return ApplyTemplatesAction.BuiltInRule();
		}

		internal ApplyTemplatesAction()
		{
		}

		private ApplyTemplatesAction(XmlQualifiedName mode)
		{
			this.mode = mode;
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			this.CompileContent(compiler);
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.Select))
			{
				this.selectKey = compiler.AddQuery(value);
			}
			else
			{
				if (!Ref.Equal(localName, compiler.Atoms.Mode))
				{
					return false;
				}
				if (compiler.AllowBuiltInMode && value == "*")
				{
					this.mode = Compiler.BuiltInMode;
				}
				else
				{
					this.mode = compiler.CreateXPathQName(value);
				}
			}
			return true;
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
						if (!Ref.Equal(namespaceURI, input.Atoms.UriXsl))
						{
							goto IL_00A7;
						}
						if (Ref.Equal(localName, input.Atoms.Sort))
						{
							base.AddAction(compiler.CreateSortAction());
						}
						else
						{
							if (!Ref.Equal(localName, input.Atoms.WithParam))
							{
								goto IL_00A0;
							}
							WithParamAction withParamAction = compiler.CreateWithParamAction();
							base.CheckDuplicateParams(withParamAction.Name);
							base.AddAction(withParamAction);
						}
						compiler.PopScope();
					}
					if (!compiler.Advance())
					{
						goto Block_6;
					}
				}
				throw XsltException.Create("The contents of '{0}' are invalid.", new string[] { "apply-templates" });
				IL_00A0:
				throw compiler.UnexpectedKeyword();
				IL_00A7:
				throw compiler.UnexpectedKeyword();
				Block_6:
				compiler.ToParent();
			}
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			switch (frame.State)
			{
			case 0:
				processor.ResetParams();
				processor.InitSortArray();
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
				goto IL_00C2;
			case 4:
				goto IL_00DB;
			case 5:
				frame.State = 3;
				goto IL_00C2;
			default:
				return;
			}
			if (this.selectKey == -1)
			{
				if (!frame.Node.HasChildren)
				{
					frame.Finished();
					return;
				}
				frame.InitNewNodeSet(frame.Node.SelectChildren(XPathNodeType.All));
			}
			else
			{
				frame.InitNewNodeSet(processor.StartQuery(frame.NodeSet, this.selectKey));
			}
			if (processor.SortArray.Count != 0)
			{
				frame.SortNewNodeSet(processor, processor.SortArray);
			}
			frame.State = 3;
			IL_00C2:
			if (!frame.NewNextNode(processor))
			{
				frame.Finished();
				return;
			}
			frame.State = 4;
			IL_00DB:
			processor.PushTemplateLookup(frame.NewNodeSet, this.mode, null);
			frame.State = 5;
		}

		private const int ProcessedChildren = 2;

		private const int ProcessNextNode = 3;

		private const int PositionAdvanced = 4;

		private const int TemplateProcessed = 5;

		private int selectKey = -1;

		private XmlQualifiedName mode;

		private static ApplyTemplatesAction s_BuiltInRule = new ApplyTemplatesAction();
	}
}
