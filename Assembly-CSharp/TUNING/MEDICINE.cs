using System;

namespace TUNING
{
	public class MEDICINE
	{
		public const float DEFAULT_MASS = 1f;

		public const float RECUPERATION_DISEASE_MULTIPLIER = 1.1f;

		public const float RECUPERATION_DOCTORED_DISEASE_MULTIPLIER = 1.2f;

		public const float WORK_TIME = 10f;

		public static readonly MedicineInfo BASICBOOSTER = new MedicineInfo("basicbooster", "Medicine_BasicBooster", MedicineInfo.MedicineType.Booster, null);

		public static readonly MedicineInfo INTERMEDIATEBOOSTER = new MedicineInfo("mediumbooster", "Medicine_IntermediateBooster", MedicineInfo.MedicineType.Booster, null);

		public static readonly MedicineInfo BASICCURE = new MedicineInfo("basiccure", null, MedicineInfo.MedicineType.CureSpecific, new string[] { "FoodSickness" });

		public static readonly MedicineInfo ANTIHISTAMINE = new MedicineInfo("antihistamine", "HistamineSuppression", MedicineInfo.MedicineType.CureSpecific, new string[] { "Allergies" });

		public static readonly MedicineInfo INTERMEDIATECURE = new MedicineInfo("mediumcure", null, MedicineInfo.MedicineType.CureSpecific, new string[] { "SlimeSickness" });

		public static readonly MedicineInfo ADVANCEDCURE = new MedicineInfo("majorcure", null, MedicineInfo.MedicineType.CureSpecific, new string[] { "SlimeSickness" });
	}
}
