using System;

namespace TUNING
{
	public class CREATURES
	{
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

		public class YIELD_BONUS_MODIFIERS
		{
			public const int NUM_PERFECT_MODIFIERS = 4;

			public const float PERFECT_TEMPERATURE = 0.00041666668f;

			public const float PERFECT_PRESSURE = 0.00041666668f;

			public const float FERTILIZED = 0.00041666668f;

			public const float IRRIGATED = 0.00041666668f;

			public const float CONDITION_NOT_MET = 0f;
		}
	}
}
