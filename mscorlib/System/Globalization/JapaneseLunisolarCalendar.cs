using System;

namespace System.Globalization
{
	[Serializable]
	public class JapaneseLunisolarCalendar : EastAsianLunisolarCalendar
	{
		[MonoTODO]
		public JapaneseLunisolarCalendar()
			: base(JapaneseLunisolarCalendar.era_handler)
		{
		}

		static JapaneseLunisolarCalendar()
		{
			JapaneseLunisolarCalendar.era_handler.appendEra(3, CCGregorianCalendar.fixed_from_dmy(25, 12, 1926), CCGregorianCalendar.fixed_from_dmy(7, 1, 1989));
			JapaneseLunisolarCalendar.era_handler.appendEra(4, CCGregorianCalendar.fixed_from_dmy(8, 1, 1989));
		}

		internal override int ActualCurrentEra
		{
			get
			{
				return 4;
			}
		}

		public override int[] Eras
		{
			get
			{
				return (int[])JapaneseLunisolarCalendar.era_handler.Eras.Clone();
			}
		}

		public override int GetEra(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			JapaneseLunisolarCalendar.era_handler.EraYear(out num2, num);
			return num2;
		}

		public override DateTime MinSupportedDateTime
		{
			get
			{
				return JapaneseLunisolarCalendar.JapanMin;
			}
		}

		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return JapaneseLunisolarCalendar.JapanMax;
			}
		}

		public const int JapaneseEra = 1;

		internal static readonly CCEastAsianLunisolarEraHandler era_handler = new CCEastAsianLunisolarEraHandler();

		private static DateTime JapanMin = new DateTime(1960, 1, 28, 0, 0, 0);

		private static DateTime JapanMax = new DateTime(2050, 1, 22, 23, 59, 59);
	}
}
