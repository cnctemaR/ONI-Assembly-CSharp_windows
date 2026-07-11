using System;
using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class TemplateManager
	{
		internal XmlQualifiedName Mode
		{
			get
			{
				return this.mode;
			}
		}

		internal TemplateManager(Stylesheet stylesheet, XmlQualifiedName mode)
		{
			this.mode = mode;
			this.stylesheet = stylesheet;
		}

		internal void AddTemplate(TemplateAction template)
		{
			if (this.templates == null)
			{
				this.templates = new ArrayList();
			}
			this.templates.Add(template);
		}

		internal void ProcessTemplates()
		{
			if (this.templates != null)
			{
				this.templates.Sort(TemplateManager.s_TemplateComparer);
			}
		}

		internal TemplateAction FindTemplate(Processor processor, XPathNavigator navigator)
		{
			if (this.templates == null)
			{
				return null;
			}
			for (int i = this.templates.Count - 1; i >= 0; i--)
			{
				TemplateAction templateAction = (TemplateAction)this.templates[i];
				int matchKey = templateAction.MatchKey;
				if (matchKey != -1 && processor.Matches(navigator, matchKey))
				{
					return templateAction;
				}
			}
			return null;
		}

		private XmlQualifiedName mode;

		internal ArrayList templates;

		private Stylesheet stylesheet;

		private static TemplateManager.TemplateComparer s_TemplateComparer = new TemplateManager.TemplateComparer();

		private class TemplateComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				TemplateAction templateAction = (TemplateAction)x;
				TemplateAction templateAction2 = (TemplateAction)y;
				if (templateAction.Priority == templateAction2.Priority)
				{
					return templateAction.TemplateId - templateAction2.TemplateId;
				}
				if (templateAction.Priority <= templateAction2.Priority)
				{
					return -1;
				}
				return 1;
			}
		}
	}
}
