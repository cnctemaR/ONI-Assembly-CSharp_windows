using System;

namespace System.Xml.Xsl
{
	[Flags]
	internal enum XmlNodeKindFlags
	{
		None = 0,
		Document = 1,
		Element = 2,
		Attribute = 4,
		Text = 8,
		Comment = 16,
		PI = 32,
		Namespace = 64,
		Content = 58,
		Any = 127
	}
}
