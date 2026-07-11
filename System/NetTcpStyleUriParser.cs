using System;

namespace System
{
	public class NetTcpStyleUriParser : UriParser
	{
		public NetTcpStyleUriParser()
			: base(UriParser.NetTcpUri.Flags)
		{
		}
	}
}
