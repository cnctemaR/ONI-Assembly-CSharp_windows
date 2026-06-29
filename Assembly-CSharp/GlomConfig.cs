using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class GlomConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = global::STRINGS.CREATURES.SPECIES.GLOM.NAME;
		string text2 = "Glom";
		string text3 = text;
		string text4 = global::STRINGS.CREATURES.SPECIES.GLOM.DESC;
		float num = 25f;
		KAnimFile anim = Assets.GetAnim("glom_kanim");
		string text5 = "idle_loop";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text2, text3, text4, num, anim, text5, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		Trait trait = Db.Get().CreateTrait("GlomBaseTrait", text, text, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, text, false, false, true));
		gameObject.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Creatures.GroundBased);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, "GlomBaseTrait", "HatchNavGrid", NavType.Floor, 32, 2f, string.Empty, 0, true, true, 30f, 293.15f, 393.15f, 273.15f, 423.15f);
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<NotCapturable>();
		gameObject.AddOrGet<FloorSwitchActivator>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		ElementDropperMonitor.Def def = gameObject.AddOrGetDef<ElementDropperMonitor.Def>();
		def.dirtyEmitElement = SimHashes.ContaminatedOxygen;
		def.dirtyProbabilityPercent = 25f;
		def.dirtyCellToTargetMass = 1f;
		def.dirtyMassPerDirty = 0.2f;
		def.dirtyMassReleaseOnDeath = 3f;
		def.emitDiseaseIdx = Db.Get().Diseases.GetIndex("SlimeLung");
		def.emitDiseasePerKg = 1000f;
		gameObject.AddOrGet<LoopingSounds>();
		LoopingSounds component = gameObject.GetComponent<LoopingSounds>();
		component.updatePosition = true;
		DiseaseSourceVisualizer diseaseSourceVisualizer = gameObject.AddOrGet<DiseaseSourceVisualizer>();
		diseaseSourceVisualizer.alwaysShowDisease = "SlimeLung";
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_movement_short", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_jump", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_land", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_expel", NOISE_POLLUTION.CREATURES.TIER4);
		EntityTemplates.CreateAndRegisterPreview("Glom_Preview", Assets.GetAnim("glom_kanim"), "idle_loop", ObjectLayer.NumLayers, 1, 1);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, string.Format(global::STRINGS.CREATURES.BAGGED_NAME_FMT, global::STRINGS.CREATURES.SPECIES.GLOM.NAME), string.Format(global::STRINGS.CREATURES.BAGGED_DESC_FMT, global::STRINGS.CREATURES.SPECIES.GLOM.NAME), Assets.GetAnim("creature_interacts_trap_glom_kanim"), "working_pre", new Tag("Glom_Preview"), true);
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new TrappedStates.Def(), true).Add(new FallStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DrowningStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new FleeStates.Def(), true)
			.Add(new DropElementStates.Def(), true)
			.Add(new IdleStates.Def(), true);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.GlomSpecies, null);
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

	public const string BASE_TRAIT_ID = "GlomBaseTrait";

	public const SimHashes dirtyEmitElement = SimHashes.ContaminatedOxygen;

	public const float dirtyProbabilityPercent = 25f;

	public const float dirtyCellToTargetMass = 1f;

	public const float dirtyMassPerDirty = 0.2f;

	public const float dirtyMassReleaseOnDeath = 3f;

	public const string emitDisease = "SlimeLung";

	public const int emitDiseasePerKg = 1000;
}
