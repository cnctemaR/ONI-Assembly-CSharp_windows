using System;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.Runtime;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt
{
	internal class XslAstAnalyzer : XslVisitor<XslFlags>
	{
		public XslFlags Analyze(Compiler compiler)
		{
			this.compiler = compiler;
			this.scope = new CompilerScopeManager<VarPar>();
			this.xpathAnalyzer = new XslAstAnalyzer.XPathAnalyzer(compiler, this.scope);
			foreach (VarPar varPar in compiler.ExternalPars)
			{
				this.scope.AddVariable(varPar.Name, varPar);
			}
			foreach (VarPar varPar2 in compiler.GlobalVars)
			{
				this.scope.AddVariable(varPar2.Name, varPar2);
			}
			foreach (VarPar varPar3 in compiler.ExternalPars)
			{
				this.Visit(varPar3);
				varPar3.Flags |= XslFlags.TypeFilter;
			}
			foreach (VarPar varPar4 in compiler.GlobalVars)
			{
				this.Visit(varPar4);
			}
			XslFlags xslFlags = XslFlags.None;
			foreach (ProtoTemplate protoTemplate in compiler.AllTemplates)
			{
				this.currentTemplate = protoTemplate;
				xslFlags |= this.Visit(protoTemplate);
			}
			foreach (ProtoTemplate protoTemplate2 in compiler.AllTemplates)
			{
				foreach (XslNode xslNode in protoTemplate2.Content)
				{
					if (xslNode.NodeType != XslNodeType.Text)
					{
						if (xslNode.NodeType != XslNodeType.Param)
						{
							break;
						}
						VarPar varPar5 = (VarPar)xslNode;
						if ((varPar5.Flags & XslFlags.MayBeDefault) != XslFlags.None)
						{
							varPar5.Flags |= varPar5.DefValueFlags;
						}
					}
				}
			}
			for (int num = 32; num != 0; num >>= 1)
			{
				this.dataFlow.PropagateFlag((XslFlags)num);
			}
			this.dataFlow = null;
			foreach (KeyValuePair<Template, Stylesheet> keyValuePair in this.fwdApplyImportsGraph)
			{
				foreach (Stylesheet stylesheet in keyValuePair.Value.Imports)
				{
					this.AddImportDependencies(stylesheet, keyValuePair.Key);
				}
			}
			this.fwdApplyImportsGraph = null;
			if ((xslFlags & XslFlags.Current) != XslFlags.None)
			{
				this.revCall0Graph.PropagateFlag(XslFlags.Current);
			}
			if ((xslFlags & XslFlags.Position) != XslFlags.None)
			{
				this.revCall0Graph.PropagateFlag(XslFlags.Position);
			}
			if ((xslFlags & XslFlags.Last) != XslFlags.None)
			{
				this.revCall0Graph.PropagateFlag(XslFlags.Last);
			}
			if ((xslFlags & XslFlags.SideEffects) != XslFlags.None)
			{
				this.PropagateSideEffectsFlag();
			}
			this.revCall0Graph = null;
			this.revCall1Graph = null;
			this.revApplyTemplatesGraph = null;
			this.FillModeFlags(compiler.Root.ModeFlags, compiler.Root.Imports[0]);
			this.TraceResults();
			return xslFlags;
		}

		private void AddImportDependencies(Stylesheet sheet, Template focusDonor)
		{
			foreach (Template template in sheet.Templates)
			{
				if (template.Mode.Equals(focusDonor.Mode))
				{
					this.revCall0Graph.AddEdge(template, focusDonor);
				}
			}
			foreach (Stylesheet stylesheet in sheet.Imports)
			{
				this.AddImportDependencies(stylesheet, focusDonor);
			}
		}

		private void FillModeFlags(Dictionary<QilName, XslFlags> parentModeFlags, Stylesheet sheet)
		{
			foreach (Stylesheet stylesheet in sheet.Imports)
			{
				this.FillModeFlags(sheet.ModeFlags, stylesheet);
			}
			foreach (KeyValuePair<QilName, XslFlags> keyValuePair in sheet.ModeFlags)
			{
				XslFlags xslFlags;
				if (!parentModeFlags.TryGetValue(keyValuePair.Key, out xslFlags))
				{
					xslFlags = XslFlags.None;
				}
				parentModeFlags[keyValuePair.Key] = xslFlags | keyValuePair.Value;
			}
			foreach (Template template in sheet.Templates)
			{
				XslFlags xslFlags2 = template.Flags & (XslFlags.Current | XslFlags.Position | XslFlags.Last | XslFlags.SideEffects);
				if (xslFlags2 != XslFlags.None)
				{
					XslFlags xslFlags3;
					if (!parentModeFlags.TryGetValue(template.Mode, out xslFlags3))
					{
						xslFlags3 = XslFlags.None;
					}
					parentModeFlags[template.Mode] = xslFlags3 | xslFlags2;
				}
			}
		}

		private void TraceResults()
		{
		}

		protected override XslFlags Visit(XslNode node)
		{
			this.scope.EnterScope(node.Namespaces);
			XslFlags xslFlags = base.Visit(node);
			this.scope.ExitScope();
			if (this.currentTemplate != null && (node.NodeType == XslNodeType.Variable || node.NodeType == XslNodeType.Param))
			{
				this.scope.AddVariable(node.Name, (VarPar)node);
			}
			return xslFlags;
		}

		protected override XslFlags VisitChildren(XslNode node)
		{
			XslFlags xslFlags = XslFlags.None;
			foreach (XslNode xslNode in node.Content)
			{
				xslFlags |= this.Visit(xslNode);
			}
			return xslFlags;
		}

		protected override XslFlags VisitAttributeSet(AttributeSet node)
		{
			node.Flags = this.VisitChildren(node);
			return node.Flags;
		}

		protected override XslFlags VisitTemplate(Template node)
		{
			node.Flags = this.VisitChildren(node);
			return node.Flags;
		}

		protected override XslFlags VisitApplyImports(XslNode node)
		{
			this.fwdApplyImportsGraph[(Template)this.currentTemplate] = (Stylesheet)node.Arg;
			return XslFlags.Rtf | XslFlags.Current | XslFlags.HasCalls;
		}

		protected override XslFlags VisitApplyTemplates(XslNode node)
		{
			XslFlags xslFlags = this.ProcessExpr(node.Select);
			foreach (XslNode xslNode in node.Content)
			{
				xslFlags |= this.Visit(xslNode);
				if (xslNode.NodeType == XslNodeType.WithParam)
				{
					XslAstAnalyzer.ModeName modeName = new XslAstAnalyzer.ModeName(node.Name, xslNode.Name);
					VarPar varPar;
					if (!this.applyTemplatesParams.TryGetValue(modeName, out varPar))
					{
						varPar = (this.applyTemplatesParams[modeName] = AstFactory.WithParam(xslNode.Name));
					}
					if (this.typeDonor != null)
					{
						this.dataFlow.AddEdge(this.typeDonor, varPar);
					}
					else
					{
						varPar.Flags |= xslNode.Flags & XslFlags.TypeFilter;
					}
				}
			}
			if (this.currentTemplate != null)
			{
				this.AddApplyTemplatesEdge(node.Name, this.currentTemplate);
			}
			return XslFlags.Rtf | XslFlags.HasCalls | xslFlags;
		}

		protected override XslFlags VisitAttribute(NodeCtor node)
		{
			return XslFlags.Rtf | this.ProcessAvt(node.NameAvt) | this.ProcessAvt(node.NsAvt) | this.VisitChildren(node);
		}

		protected override XslFlags VisitCallTemplate(XslNode node)
		{
			XslFlags xslFlags = XslFlags.None;
			Template template;
			if (this.compiler.NamedTemplates.TryGetValue(node.Name, out template) && this.currentTemplate != null)
			{
				if (this.forEachDepth == 0)
				{
					this.revCall0Graph.AddEdge(template, this.currentTemplate);
				}
				else
				{
					this.revCall1Graph.AddEdge(template, this.currentTemplate);
				}
			}
			VarPar[] array = new VarPar[node.Content.Count];
			int num = 0;
			foreach (XslNode xslNode in node.Content)
			{
				xslFlags |= this.Visit(xslNode);
				array[num++] = this.typeDonor;
			}
			if (template != null)
			{
				foreach (XslNode xslNode2 in template.Content)
				{
					if (xslNode2.NodeType != XslNodeType.Text)
					{
						if (xslNode2.NodeType != XslNodeType.Param)
						{
							break;
						}
						VarPar varPar = (VarPar)xslNode2;
						VarPar varPar2 = null;
						num = 0;
						foreach (XslNode xslNode3 in node.Content)
						{
							if (xslNode3.Name.Equals(varPar.Name))
							{
								varPar2 = (VarPar)xslNode3;
								this.typeDonor = array[num];
								break;
							}
							num++;
						}
						if (varPar2 != null)
						{
							if (this.typeDonor != null)
							{
								this.dataFlow.AddEdge(this.typeDonor, varPar);
							}
							else
							{
								varPar.Flags |= varPar2.Flags & XslFlags.TypeFilter;
							}
						}
						else
						{
							varPar.Flags |= XslFlags.MayBeDefault;
						}
					}
				}
			}
			return XslFlags.Rtf | XslFlags.HasCalls | xslFlags;
		}

		protected override XslFlags VisitComment(XslNode node)
		{
			return XslFlags.Rtf | this.VisitChildren(node);
		}

		protected override XslFlags VisitCopy(XslNode node)
		{
			return XslFlags.Rtf | XslFlags.Current | this.VisitChildren(node);
		}

		protected override XslFlags VisitCopyOf(XslNode node)
		{
			return XslFlags.Rtf | this.ProcessExpr(node.Select);
		}

		protected override XslFlags VisitElement(NodeCtor node)
		{
			return XslFlags.Rtf | this.ProcessAvt(node.NameAvt) | this.ProcessAvt(node.NsAvt) | this.VisitChildren(node);
		}

		protected override XslFlags VisitError(XslNode node)
		{
			return (this.VisitChildren(node) & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf)) | XslFlags.SideEffects;
		}

		protected override XslFlags VisitForEach(XslNode node)
		{
			XslFlags xslFlags = this.ProcessExpr(node.Select);
			this.forEachDepth++;
			foreach (XslNode xslNode in node.Content)
			{
				if (xslNode.NodeType == XslNodeType.Sort)
				{
					xslFlags |= this.Visit(xslNode);
				}
				else
				{
					xslFlags |= this.Visit(xslNode) & ~(XslFlags.Current | XslFlags.Position | XslFlags.Last);
				}
			}
			this.forEachDepth--;
			return xslFlags;
		}

		protected override XslFlags VisitIf(XslNode node)
		{
			return this.ProcessExpr(node.Select) | this.VisitChildren(node);
		}

		protected override XslFlags VisitLiteralAttribute(XslNode node)
		{
			return XslFlags.Rtf | this.ProcessAvt(node.Select) | this.VisitChildren(node);
		}

		protected override XslFlags VisitLiteralElement(XslNode node)
		{
			return XslFlags.Rtf | this.VisitChildren(node);
		}

		protected override XslFlags VisitMessage(XslNode node)
		{
			return (this.VisitChildren(node) & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf)) | XslFlags.SideEffects;
		}

		protected override XslFlags VisitNumber(Number node)
		{
			return XslFlags.Rtf | this.ProcessPattern(node.Count) | this.ProcessPattern(node.From) | ((node.Value != null) ? this.ProcessExpr(node.Value) : XslFlags.Current) | this.ProcessAvt(node.Format) | this.ProcessAvt(node.Lang) | this.ProcessAvt(node.LetterValue) | this.ProcessAvt(node.GroupingSeparator) | this.ProcessAvt(node.GroupingSize);
		}

		protected override XslFlags VisitPI(XslNode node)
		{
			return XslFlags.Rtf | this.ProcessAvt(node.Select) | this.VisitChildren(node);
		}

		protected override XslFlags VisitSort(Sort node)
		{
			return (this.ProcessExpr(node.Select) & ~(XslFlags.Current | XslFlags.Position | XslFlags.Last)) | this.ProcessAvt(node.Lang) | this.ProcessAvt(node.DataType) | this.ProcessAvt(node.Order) | this.ProcessAvt(node.CaseOrder);
		}

		protected override XslFlags VisitText(Text node)
		{
			return XslFlags.Rtf | this.VisitChildren(node);
		}

		protected override XslFlags VisitUseAttributeSet(XslNode node)
		{
			AttributeSet attributeSet;
			if (this.compiler.AttributeSets.TryGetValue(node.Name, out attributeSet) && this.currentTemplate != null)
			{
				if (this.forEachDepth == 0)
				{
					this.revCall0Graph.AddEdge(attributeSet, this.currentTemplate);
				}
				else
				{
					this.revCall1Graph.AddEdge(attributeSet, this.currentTemplate);
				}
			}
			return XslFlags.Rtf | XslFlags.HasCalls;
		}

		protected override XslFlags VisitValueOf(XslNode node)
		{
			return XslFlags.Rtf | this.ProcessExpr(node.Select);
		}

		protected override XslFlags VisitValueOfDoe(XslNode node)
		{
			return XslFlags.Rtf | this.ProcessExpr(node.Select);
		}

		protected override XslFlags VisitParam(VarPar node)
		{
			Template template = this.currentTemplate as Template;
			if (template != null && template.Match != null)
			{
				node.Flags |= XslFlags.MayBeDefault;
				XslAstAnalyzer.ModeName modeName = new XslAstAnalyzer.ModeName(template.Mode, node.Name);
				VarPar varPar;
				if (!this.applyTemplatesParams.TryGetValue(modeName, out varPar))
				{
					varPar = (this.applyTemplatesParams[modeName] = AstFactory.WithParam(node.Name));
				}
				this.dataFlow.AddEdge(varPar, node);
			}
			node.DefValueFlags = this.ProcessVarPar(node);
			return node.DefValueFlags & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf);
		}

		protected override XslFlags VisitVariable(VarPar node)
		{
			node.Flags = this.ProcessVarPar(node);
			return node.Flags & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf);
		}

		protected override XslFlags VisitWithParam(VarPar node)
		{
			node.Flags = this.ProcessVarPar(node);
			return node.Flags & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf);
		}

		private XslFlags ProcessVarPar(VarPar node)
		{
			XslFlags xslFlags;
			if (node.Select != null)
			{
				if (node.Content.Count != 0)
				{
					xslFlags = this.xpathAnalyzer.Analyze(node.Select) | this.VisitChildren(node) | XslFlags.TypeFilter;
					this.typeDonor = null;
				}
				else
				{
					xslFlags = this.xpathAnalyzer.Analyze(node.Select);
					this.typeDonor = this.xpathAnalyzer.TypeDonor;
					if (this.typeDonor != null && node.NodeType != XslNodeType.WithParam)
					{
						this.dataFlow.AddEdge(this.typeDonor, node);
					}
				}
			}
			else if (node.Content.Count != 0)
			{
				xslFlags = XslFlags.Rtf | this.VisitChildren(node);
				this.typeDonor = null;
			}
			else
			{
				xslFlags = XslFlags.String;
				this.typeDonor = null;
			}
			return xslFlags;
		}

		private XslFlags ProcessExpr(string expr)
		{
			return this.xpathAnalyzer.Analyze(expr) & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf);
		}

		private XslFlags ProcessAvt(string avt)
		{
			return this.xpathAnalyzer.AnalyzeAvt(avt) & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf);
		}

		private XslFlags ProcessPattern(string pattern)
		{
			return this.xpathAnalyzer.Analyze(pattern) & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf) & ~(XslFlags.Current | XslFlags.Position | XslFlags.Last);
		}

		private void AddApplyTemplatesEdge(QilName mode, ProtoTemplate dependentTemplate)
		{
			List<ProtoTemplate> list;
			if (!this.revApplyTemplatesGraph.TryGetValue(mode, out list))
			{
				list = new List<ProtoTemplate>();
				this.revApplyTemplatesGraph.Add(mode, list);
			}
			else if (list[list.Count - 1] == dependentTemplate)
			{
				return;
			}
			list.Add(dependentTemplate);
		}

		private void PropagateSideEffectsFlag()
		{
			foreach (ProtoTemplate protoTemplate in this.revCall0Graph.Keys)
			{
				protoTemplate.Flags &= ~XslFlags.Stop;
			}
			foreach (ProtoTemplate protoTemplate2 in this.revCall1Graph.Keys)
			{
				protoTemplate2.Flags &= ~XslFlags.Stop;
			}
			foreach (ProtoTemplate protoTemplate3 in this.revCall0Graph.Keys)
			{
				if ((protoTemplate3.Flags & XslFlags.Stop) == XslFlags.None && (protoTemplate3.Flags & XslFlags.SideEffects) != XslFlags.None)
				{
					this.DepthFirstSearch(protoTemplate3);
				}
			}
			foreach (ProtoTemplate protoTemplate4 in this.revCall1Graph.Keys)
			{
				if ((protoTemplate4.Flags & XslFlags.Stop) == XslFlags.None && (protoTemplate4.Flags & XslFlags.SideEffects) != XslFlags.None)
				{
					this.DepthFirstSearch(protoTemplate4);
				}
			}
		}

		private void DepthFirstSearch(ProtoTemplate t)
		{
			t.Flags |= XslFlags.SideEffects | XslFlags.Stop;
			foreach (ProtoTemplate protoTemplate in this.revCall0Graph.GetAdjList(t))
			{
				if ((protoTemplate.Flags & XslFlags.Stop) == XslFlags.None)
				{
					this.DepthFirstSearch(protoTemplate);
				}
			}
			foreach (ProtoTemplate protoTemplate2 in this.revCall1Graph.GetAdjList(t))
			{
				if ((protoTemplate2.Flags & XslFlags.Stop) == XslFlags.None)
				{
					this.DepthFirstSearch(protoTemplate2);
				}
			}
			Template template = t as Template;
			List<ProtoTemplate> list;
			if (template != null && this.revApplyTemplatesGraph.TryGetValue(template.Mode, out list))
			{
				this.revApplyTemplatesGraph.Remove(template.Mode);
				foreach (ProtoTemplate protoTemplate3 in list)
				{
					if ((protoTemplate3.Flags & XslFlags.Stop) == XslFlags.None)
					{
						this.DepthFirstSearch(protoTemplate3);
					}
				}
			}
		}

		private CompilerScopeManager<VarPar> scope;

		private Compiler compiler;

		private int forEachDepth;

		private XslAstAnalyzer.XPathAnalyzer xpathAnalyzer;

		private ProtoTemplate currentTemplate;

		private VarPar typeDonor;

		private XslAstAnalyzer.Graph<ProtoTemplate> revCall0Graph = new XslAstAnalyzer.Graph<ProtoTemplate>();

		private XslAstAnalyzer.Graph<ProtoTemplate> revCall1Graph = new XslAstAnalyzer.Graph<ProtoTemplate>();

		private Dictionary<Template, Stylesheet> fwdApplyImportsGraph = new Dictionary<Template, Stylesheet>();

		private Dictionary<QilName, List<ProtoTemplate>> revApplyTemplatesGraph = new Dictionary<QilName, List<ProtoTemplate>>();

		private XslAstAnalyzer.Graph<VarPar> dataFlow = new XslAstAnalyzer.Graph<VarPar>();

		private Dictionary<XslAstAnalyzer.ModeName, VarPar> applyTemplatesParams = new Dictionary<XslAstAnalyzer.ModeName, VarPar>();

		internal class Graph<V> : Dictionary<V, List<V>> where V : XslNode
		{
			public IEnumerable<V> GetAdjList(V v)
			{
				List<V> list;
				if (base.TryGetValue(v, out list) && list != null)
				{
					return list;
				}
				return XslAstAnalyzer.Graph<V>.empty;
			}

			public void AddEdge(V v1, V v2)
			{
				if (v1 == v2)
				{
					return;
				}
				List<V> list;
				if (!base.TryGetValue(v1, out list) || list == null)
				{
					list = (base[v1] = new List<V>());
				}
				list.Add(v2);
				if (!base.TryGetValue(v2, out list))
				{
					base[v2] = null;
				}
			}

			public void PropagateFlag(XslFlags flag)
			{
				foreach (V v in base.Keys)
				{
					v.Flags &= ~XslFlags.Stop;
				}
				foreach (V v2 in base.Keys)
				{
					if ((v2.Flags & XslFlags.Stop) == XslFlags.None && (v2.Flags & flag) != XslFlags.None)
					{
						this.DepthFirstSearch(v2, flag);
					}
				}
			}

			private void DepthFirstSearch(V v, XslFlags flag)
			{
				v.Flags |= flag | XslFlags.Stop;
				foreach (V v2 in this.GetAdjList(v))
				{
					if ((v2.Flags & XslFlags.Stop) == XslFlags.None)
					{
						this.DepthFirstSearch(v2, flag);
					}
				}
			}

			private static IList<V> empty = new List<V>().AsReadOnly();
		}

		internal struct ModeName
		{
			public ModeName(QilName mode, QilName name)
			{
				this.Mode = mode;
				this.Name = name;
			}

			public override int GetHashCode()
			{
				return this.Mode.GetHashCode() ^ this.Name.GetHashCode();
			}

			public QilName Mode;

			public QilName Name;
		}

		internal struct NullErrorHelper : IErrorHelper
		{
			public void ReportError(string res, params string[] args)
			{
			}

			public void ReportWarning(string res, params string[] args)
			{
			}
		}

		internal class XPathAnalyzer : IXPathBuilder<XslFlags>
		{
			public VarPar TypeDonor
			{
				get
				{
					return this.typeDonor;
				}
			}

			public XPathAnalyzer(Compiler compiler, CompilerScopeManager<VarPar> scope)
			{
				this.compiler = compiler;
				this.scope = scope;
			}

			public XslFlags Analyze(string xpathExpr)
			{
				this.typeDonor = null;
				if (xpathExpr == null)
				{
					return XslFlags.None;
				}
				XslFlags xslFlags2;
				try
				{
					this.xsltCurrentNeeded = false;
					XPathScanner xpathScanner = new XPathScanner(xpathExpr);
					XslFlags xslFlags = this.xpathParser.Parse(xpathScanner, this, LexKind.Eof);
					if (this.xsltCurrentNeeded)
					{
						xslFlags |= XslFlags.Current;
					}
					xslFlags2 = xslFlags;
				}
				catch (XslLoadException)
				{
					xslFlags2 = XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf | XslFlags.Current | XslFlags.Position | XslFlags.Last;
				}
				return xslFlags2;
			}

			public XslFlags AnalyzeAvt(string source)
			{
				this.typeDonor = null;
				if (source == null)
				{
					return XslFlags.None;
				}
				XslFlags xslFlags2;
				try
				{
					this.xsltCurrentNeeded = false;
					XslFlags xslFlags = XslFlags.None;
					int i = 0;
					while (i < source.Length)
					{
						i = source.IndexOf('{', i);
						if (i == -1)
						{
							break;
						}
						i++;
						if (i < source.Length && source[i] == '{')
						{
							i++;
						}
						else if (i < source.Length)
						{
							XPathScanner xpathScanner = new XPathScanner(source, i);
							xslFlags |= this.xpathParser.Parse(xpathScanner, this, LexKind.RBrace);
							i = xpathScanner.LexStart + 1;
						}
					}
					if (this.xsltCurrentNeeded)
					{
						xslFlags |= XslFlags.Current;
					}
					xslFlags2 = xslFlags & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf);
				}
				catch (XslLoadException)
				{
					xslFlags2 = XslFlags.FocusFilter;
				}
				return xslFlags2;
			}

			private VarPar ResolveVariable(string prefix, string name)
			{
				string text = this.ResolvePrefix(prefix);
				if (text == null)
				{
					return null;
				}
				return this.scope.LookupVariable(name, text);
			}

			private string ResolvePrefix(string prefix)
			{
				if (prefix.Length == 0)
				{
					return string.Empty;
				}
				return this.scope.LookupNamespace(prefix);
			}

			public virtual void StartBuild()
			{
			}

			public virtual XslFlags EndBuild(XslFlags result)
			{
				return result;
			}

			public virtual XslFlags String(string value)
			{
				this.typeDonor = null;
				return XslFlags.String;
			}

			public virtual XslFlags Number(double value)
			{
				this.typeDonor = null;
				return XslFlags.Number;
			}

			public virtual XslFlags Operator(XPathOperator op, XslFlags left, XslFlags right)
			{
				this.typeDonor = null;
				return ((left | right) & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf)) | XslAstAnalyzer.XPathAnalyzer.OperatorType[(int)op];
			}

			public virtual XslFlags Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name)
			{
				this.typeDonor = null;
				if (xpathAxis == XPathAxis.Self && nodeType == XPathNodeType.All && prefix == null && name == null)
				{
					return XslFlags.Node | XslFlags.Current;
				}
				return XslFlags.Nodeset | XslFlags.Current;
			}

			public virtual XslFlags JoinStep(XslFlags left, XslFlags right)
			{
				this.typeDonor = null;
				return (left & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf)) | XslFlags.Nodeset;
			}

			public virtual XslFlags Predicate(XslFlags nodeset, XslFlags predicate, bool isReverseStep)
			{
				this.typeDonor = null;
				return (nodeset & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf)) | XslFlags.Nodeset | (predicate & XslFlags.SideEffects);
			}

			public virtual XslFlags Variable(string prefix, string name)
			{
				this.typeDonor = this.ResolveVariable(prefix, name);
				if (this.typeDonor == null)
				{
					return XslFlags.TypeFilter;
				}
				return XslFlags.None;
			}

			public virtual XslFlags Function(string prefix, string name, IList<XslFlags> args)
			{
				this.typeDonor = null;
				XslFlags xslFlags = XslFlags.None;
				foreach (XslFlags xslFlags2 in args)
				{
					xslFlags |= xslFlags2;
				}
				XslFlags xslFlags3 = XslFlags.None;
				if (prefix.Length == 0)
				{
					XPathBuilder.FunctionInfo<XPathBuilder.FuncId> functionInfo;
					XPathBuilder.FunctionInfo<QilGenerator.FuncId> functionInfo2;
					if (XPathBuilder.FunctionTable.TryGetValue(name, out functionInfo))
					{
						XPathBuilder.FuncId id = functionInfo.id;
						xslFlags3 = XslAstAnalyzer.XPathAnalyzer.XPathFunctionFlags[(int)id];
						if (args.Count == 0 && (id == XPathBuilder.FuncId.LocalName || id == XPathBuilder.FuncId.NamespaceUri || id == XPathBuilder.FuncId.Name || id == XPathBuilder.FuncId.String || id == XPathBuilder.FuncId.Number || id == XPathBuilder.FuncId.StringLength || id == XPathBuilder.FuncId.Normalize))
						{
							xslFlags3 |= XslFlags.Current;
						}
					}
					else if (QilGenerator.FunctionTable.TryGetValue(name, out functionInfo2))
					{
						QilGenerator.FuncId id2 = functionInfo2.id;
						xslFlags3 = XslAstAnalyzer.XPathAnalyzer.XsltFunctionFlags[(int)id2];
						if (id2 == QilGenerator.FuncId.Current)
						{
							this.xsltCurrentNeeded = true;
						}
						else if (id2 == QilGenerator.FuncId.GenerateId && args.Count == 0)
						{
							xslFlags3 |= XslFlags.Current;
						}
					}
				}
				else
				{
					string text = this.ResolvePrefix(prefix);
					if (text == "urn:schemas-microsoft-com:xslt")
					{
						uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
						if (num <= 1033099933U)
						{
							if (num <= 467038368U)
							{
								if (num != 325300801U)
								{
									if (num == 467038368U)
									{
										if (name == "number")
										{
											xslFlags3 = XslFlags.Number;
										}
									}
								}
								else if (name == "format-date")
								{
									xslFlags3 = XslFlags.String;
								}
							}
							else if (num != 999037500U)
							{
								if (num == 1033099933U)
								{
									if (name == "utc")
									{
										xslFlags3 = XslFlags.String;
									}
								}
							}
							else if (name == "local-name")
							{
								xslFlags3 = XslFlags.String;
							}
						}
						else if (num <= 2518485839U)
						{
							if (num != 2056321742U)
							{
								if (num == 2518485839U)
								{
									if (name == "namespace-uri")
									{
										xslFlags3 = XslFlags.String | XslFlags.Current;
									}
								}
							}
							else if (name == "string-compare")
							{
								xslFlags3 = XslFlags.Number;
							}
						}
						else if (num != 3208980016U)
						{
							if (num == 3804234668U)
							{
								if (name == "format-time")
								{
									xslFlags3 = XslFlags.String;
								}
							}
						}
						else if (name == "node-set")
						{
							xslFlags3 = XslFlags.Nodeset;
						}
					}
					else if (text == "http://exslt.org/common")
					{
						if (!(name == "node-set"))
						{
							if (name == "object-type")
							{
								xslFlags3 = XslFlags.String;
							}
						}
						else
						{
							xslFlags3 = XslFlags.Nodeset;
						}
					}
					if (xslFlags3 == XslFlags.None)
					{
						xslFlags3 = XslFlags.TypeFilter;
						if (this.compiler.Settings.EnableScript && text != null)
						{
							XmlExtensionFunction xmlExtensionFunction = this.compiler.Scripts.ResolveFunction(name, text, args.Count, default(XslAstAnalyzer.NullErrorHelper));
							if (xmlExtensionFunction != null)
							{
								XmlQueryType xmlReturnType = xmlExtensionFunction.XmlReturnType;
								if (xmlReturnType == XmlQueryTypeFactory.StringX)
								{
									xslFlags3 = XslFlags.String;
								}
								else if (xmlReturnType == XmlQueryTypeFactory.DoubleX)
								{
									xslFlags3 = XslFlags.Number;
								}
								else if (xmlReturnType == XmlQueryTypeFactory.BooleanX)
								{
									xslFlags3 = XslFlags.Boolean;
								}
								else if (xmlReturnType == XmlQueryTypeFactory.NodeNotRtf)
								{
									xslFlags3 = XslFlags.Node;
								}
								else if (xmlReturnType == XmlQueryTypeFactory.NodeSDod)
								{
									xslFlags3 = XslFlags.Nodeset;
								}
								else if (xmlReturnType == XmlQueryTypeFactory.ItemS)
								{
									xslFlags3 = XslFlags.TypeFilter;
								}
								else if (xmlReturnType == XmlQueryTypeFactory.Empty)
								{
									xslFlags3 = XslFlags.Nodeset;
								}
							}
						}
						xslFlags3 |= XslFlags.SideEffects;
					}
				}
				return (xslFlags & ~(XslFlags.String | XslFlags.Number | XslFlags.Boolean | XslFlags.Node | XslFlags.Nodeset | XslFlags.Rtf)) | xslFlags3;
			}

			private XPathParser<XslFlags> xpathParser = new XPathParser<XslFlags>();

			private CompilerScopeManager<VarPar> scope;

			private Compiler compiler;

			private bool xsltCurrentNeeded;

			private VarPar typeDonor;

			private static XslFlags[] OperatorType = new XslFlags[]
			{
				XslFlags.TypeFilter,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Number,
				XslFlags.Number,
				XslFlags.Number,
				XslFlags.Number,
				XslFlags.Number,
				XslFlags.Number,
				XslFlags.Nodeset
			};

			private static XslFlags[] XPathFunctionFlags = new XslFlags[]
			{
				XslFlags.Number | XslFlags.Last,
				XslFlags.Number | XslFlags.Position,
				XslFlags.Number,
				XslFlags.String,
				XslFlags.String,
				XslFlags.String,
				XslFlags.String,
				XslFlags.Number,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.Nodeset | XslFlags.Current,
				XslFlags.String,
				XslFlags.Boolean,
				XslFlags.Boolean,
				XslFlags.String,
				XslFlags.String,
				XslFlags.String,
				XslFlags.Number,
				XslFlags.String,
				XslFlags.String,
				XslFlags.Boolean | XslFlags.Current,
				XslFlags.Number,
				XslFlags.Number,
				XslFlags.Number,
				XslFlags.Number
			};

			private static XslFlags[] XsltFunctionFlags = new XslFlags[]
			{
				XslFlags.Node,
				XslFlags.Nodeset,
				XslFlags.Nodeset | XslFlags.Current,
				XslFlags.String,
				XslFlags.String,
				XslFlags.String,
				XslFlags.String | XslFlags.Number,
				XslFlags.Boolean,
				XslFlags.Boolean
			};
		}
	}
}
