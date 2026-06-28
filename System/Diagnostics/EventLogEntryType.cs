using System;

namespace System.Diagnostics
{
	public enum EventLogEntryType
	{
		Error = 1,
		Warning,
		Information = 4,
		SuccessAudit = 8,
		FailureAudit = 16
	}
}
