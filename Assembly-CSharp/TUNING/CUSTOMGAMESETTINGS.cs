using System;

namespace TUNING
{
	public class CUSTOMGAMESETTINGS
	{
		public class HUNGER
		{
			public const float VERYHARD = 2f;

			public const float HARD = 1.5f;

			public const float EASY = 0.5f;

			public const float DISABLED = 0f;
		}

		public class BIONICWATTAGE
		{
			public const float VERYHARD = 200f;

			public const float HARD = 100f;

			public const float EASY = -100f;

			public const float VERYEASY = -150f;
		}

		public class DEMOLIOR
		{
			public class IMPACT_TIME
			{
				public const float VERYHARD = 100f;

				public const float HARD = 150f;

				public const float DEFAULT = 200f;

				public const float EASY = 300f;

				public const float VERYEASY = 500f;

				public const float IMMINENT = 10f;
			}
		}
	}
}
