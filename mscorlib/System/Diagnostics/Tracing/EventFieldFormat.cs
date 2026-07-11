using System;

namespace System.Diagnostics.Tracing
{
	public enum EventFieldFormat
	{
		Default,
		String = 2,
		Boolean,
		Hexadecimal,
		Xml = 11,
		Json,
		HResult = 15
	}
}
