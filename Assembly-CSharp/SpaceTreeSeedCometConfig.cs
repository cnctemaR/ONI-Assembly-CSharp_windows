using System;
using STRINGS;
using UnityEngine;

public class SpaceTreeSeedCometConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(SpaceTreeSeedCometConfig.ID, UI.SPACEDESTINATIONS.COMETS.SPACETREESEEDCOMET.NAME, true);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<LoopingSounds>();
		SpaceTreeSeededComet spaceTreeSeededComet = gameObject.AddOrGet<SpaceTreeSeededComet>();
		spaceTreeSeededComet.massRange = new Vector2(50f, 100f);
		spaceTreeSeededComet.temperatureRange = new Vector2(253.15f, 263.15f);
		spaceTreeSeededComet.explosionTemperatureRange = spaceTreeSeededComet.temperatureRange;
		spaceTreeSeededComet.impactSound = "Meteor_copper_Impact";
		spaceTreeSeededComet.flyingSoundID = 5;
		spaceTreeSeededComet.EXHAUST_ELEMENT = SimHashes.Void;
		spaceTreeSeededComet.explosionEffectHash = SpawnFXHashes.None;
		spaceTreeSeededComet.entityDamage = 0;
		spaceTreeSeededComet.totalTileDamage = 0f;
		spaceTreeSeededComet.splashRadius = 1;
		spaceTreeSeededComet.addTiles = 3;
		spaceTreeSeededComet.addTilesMinHeight = 1;
		spaceTreeSeededComet.addTilesMaxHeight = 2;
		spaceTreeSeededComet.lootOnDestroyedByMissile = new string[] { "SpaceTreeSeed" };
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.SetElement(SimHashes.Snow, true);
		primaryElement.Temperature = (spaceTreeSeededComet.temperatureRange.x + spaceTreeSeededComet.temperatureRange.y) / 2f;
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim("meteor_bonbon_snow_kanim") };
		kbatchedAnimController.isMovable = true;
		kbatchedAnimController.initialAnim = "fall_loop";
		kbatchedAnimController.initialMode = KAnim.PlayMode.Loop;
		kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		gameObject.AddOrGet<KCircleCollider2D>().radius = 0.5f;
		gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
		gameObject.AddTag(GameTags.Comet);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "SpaceTreeSeedComet";
}
