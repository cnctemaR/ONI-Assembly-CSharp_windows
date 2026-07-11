using System;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class Stylesheet : StylesheetLevel
	{
		public void AddTemplateMatch(Template template, QilLoop filter)
		{
			List<TemplateMatch> list;
			if (!this.TemplateMatches.TryGetValue(template.Mode, out list))
			{
				list = (this.TemplateMatches[template.Mode] = new List<TemplateMatch>());
			}
			list.Add(new TemplateMatch(template, filter));
		}

		public void SortTemplateMatches()
		{
			foreach (QilName qilName in this.TemplateMatches.Keys)
			{
				this.TemplateMatches[qilName].Sort(TemplateMatch.Comparer);
			}
		}

		public Stylesheet(Compiler compiler, int importPrecedence)
		{
			this.compiler = compiler;
			this.importPrecedence = importPrecedence;
			this.WhitespaceRules[0] = new List<WhitespaceRule>();
			this.WhitespaceRules[1] = new List<WhitespaceRule>();
			this.WhitespaceRules[2] = new List<WhitespaceRule>();
		}

		public int ImportPrecedence
		{
			get
			{
				return this.importPrecedence;
			}
		}

		public void AddWhitespaceRule(int index, WhitespaceRule rule)
		{
			this.WhitespaceRules[index].Add(rule);
		}

		public bool AddVarPar(VarPar var)
		{
			using (List<XslNode>.Enumerator enumerator = this.GlobalVarPars.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Name.Equals(var.Name))
					{
						return this.compiler.AllGlobalVarPars.ContainsKey(var.Name);
					}
				}
			}
			this.GlobalVarPars.Add(var);
			return true;
		}

		public bool AddTemplate(Template template)
		{
			template.ImportPrecedence = this.importPrecedence;
			int num = this.orderNumber;
			this.orderNumber = num + 1;
			template.OrderNumber = num;
			this.compiler.AllTemplates.Add(template);
			if (template.Name != null)
			{
				Template template2;
				if (!this.compiler.NamedTemplates.TryGetValue(template.Name, out template2))
				{
					this.compiler.NamedTemplates[template.Name] = template;
				}
				else if (template2.ImportPrecedence == template.ImportPrecedence)
				{
					return false;
				}
			}
			if (template.Match != null)
			{
				this.Templates.Add(template);
			}
			return true;
		}

		private Compiler compiler;

		public List<Uri> ImportHrefs = new List<Uri>();

		public List<XslNode> GlobalVarPars = new List<XslNode>();

		public Dictionary<QilName, AttributeSet> AttributeSets = new Dictionary<QilName, AttributeSet>();

		private int importPrecedence;

		private int orderNumber;

		public List<WhitespaceRule>[] WhitespaceRules = new List<WhitespaceRule>[3];

		public List<Template> Templates = new List<Template>();

		public Dictionary<QilName, List<TemplateMatch>> TemplateMatches = new Dictionary<QilName, List<TemplateMatch>>();
	}
}
