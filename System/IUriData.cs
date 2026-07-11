using System;

namespace System
{
	internal interface IUriData
	{
		string AbsolutePath { get; }

		string AbsoluteUri { get; }

		string AbsoluteUri_SafeUnescaped { get; }

		string Authority { get; }

		string Fragment { get; }

		string Host { get; }

		string PathAndQuery { get; }

		string StrongPort { get; }

		string Query { get; }

		string UserInfo { get; }
	}
}
