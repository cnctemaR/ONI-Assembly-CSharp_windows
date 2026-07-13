using System;
using STRINGS;
using UnityEngine;

public class DieselMooCometConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(DieselMooCometConfig.ID, UI.SPACEDESTINATIONS.COMETS.DIESELMOOCOMET.NAME, true);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<LoopingSounds>();
		GassyMooComet gassyMooComet = gameObject.AddOrGet<GassyMooComet>();
		gassyMooComet.massRange = new Vector2(100f, 200f);
		gassyMooComet.EXHAUST_ELEMENT = SimHashes.CarbonDioxide;
		gassyMooComet.temperatureRange = new Vector2(296.15f, 318.15f);
		gassyMooComet.entityDamage = 0;
		gassyMooComet.explosionOreCount = new Vector2I(0, 0);
		gassyMooComet.totalTileDamage = 0f;
		gassyMooComet.splashRadius = 1;
		gassyMooComet.impactSound = "Meteor_GassyMoo_Impact";
		gassyMooComet.flyingSoundID = 4;
		gassyMooComet.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
		gassyMooComet.addTiles = 0;
		gassyMooComet.affectedByDifficulty = false;
		gassyMooComet.lootOnDestroyedByMissile = new string[] { "Meat", "Meat", "Meat" };
		gassyMooComet.destroyOnExplode = false;
		gassyMooComet.craterPrefabs = new string[] { "DieselMoo" };
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.SetElement(SimHashes.Creature, true);
		primaryElement.Temperature = (gassyMooComet.temperatureRange.x + gassyMooComet.temperatureRange.y) / 2f;
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim("meteor_huskymoo_kanim") };
		kbatchedAnimController.isMovable = true;
		kbatchedAnimController.initialAnim = "fall_loop";
		kbatchedAnimController.initialMode = KAnim.PlayMode.Loop;
		kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		gameObject.AddOrGet<KCircleCollider2D>().radius = 0.5f;
		gameObject.AddTag(GameTags.Comet);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "DieselMooComet";
}
