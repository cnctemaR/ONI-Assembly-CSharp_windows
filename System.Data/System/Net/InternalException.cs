using System;

namespace System.Net
{
	internal class InternalException : Exception
	{
		internal InternalException()
		{
			NetEventSource.Fail(this, "InternalException thrown.", ".ctor");
		}
	}
}
