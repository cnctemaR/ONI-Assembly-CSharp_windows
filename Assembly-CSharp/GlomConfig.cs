using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GlomConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Glom", global::STRINGS.CREATURES.SPECIES.GLOM.NAME, global::STRINGS.CREATURES.SPECIES.GLOM.DESC, 25f, Assets.GetAnim("glom_kanim"), "idle", Grid.SceneLayer.Creatures, 1, 1, tier, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, 25f, "HatchNavGrid", NavType.Floor, 2f, string.Empty, 0, true, true, 30f, 293f, 310f, 283f, 330f);
		gameObject.UpdateComponentRequirement<Glom>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		LoopingSounds component = gameObject.GetComponent<LoopingSounds>();
		component.updatePosition = true;
		gameObject.AddElementEmitter(SimHashes.ContaminatedOxygen, 0f, 0f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const SimHashes dirtyEmitElement = SimHashes.ContaminatedOxygen;

	public const float dirtyProbabilityPercent = 25f;

	public const float dirtyCellToTargetMass = 1f;

	public const float dirtyMassPerDirty = 0.2f;

	public const float dirtyMassReleaseOnDeath = 3f;
}
