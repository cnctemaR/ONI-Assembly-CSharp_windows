using System;

namespace System.Xml.Serialization
{
	internal enum XmlAttributeFlags
	{
		Enum = 1,
		Array,
		Text = 4,
		ArrayItems = 8,
		Elements = 16,
		Attribute = 32,
		Root = 64,
		Type = 128,
		AnyElements = 256,
		AnyAttribute = 512,
		ChoiceIdentifier = 1024,
		XmlnsDeclarations = 2048
	}
}
