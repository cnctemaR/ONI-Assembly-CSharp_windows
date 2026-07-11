using System;

namespace System.Runtime.Diagnostics
{
	internal enum EventFacility : uint
	{
		Tracing = 65536U,
		ServiceModel = 131072U,
		TransactionBridge = 196608U,
		SMSvcHost = 262144U,
		InfoCards = 327680U,
		SecurityAudit = 393216U
	}
}
