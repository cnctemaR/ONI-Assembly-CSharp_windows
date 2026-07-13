using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SapTreeConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "SapTree";
		string text2 = global::STRINGS.CREATURES.SPECIES.SAPTREE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SAPTREE.DESC;
		float num = 1f;
		EffectorValues positive_DECOR_EFFECT = SapTreeConfig.POSITIVE_DECOR_EFFECT;
		KAnimFile anim = Assets.GetAnim("gravitas_sap_tree_kanim");
		string text4 = "idle";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingFront;
		int num2 = 5;
		int num3 = 5;
		EffectorValues effectorValues = positive_DECOR_EFFECT;
		List<Tag> list = new List<Tag> { GameTags.Decoration };
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, list, 293f);
		SapTree.Def def = gameObject.AddOrGetDef<SapTree.Def>();
		def.foodSenseArea = new Vector2I(5, 1);
		def.massEatRate = 0.05f;
		def.kcalorieToKGConversionRatio = 0.005f;
		def.stomachSize = 5f;
		def.oozeRate = 2f;
		def.oozeOffsets = new List<Vector3>
		{
			new Vector3(-2f, 2f),
			new Vector3(2f, 1f)
		};
		def.attackSenseArea = new Vector2I(5, 5);
		def.attackCooldown = 5f;
		gameObject.AddOrGet<Storage>();
		FactionAlignment factionAlignment = gameObject.AddOrGet<FactionAlignment>();
		factionAlignment.Alignment = FactionManager.FactionID.Hostile;
		factionAlignment.canBePlayerTargeted = false;
		gameObject.AddOrGet<RangedAttackable>();
		gameObject.AddWeapon(5f, 5f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.AreaOfEffect, 1, 2f);
		gameObject.AddOrGet<WiltCondition>();
		gameObject.AddOrGet<TemperatureVulnerable>().Configure(173.15f, 0f, 373.15f, 1023.15f);
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SapTree";

	public static readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER5;

	private const int WIDTH = 5;

	private const int HEIGHT = 5;

	private const int ATTACK_RADIUS = 2;

	public const float MASS_EAT_RATE = 0.05f;

	public const float KCAL_TO_KG_RATIO = 0.005f;
}
