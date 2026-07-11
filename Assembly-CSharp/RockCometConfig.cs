using System;
using UnityEngine;

public class RockCometConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(RockCometConfig.ID, RockCometConfig.ID, true);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<LoopingSounds>();
		Comet comet = gameObject.AddOrGet<Comet>();
		float mass = ElementLoader.FindElementByHash(SimHashes.Regolith).defaultValues.mass;
		comet.massRange = new Vector2(mass * 0.8f * 6f, mass * 1.2f * 6f);
		comet.temperatureRange = new Vector2(323.15f, 423.15f);
		comet.addTiles = 6;
		comet.dissimilarElementAddTiles = 2;
		comet.entityDamage = 20;
		comet.totalTileDamage = 0.2f;
		comet.splashRadius = 1;
		comet.impactSound = "Meteor_Large_Impact";
		comet.flyingSoundID = 2;
		comet.explosionEffectHash = SpawnFXHashes.MeteorImpactDirt;
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.SetElement(SimHashes.Regolith);
		primaryElement.Temperature = (comet.temperatureRange.x + comet.temperatureRange.y) / 2f;
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim("meteor_rock_kanim") };
		kbatchedAnimController.isMovable = true;
		kbatchedAnimController.initialAnim = "fall_loop";
		kbatchedAnimController.initialMode = KAnim.PlayMode.Loop;
		KCircleCollider2D kcircleCollider2D = gameObject.AddOrGet<KCircleCollider2D>();
		kcircleCollider2D.radius = 0.5f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
		Comet comet = go.AddOrGet<Comet>();
		comet.addTilesMinHeight = Grid.HeightInCells - 35;
		comet.addTilesMaxHeight = Grid.HeightInCells - 15;
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static readonly string ID = "RockComet";

	private const SimHashes element = SimHashes.Regolith;

	private const int ADDED_CELLS = 6;
}
