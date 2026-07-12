using System;

namespace Unity.IO.LowLevel.Unsafe
{
	public enum ReadStatus
	{
		Complete,
		InProgress,
		Failed,
		Truncated = 4,
		Canceled
	}
}
