using System;

namespace TUNING
{
	public class STRESS
	{
		public static float ACTING_OUT_RESET = 60f;

		public static float VOMIT_AMOUNT = 0.90000004f;

		public static float TEARS_RATE = 0.040000003f;

		public static int BANSHEE_WAIL_RADIUS = 8;

		public class SHOCKER
		{
			public static int SHOCK_RADIUS = 4;

			public static float DAMAGE_RATE = 2.5f;

			public static float POWER_CONSUMPTION_RATE = 2000f;

			public static float FAKE_POWER_CONSUMPTION_RATE = STRESS.SHOCKER.POWER_CONSUMPTION_RATE * 0.25f;

			public static float MAX_POWER_USE = 120000f;
		}
	}
}
