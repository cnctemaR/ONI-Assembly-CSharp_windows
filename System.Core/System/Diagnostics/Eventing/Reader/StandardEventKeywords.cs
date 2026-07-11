using System;

namespace System.Diagnostics.Eventing.Reader
{
	[Flags]
	public enum StandardEventKeywords : long
	{
		AuditFailure = 4503599627370496L,
		AuditSuccess = 9007199254740992L,
		[Obsolete("Incorrect value: use CorrelationHint2 instead", false)]
		CorrelationHint = 4503599627370496L,
		CorrelationHint2 = 18014398509481984L,
		EventLogClassic = 36028797018963968L,
		None = 0L,
		ResponseTime = 281474976710656L,
		Sqm = 2251799813685248L,
		WdiContext = 562949953421312L,
		WdiDiagnostic = 1125899906842624L
	}
}
