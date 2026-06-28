using System;

namespace System.Xml
{
	[Flags]
	internal enum NamespaceHandling
	{
		Default = 0,
		OmitDuplicates = 1
	}
}
