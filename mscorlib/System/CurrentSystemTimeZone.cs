using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System
{
	[Serializable]
	internal class CurrentSystemTimeZone : TimeZone
	{
		internal CurrentSystemTimeZone()
		{
			this.LocalTimeZone = TimeZoneInfo.Local;
		}

		public override string DaylightName
		{
			get
			{
				return this.LocalTimeZone.DaylightName;
			}
		}

		public override string StandardName
		{
			get
			{
				return this.LocalTimeZone.StandardName;
			}
		}

		public override DaylightTime GetDaylightChanges(int year)
		{
			return this.LocalTimeZone.GetDaylightChanges(year);
		}

		public override TimeSpan GetUtcOffset(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return TimeSpan.Zero;
			}
			return this.LocalTimeZone.GetUtcOffset(dateTime);
		}

		public override bool IsDaylightSavingTime(DateTime dateTime)
		{
			return dateTime.Kind != DateTimeKind.Utc && this.LocalTimeZone.IsDaylightSavingTime(dateTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetTimeZoneData(int year, out long[] data, out string[] names, out bool daylight_inverted);

		private readonly TimeZoneInfo LocalTimeZone;
	}
}
