using System;

namespace TUNING
{
	public class MEDICINE
	{
		public const float DEFAULT_MASS = 1f;

		public const float RECUPERATION_DISEASE_MULTIPLIER = 1.1f;

		private static string ALLDISEASES = "ALLDISEASES";

		private static string DWEEBCHEPHALY = "Dweebcephaly";

		private static string DIARRHEA = "Diarrhea";

		private static string FOODPOISONING = "FoodPoisoning";

		private static string PUTRIDODOUR = "PutridOdour";

		public static readonly MedicineInfo GENERICPILL = new MedicineInfo(new string[] { MEDICINE.ALLDISEASES }, new float[] { 1.01f }, MedicinalPill.MedicineType.BoostCureSpeed);

		public static readonly MedicineInfo GINKONUT = new MedicineInfo(new string[] { MEDICINE.DWEEBCHEPHALY }, new float[0], MedicinalPill.MedicineType.InstantCure);

		public static readonly MedicineInfo HERBALREMEDY = new MedicineInfo(new string[] { MEDICINE.DIARRHEA }, new float[] { 1.5f }, MedicinalPill.MedicineType.BoostCureSpeed);

		public static readonly MedicineInfo MUSCARINICANTAGONIST = new MedicineInfo(new string[] { MEDICINE.FOODPOISONING }, new float[0], MedicinalPill.MedicineType.InstantCure);

		public static readonly MedicineInfo PUTRIDODOURMEDICINE = new MedicineInfo(new string[] { MEDICINE.PUTRIDODOUR }, new float[] { 1.5f }, MedicinalPill.MedicineType.BoostCureSpeed);
	}
}
