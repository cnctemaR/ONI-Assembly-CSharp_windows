using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BulbloomConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "Bulbloom";
		string text2 = global::STRINGS.CREATURES.SPECIES.BULBLOOM.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.BULBLOOM.DESC;
		float num = 1f;
		EffectorValues positive_DECOR_EFFECT = BulbloomConfig.POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("bulbloom_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, positive_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 333.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 293.15f, 313.15f, 353.15f, 383.15f, PLANTS.SAFE_ELEMENTS.MurkyWaters, false, 0f, 0.15f, null, false, false, true, false, true, 2400f, 0f, 2200f, "BulbloomOriginal", global::STRINGS.CREATURES.SPECIES.BULBLOOM.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		gameObject.AddOrGetDef<DecorPlantMonitor.Def>();
		prickleGrass.positive_decor_effect = BulbloomConfig.POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = BulbloomConfig.NEGATIVE_DECOR_EFFECT;
		Light2D light2D = gameObject.AddOrGet<Light2D>();
		light2D.Color = new Color(0.4f, 0.5f, 1f, 0.15f);
		light2D.overlayColour = LIGHT2D.LIGHT_OVERLAY;
		light2D.Range = 2f;
		light2D.Direction = LIGHT2D.DEFAULT_DIRECTION;
		light2D.Offset = new Vector2(0.05f, 0.5f);
		light2D.shape = global::LightShape.Circle;
		light2D.drawOverlay = true;
		light2D.Lux = DUPLICANTSTATS.STANDARD.Light.LOW_LIGHT;
		gameObject.AddComponent<PlantGlowController>();
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		string text4 = "BulbloomSeed";
		string text5 = global::STRINGS.CREATURES.SPECIES.SEEDS.BULBLOOM.NAME;
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.BULBLOOM.DESC;
		KAnimFile anim = Assets.GetAnim("seed_bulbloom_kanim");
		string text7 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text8 = global::STRINGS.CREATURES.SPECIES.BULBLOOM.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, this, productionType, text4, text5, text6, anim, text7, num2, list, receptacleDirection, default(Tag), 13, text8, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false), "Bulbloom_preview", Assets.GetAnim("bulbloom_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Bulbloom";

	public const string SEED_ID = "BulbloomSeed";

	public static readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public static readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;
}
