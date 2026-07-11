using System;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class PatternBag
	{
		public void Clear()
		{
			this.FixedNamePatterns.Clear();
			this.FixedNamePatternsNames.Clear();
			this.NonFixedNamePatterns.Clear();
		}

		public void Add(Pattern pattern)
		{
			QilName qname = pattern.Match.QName;
			List<Pattern> list;
			if (qname == null)
			{
				list = this.NonFixedNamePatterns;
			}
			else if (!this.FixedNamePatterns.TryGetValue(qname, out list))
			{
				this.FixedNamePatternsNames.Add(qname);
				list = (this.FixedNamePatterns[qname] = new List<Pattern>());
			}
			list.Add(pattern);
		}

		public Dictionary<QilName, List<Pattern>> FixedNamePatterns = new Dictionary<QilName, List<Pattern>>();

		public List<QilName> FixedNamePatternsNames = new List<QilName>();

		public List<Pattern> NonFixedNamePatterns = new List<Pattern>();
	}
}
