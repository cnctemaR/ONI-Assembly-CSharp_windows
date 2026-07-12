using System;

namespace System.Globalization
{
	[Serializable]
	public class DaylightTime
	{
		public DaylightTime(DateTime start, DateTime end, TimeSpan delta)
		{
			this._start = start;
			this._end = end;
			this._delta = delta;
		}

		public DateTime Start
		{
			get
			{
				return this._start;
			}
		}

		public DateTime End
		{
			get
			{
				return this._end;
			}
		}

		public TimeSpan Delta
		{
			get
			{
				return this._delta;
			}
		}

		private readonly DateTime _start;

		private readonly DateTime _end;

		private readonly TimeSpan _delta;
	}
}
