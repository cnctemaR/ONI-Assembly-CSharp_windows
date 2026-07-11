using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class ChooseAction : ContainerAction
	{
		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			if (compiler.Recurse())
			{
				this.CompileConditions(compiler);
				compiler.ToParent();
			}
		}

		private void CompileConditions(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			bool flag = false;
			bool flag2 = false;
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
						goto IL_00C6;
					}
					IfAction ifAction;
					if (Ref.Equal(localName, input.Atoms.When))
					{
						if (flag2)
						{
							goto Block_5;
						}
						ifAction = compiler.CreateIfAction(IfAction.ConditionType.ConditionWhen);
						flag = true;
					}
					else
					{
						if (!Ref.Equal(localName, input.Atoms.Otherwise))
						{
							goto IL_00B5;
						}
						if (flag2)
						{
							goto Block_7;
						}
						ifAction = compiler.CreateIfAction(IfAction.ConditionType.ConditionOtherwise);
						flag2 = true;
					}
					base.AddAction(ifAction);
					compiler.PopScope();
				}
				if (!compiler.Advance())
				{
					goto Block_8;
				}
			}
			throw XsltException.Create("The contents of '{0}' are invalid.", new string[] { "choose" });
			Block_5:
			throw XsltException.Create("'xsl:when' must precede the 'xsl:otherwise' element.", Array.Empty<string>());
			Block_7:
			throw XsltException.Create("An 'xsl:choose' element can have only one 'xsl:otherwise' child.", Array.Empty<string>());
			IL_00B5:
			throw compiler.UnexpectedKeyword();
			IL_00C6:
			throw compiler.UnexpectedKeyword();
			Block_8:
			if (!flag)
			{
				throw XsltException.Create("An 'xsl:choose' element must have at least one 'xsl:when' child.", Array.Empty<string>());
			}
		}
	}
}
