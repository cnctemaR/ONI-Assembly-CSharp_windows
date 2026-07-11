using System;

namespace System.Xml.Serialization
{
	internal enum TypeFlags
	{
		None,
		Abstract,
		Reference,
		Special = 4,
		CanBeAttributeValue = 8,
		CanBeTextValue = 16,
		CanBeElementValue = 32,
		HasCustomFormatter = 64,
		AmbiguousDataType = 128,
		IgnoreDefault = 512,
		HasIsEmpty = 1024,
		HasDefaultConstructor = 2048,
		XmlEncodingNotRequired = 4096,
		UseReflection = 16384,
		CollapseWhitespace = 32768,
		OptionalValue = 65536,
		CtorInaccessible = 131072,
		UsePrivateImplementation = 262144,
		GenericInterface = 524288,
		Unsupported = 1048576
	}
}
