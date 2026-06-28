using System;
using Klei;
using STRINGS;
using TUNING;
using UnityEngine;

public class GlomConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Glom", global::STRINGS.CREATURES.SPECIES.GLOM.NAME, global::STRINGS.CREATURES.SPECIES.GLOM.DESC, 25f, Assets.GetAnim("glom_kanim"), "idle", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, 25f, "HatchNavGrid", NavType.Floor, 2f, string.Empty, 0, true, true, 30f, 293.15f, 393.15f, 273.15f, 423.15f);
		Glom glom = gameObject.UpdateComponentRequirement<Glom>(true);
		glom.dirtyEmitElement = SimHashes.ContaminatedOxygen;
		glom.dirtyProbabilityPercent = 25f;
		glom.dirtyCellToTargetMass = 1f;
		glom.dirtyMassPerDirty = 0.2f;
		glom.dirtyMassReleaseOnDeath = 3f;
		glom.emitDiseaseIdx = Db.Get().Diseases.GetIndex("SlimeLung");
		glom.emitDiseasePerKg = 1000;
		gameObject.UpdateComponentRequirement<Trappable>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		LoopingSounds component = gameObject.GetComponent<LoopingSounds>();
		component.updatePosition = true;
		ElementEmitter elementEmitter = gameObject.AddElementEmitter(SimHashes.ContaminatedOxygen, 0f, 0f, SimUtil.DiseaseInfo.Invalid);
		elementEmitter.showDescriptor = false;
		DiseaseSourceVisualizer diseaseSourceVisualizer = gameObject.UpdateComponentRequirement<DiseaseSourceVisualizer>(true);
		diseaseSourceVisualizer.alwaysShowDisease = "SlimeLung";
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_movement_short", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_jump", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_land", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_expel", NOISE_POLLUTION.CREATURES.TIER4);
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterPreview("Glom_Preview", Assets.GetAnim("glom_kanim"), "idle", ObjectLayer.NumLayers, 1, 1);
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, string.Format(global::STRINGS.CREATURES.BAGGED_NAME_FMT, global::STRINGS.CREATURES.SPECIES.GLOM.NAME), string.Format(global::STRINGS.CREATURES.BAGGED_DESC_FMT, global::STRINGS.CREATURES.SPECIES.GLOM.NAME), Assets.GetAnim("creature_interacts_trap_glom_kanim"), "working_pre", new Tag("Glom_Preview"));
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Glom";

	public const string PREVIEW_ID = "Glom_Preview";

	public const SimHashes dirtyEmitElement = SimHashes.ContaminatedOxygen;

	public const float dirtyProbabilityPercent = 25f;

	public const float dirtyCellToTargetMass = 1f;

	public const float dirtyMassPerDirty = 0.2f;

	public const float dirtyMassReleaseOnDeath = 3f;

	public const string emitDisease = "SlimeLung";

	public const int emitDiseasePerKg = 1000;
}
