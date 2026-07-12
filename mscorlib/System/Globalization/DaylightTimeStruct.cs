using System;

namespace System.Globalization
{
	internal readonly struct DaylightTimeStruct
	{
		public DaylightTimeStruct(DateTime start, DateTime end, TimeSpan delta)
		{
			this.Start = start;
			this.End = end;
			this.Delta = delta;
		}

		public readonly DateTime Start;

		public readonly DateTime End;

		public readonly TimeSpan Delta;
	}
}
