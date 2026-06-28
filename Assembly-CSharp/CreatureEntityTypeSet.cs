using System;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class CreatureEntityTypeSet : CommonEntityTypeSet
{
	public CreatureEntityTypeSet(Db modifier_set)
	{
		this.CreateMysteryEgg(modifier_set);
	}

	private EntityType CreateCreature(string id, string name, Db modifier_set)
	{
		EntityType entityType = base.Add(new EntityType(id, name));
		entityType.amounts.Add(Db.Get().Amounts.Stress);
		entityType.amounts.Add(Db.Get().Amounts.Stamina);
		entityType.amounts.Add(Db.Get().Amounts.Calories);
		entityType.amounts.Add(Db.Get().Amounts.Temperature);
		entityType.amounts.Add(Db.Get().Amounts.ExternalTemperature);
		entityType.amounts.Add(Db.Get().Amounts.Breath);
		Trait trait = GameEntityTypeSet.CreateLivingEntityBaseTrait(id, name, -0.16666667f, -0f, -0f, -0.041666668f, 0f, (float)Grid.CellCount, modifier_set);
		entityType.baseTraits.Add(trait);
		return entityType;
	}

	private void CreatePuft(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("Puft", CREATURES.SPECIES.PUFT.NAME, true);
		entityType.prefab.UpdateComponentRequirement<Puft>(true);
		entityType.prefab.UpdateComponentRequirement<Health>(true);
		entityType.prefab.UpdateComponentRequirement<FactionAlignment>(true).Alignment = FactionManager.FactionID.Prey;
		entityType.prefab.UpdateComponentRequirement<RangedAttackable>(true);
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(302f, 295f, 318f, 325f, 0f, 0f);
		entityType.prefab.UpdateComponentRequirement<DrowningMonitor>(true).Configure(30f, 10f, 0.5f);
		entityType.prefab.UpdateComponentRequirement<EntombVulnerable>(true);
		entityType.prefab.UpdateComponentRequirement<LoopingSounds>(true);
		entityType.prefab.AddElementEmitter(SimHashes.SlimeMold, 0f, 0f, SimUtil.DiseaseInfo.Invalid);
		entityType.prefab.AddElementConsumer(SimHashes.ContaminatedOxygen, 0.25f, 0.05f, 3);
		Navigator navigator = entityType.prefab.UpdateComponentRequirement<Navigator>(true);
		navigator.NavGridName = "FlyerNavGrid1x1";
		navigator.CurrentNavType = NavType.Hover;
		navigator.defaultSpeed = 1f;
		navigator.updateProber = true;
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("puft_kanim", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
	}

	private void CreateShockWorm(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("ShockWorm", CREATURES.SPECIES.SHOCKWORM.NAME, true);
		entityType.prefab.UpdateComponentRequirement<Shockworm>(true);
		entityType.prefab.UpdateComponentRequirement<CharacterOverlay>(true);
		entityType.prefab.UpdateComponentRequirement<Health>(true);
		entityType.prefab.UpdateComponentRequirement<FactionAlignment>(true).Alignment = FactionManager.FactionID.Hostile;
		entityType.prefab.UpdateComponentRequirement<RangedAttackable>(true);
		entityType.prefab.UpdateComponentRequirement<LoopingSounds>(true);
		Weapon weapon = entityType.prefab.UpdateComponentRequirement<Weapon>(true);
		weapon.Configure(3f, 6f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.AreaOfEffect, 10, 4f);
		weapon.AddEffect("WasAttacked", 1f);
		Navigator navigator = entityType.prefab.UpdateComponentRequirement<Navigator>(true);
		navigator.NavGridName = "FlyerNavGrid1x2";
		navigator.CurrentNavType = NavType.Hover;
		navigator.defaultSpeed = 2f;
		navigator.updateProber = true;
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(TemperatureTuning.Freezing_1, TemperatureTuning.Freezing_2, TemperatureTuning.Hot_1, TemperatureTuning.Hot_2, 0f, 0f);
		entityType.prefab.UpdateComponentRequirement<DrowningMonitor>(true).Configure(30f, 10f, 0.5f);
		entityType.prefab.UpdateComponentRequirement<EntombVulnerable>(true);
		entityType.prefab.AddButcherable(new string[] { "Meat", "Meat", "Meat" });
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("shockworm_kanim", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
	}

	private void CreateHaunt(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("Haunt", CREATURES.SPECIES.HAUNT.NAME, true);
		entityType.prefab.UpdateComponentRequirement<SimpleMover>(true);
		entityType.prefab.GetComponent<BoxCollider2D>().size = new Vector2(2f, 3f);
		entityType.prefab.GetComponent<BoxCollider2D>().offset = new Vector2(1f, 2f);
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("haunt_kanim", Grid.SceneLayer.Front);
		kbatchedAnimController.Offset = new Vector3(0.5f, 0f, 0f);
		kbatchedAnimController.isMovable = true;
	}

	private void CreateMysteryEgg(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("MysteryEgg", CREATURES.SPECIES.MYSTERYEGG.NAME, false);
		entityType.prefab.UpdateComponentRequirement<MysteryEgg>(true);
		CircleCollider2D circleCollider2D = entityType.prefab.UpdateComponentRequirement<CircleCollider2D>(true);
		circleCollider2D.offset = new Vector2(0f, 0.3f);
		circleCollider2D.radius = 0.33f;
		PrimaryElement primaryElement = entityType.prefab.UpdateComponentRequirement<PrimaryElement>(true);
		primaryElement.SetElement(SimHashes.Carbon);
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(302f, 295f, 318f, 325f, 0f, 0f);
		entityType.prefab.UpdateComponentRequirement<Pickupable>(true);
		entityType.prefab.UpdateComponentRequirement<Clearable>(true);
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("egg_kanim", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
	}

	private void CreateFlutEgg(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("FlutEgg", CREATURES.SPECIES.FLUTEGG.NAME, true);
		entityType.prefab.UpdateComponentRequirement<FlutEgg>(true);
		entityType.prefab.UpdateComponentRequirement<RangedAttackable>(true);
		entityType.prefab.UpdateComponentRequirement<FactionAlignment>(true).Alignment = FactionManager.FactionID.Prey;
		entityType.prefab.UpdateComponentRequirement<Health>(true);
		entityType.prefab.UpdateComponentRequirement<PrimaryElement>(true).SetElement(SimHashes.Carbon);
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(283f, 263f, 294f, 343f, 0f, 0f);
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("flut_egg_kanim", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
	}

	private void CreateFlut(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("Flut", CREATURES.SPECIES.FLUT.NAME, true);
		entityType.prefab.UpdateComponentRequirement<Flut>(true);
		entityType.prefab.UpdateComponentRequirement<Health>(true);
		entityType.prefab.UpdateComponentRequirement<RangedAttackable>(true);
		entityType.prefab.UpdateComponentRequirement<FactionAlignment>(true).Alignment = FactionManager.FactionID.Prey;
		Navigator navigator = entityType.prefab.UpdateComponentRequirement<Navigator>(true);
		navigator.NavGridName = "SwimmerNavGrid";
		navigator.CurrentNavType = NavType.Swim;
		navigator.defaultSpeed = 1f;
		navigator.updateProber = true;
		entityType.prefab.UpdateComponentRequirement<EntombVulnerable>(true);
		entityType.prefab.UpdateComponentRequirement<Catchable>(true);
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(283f, 263f, 294f, 343f, 0f, 0f);
		entityType.prefab.AddButcherable(new string[] { "Meat" });
		AquaticReproducer aquaticReproducer = entityType.prefab.UpdateComponentRequirement<AquaticReproducer>(true);
		aquaticReproducer.EggPrefabTag = new Tag("FlutEgg");
		aquaticReproducer.SpawnEgg = true;
		entityType.prefab.UpdateComponentRequirement<Storage>(true);
		entityType.prefab.UpdateComponentRequirement<Operational>(true);
		ElementConverter elementConverter = entityType.prefab.UpdateComponentRequirement<ElementConverter>(true);
		elementConverter.conversionInterval = 150f;
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.25f / elementConverter.conversionInterval, SimHashes.Fertilizer, 0f, false, 0f, 0.5f, false, 1f, byte.MaxValue, 0)
		};
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("flut_single_kanim", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
	}

	private void CreateGlom(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("Glom", CREATURES.SPECIES.GLOM.NAME, true);
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("glom_kanim", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
		entityType.prefab.UpdateComponentRequirement<Glom>(true);
		entityType.prefab.UpdateComponentRequirement<Health>(true);
		entityType.prefab.UpdateComponentRequirement<RangedAttackable>(true);
		entityType.prefab.UpdateComponentRequirement<FactionAlignment>(true).Alignment = FactionManager.FactionID.Pest;
		Navigator navigator = entityType.prefab.UpdateComponentRequirement<Navigator>(true);
		navigator.NavGridName = "HatchNavGrid";
		navigator.CurrentNavType = NavType.Floor;
		navigator.defaultSpeed = 2f;
		navigator.updateProber = true;
		entityType.prefab.UpdateComponentRequirement<EntombVulnerable>(true);
		ElementEmitter elementEmitter = entityType.prefab.UpdateComponentRequirement<ElementEmitter>(true);
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(293f, 283f, 310f, 330f, 0f, 0f);
		elementEmitter.outputElement = new ElementConverter.OutputElement(0f, SimHashes.ContaminatedOxygen, 0f, false, 0f, 0.5f, false, 1f, byte.MaxValue, 0);
		elementEmitter.emissionFrequency = 0f;
		elementEmitter.SetEmitting(false);
	}

	private EntityType CreateCommon(string id, string name, bool boxCollider = true)
	{
		GameObject gameObject = new GameObject();
		EntityType entityType = base.Add(new EntityType(id, name));
		entityType.SetupPrefab(gameObject);
		if (boxCollider)
		{
			BoxCollider2D boxCollider2D = entityType.prefab.UpdateComponentRequirement<BoxCollider2D>(true);
			boxCollider2D.offset = new Vector2(0f, 0.5f);
		}
		KSelectable kselectable = entityType.prefab.UpdateComponentRequirement<KSelectable>(true);
		kselectable.SetName(name);
		PrimaryElement primaryElement = entityType.prefab.UpdateComponentRequirement<PrimaryElement>(true);
		primaryElement.ElementID = SimHashes.Carbon;
		primaryElement.MassPerUnit = 100f;
		primaryElement.Temperature = 293f;
		entityType.prefab.UpdateComponentRequirement<OccupyArea>(true);
		entityType.prefab.UpdateComponentRequirement<Modifiers>(true);
		entityType.prefab.UpdateComponentRequirement<SaveLoadRoot>(true);
		entityType.prefab.UpdateComponentRequirement<SavedObject>(true);
		entityType.prefab.UpdateComponentRequirement<StateMachineController>(true);
		entityType.prefab.UpdateComponentRequirement<CharacterOverlay>(true);
		entityType.prefab.AddDescription(Strings.Get(new StringKey("STRINGS.CREATURES.SPECIES." + id.ToString().ToUpper() + ".DESC")));
		return entityType;
	}

	public EntityType Flut;

	public EntityType FlutEgg;

	public EntityType MysteryEgg;
}
