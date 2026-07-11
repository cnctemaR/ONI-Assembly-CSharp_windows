using System;

namespace System
{
	public class FileStyleUriParser : UriParser
	{
		public FileStyleUriParser()
			: base(UriParser.FileUri.Flags)
		{
		}
	}
}
