using System;

namespace System
{
	public class FtpStyleUriParser : UriParser
	{
		public FtpStyleUriParser()
			: base(UriParser.FtpUri.Flags)
		{
		}
	}
}
