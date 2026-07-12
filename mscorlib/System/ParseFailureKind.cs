using System;

namespace System
{
	internal enum ParseFailureKind
	{
		None,
		ArgumentNull,
		Format,
		FormatWithParameter,
		FormatWithOriginalDateTime,
		FormatWithFormatSpecifier,
		FormatWithOriginalDateTimeAndParameter,
		FormatBadDateTimeCalendar
	}
}
