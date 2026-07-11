using System;

namespace System.Xml.Schema
{
	[Flags]
	internal enum XsdDateTimeFlags
	{
		DateTime = 1,
		Time = 2,
		Date = 4,
		GYearMonth = 8,
		GYear = 16,
		GMonthDay = 32,
		GDay = 64,
		GMonth = 128,
		XdrDateTimeNoTz = 256,
		XdrDateTime = 512,
		XdrTimeNoTz = 1024,
		AllXsd = 255
	}
}
