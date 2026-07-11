using System;

namespace System.Xml
{
	public interface IXmlLineInfo
	{
		bool HasLineInfo();

		int LineNumber { get; }

		int LinePosition { get; }
	}
}
