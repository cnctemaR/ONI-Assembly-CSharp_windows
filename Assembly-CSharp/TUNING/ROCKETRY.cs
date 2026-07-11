using System;
using UnityEngine;

namespace TUNING
{
	public class ROCKETRY
	{
		public static float MassPentaltyPercentage(float totalMass)
		{
			return 1f - totalMass / ROCKETRY.CalculateMassWithPenalty(totalMass);
		}

		public static float MassFromPenaltyPercentage(float penaltyPercentage = 0.5f)
		{
			return -(1f / Mathf.Pow(penaltyPercentage - 1f, 5f));
		}

		public static float CalculateMassWithPenalty(float realMass)
		{
			float num = Mathf.Pow(realMass / ROCKETRY.MASS_PENALTY_DIVISOR, ROCKETRY.MASS_PENALTY_EXPONENT);
			return Mathf.Max(realMass, num);
		}

		public static float MISSION_DURATION_SCALE = 1800f;

		public static float MASS_PENALTY_EXPONENT = 3f;

		public static float MASS_PENALTY_DIVISOR = 200f;

		public class DESTINATION_THRUST_COSTS
		{
			public static int LOW = 3;

			public static int MID = 5;

			public static int HIGH = 7;

			public static int VERY_HIGH = 9;
		}

		public class MODULE_THRUST_SCORE
		{
			public class ENGINES
			{
				public static int WEAK = 10000;

				public static int MEDIUM = 20000;

				public static int STRONG = 30000;
			}
		}

		public class DESTINATION_ANALYSIS
		{
			public static int DISCOVERED = 50;

			public static int COMPLETE = 100;
		}
	}
}
