using System;

namespace System.Xml.Xsl.Xslt
{
	internal struct Pattern
	{
		public Pattern(TemplateMatch match, int priority)
		{
			this.Match = match;
			this.Priority = priority;
		}

		public readonly TemplateMatch Match;

		public readonly int Priority;
	}
}
