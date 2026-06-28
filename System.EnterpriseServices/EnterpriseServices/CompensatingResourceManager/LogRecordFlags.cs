using System;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	[Flags]
	[Serializable]
	public enum LogRecordFlags
	{
		ForgetTarget = 1,
		WrittenDuringPrepare = 2,
		WrittenDuringCommit = 4,
		WrittenDuringAbort = 8,
		WrittenDurringRecovery = 16,
		WrittenDuringReplay = 32,
		ReplayInProgress = 64
	}
}
