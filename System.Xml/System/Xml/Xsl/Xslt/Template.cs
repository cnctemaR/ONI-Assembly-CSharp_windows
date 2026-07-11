using System;
using System.Globalization;
using System.Text;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class Template : ProtoTemplate
	{
		public Template(QilName name, string match, QilName mode, double priority, XslVersion xslVer)
			: base(XslNodeType.Template, name, xslVer)
		{
			this.Match = match;
			this.Mode = mode;
			this.Priority = priority;
		}

		public override string GetDebugName()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<xsl:template");
			if (this.Match != null)
			{
				stringBuilder.Append(" match=\"");
				stringBuilder.Append(this.Match);
				stringBuilder.Append('"');
			}
			if (this.Name != null)
			{
				stringBuilder.Append(" name=\"");
				stringBuilder.Append(this.Name.QualifiedName);
				stringBuilder.Append('"');
			}
			if (!double.IsNaN(this.Priority))
			{
				stringBuilder.Append(" priority=\"");
				stringBuilder.Append(this.Priority.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append('"');
			}
			if (this.Mode.LocalName.Length != 0)
			{
				stringBuilder.Append(" mode=\"");
				stringBuilder.Append(this.Mode.QualifiedName);
				stringBuilder.Append('"');
			}
			stringBuilder.Append('>');
			return stringBuilder.ToString();
		}

		public readonly string Match;

		public readonly QilName Mode;

		public readonly double Priority;

		public int ImportPrecedence;

		public int OrderNumber;
	}
}
