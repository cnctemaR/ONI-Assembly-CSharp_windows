using System;

namespace System.EnterpriseServices
{
	[Serializable]
	public enum TransactionIsolationLevel
	{
		Any,
		ReadCommitted = 2,
		ReadUncommitted = 1,
		RepeatableRead = 3,
		Serializable
	}
}
