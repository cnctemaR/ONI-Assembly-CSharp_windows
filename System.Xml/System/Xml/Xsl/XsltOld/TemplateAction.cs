using System;
using System.Xml.XPath;
using MS.Internal.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class TemplateAction : TemplateBaseAction
	{
		internal int MatchKey
		{
			get
			{
				return this.matchKey;
			}
		}

		internal XmlQualifiedName Name
		{
			get
			{
				return this.name;
			}
		}

		internal double Priority
		{
			get
			{
				return this.priority;
			}
		}

		internal XmlQualifiedName Mode
		{
			get
			{
				return this.mode;
			}
		}

		internal int TemplateId
		{
			get
			{
				return this.templateId;
			}
			set
			{
				this.templateId = value;
			}
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			if (this.matchKey == -1)
			{
				if (this.name == null)
				{
					throw XsltException.Create("The 'xsl:template' instruction must have the 'match' and/or 'name' attribute present.", Array.Empty<string>());
				}
				if (this.mode != null)
				{
					throw XsltException.Create("An 'xsl:template' element without a 'match' attribute cannot have a 'mode' attribute.", Array.Empty<string>());
				}
			}
			compiler.BeginTemplate(this);
			if (compiler.Recurse())
			{
				this.CompileParameters(compiler);
				base.CompileTemplate(compiler);
				compiler.ToParent();
			}
			compiler.EndTemplate();
			this.AnalyzePriority(compiler);
		}

		internal virtual void CompileSingle(Compiler compiler)
		{
			this.matchKey = compiler.AddQuery("/", false, true, true);
			this.priority = 0.5;
			base.CompileOnceTemplate(compiler);
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.Match))
			{
				this.matchKey = compiler.AddQuery(value, false, true, true);
			}
			else if (Ref.Equal(localName, compiler.Atoms.Name))
			{
				this.name = compiler.CreateXPathQName(value);
			}
			else if (Ref.Equal(localName, compiler.Atoms.Priority))
			{
				this.priority = XmlConvert.ToXPathDouble(value);
				if (double.IsNaN(this.priority) && !compiler.ForwardCompatibility)
				{
					throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "priority", value });
				}
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

		private void AnalyzePriority(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			if (!double.IsNaN(this.priority) || this.matchKey == -1)
			{
				return;
			}
			TheQuery theQuery = compiler.QueryStore[this.MatchKey];
			CompiledXpathExpr compiledQuery = theQuery.CompiledQuery;
			Query query = compiledQuery.QueryTree;
			UnionExpr unionExpr;
			while ((unionExpr = query as UnionExpr) != null)
			{
				TemplateAction templateAction = this.CloneWithoutName();
				compiler.QueryStore.Add(new TheQuery(new CompiledXpathExpr(unionExpr.qy2, compiledQuery.Expression, false), theQuery._ScopeManager));
				templateAction.matchKey = compiler.QueryStore.Count - 1;
				templateAction.priority = unionExpr.qy2.XsltDefaultPriority;
				compiler.AddTemplate(templateAction);
				query = unionExpr.qy1;
			}
			if (compiledQuery.QueryTree != query)
			{
				compiler.QueryStore[this.MatchKey] = new TheQuery(new CompiledXpathExpr(query, compiledQuery.Expression, false), theQuery._ScopeManager);
			}
			this.priority = query.XsltDefaultPriority;
		}

		protected void CompileParameters(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			for (;;)
			{
				switch (input.NodeType)
				{
				case XPathNodeType.Element:
					if (!Ref.Equal(input.NamespaceURI, input.Atoms.UriXsl) || !Ref.Equal(input.LocalName, input.Atoms.Param))
					{
						return;
					}
					compiler.PushNamespaceScope();
					base.AddAction(compiler.CreateVariableAction(VariableType.LocalParameter));
					compiler.PopScope();
					break;
				case XPathNodeType.Text:
					return;
				case XPathNodeType.SignificantWhitespace:
					base.AddEvent(compiler.CreateTextEvent());
					break;
				}
				if (!input.Advance())
				{
					return;
				}
			}
		}

		private TemplateAction CloneWithoutName()
		{
			return new TemplateAction
			{
				containedActions = this.containedActions,
				mode = this.mode,
				variableCount = this.variableCount,
				replaceNSAliasesDone = true
			};
		}

		internal override void ReplaceNamespaceAlias(Compiler compiler)
		{
			if (!this.replaceNSAliasesDone)
			{
				base.ReplaceNamespaceAlias(compiler);
				this.replaceNSAliasesDone = true;
			}
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state != 0)
			{
				if (state != 1)
				{
					return;
				}
				frame.Finished();
				return;
			}
			else
			{
				if (this.variableCount > 0)
				{
					frame.AllocateVariables(this.variableCount);
				}
				if (this.containedActions != null && this.containedActions.Count > 0)
				{
					processor.PushActionFrame(frame);
					frame.State = 1;
					return;
				}
				frame.Finished();
				return;
			}
		}

		private int matchKey = -1;

		private XmlQualifiedName name;

		private double priority = double.NaN;

		private XmlQualifiedName mode;

		private int templateId;

		private bool replaceNSAliasesDone;
	}
}
