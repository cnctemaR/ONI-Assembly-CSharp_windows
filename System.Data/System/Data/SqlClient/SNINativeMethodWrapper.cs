using System;

namespace System.Data.SqlClient
{
	internal static class SNINativeMethodWrapper
	{
		internal enum SniSpecialErrors : uint
		{
			LocalDBErrorCode = 50U,
			MultiSubnetFailoverWithMoreThan64IPs = 47U,
			MultiSubnetFailoverWithInstanceSpecified,
			MultiSubnetFailoverWithNonTcpProtocol,
			MaxErrorValue = 50157U
		}
	}
}
