using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SeedProducer")]
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
		base.Subscribe<SeedProducer>(-216549700, SeedProducer.DropSeedDelegate);
		base.Subscribe<SeedProducer>(1623392196, SeedProducer.DropSeedDelegate);
		base.Subscribe<SeedProducer>(-1072826864, SeedProducer.CropPickedDelegate);
	}

	private GameObject ProduceSeed(string seedId, int units = 1, bool canMutate = true)
	{
		if (seedId != null && units > 0)
		{
			Vector3 vector = base.gameObject.transform.GetPosition() + new Vector3(0f, 0.5f, 0f);
			GameObject prefab = Assets.GetPrefab(new Tag(seedId));
			GameObject gameObject = GameUtil.KInstantiate(prefab, vector, Grid.SceneLayer.Ore, null, 0);
			MutantPlant component = base.GetComponent<MutantPlant>();
			if (component != null)
			{
				MutantPlant component2 = gameObject.GetComponent<MutantPlant>();
				bool flag = false;
				if (canMutate && component2 != null && component2.IsOriginal)
				{
					flag = this.RollForMutation();
				}
				if (flag)
				{
					component2.Mutate();
				}
				else
				{
					component.CopyMutationsTo(component2);
				}
			}
			PrimaryElement component3 = base.gameObject.GetComponent<PrimaryElement>();
			PrimaryElement component4 = gameObject.GetComponent<PrimaryElement>();
			component4.Temperature = component3.Temperature;
			component4.Units = (float)units;
			base.Trigger(472291861, gameObject);
			gameObject.SetActive(true);
			string text = gameObject.GetProperName();
			if (component != null)
			{
				text = component.GetSubSpeciesInfo().GetNameWithMutations(text, component.IsIdentified, false);
			}
			PopFXManager.Instance.SpawnFX(Def.GetUISprite(prefab, "ui", false).first, PopFXManager.Instance.sprite_Plus, text, gameObject.transform, Vector3.zero, 1.5f, true, false, false);
			return gameObject;
		}
		return null;
	}

	public void DropSeed(object data = null)
	{
		if (this.droppedSeedAlready)
		{
			return;
		}
		if (this.seedInfo.newSeedsProduced <= 0)
		{
			return;
		}
		GameObject gameObject = this.ProduceSeed(this.seedInfo.seedId, this.seedInfo.newSeedsProduced, false);
		Uprootable component = base.GetComponent<Uprootable>();
		if (component != null && component.worker != null)
		{
			gameObject.Trigger(580035959, component.worker);
		}
		base.Trigger(-1736624145, gameObject);
		this.droppedSeedAlready = true;
	}

	public void CropDepleted(object data)
	{
		this.DropSeed(null);
	}

	public void CropPicked(object data)
	{
		if (this.seedInfo.productionType == SeedProducer.ProductionType.Harvest || this.seedInfo.productionType == SeedProducer.ProductionType.HarvestOnly)
		{
			WorkerBase completed_by = base.GetComponent<Harvestable>().completed_by;
			this.CropPickedInternal(completed_by);
		}
	}

	public void SimulateCropPicked(WorkerBase worker)
	{
		this.CropPickedInternal(worker);
	}

	private void CropPickedInternal(WorkerBase worker)
	{
		if (this.seedInfo.productionType == SeedProducer.ProductionType.Harvest || this.seedInfo.productionType == SeedProducer.ProductionType.HarvestOnly)
		{
			float num = this.seedDropChances;
			if (worker != null)
			{
				num += worker.GetComponent<AttributeConverters>().Get(Db.Get().AttributeConverters.SeedHarvestChance).Evaluate();
			}
			num *= this.seedDropChanceMultiplier;
			int num2 = ((global::UnityEngine.Random.Range(0f, 1f) <= num) ? 1 : 0);
			if (num2 > 0)
			{
				this.ProduceSeed(this.seedInfo.seedId, num2, true).Trigger(580035959, worker);
			}
		}
	}

	public bool RollForMutation()
	{
		AttributeInstance attributeInstance = Db.Get().PlantAttributes.MaxRadiationThreshold.Lookup(this);
		int num = Grid.PosToCell(base.gameObject);
		float num2 = Mathf.Clamp(Grid.IsValidCell(num) ? Grid.Radiation[num] : 0f, 0f, attributeInstance.GetTotalValue()) / attributeInstance.GetTotalValue() * 0.8f;
		return global::UnityEngine.Random.value < num2;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Assets.GetPrefab(new Tag(this.seedInfo.seedId)) != null;
		switch (this.seedInfo.productionType)
		{
		case SeedProducer.ProductionType.Hidden:
		case SeedProducer.ProductionType.DigOnly:
		case SeedProducer.ProductionType.Crop:
			return null;
		case SeedProducer.ProductionType.Harvest:
		case SeedProducer.ProductionType.HarvestOnly:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_HARVEST, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_HARVEST, Descriptor.DescriptorType.Lifecycle, true));
			list.Add(new Descriptor(string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.BONUS_SEEDS, GameUtil.GetFormattedPercent(this.seedDropChances * 100f * this.seedDropChanceMultiplier, GameUtil.TimeSlice.None)), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.BONUS_SEEDS, GameUtil.GetFormattedPercent(this.seedDropChances * 100f * this.seedDropChanceMultiplier, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect, false));
			break;
		case SeedProducer.ProductionType.Fruit:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_FRUIT, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_DIG_ONLY, Descriptor.DescriptorType.Lifecycle, true));
			break;
		case SeedProducer.ProductionType.Sterile:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.MUTANT_STERILE, UI.GAMEOBJECTEFFECTS.TOOLTIPS.MUTANT_STERILE, Descriptor.DescriptorType.Effect, false));
			break;
		default:
			DebugUtil.Assert(false, "Seed producer type descriptor not specified");
			return null;
		}
		return list;
	}

	public SeedProducer.SeedInfo seedInfo;

	public float seedDropChanceMultiplier = 1f;

	public float seedDropChances = 0.1f;

	private bool droppedSeedAlready;

	private static readonly EventSystem.IntraObjectHandler<SeedProducer> DropSeedDelegate = new EventSystem.IntraObjectHandler<SeedProducer>(delegate(SeedProducer component, object data)
	{
		if (component.seedInfo.productionType != SeedProducer.ProductionType.HarvestOnly)
		{
			component.DropSeed(data);
		}
	});

	private static readonly EventSystem.IntraObjectHandler<SeedProducer> CropPickedDelegate = new EventSystem.IntraObjectHandler<SeedProducer>(delegate(SeedProducer component, object data)
	{
		component.CropPicked(data);
	});

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
		Fruit,
		Sterile,
		Crop,
		HarvestOnly
	}
}
