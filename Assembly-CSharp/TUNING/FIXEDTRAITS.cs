using System;

namespace TUNING
{
	public class FIXEDTRAITS
	{
		public class NORTHERNLIGHTS
		{
			public static int NONE = 0;

			public static int ENABLED = 1;

			public static int DEFAULT_VALUE = FIXEDTRAITS.NORTHERNLIGHTS.NONE;

			public class NAME
			{
				public static string NONE = "northernLightsNone";

				public static string ENABLED = "northernLightsOn";

				public static string DEFAULT = FIXEDTRAITS.NORTHERNLIGHTS.NAME.NONE;
			}
		}

		public class LARGEIMPACTORFRAGMENTS
		{
			public static int NONE = 0;

			public static int ALLOWED = 1;

			public static int DEFAULT_VALUE = FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.NONE;

			public class NAME
			{
				public static string NONE = "largeImpactorFragmentsNone";

				public static string ALLOWED = "largeImpactorFragmentsAllowed";

				public static string DEFAULT = FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.NAME.NONE;
			}
		}

		public class SUNLIGHT
		{
			public static int DEFAULT_SPACED_OUT_SUNLIGHT = 40000;

			public static int NONE = 0;

			public static int VERY_VERY_LOW = (int)((float)FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT * 0.25f);

			public static int VERY_LOW = (int)((float)FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT * 0.5f);

			public static int LOW = (int)((float)FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT * 0.75f);

			public static int MED_LOW = (int)((float)FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT * 0.875f);

			public static int MED = FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT;

			public static int MED_HIGH = (int)((float)FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT * 1.25f);

			public static int HIGH = (int)((float)FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT * 1.5f);

			public static int VERY_HIGH = FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT * 2;

			public static int VERY_VERY_HIGH = (int)((float)FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT * 2.5f);

			public static int VERY_VERY_VERY_HIGH = FIXEDTRAITS.SUNLIGHT.DEFAULT_SPACED_OUT_SUNLIGHT * 3;

			public static int DEFAULT_VALUE = FIXEDTRAITS.SUNLIGHT.VERY_HIGH;

			public class NAME
			{
				public static string NONE = "sunlightNone";

				public static string VERY_VERY_LOW = "sunlightVeryVeryLow";

				public static string VERY_LOW = "sunlightVeryLow";

				public static string LOW = "sunlightLow";

				public static string MED_LOW = "sunlightMedLow";

				public static string MED = "sunlightMed";

				public static string MED_HIGH = "sunlightMedHigh";

				public static string HIGH = "sunlightHigh";

				public static string VERY_HIGH = "sunlightVeryHigh";

				public static string VERY_VERY_HIGH = "sunlightVeryVeryHigh";

				public static string VERY_VERY_VERY_HIGH = "sunlightVeryVeryVeryHigh";

				public static string DEFAULT = FIXEDTRAITS.SUNLIGHT.NAME.VERY_HIGH;
			}
		}

		public class COSMICRADIATION
		{
			public static int BASELINE = 250;

			public static int NONE = 0;

			public static int VERY_VERY_LOW = (int)((float)FIXEDTRAITS.COSMICRADIATION.BASELINE * 0.25f);

			public static int VERY_LOW = (int)((float)FIXEDTRAITS.COSMICRADIATION.BASELINE * 0.5f);

			public static int LOW = (int)((float)FIXEDTRAITS.COSMICRADIATION.BASELINE * 0.75f);

			public static int MED_LOW = (int)((float)FIXEDTRAITS.COSMICRADIATION.BASELINE * 0.875f);

			public static int MED = FIXEDTRAITS.COSMICRADIATION.BASELINE;

			public static int MED_HIGH = (int)((float)FIXEDTRAITS.COSMICRADIATION.BASELINE * 1.25f);

			public static int HIGH = (int)((float)FIXEDTRAITS.COSMICRADIATION.BASELINE * 1.5f);

			public static int VERY_HIGH = FIXEDTRAITS.COSMICRADIATION.BASELINE * 2;

			public static int VERY_VERY_HIGH = FIXEDTRAITS.COSMICRADIATION.BASELINE * 3;

			public static int DEFAULT_VALUE = FIXEDTRAITS.COSMICRADIATION.MED;

			public static float TELESCOPE_RADIATION_SHIELDING = 0.5f;

			public class NAME
			{
				public static string NONE = "cosmicRadiationNone";

				public static string VERY_VERY_LOW = "cosmicRadiationVeryVeryLow";

				public static string VERY_LOW = "cosmicRadiationVeryLow";

				public static string LOW = "cosmicRadiationLow";

				public static string MED_LOW = "cosmicRadiationMedLow";

				public static string MED = "cosmicRadiationMed";

				public static string MED_HIGH = "cosmicRadiationMedHigh";

				public static string HIGH = "cosmicRadiationHigh";

				public static string VERY_HIGH = "cosmicRadiationVeryHigh";

				public static string VERY_VERY_HIGH = "cosmicRadiationVeryVeryHigh";

				public static string DEFAULT = FIXEDTRAITS.COSMICRADIATION.NAME.MED;
			}
		}
	}
}
