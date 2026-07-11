using System;

namespace System.Net
{
	internal enum WebExceptionInternalStatus
	{
		RequestFatal,
		ServicePointFatal,
		Recoverable,
		Isolated
	}
}
