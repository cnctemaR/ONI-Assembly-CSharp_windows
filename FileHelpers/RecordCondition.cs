using System;

namespace FileHelpers
{
	public enum RecordCondition
	{
		None,
		IncludeIfContains,
		IncludeIfBegins,
		IncludeIfEnds,
		IncludeIfEnclosed,
		IncludeIfMatchRegex,
		ExcludeIfContains,
		ExcludeIfBegins,
		ExcludeIfEnds,
		ExcludeIfEnclosed,
		ExcludeIfMatchRegex
	}
}
