using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasGrassConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GasGrass";
		string text2 = global::STRINGS.CREATURES.SPECIES.GASGRASS.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.GASGRASS.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER3;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gassygrass_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 3, tier, default(EffectorValues), SimHashes.Creature, null, 255f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 0f, 348.15f, 373.15f, null, false, 0f, 0.15f, "GasGrassHarvested", true, true, true, true, 2400f, 0f, 12200f, "GasGrassOriginal", global::STRINGS.CREATURES.SPECIES.GASGRASS.NAME);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Chlorine,
				massConsumptionRate = 0.00083333335f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Dirt.CreateTag(),
				massConsumptionRate = 0.041666668f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<DirectlyEdiblePlant_Growth>();
		gameObject.AddOrGet<HarvestDesignatable>().defaultHarvestStateWhenPlanted = false;
		Modifiers component = gameObject.GetComponent<Modifiers>();
		Db.Get().traits.Get(component.initialTraits[0]).Add(new AttributeModifier(Db.Get().PlantAttributes.MinLightLux.Id, 10000f, global::STRINGS.CREATURES.SPECIES.GASGRASS.NAME, false, false, true));
		component.initialAttributes.Add(Db.Get().PlantAttributes.MinLightLux.Id);
		gameObject.AddOrGet<IlluminationVulnerable>().SetPrefersDarkness(false);
		GameObject gameObject2 = gameObject;
		IHasDlcRestrictions hasDlcRestrictions = this as IHasDlcRestrictions;
		SeedProducer.ProductionType productionType = (DlcManager.FeaturePlantMutationsEnabled() ? SeedProducer.ProductionType.Harvest : SeedProducer.ProductionType.Hidden);
		string text4 = "GasGrassSeed";
		string text5 = global::STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.NAME;
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.DESC;
		KAnimFile anim = Assets.GetAnim("seed_gassygrass_kanim");
		string text7 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text8 = global::STRINGS.CREATURES.SPECIES.GASGRASS.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, hasDlcRestrictions, productionType, text4, text5, text6, anim, text7, num2, list, receptacleDirection, default(Tag), 22, text8, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f, null, "", false), "GasGrass_preview", Assets.GetAnim("gassygrass_kanim"), "place", 1, 1);
		SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GasGrass";

	public const string SEED_ID = "GasGrassSeed";

	public const float CHLORINE_FERTILIZATION_RATE = 0.00083333335f;

	public const float DIRT_FERTILIZATION_RATE = 0.041666668f;
}
