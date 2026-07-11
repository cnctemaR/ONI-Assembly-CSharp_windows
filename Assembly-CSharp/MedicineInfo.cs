using System;
using System.Collections.Generic;

[Serializable]
public class MedicineInfo
{
	public MedicineInfo(string id, string effect, MedicineInfo.MedicineType medicineType, string[] curedDiseases = null)
	{
		Debug.Assert(!string.IsNullOrEmpty(effect) || (curedDiseases != null && curedDiseases.Length > 0), "Medicine should have an effect or cure diseases");
		this.id = id;
		this.effect = effect;
		this.medicineType = medicineType;
		if (curedDiseases != null)
		{
			this.curedSicknesses = new List<string>(curedDiseases);
		}
		else
		{
			this.curedSicknesses = new List<string>();
		}
	}

	public string id;

	public string effect;

	public MedicineInfo.MedicineType medicineType;

	public List<string> curedSicknesses;

	public enum MedicineType
	{
		Booster,
		CureAny,
		CureSpecific
	}
}
