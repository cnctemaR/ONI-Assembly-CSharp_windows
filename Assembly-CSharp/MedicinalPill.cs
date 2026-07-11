using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/MedicinalPill")]
public class MedicinalPill : Workable, IGameObjectEffectDescriptor, IConsumableUIItem
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.SetWorkTime(10f);
		this.showProgressBar = false;
		this.synchronizeAnims = false;
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
		this.CreateChore();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (!string.IsNullOrEmpty(this.info.effect))
		{
			Effects component = worker.GetComponent<Effects>();
			EffectInstance effectInstance = component.Get(this.info.effect);
			if (effectInstance != null)
			{
				effectInstance.timeRemaining = effectInstance.effect.duration;
			}
			else
			{
				component.Add(this.info.effect, true);
			}
		}
		Sicknesses sicknesses = worker.GetSicknesses();
		foreach (string text in this.info.curedSicknesses)
		{
			SicknessInstance sicknessInstance = sicknesses.Get(text);
			if (sicknessInstance != null)
			{
				Game.Instance.savedInfo.curedDisease = true;
				sicknessInstance.Cure();
			}
		}
		base.gameObject.DeleteObject();
	}

	private void CreateChore()
	{
		new TakeMedicineChore(this);
	}

	public bool CanBeTakenBy(GameObject consumer)
	{
		if (!string.IsNullOrEmpty(this.info.effect))
		{
			Effects component = consumer.GetComponent<Effects>();
			if (component == null || component.HasEffect(this.info.effect))
			{
				return false;
			}
		}
		if (this.info.medicineType == MedicineInfo.MedicineType.Booster)
		{
			return true;
		}
		Sicknesses sicknesses = consumer.GetSicknesses();
		if (this.info.medicineType == MedicineInfo.MedicineType.CureAny && sicknesses.Count > 0)
		{
			return true;
		}
		foreach (SicknessInstance sicknessInstance in sicknesses)
		{
			if (this.info.curedSicknesses.Contains(sicknessInstance.modifier.Id))
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
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.BOOSTER, Array.Empty<object>()), string.Format(DUPLICANTS.DISEASES.MEDICINE.BOOSTER_TOOLTIP, Array.Empty<object>()), Descriptor.DescriptorType.Effect, false));
			break;
		case MedicineInfo.MedicineType.CureAny:
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_ANY, Array.Empty<object>()), string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_ANY_TOOLTIP, Array.Empty<object>()), Descriptor.DescriptorType.Effect, false));
			break;
		case MedicineInfo.MedicineType.CureSpecific:
		{
			List<string> list2 = new List<string>();
			foreach (string text in this.info.curedSicknesses)
			{
				list2.Add(Strings.Get("STRINGS.DUPLICANTS.DISEASES." + text.ToUpper() + ".NAME"));
			}
			string text2 = string.Join(",", list2.ToArray());
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES, text2), string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_TOOLTIP, text2), Descriptor.DescriptorType.Effect, false));
			break;
		}
		}
		if (!string.IsNullOrEmpty(this.info.effect))
		{
			Effect effect = Db.Get().effects.Get(this.info.effect);
			list.Add(new Descriptor(string.Format(DUPLICANTS.MODIFIERS.MEDICINE_GENERICPILL.EFFECT_DESC, effect.Name), string.Format("{0}\n{1}", effect.description, Effect.CreateTooltip(effect, true, "\n")), Descriptor.DescriptorType.Effect, false));
		}
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
