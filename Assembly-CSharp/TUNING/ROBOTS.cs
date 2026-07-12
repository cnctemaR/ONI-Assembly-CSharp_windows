using System;

namespace TUNING
{
	public class ROBOTS
	{
		public class SCOUTBOT
		{
			public const float CARRY_CAPACITY = 200f;

			public static readonly float DIGGING = 1f;

			public static readonly float CONSTRUCTION = 1f;

			public static readonly float ATHLETICS = 1f;

			public static readonly float HIT_POINTS = 100f;

			public static readonly float BATTERY_DEPLETION_RATE = 30f;

			public static readonly float BATTERY_CAPACITY = ROBOTS.SCOUTBOT.BATTERY_DEPLETION_RATE * 10f * 600f;
		}

		public class MORBBOT
		{
			public const float CARRY_CAPACITY = 400f;

			public const float DIGGING = 1f;

			public const float CONSTRUCTION = 1f;

			public const float ATHLETICS = 3f;

			public static readonly float HIT_POINTS = 100f;

			public const float LIFETIME = 6000f;

			public const float BATTERY_DEPLETION_RATE = 30f;

			public const float BATTERY_CAPACITY = 180000f;

			public const float DECONSTRUCTION_WORK_TIME = 10f;
		}
	}
}
