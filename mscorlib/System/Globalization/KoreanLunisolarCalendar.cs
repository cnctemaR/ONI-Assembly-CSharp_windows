using System;

namespace System.Globalization
{
	[Serializable]
	public class KoreanLunisolarCalendar : EastAsianLunisolarCalendar
	{
		[MonoTODO]
		public KoreanLunisolarCalendar()
			: base(KoreanLunisolarCalendar.era_handler)
		{
		}

		static KoreanLunisolarCalendar()
		{
			KoreanLunisolarCalendar.era_handler.appendEra(1, CCFixed.FromDateTime(new DateTime(1, 1, 1)));
		}

		public override int[] Eras
		{
			get
			{
				return (int[])KoreanLunisolarCalendar.era_handler.Eras.Clone();
			}
		}

		public override int GetEra(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			KoreanLunisolarCalendar.era_handler.EraYear(out num2, num);
			return num2;
		}

		public override DateTime MinSupportedDateTime
		{
			get
			{
				return KoreanLunisolarCalendar.KoreanMin;
			}
		}

		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return KoreanLunisolarCalendar.KoreanMax;
			}
		}

		public const int GregorianEra = 1;

		internal static readonly CCEastAsianLunisolarEraHandler era_handler = new CCEastAsianLunisolarEraHandler();

		private static DateTime KoreanMin = new DateTime(918, 2, 14, 0, 0, 0);

		private static DateTime KoreanMax = new DateTime(2051, 2, 10, 23, 59, 59);
	}
}
