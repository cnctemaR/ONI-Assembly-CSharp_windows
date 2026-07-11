using System;

namespace System.Xml.Xsl.Xslt
{
	internal class Output
	{
		public Output()
		{
			this.Settings = new XmlWriterSettings();
			this.Settings.OutputMethod = XmlOutputMethod.AutoDetect;
			this.Settings.AutoXmlDeclaration = true;
			this.Settings.ConformanceLevel = ConformanceLevel.Auto;
			this.Settings.MergeCDataSections = true;
		}

		public XmlWriterSettings Settings;

		public string Version;

		public string Encoding;

		public XmlQualifiedName Method;

		public const int NeverDeclaredPrec = -2147483648;

		public int MethodPrec = int.MinValue;

		public int VersionPrec = int.MinValue;

		public int EncodingPrec = int.MinValue;

		public int OmitXmlDeclarationPrec = int.MinValue;

		public int StandalonePrec = int.MinValue;

		public int DocTypePublicPrec = int.MinValue;

		public int DocTypeSystemPrec = int.MinValue;

		public int IndentPrec = int.MinValue;

		public int MediaTypePrec = int.MinValue;
	}
}
