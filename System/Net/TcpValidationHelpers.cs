using System;

namespace System.Net
{
	internal static class TcpValidationHelpers
	{
		public static bool ValidatePortNumber(int port)
		{
			return port >= 0 && port <= 65535;
		}
	}
}
