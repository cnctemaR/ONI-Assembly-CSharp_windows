using System;
using System.Collections.Generic;

[Serializable]
public class MedicineInfo
{
	public MedicineInfo(string id, string effect, MedicineInfo.MedicineType medicineType, string doctorStationId, string[] curedDiseases = null)
		: this(id, effect, medicineType, doctorStationId, curedDiseases, null)
	{
	}

	public MedicineInfo(string id, string effect, MedicineInfo.MedicineType medicineType, string doctorStationId, string[] curedDiseases, string[] curedEffects)
	{
		Debug.Assert(!string.IsNullOrEmpty(effect) || (curedDiseases != null && curedDiseases.Length != 0), "Medicine should have an effect or cure diseases");
		this.id = id;
		this.effect = effect;
		this.medicineType = medicineType;
		this.doctorStationId = doctorStationId;
		if (curedDiseases != null)
		{
			this.curedSicknesses = new List<string>(curedDiseases);
		}
		else
		{
			this.curedSicknesses = new List<string>();
		}
		if (curedEffects != null)
		{
			this.curedEffects = new List<string>(curedEffects);
			return;
		}
		this.curedEffects = new List<string>();
	}

	public Tag GetSupplyTag()
	{
		return MedicineInfo.GetSupplyTagForStation(this.doctorStationId);
	}

	public static Tag GetSupplyTagForStation(string stationID)
	{
		Tag tag = TagManager.Create(stationID + GameTags.MedicalSupplies.Name);
		Assets.AddCountableTag(tag);
		return tag;
	}

	public string id;

	public string effect;

	public MedicineInfo.MedicineType medicineType;

	public List<string> curedSicknesses;

	public List<string> curedEffects;

	public string doctorStationId;

	public enum MedicineType
	{
		Booster,
		CureAny,
		CureSpecific
	}
}
