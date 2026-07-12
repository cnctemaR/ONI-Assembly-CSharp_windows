using System;

namespace System.Drawing.Printing
{
	public enum PaperSourceKind
	{
		Upper = 1,
		Lower,
		Middle,
		Manual,
		Envelope,
		ManualFeed,
		AutomaticFeed,
		TractorFeed,
		SmallFormat,
		LargeFormat,
		LargeCapacity,
		Cassette = 14,
		FormSource,
		Custom = 257
	}
}
