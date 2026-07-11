using System;

namespace STRINGS
{
	public class ROBOTS
	{
		public static LocString CATEGORY_NAME = "Robots";

		public class STATUSITEMS
		{
			public class CANTREACHSTATION
			{
				public static LocString NAME = "Can't reach base station";

				public static LocString DESC = "Something is blocking this robot from reaching its base station";

				public static LocString TOOLTIP = "Something is blocking this robot from reaching its base station";
			}

			public class LOWBATTERY
			{
				public static LocString NAME = "Low Battery";

				public static LocString DESC = "This robot's battery is low and needs to recharge";

				public static LocString TOOLTIP = "This robot's battery is low and needs to recharge";
			}

			public class DUSTBINFULL
			{
				public static LocString NAME = "Dust Bin Full";

				public static LocString DESC = "This robot must return to its base station to unload";

				public static LocString TOOLTIP = "This robot must return to its base station to unload";
			}
		}

		public class MODELS
		{
			public class SWEEPBOT
			{
				public static LocString NAME = "Sweepy";

				public static LocString DESC = string.Concat(new string[]
				{
					"An automated sweeping robot.\n\nPicks up both ",
					UI.FormatAsLink("Solid", "ELEMENTS_SOLID"),
					" and ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" debris and stores it in a ",
					UI.FormatAsLink("Sweepy Dock", "SWEEPBOTSTATION"),
					"."
				});
			}
		}
	}
}
