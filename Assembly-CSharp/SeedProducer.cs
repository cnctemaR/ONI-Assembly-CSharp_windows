using System;
using System.Collections.Generic;
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
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, gameObject.GetProperName(), gameObject.transform, 1.5f, false);
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
		GameObject gameObject = this.ProduceSeed(this.seedInfo.seedId, 1);
		this.Trigger(-1736624145, gameObject.GetComponent<PlantableSeed>());
		this.droppedSeedAlready = true;
	}

	public void CropDepleted(object data)
	{
		this.DropSeed(null);
	}

	public void CropPicked(object data)
	{
		if (this.seedInfo.productionType == SeedProducer.ProductionType.Harvest)
		{
			int num = (((float)global::UnityEngine.Random.Range(0, 100) > 33f) ? 0 : 1);
			this.ProduceSeed(this.seedInfo.seedId, num);
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		GameObject prefab = Assets.GetPrefab(new Tag(this.seedInfo.seedId));
		if (prefab != null)
		{
		}
		switch (this.seedInfo.productionType)
		{
		default:
			return null;
		case SeedProducer.ProductionType.DigOnly:
			return null;
		case SeedProducer.ProductionType.Harvest:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_HARVEST, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_HARVEST, Descriptor.DescriptorType.Lifecycle, true));
			break;
		case SeedProducer.ProductionType.Fruit:
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_FRUIT, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_DIG_ONLY, Descriptor.DescriptorType.Lifecycle, true));
			break;
		}
		return list;
	}

	public SeedProducer.SeedInfo seedInfo;

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
		Fruit
	}
}
