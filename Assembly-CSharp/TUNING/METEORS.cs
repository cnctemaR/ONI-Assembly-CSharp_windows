using System;

namespace TUNING
{
	public class METEORS
	{
		public class DIFFICULTY
		{
			public class PEROID_MULTIPLIER
			{
				public const float INFREQUENT = 2f;

				public const float INTENSE = 1f;

				public const float DOOMED = 1f;
			}

			public class SECONDS_PER_METEOR_MULTIPLIER
			{
				public const float INFREQUENT = 1.5f;

				public const float INTENSE = 0.8f;

				public const float DOOMED = 0.5f;
			}

			public class BOMBARD_OFF_MULTIPLIER
			{
				public const float INFREQUENT = 1f;

				public const float INTENSE = 1f;

				public const float DOOMED = 0.5f;
			}

			public class BOMBARD_ON_MULTIPLIER
			{
				public const float INFREQUENT = 1f;

				public const float INTENSE = 1f;

				public const float DOOMED = 1f;
			}

			public class MASS_MULTIPLIER
			{
				public const float INFREQUENT = 1f;

				public const float INTENSE = 0.8f;

				public const float DOOMED = 0.5f;
			}
		}

		public class IDENTIFY_DURATION
		{
			public const float TIER1 = 20f;
		}

		public class PEROID
		{
			public const float TIER1 = 5f;

			public const float TIER2 = 10f;

			public const float TIER3 = 20f;

			public const float TIER4 = 30f;
		}

		public class DURATION
		{
			public const float TIER0 = 1800f;

			public const float TIER1 = 3000f;

			public const float TIER2 = 4200f;

			public const float TIER3 = 6000f;
		}

		public class DURATION_CLUSTER
		{
			public const float TIER0 = 75f;

			public const float TIER1 = 150f;

			public const float TIER2 = 300f;

			public const float TIER3 = 600f;

			public const float TIER4 = 1800f;

			public const float TIER5 = 3000f;
		}

		public class TRAVEL_DURATION
		{
			public const float TIER0 = 600f;

			public const float TIER1 = 3000f;

			public const float TIER2 = 4500f;

			public const float TIER3 = 6000f;

			public const float TIER4 = 12000f;

			public const float TIER5 = 30000f;

			public const float TIER6 = 60000f;
		}

		public class BOMBARDMENT_ON
		{
			public static MathUtil.MinMax NONE = new MathUtil.MinMax(1f, 1f);

			public static MathUtil.MinMax UNLIMITED = new MathUtil.MinMax(10000f, 10000f);

			public static MathUtil.MinMax CYCLE = new MathUtil.MinMax(600f, 600f);
		}

		public class BOMBARDMENT_OFF
		{
			public static MathUtil.MinMax NONE = new MathUtil.MinMax(1f, 1f);
		}

		public class TRAVELDURATION
		{
			public static float TIER0 = 0f;

			public static float TIER1 = 5f;

			public static float TIER2 = 10f;

			public static float TIER3 = 20f;

			public static float TIER4 = 30f;
		}
	}
}
