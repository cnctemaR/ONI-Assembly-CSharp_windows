using System;

namespace FileHelpers.MasterDetail
{
	public enum CommonSelector
	{
		MasterIfContains,
		MasterIfBegins,
		MasterIfEnds,
		MasterIfEnclosed,
		DetailIfContains,
		DetailIfBegins,
		DetailIfEnds,
		DetailIfEnclosed
	}
}
