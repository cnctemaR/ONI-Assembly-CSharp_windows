using System;

namespace System.Xml.Xsl.Xslt
{
	internal class DecimalFormatDecl
	{
		public DecimalFormatDecl(XmlQualifiedName name, string infinitySymbol, string nanSymbol, string characters)
		{
			this.Name = name;
			this.InfinitySymbol = infinitySymbol;
			this.NanSymbol = nanSymbol;
			this.Characters = characters.ToCharArray();
		}

		public readonly XmlQualifiedName Name;

		public readonly string InfinitySymbol;

		public readonly string NanSymbol;

		public readonly char[] Characters;

		public static DecimalFormatDecl Default = new DecimalFormatDecl(new XmlQualifiedName(), "Infinity", "NaN", ".,%‰0#;-");
	}
}
