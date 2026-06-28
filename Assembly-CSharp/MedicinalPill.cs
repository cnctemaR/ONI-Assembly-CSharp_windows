using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MedicinalPill : Workable, IGameObjectEffectDescriptor, IConsumableUIItem
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.SetWorkTime(10f);
		this.showProgressBar = false;
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
		this.CreateChore();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		EffectInstance effectInstance = component.Get(this.info.effect);
		if (effectInstance != null)
		{
			effectInstance.startTime = Time.time;
		}
		else
		{
			component.Add(this.info.effect, true);
		}
	}

	private void CreateChore()
	{
		new TakeMedicineChore(this);
	}

	public bool CanBeTakenBy(GameObject consumer)
	{
		Effects component = consumer.GetComponent<Effects>();
		if (component.HasEffect(this.info.effect))
		{
			return false;
		}
		if (this.info.medicineType == MedicineInfo.MedicineType.Booster)
		{
			AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup(consumer);
			return amountInstance != null && amountInstance.value < amountInstance.GetMax();
		}
		Diseases diseases = consumer.GetDiseases();
		if (this.info.medicineType == MedicineInfo.MedicineType.CureAny && diseases.Count > 0)
		{
			return true;
		}
		foreach (DiseaseInstance diseaseInstance in diseases)
		{
			if (this.info.curedDiseases.Contains(diseaseInstance.modifier.Id))
			{
				return true;
			}
		}
		return false;
	}

	public List<Descriptor> EffectDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		switch (this.info.medicineType)
		{
		case MedicineInfo.MedicineType.Booster:
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.BOOSTER, new object[0]), string.Format(DUPLICANTS.DISEASES.MEDICINE.BOOSTER_TOOLTIP, new object[0]), Descriptor.DescriptorType.Effect, false));
			break;
		case MedicineInfo.MedicineType.CureAny:
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_ANY, new object[0]), string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_ANY_TOOLTIP, new object[0]), Descriptor.DescriptorType.Effect, false));
			break;
		case MedicineInfo.MedicineType.CureSpecific:
		{
			List<string> list2 = new List<string>();
			foreach (string text in this.info.curedDiseases)
			{
				list2.Add(Strings.Get("STRINGS.DUPLICANTS.DISEASES." + text.ToUpper() + ".NAME"));
			}
			string text2 = string.Join(",", list2.ToArray());
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES, text2), string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_TOOLTIP, text2), Descriptor.DescriptorType.Effect, false));
			break;
		}
		}
		Effect effect = Db.Get().effects.Get(this.info.effect);
		list.Add(new Descriptor(string.Format("Applies the <style=\"disease\">{0}</style> effect", effect.Name), string.Format("{0}\n{1}", effect.description, Effect.CreateTooltip(effect, true)), Descriptor.DescriptorType.Effect, false));
		return list;
	}

	public new List<Descriptor> GetDescriptors(GameObject go)
	{
		return this.EffectDescriptors(go);
	}

	public string ConsumableId
	{
		get
		{
			return this.PrefabID().Name;
		}
	}

	public string ConsumableName
	{
		get
		{
			return this.GetProperName();
		}
	}

	public int MajorOrder
	{
		get
		{
			return (int)(this.info.medicineType + 1000);
		}
	}

	public int MinorOrder
	{
		get
		{
			return 0;
		}
	}

	public bool Display
	{
		get
		{
			return true;
		}
	}

	public MedicineInfo info;
}
