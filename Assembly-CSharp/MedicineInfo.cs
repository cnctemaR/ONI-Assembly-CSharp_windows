using System;

public struct MedicineInfo
{
	public MedicineInfo(string[] curedDiseases, float[] boostMultipliers, MedicinalPill.MedicineType medicineType)
	{
		this.curedDiseases = curedDiseases;
		this.boostMultipliers = boostMultipliers;
		this.medicineType = medicineType;
	}

	public string[] curedDiseases;

	public float[] boostMultipliers;

	public MedicinalPill.MedicineType medicineType;
}
