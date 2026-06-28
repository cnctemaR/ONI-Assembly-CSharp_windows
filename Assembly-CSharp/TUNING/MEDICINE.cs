using System;

namespace TUNING
{
	public class MEDICINE
	{
		public const float DEFAULT_MASS = 1f;

		public const float RECUPERATION_DISEASE_MULTIPLIER = 1.1f;

		public const float RECUPERATION_DOCTORED_DISEASE_MULTIPLIER = 1.2f;

		private static string ALLDISEASES = "ALLDISEASES";

		public static readonly MedicineInfo GENERICPILL = new MedicineInfo(new string[] { MEDICINE.ALLDISEASES }, new float[] { 1.01f }, MedicinalPill.MedicineType.BoostCureSpeed);

		public static readonly MedicineInfo VITAMINSUPPLEMENT = new MedicineInfo(new string[0], new float[] { 1.01f }, MedicinalPill.MedicineType.BoostImmunity);

		public static readonly MedicineInfo GINKONUT = new MedicineInfo(new string[0], new float[0], MedicinalPill.MedicineType.InstantCure);

		public static readonly MedicineInfo HERBALREMEDY = new MedicineInfo(new string[0], new float[] { 1.5f }, MedicinalPill.MedicineType.BoostCureSpeed);

		public static readonly MedicineInfo MUSCARINICANTAGONIST = new MedicineInfo(new string[] { "FoodPoisoning" }, new float[0], MedicinalPill.MedicineType.InstantCure);

		public static readonly MedicineInfo PUTRIDODOURMEDICINE = new MedicineInfo(new string[] { "PutridOdour" }, new float[] { 1.5f }, MedicinalPill.MedicineType.BoostCureSpeed);
	}
}
