using System;
using System.Text;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class Key : XslNode
	{
		public Key(QilName name, string match, string use, XslVersion xslVer)
			: base(XslNodeType.Key, name, null, xslVer)
		{
			this.Match = match;
			this.Use = use;
		}

		public string GetDebugName()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<xsl:key name=\"");
			stringBuilder.Append(this.Name.QualifiedName);
			stringBuilder.Append('"');
			if (this.Match != null)
			{
				stringBuilder.Append(" match=\"");
				stringBuilder.Append(this.Match);
				stringBuilder.Append('"');
			}
			if (this.Use != null)
			{
				stringBuilder.Append(" use=\"");
				stringBuilder.Append(this.Use);
				stringBuilder.Append('"');
			}
			stringBuilder.Append('>');
			return stringBuilder.ToString();
		}

		public readonly string Match;

		public readonly string Use;

		public QilFunction Function;
	}
}
