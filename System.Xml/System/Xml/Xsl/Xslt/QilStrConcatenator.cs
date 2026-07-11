using System;
using System.Text;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt
{
	internal class QilStrConcatenator
	{
		public QilStrConcatenator(XPathQilFactory f)
		{
			this.f = f;
			this.builder = new StringBuilder();
		}

		public void Reset()
		{
			this.inUse = true;
			this.builder.Length = 0;
			this.concat = null;
		}

		private void FlushBuilder()
		{
			if (this.concat == null)
			{
				this.concat = this.f.BaseFactory.Sequence();
			}
			if (this.builder.Length != 0)
			{
				this.concat.Add(this.f.String(this.builder.ToString()));
				this.builder.Length = 0;
			}
		}

		public void Append(string value)
		{
			this.builder.Append(value);
		}

		public void Append(char value)
		{
			this.builder.Append(value);
		}

		public void Append(QilNode value)
		{
			if (value != null)
			{
				if (value.NodeType == QilNodeType.LiteralString)
				{
					this.builder.Append((QilLiteral)value);
					return;
				}
				this.FlushBuilder();
				this.concat.Add(value);
			}
		}

		public QilNode ToQil()
		{
			this.inUse = false;
			if (this.concat == null)
			{
				return this.f.String(this.builder.ToString());
			}
			this.FlushBuilder();
			return this.f.StrConcat(this.concat);
		}

		private XPathQilFactory f;

		private StringBuilder builder;

		private QilList concat;

		private bool inUse;
	}
}
