using System;
using System.Collections.Generic;

[Serializable]
public class MedicineInfo
{
	public MedicineInfo(string id, string effect, MedicineInfo.MedicineType medicineType, string[] curedDiseases = null)
	{
		this.id = id;
		this.effect = effect;
		this.medicineType = medicineType;
		if (curedDiseases != null)
		{
			this.curedDiseases = new List<string>(curedDiseases);
		}
		else
		{
			this.curedDiseases = new List<string>();
		}
	}

	public string id;

	public string effect;

	public MedicineInfo.MedicineType medicineType;

	public List<string> curedDiseases;

	public enum MedicineType
	{
		Booster,
		CureAny,
		CureSpecific
	}
}
