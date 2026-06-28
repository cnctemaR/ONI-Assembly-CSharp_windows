using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class SeedProducer : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public void Configure(string SeedID, SeedProducer.ProductionType productionType, int newSeedsProduced = 1)
	{
		this.seedInfo.seedId = SeedID;
		this.seedInfo.productionType = productionType;
		this.seedInfo.newSeedsProduced = newSeedsProduced;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(-216549700, new Action<object>(this.DropSeed));
		this.Subscribe(1623392196, new Action<object>(this.DropSeed));
		this.Subscribe(591871899, new Action<object>(this.CropDepleted));
		this.Subscribe(-1072826864, new Action<object>(this.CropPicked));
	}

	public GameObject ProduceSeed(string seedId, int units = 1)
	{
		if (seedId != null && units > 0)
		{
			Vector3 vector = base.gameObject.transform.position + new Vector3(0f, 0.5f, 0f);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag(seedId)), vector, Grid.SceneLayer.Use, SceneOrganizer.Instance.GetFolder(Folder.Entities), null, 0);
			PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			component2.Temperature = component.Temperature;
			component2.Units = (float)units;
			this.Trigger(472291861, gameObject.GetComponent<PlantableSeed>());
			gameObject.SetActive(true);
			return gameObject;
		}
		return null;
	}

	public void DropSeed(object data)
	{
		if (this.droppedSeedAlready)
		{
			return;
		}
		GameObject gameObject = this.ProduceSeed(this.seedInfo.seedId, 1);
		this.Trigger(-1736624145, gameObject.GetComponent<PlantableSeed>());
		this.droppedSeedAlready = true;
	}

	public void CropDepleted(object data)
	{
		this.droppedSeedAlready = true;
		List<IYieldEffect> list = (List<IYieldEffect>)data;
		SeedProducer.SeedInfo seedInfo = this.seedInfo;
		if (list != null)
		{
			foreach (IYieldEffect yieldEffect in list)
			{
				seedInfo = yieldEffect.ApplyToSeed(base.gameObject, seedInfo, this.seedInfo);
			}
		}
		if (seedInfo.productionType == SeedProducer.ProductionType.FinalHarvest)
		{
			this.ProduceSeed(seedInfo.seedId, seedInfo.newSeedsProduced);
		}
	}

	public void CropPicked(object data)
	{
		List<IYieldEffect> list = (List<IYieldEffect>)data;
		SeedProducer.SeedInfo seedInfo = this.seedInfo;
		if (list != null)
		{
			foreach (IYieldEffect yieldEffect in list)
			{
				seedInfo = yieldEffect.ApplyToSeed(base.gameObject, seedInfo, this.seedInfo);
			}
		}
		if (seedInfo.productionType == SeedProducer.ProductionType.Harvest)
		{
			this.ProduceSeed(seedInfo.seedId, seedInfo.newSeedsProduced);
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		string text = "Seed";
		string text2 = string.Empty;
		GameObject prefab = Assets.GetPrefab(new Tag(this.seedInfo.seedId));
		if (prefab != null)
		{
			text = prefab.GetProperName();
			InfoDescription component = prefab.GetComponent<InfoDescription>();
			if (component)
			{
				text2 = component.description;
			}
		}
		SeedProducer.SeedInfo seedInfo = this.seedInfo;
		SeedProducer.SeedInfo seedInfo2 = this.seedInfo;
		Crop component2 = go.GetComponent<Crop>();
		IYieldEffect[] array = new IYieldEffect[0];
		IYieldEffect[] array2 = new IYieldEffect[0];
		if (component2 != null)
		{
			array = component2.medYieldEffects;
			array2 = component2.highYieldEffects;
		}
		if (array != null)
		{
			foreach (IYieldEffect yieldEffect in array)
			{
				seedInfo = yieldEffect.ApplyToSeed(go, seedInfo, this.seedInfo);
				seedInfo2 = yieldEffect.ApplyToSeed(go, seedInfo2, this.seedInfo);
			}
		}
		if (array2 != null)
		{
			foreach (IYieldEffect yieldEffect2 in array2)
			{
				seedInfo2 = yieldEffect2.ApplyToSeed(go, seedInfo2, this.seedInfo);
			}
		}
		LocString locString = UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED;
		switch (this.seedInfo.productionType)
		{
		default:
			return null;
		case SeedProducer.ProductionType.DigOnly:
			return null;
		case SeedProducer.ProductionType.Harvest:
			if (this.seedInfo.newSeedsProduced > 0)
			{
				locString = ((this.seedInfo.newSeedsProduced != 1) ? UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED : UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED_SINGLE);
				list.Add(new Descriptor(string.Format(locString, text, this.seedInfo.newSeedsProduced), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD_SEED, text2), Descriptor.DescriptorType.HarvestLowYield, false));
			}
			if (seedInfo.newSeedsProduced > 0)
			{
				locString = ((seedInfo.newSeedsProduced != 1) ? UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED : UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED_SINGLE);
				list.Add(new Descriptor(string.Format(locString, text, seedInfo.newSeedsProduced), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD_SEED, text2), Descriptor.DescriptorType.HarvestMedYield, false));
			}
			if (seedInfo2.newSeedsProduced > 0)
			{
				locString = ((seedInfo2.newSeedsProduced != 1) ? UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED : UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED_SINGLE);
				list.Add(new Descriptor(string.Format(locString, text, seedInfo2.newSeedsProduced), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD_SEED, text2), Descriptor.DescriptorType.HarvestHighYield, false));
			}
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_HARVEST, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_HARVEST, Descriptor.DescriptorType.CropHarvest, true));
			break;
		case SeedProducer.ProductionType.FinalHarvest:
			if (this.seedInfo.newSeedsProduced > 0)
			{
				locString = ((this.seedInfo.newSeedsProduced != 1) ? UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED_FINAL_HARVEST : UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED_SINGLE_FINAL_HARVEST);
				list.Add(new Descriptor(string.Format(locString, text, this.seedInfo.newSeedsProduced), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD_SEED_FINAL_HARVEST, text2), Descriptor.DescriptorType.HarvestLowYield, false));
			}
			if (seedInfo.newSeedsProduced > 0)
			{
				locString = ((seedInfo.newSeedsProduced != 1) ? UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED_FINAL_HARVEST : UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED_SINGLE_FINAL_HARVEST);
				list.Add(new Descriptor(string.Format(locString, text, seedInfo.newSeedsProduced), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD_SEED_FINAL_HARVEST, text2), Descriptor.DescriptorType.HarvestMedYield, false));
			}
			if (seedInfo2.newSeedsProduced > 0)
			{
				locString = ((seedInfo2.newSeedsProduced != 1) ? UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED_FINAL_HARVEST : UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_SEED_SINGLE_FINAL_HARVEST);
				list.Add(new Descriptor(string.Format(locString, text, seedInfo2.newSeedsProduced), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD_SEED_FINAL_HARVEST, text2), Descriptor.DescriptorType.HarvestHighYield, false));
			}
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_FINAL_HARVEST, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_FINAL_HARVEST, Descriptor.DescriptorType.CropHarvest, true));
			break;
		case SeedProducer.ProductionType.Fruit:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_FRUIT, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_DIG_ONLY, Descriptor.DescriptorType.CropHarvest, true));
			break;
		}
		return list;
	}

	public SeedProducer.SeedInfo seedInfo;

	[Serialize]
	private bool droppedSeedAlready;

	[Serializable]
	public struct SeedInfo
	{
		public string seedId;

		public SeedProducer.ProductionType productionType;

		public int newSeedsProduced;
	}

	public enum ProductionType
	{
		Hidden,
		DigOnly,
		Harvest,
		FinalHarvest,
		Fruit
	}
}
