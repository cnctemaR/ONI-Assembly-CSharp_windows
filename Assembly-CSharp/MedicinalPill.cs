using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MedicinalPill : Workable, IGameObjectEffectDescriptor
{
	public int DescriptionOrder { get; set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
		this.CreateChore();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Klei.AI.Diseases diseases = worker.GetComponent<MinionModifiers>().diseases;
		global::Database.Diseases diseases2 = Db.Get().Diseases;
		for (int i = 0; i < this.curedDiseases.Length; i++)
		{
			string text = this.curedDiseases[i];
			for (int j = 0; j < diseases2.Count; j++)
			{
				if (Klei.AI.Diseases.CanCure(text, diseases2[j].Id))
				{
					Disease disease = diseases2[j];
					if (this.medicineType == MedicinalPill.MedicineType.InstantCure)
					{
						diseases.Cure(disease);
					}
					else
					{
						diseases.ApplyPill(disease, base.gameObject.GetProperName(), this.boostMultipliers[i]);
					}
					break;
				}
			}
		}
	}

	private void CreateChore()
	{
		new TakeMedicineChore(this);
	}

	public List<Descriptor> GetRequirementDescriptions(GameObject go)
	{
		return null;
	}

	public List<string> GetEffectDescriptions(GameObject go)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (string text in this.curedDiseases)
		{
			list2.Add(Strings.Get("STRINGS.DUPLICANTS.DISEASES." + text.ToUpper() + ".NAME"));
		}
		if (this.medicineType == MedicinalPill.MedicineType.InstantCure)
		{
			string text2 = string.Empty;
			for (int j = 0; j < this.curedDiseases.Length; j++)
			{
				text2 += list2[j];
				if (j < this.curedDiseases.Length - 1)
				{
					text2 += ", ";
				}
			}
			list.Add(string.Format(DUPLICANTS.DISEASES.CURES, text2));
		}
		else
		{
			for (int k = 0; k < this.curedDiseases.Length; k++)
			{
				float num = (this.boostMultipliers[k] - 1f) * 100f;
				if (num >= 0f)
				{
					list.Add(string.Format(DUPLICANTS.DISEASES.BOOSTSCURESPEED, list2[k], GameUtil.GetFormattedPercent(num, GameUtil.TimeSlice.None)));
				}
				else
				{
					list.Add(string.Format(DUPLICANTS.DISEASES.REDUCECURESPEED, list2[k], GameUtil.GetFormattedPercent(num, GameUtil.TimeSlice.None)));
				}
			}
		}
		return list;
	}

	public string[] curedDiseases;

	public float[] boostMultipliers;

	public MedicinalPill.MedicineType medicineType;

	public enum MedicineType
	{
		InstantCure,
		BoostCureSpeed
	}
}
