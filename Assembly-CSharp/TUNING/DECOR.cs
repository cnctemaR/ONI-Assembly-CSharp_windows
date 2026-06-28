using System;

namespace TUNING
{
	public class DECOR
	{
		public static int LIT_BONUS = 15;

		public class BONUS
		{
			public static DecorValues TIER0 = new DecorValues
			{
				decor = 10,
				radius = 1
			};

			public static DecorValues TIER1 = new DecorValues
			{
				decor = 15,
				radius = 2
			};

			public static DecorValues TIER2 = new DecorValues
			{
				decor = 20,
				radius = 3
			};

			public static DecorValues TIER3 = new DecorValues
			{
				decor = 25,
				radius = 4
			};

			public static DecorValues TIER4 = new DecorValues
			{
				decor = 30,
				radius = 5
			};

			public static DecorValues TIER5 = new DecorValues
			{
				decor = 35,
				radius = 6
			};
		}

		public class PENALTY
		{
			public static DecorValues TIER0 = new DecorValues
			{
				decor = -5,
				radius = 1
			};

			public static DecorValues TIER1 = new DecorValues
			{
				decor = -10,
				radius = 2
			};

			public static DecorValues TIER2 = new DecorValues
			{
				decor = -15,
				radius = 3
			};

			public static DecorValues TIER3 = new DecorValues
			{
				decor = -20,
				radius = 4
			};

			public static DecorValues TIER4 = new DecorValues
			{
				decor = -20,
				radius = 5
			};

			public static DecorValues TIER5 = new DecorValues
			{
				decor = -25,
				radius = 6
			};
		}
	}
}
