using System;

namespace TUNING
{
	public class DECOR
	{
		public static int LIT_BONUS = 15;

		public static EffectorValues NONE = new EffectorValues
		{
			amount = 0,
			radius = 0
		};

		public class BONUS
		{
			public static EffectorValues TIER0 = new EffectorValues
			{
				amount = 10,
				radius = 1
			};

			public static EffectorValues TIER1 = new EffectorValues
			{
				amount = 15,
				radius = 2
			};

			public static EffectorValues TIER2 = new EffectorValues
			{
				amount = 20,
				radius = 3
			};

			public static EffectorValues TIER3 = new EffectorValues
			{
				amount = 25,
				radius = 4
			};

			public static EffectorValues TIER4 = new EffectorValues
			{
				amount = 30,
				radius = 5
			};

			public static EffectorValues TIER5 = new EffectorValues
			{
				amount = 35,
				radius = 6
			};
		}

		public class PENALTY
		{
			public static EffectorValues TIER0 = new EffectorValues
			{
				amount = -5,
				radius = 1
			};

			public static EffectorValues TIER1 = new EffectorValues
			{
				amount = -10,
				radius = 2
			};

			public static EffectorValues TIER2 = new EffectorValues
			{
				amount = -15,
				radius = 3
			};

			public static EffectorValues TIER3 = new EffectorValues
			{
				amount = -20,
				radius = 4
			};

			public static EffectorValues TIER4 = new EffectorValues
			{
				amount = -20,
				radius = 5
			};

			public static EffectorValues TIER5 = new EffectorValues
			{
				amount = -25,
				radius = 6
			};
		}
	}
}
