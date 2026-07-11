using System;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal sealed class XslAstRewriter
	{
		public void Rewrite(Compiler compiler)
		{
			this.compiler = compiler;
			this.scope = new CompilerScopeManager<VarPar>();
			this.newTemplates = new Stack<Template>();
			using (List<ProtoTemplate>.Enumerator enumerator = compiler.AllTemplates.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ProtoTemplate protoTemplate = enumerator.Current;
					this.scope.EnterScope();
					this.CheckNodeCost(protoTemplate);
				}
				goto IL_009C;
			}
			IL_005F:
			Template template = this.newTemplates.Pop();
			compiler.AllTemplates.Add(template);
			compiler.NamedTemplates.Add(template.Name, template);
			this.scope.EnterScope();
			this.CheckNodeCost(template);
			IL_009C:
			if (this.newTemplates.Count <= 0)
			{
				return;
			}
			goto IL_005F;
		}

		private static int NodeCostForXPath(string xpath)
		{
			int num = 0;
			if (xpath != null)
			{
				num = 2;
				for (int i = 2; i < xpath.Length; i += 2)
				{
					if (xpath[i] == '/' || xpath[i - 1] == '/')
					{
						num += 2;
					}
				}
			}
			return num;
		}

		private static bool NodeTypeTest(XslNodeType nodetype, int flags)
		{
			return ((flags >> (int)nodetype) & 1) != 0;
		}

		private int CheckNodeCost(XslNode node)
		{
			this.scope.EnterScope(node.Namespaces);
			bool flag = false;
			int num = 1;
			if (XslAstRewriter.NodeTypeTest(node.NodeType, -247451132))
			{
				num += XslAstRewriter.NodeCostForXPath(node.Select);
			}
			IList<XslNode> content = node.Content;
			int num2 = content.Count - 1;
			int i = 0;
			while (i <= num2)
			{
				XslNode xslNode = content[i];
				int num3 = this.CheckNodeCost(xslNode);
				num += num3;
				if (flag && num > 100)
				{
					if (i < num2 || num3 > 1)
					{
						this.Refactor(node, i);
						num -= num3;
						num++;
						break;
					}
					break;
				}
				else
				{
					if (xslNode.NodeType == XslNodeType.Variable || xslNode.NodeType == XslNodeType.Param)
					{
						this.scope.AddVariable(xslNode.Name, (VarPar)xslNode);
						if (xslNode.NodeType == XslNodeType.Param)
						{
							num -= num3;
						}
					}
					else if (!flag)
					{
						flag = XslAstRewriter.NodeTypeTest(node.NodeType, -1025034872);
					}
					i++;
				}
			}
			this.scope.ExitScope();
			return num;
		}

		private void Refactor(XslNode parent, int split)
		{
			List<XslNode> list = (List<XslNode>)parent.Content;
			XslNode xslNode = list[split];
			QilName qilName = AstFactory.QName("generated", this.compiler.CreatePhantomNamespace(), "compiler");
			XsltInput.ContextInfo contextInfo = new XsltInput.ContextInfo(xslNode.SourceLine);
			XslNodeEx xslNodeEx = AstFactory.CallTemplate(qilName, contextInfo);
			XsltLoader.SetInfo(xslNodeEx, null, contextInfo);
			Template template = AstFactory.Template(qilName, null, XsltLoader.nullMode, double.NaN, xslNode.XslVersion);
			XsltLoader.SetInfo(template, null, contextInfo);
			this.newTemplates.Push(template);
			template.SetContent(new List<XslNode>(list.Count - split + 8));
			foreach (CompilerScopeManager<VarPar>.ScopeRecord scopeRecord in this.scope.GetActiveRecords())
			{
				if (!scopeRecord.IsVariable)
				{
					template.Namespaces = new NsDecl(template.Namespaces, scopeRecord.ncName, scopeRecord.nsUri);
				}
				else
				{
					VarPar value = scopeRecord.value;
					if (!this.compiler.IsPhantomNamespace(value.Name.NamespaceUri))
					{
						QilName qilName2 = AstFactory.QName(value.Name.LocalName, value.Name.NamespaceUri, value.Name.Prefix);
						VarPar varPar = AstFactory.VarPar(XslNodeType.WithParam, qilName2, "$" + qilName2.QualifiedName, XslVersion.Version10);
						XsltLoader.SetInfo(varPar, null, contextInfo);
						varPar.Namespaces = value.Namespaces;
						xslNodeEx.AddContent(varPar);
						VarPar varPar2 = AstFactory.VarPar(XslNodeType.Param, qilName2, null, XslVersion.Version10);
						XsltLoader.SetInfo(varPar2, null, contextInfo);
						varPar2.Namespaces = value.Namespaces;
						template.AddContent(varPar2);
					}
				}
			}
			for (int i = split; i < list.Count; i++)
			{
				template.AddContent(list[i]);
			}
			list[split] = xslNodeEx;
			list.RemoveRange(split + 1, list.Count - split - 1);
		}

		private static readonly QilName nullMode = AstFactory.QName(string.Empty);

		private CompilerScopeManager<VarPar> scope;

		private Stack<Template> newTemplates;

		private Compiler compiler;

		private const int FixedNodeCost = 1;

		private const int IteratorNodeCost = 2;

		private const int CallTemplateCost = 1;

		private const int RewriteThreshold = 100;

		private const int NodesWithSelect = -247451132;

		private const int ParentsOfCallTemplate = -1025034872;
	}
}
