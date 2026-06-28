using System;

namespace TUNING
{
	public class CREATURES
	{
		public const int DEFAULT_PROBING_RADIUS = 32;

		public const float INCUBATOR_INCUBATION_RATE = 0.016666668f;

		public class HITPOINTS
		{
			public const float TIER0 = 5f;

			public const float TIER1 = 25f;

			public const float TIER2 = 50f;

			public const float TIER3 = 100f;

			public const float TIER4 = 150f;

			public const float TIER5 = 200f;

			public const float TIER6 = 400f;
		}

		public class TEMPERATURE
		{
			public static float FREEZING_3 = 243f;

			public static float FREEZING_2 = 253f;

			public static float FREEZING_1 = 263f;

			public static float FREEZING = 273f;

			public static float COOL = 283f;

			public static float MODERATE = 293f;

			public static float HOT = 303f;

			public static float HOT_1 = 313f;

			public static float HOT_2 = 323f;

			public static float HOT_3 = 333f;
		}

		public class DOMESTICATION_RATE
		{
			public const float INCREASING = 0.8333333f;

			public const float DECREASING = -0.8333333f;
		}
	}
}
