using System;

namespace System.Globalization
{
	[Serializable]
	public class TaiwanLunisolarCalendar : EastAsianLunisolarCalendar
	{
		[MonoTODO]
		public TaiwanLunisolarCalendar()
			: base(TaiwanLunisolarCalendar.era_handler)
		{
		}

		static TaiwanLunisolarCalendar()
		{
			TaiwanLunisolarCalendar.era_handler.appendEra(1, CCFixed.FromDateTime(TaiwanLunisolarCalendar.TaiwanMin), CCFixed.FromDateTime(TaiwanLunisolarCalendar.TaiwanMax));
		}

		public override int[] Eras
		{
			get
			{
				return (int[])TaiwanLunisolarCalendar.era_handler.Eras.Clone();
			}
		}

		public override int GetEra(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			TaiwanLunisolarCalendar.era_handler.EraYear(out num2, num);
			return num2;
		}

		public override DateTime MinSupportedDateTime
		{
			get
			{
				return TaiwanLunisolarCalendar.TaiwanMin;
			}
		}

		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return TaiwanLunisolarCalendar.TaiwanMax;
			}
		}

		private const int TaiwanEra = 1;

		internal static readonly CCEastAsianLunisolarEraHandler era_handler = new CCEastAsianLunisolarEraHandler();

		private static DateTime TaiwanMin = new DateTime(1912, 2, 18);

		private static DateTime TaiwanMax = new DateTime(2051, 2, 10, 23, 59, 59, 999);
	}
}
