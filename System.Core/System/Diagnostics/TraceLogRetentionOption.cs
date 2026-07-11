using System;

namespace System.Diagnostics
{
	public enum TraceLogRetentionOption
	{
		LimitedCircularFiles = 1,
		LimitedSequentialFiles = 3,
		SingleFileBoundedSize,
		SingleFileUnboundedSize = 2,
		UnlimitedSequentialFiles = 0
	}
}
