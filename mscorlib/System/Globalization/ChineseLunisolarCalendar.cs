using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	public class ChineseLunisolarCalendar : EastAsianLunisolarCalendar
	{
		[MonoTODO]
		public ChineseLunisolarCalendar()
			: base(ChineseLunisolarCalendar.era_handler)
		{
		}

		static ChineseLunisolarCalendar()
		{
			ChineseLunisolarCalendar.era_handler.appendEra(1, CCFixed.FromDateTime(new DateTime(1, 1, 1)));
		}

		[ComVisible(false)]
		public override int[] Eras
		{
			get
			{
				return (int[])ChineseLunisolarCalendar.era_handler.Eras.Clone();
			}
		}

		[ComVisible(false)]
		public override int GetEra(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			ChineseLunisolarCalendar.era_handler.EraYear(out num2, num);
			return num2;
		}

		[ComVisible(false)]
		public override DateTime MinSupportedDateTime
		{
			get
			{
				return ChineseLunisolarCalendar.ChineseMin;
			}
		}

		[ComVisible(false)]
		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return ChineseLunisolarCalendar.ChineseMax;
			}
		}

		public const int ChineseEra = 1;

		internal static readonly CCEastAsianLunisolarEraHandler era_handler = new CCEastAsianLunisolarEraHandler();

		private static DateTime ChineseMin = new DateTime(1901, 2, 19);

		private static DateTime ChineseMax = new DateTime(2101, 1, 28, 23, 59, 59, 999);
	}
}
