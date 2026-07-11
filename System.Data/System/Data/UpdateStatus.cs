using System;

namespace System.Data
{
	public enum UpdateStatus
	{
		Continue,
		ErrorsOccurred,
		SkipAllRemainingRows = 3,
		SkipCurrentRow = 2
	}
}
