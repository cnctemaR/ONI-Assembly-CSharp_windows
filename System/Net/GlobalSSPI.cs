using System;

namespace System.Net
{
	internal static class GlobalSSPI
	{
		internal static readonly SSPIInterface SSPIAuth = new SSPIAuthType();

		internal static readonly SSPIInterface SSPISecureChannel = new SSPISecureChannelType();
	}
}
