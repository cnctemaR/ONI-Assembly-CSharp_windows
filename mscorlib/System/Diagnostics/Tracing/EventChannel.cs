using System;

namespace System.Diagnostics.Tracing
{
	public enum EventChannel : byte
	{
		None,
		Admin = 16,
		Operational,
		Analytic,
		Debug
	}
}
