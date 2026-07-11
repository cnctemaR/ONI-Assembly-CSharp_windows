using System;
using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class Stylesheet
	{
		internal bool Whitespace
		{
			get
			{
				return this.whitespace;
			}
		}

		internal ArrayList Imports
		{
			get
			{
				return this.imports;
			}
		}

		internal Hashtable AttributeSetTable
		{
			get
			{
				return this.attributeSetTable;
			}
		}

		internal void AddSpace(Compiler compiler, string query, double Priority, bool PreserveSpace)
		{
			Stylesheet.WhitespaceElement whitespaceElement;
			if (this.queryKeyTable != null)
			{
				if (this.queryKeyTable.Contains(query))
				{
					whitespaceElement = (Stylesheet.WhitespaceElement)this.queryKeyTable[query];
					whitespaceElement.ReplaceValue(PreserveSpace);
					return;
				}
			}
			else
			{
				this.queryKeyTable = new Hashtable();
				this.whitespaceList = new ArrayList();
			}
			whitespaceElement = new Stylesheet.WhitespaceElement(compiler.AddQuery(query), Priority, PreserveSpace);
			this.queryKeyTable[query] = whitespaceElement;
			this.whitespaceList.Add(whitespaceElement);
		}

		internal void SortWhiteSpace()
		{
			if (this.queryKeyTable != null)
			{
				for (int i = 0; i < this.whitespaceList.Count; i++)
				{
					for (int j = this.whitespaceList.Count - 1; j > i; j--)
					{
						Stylesheet.WhitespaceElement whitespaceElement = (Stylesheet.WhitespaceElement)this.whitespaceList[j - 1];
						Stylesheet.WhitespaceElement whitespaceElement2 = (Stylesheet.WhitespaceElement)this.whitespaceList[j];
						if (whitespaceElement2.Priority < whitespaceElement.Priority)
						{
							this.whitespaceList[j - 1] = whitespaceElement2;
							this.whitespaceList[j] = whitespaceElement;
						}
					}
				}
				this.whitespace = true;
			}
			if (this.imports != null)
			{
				for (int k = this.imports.Count - 1; k >= 0; k--)
				{
					Stylesheet stylesheet = (Stylesheet)this.imports[k];
					if (stylesheet.Whitespace)
					{
						stylesheet.SortWhiteSpace();
						this.whitespace = true;
					}
				}
			}
		}

		internal bool PreserveWhiteSpace(Processor proc, XPathNavigator node)
		{
			if (this.whitespaceList != null)
			{
				int num = this.whitespaceList.Count - 1;
				while (0 <= num)
				{
					Stylesheet.WhitespaceElement whitespaceElement = (Stylesheet.WhitespaceElement)this.whitespaceList[num];
					if (proc.Matches(node, whitespaceElement.Key))
					{
						return whitespaceElement.PreserveSpace;
					}
					num--;
				}
			}
			if (this.imports != null)
			{
				for (int i = this.imports.Count - 1; i >= 0; i--)
				{
					if (!((Stylesheet)this.imports[i]).PreserveWhiteSpace(proc, node))
					{
						return false;
					}
				}
			}
			return true;
		}

		internal void AddAttributeSet(AttributeSetAction attributeSet)
		{
			if (this.attributeSetTable == null)
			{
				this.attributeSetTable = new Hashtable();
			}
			if (!this.attributeSetTable.ContainsKey(attributeSet.Name))
			{
				this.attributeSetTable[attributeSet.Name] = attributeSet;
				return;
			}
			((AttributeSetAction)this.attributeSetTable[attributeSet.Name]).Merge(attributeSet);
		}

		internal void AddTemplate(TemplateAction template)
		{
			XmlQualifiedName xmlQualifiedName = template.Mode;
			if (template.Name != null)
			{
				if (this.templateNameTable.ContainsKey(template.Name))
				{
					throw XsltException.Create("'{0}' is a duplicate template name.", new string[] { template.Name.ToString() });
				}
				this.templateNameTable[template.Name] = template;
			}
			if (template.MatchKey != -1)
			{
				if (this.modeManagers == null)
				{
					this.modeManagers = new Hashtable();
				}
				if (xmlQualifiedName == null)
				{
					xmlQualifiedName = XmlQualifiedName.Empty;
				}
				TemplateManager templateManager = (TemplateManager)this.modeManagers[xmlQualifiedName];
				if (templateManager == null)
				{
					templateManager = new TemplateManager(this, xmlQualifiedName);
					this.modeManagers[xmlQualifiedName] = templateManager;
					if (xmlQualifiedName.IsEmpty)
					{
						this.templates = templateManager;
					}
				}
				int num = this.templateCount + 1;
				this.templateCount = num;
				template.TemplateId = num;
				templateManager.AddTemplate(template);
			}
		}

		internal void ProcessTemplates()
		{
			if (this.modeManagers != null)
			{
				IDictionaryEnumerator enumerator = this.modeManagers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					((TemplateManager)enumerator.Value).ProcessTemplates();
				}
			}
			if (this.imports != null)
			{
				for (int i = this.imports.Count - 1; i >= 0; i--)
				{
					((Stylesheet)this.imports[i]).ProcessTemplates();
				}
			}
		}

		internal void ReplaceNamespaceAlias(Compiler compiler)
		{
			if (this.modeManagers != null)
			{
				IDictionaryEnumerator enumerator = this.modeManagers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TemplateManager templateManager = (TemplateManager)enumerator.Value;
					if (templateManager.templates != null)
					{
						for (int i = 0; i < templateManager.templates.Count; i++)
						{
							((TemplateAction)templateManager.templates[i]).ReplaceNamespaceAlias(compiler);
						}
					}
				}
			}
			if (this.templateNameTable != null)
			{
				IDictionaryEnumerator enumerator2 = this.templateNameTable.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					((TemplateAction)enumerator2.Value).ReplaceNamespaceAlias(compiler);
				}
			}
			if (this.imports != null)
			{
				for (int j = this.imports.Count - 1; j >= 0; j--)
				{
					((Stylesheet)this.imports[j]).ReplaceNamespaceAlias(compiler);
				}
			}
		}

		internal TemplateAction FindTemplate(Processor processor, XPathNavigator navigator, XmlQualifiedName mode)
		{
			TemplateAction templateAction = null;
			if (this.modeManagers != null)
			{
				TemplateManager templateManager = (TemplateManager)this.modeManagers[mode];
				if (templateManager != null)
				{
					templateAction = templateManager.FindTemplate(processor, navigator);
				}
			}
			if (templateAction == null)
			{
				templateAction = this.FindTemplateImports(processor, navigator, mode);
			}
			return templateAction;
		}

		internal TemplateAction FindTemplateImports(Processor processor, XPathNavigator navigator, XmlQualifiedName mode)
		{
			TemplateAction templateAction = null;
			if (this.imports != null)
			{
				for (int i = this.imports.Count - 1; i >= 0; i--)
				{
					templateAction = ((Stylesheet)this.imports[i]).FindTemplate(processor, navigator, mode);
					if (templateAction != null)
					{
						return templateAction;
					}
				}
			}
			return templateAction;
		}

		internal TemplateAction FindTemplate(Processor processor, XPathNavigator navigator)
		{
			TemplateAction templateAction = null;
			if (this.templates != null)
			{
				templateAction = this.templates.FindTemplate(processor, navigator);
			}
			if (templateAction == null)
			{
				templateAction = this.FindTemplateImports(processor, navigator);
			}
			return templateAction;
		}

		internal TemplateAction FindTemplate(XmlQualifiedName name)
		{
			TemplateAction templateAction = null;
			if (this.templateNameTable != null)
			{
				templateAction = (TemplateAction)this.templateNameTable[name];
			}
			if (templateAction == null && this.imports != null)
			{
				for (int i = this.imports.Count - 1; i >= 0; i--)
				{
					templateAction = ((Stylesheet)this.imports[i]).FindTemplate(name);
					if (templateAction != null)
					{
						return templateAction;
					}
				}
			}
			return templateAction;
		}

		internal TemplateAction FindTemplateImports(Processor processor, XPathNavigator navigator)
		{
			TemplateAction templateAction = null;
			if (this.imports != null)
			{
				for (int i = this.imports.Count - 1; i >= 0; i--)
				{
					templateAction = ((Stylesheet)this.imports[i]).FindTemplate(processor, navigator);
					if (templateAction != null)
					{
						return templateAction;
					}
				}
			}
			return templateAction;
		}

		internal Hashtable ScriptObjectTypes
		{
			get
			{
				return this.scriptObjectTypes;
			}
		}

		private ArrayList imports = new ArrayList();

		private Hashtable modeManagers;

		private Hashtable templateNameTable = new Hashtable();

		private Hashtable attributeSetTable;

		private int templateCount;

		private Hashtable queryKeyTable;

		private ArrayList whitespaceList;

		private bool whitespace;

		private Hashtable scriptObjectTypes = new Hashtable();

		private TemplateManager templates;

		private class WhitespaceElement
		{
			internal double Priority
			{
				get
				{
					return this.priority;
				}
			}

			internal int Key
			{
				get
				{
					return this.key;
				}
			}

			internal bool PreserveSpace
			{
				get
				{
					return this.preserveSpace;
				}
			}

			internal WhitespaceElement(int Key, double priority, bool PreserveSpace)
			{
				this.key = Key;
				this.priority = priority;
				this.preserveSpace = PreserveSpace;
			}

			internal void ReplaceValue(bool PreserveSpace)
			{
				this.preserveSpace = PreserveSpace;
			}

			private int key;

			private double priority;

			private bool preserveSpace;
		}
	}
}
