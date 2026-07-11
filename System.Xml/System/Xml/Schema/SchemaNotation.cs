using System;

namespace System.Xml.Schema
{
	internal sealed class SchemaNotation
	{
		internal SchemaNotation(XmlQualifiedName name)
		{
			this.name = name;
		}

		internal XmlQualifiedName Name
		{
			get
			{
				return this.name;
			}
		}

		internal string SystemLiteral
		{
			get
			{
				return this.systemLiteral;
			}
			set
			{
				this.systemLiteral = value;
			}
		}

		internal string Pubid
		{
			get
			{
				return this.pubid;
			}
			set
			{
				this.pubid = value;
			}
		}

		internal const int SYSTEM = 0;

		internal const int PUBLIC = 1;

		private XmlQualifiedName name;

		private string systemLiteral;

		private string pubid;
	}
}
