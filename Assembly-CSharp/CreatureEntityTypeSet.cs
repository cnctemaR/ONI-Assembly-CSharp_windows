using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class CreatureEntityTypeSet : CommonEntityTypeSet
{
	public CreatureEntityTypeSet(Db modifier_set)
	{
		this.CreateGroneHog(modifier_set);
		this.CreateGroneHogMound(modifier_set);
		this.CreateMysteryEgg(modifier_set);
		this.CreateHaunt(modifier_set);
	}

	private void CreateGroneHog(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("GroneHog", CREATURES.SPECIES.GRONEHOG.NAME, true);
		BoxCollider2D boxCollider2D = entityType.prefab.UpdateComponentRequirement<BoxCollider2D>(true);
		boxCollider2D.size = new Vector2(2f, 1f);
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("gronehog", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
		entityType.prefab.UpdateComponentRequirement<GroneHog>(true);
		entityType.prefab.AddElementConsumer(SimHashes.Oxygen, 0.005f, 0f, 1);
		entityType.prefab.AddElementEmitter(SimHashes.CarbonDioxide, 5f, 0.008333334f);
		entityType.prefab.UpdateComponentRequirement<SimpleMover>(true);
		entityType.prefab.UpdateComponentRequirement<Harvestable>(true);
		entityType.prefab.AddButcherable(new string[] { "Meat" });
	}

	private void CreateGroneHogMound(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("GroneHogMound", CREATURES.SPECIES.GRONEHOGMOUND.NAME, true);
		BoxCollider2D boxCollider2D = entityType.prefab.UpdateComponentRequirement<BoxCollider2D>(true);
		boxCollider2D.size = new Vector2(3f, 3.5f);
		boxCollider2D.offset = new Vector2(0f, 1.75f);
		entityType.prefab.AddAnimController("gronehogmound", Grid.SceneLayer.Front);
		entityType.prefab.UpdateComponentRequirement<GroneHogMound>(true);
		entityType.prefab.UpdateComponentRequirement<Harvestable>(true);
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
		Trait trait = GameEntityTypeSet.CreateLivingEntityBaseTrait(id, name, -100f, -0f, -0f, -25f, 0f, (float)Grid.CellCount, modifier_set);
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
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(302f, 295f, 318f, 325f, 0.3f);
		entityType.prefab.UpdateComponentRequirement<DrowningMonitor>(true).Configure(30f, 10f, 0.5f);
		entityType.prefab.UpdateComponentRequirement<EntombVulnerable>(true);
		entityType.prefab.UpdateComponentRequirement<LoopingSounds>(true);
		entityType.prefab.AddElementEmitter(SimHashes.SlimeMold, 0f, 0f);
		entityType.prefab.AddElementConsumer(SimHashes.ContaminatedOxygen, 0.25f, 0.05f, 3);
		Navigator navigator = entityType.prefab.UpdateComponentRequirement<Navigator>(true);
		navigator.NavGridName = "FlyerNavGrid";
		navigator.CurrentNavType = NavType.Hover;
		navigator.defaultSpeed = 1f;
		navigator.updateProber = true;
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("puft", Grid.SceneLayer.Front);
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
		navigator.NavGridName = "FlyerNavGrid";
		navigator.CurrentNavType = NavType.Hover;
		navigator.defaultSpeed = 2f;
		navigator.updateProber = true;
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(TemperatureTuning.Freezing_1, TemperatureTuning.Freezing_2, TemperatureTuning.Hot_1, TemperatureTuning.Hot_2, 0.3f);
		entityType.prefab.UpdateComponentRequirement<DrowningMonitor>(true).Configure(30f, 10f, 0.5f);
		entityType.prefab.UpdateComponentRequirement<EntombVulnerable>(true);
		entityType.prefab.AddButcherable(new string[] { "Meat", "Meat", "Meat" });
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("shockworm", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
		KPrefabID component = entityType.prefab.GetComponent<KPrefabID>();
		component.prefabSpawnFn += delegate(GameObject go)
		{
			Navigator component2 = go.GetComponent<Navigator>();
			component2.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(component2));
		};
	}

	private void CreateHaunt(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("Haunt", CREATURES.SPECIES.HAUNT.NAME, true);
		entityType.prefab.UpdateComponentRequirement<SimpleMover>(true);
		entityType.prefab.GetComponent<BoxCollider2D>().size = new Vector2(2f, 3f);
		entityType.prefab.GetComponent<BoxCollider2D>().offset = new Vector2(1f, 2f);
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("haunt", Grid.SceneLayer.Front);
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
		primaryElement.UpdateElementTags = true;
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(302f, 295f, 318f, 325f, 0.3f);
		entityType.prefab.UpdateComponentRequirement<Pickupable>(true);
		entityType.prefab.UpdateComponentRequirement<Clearable>(true);
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("egg", Grid.SceneLayer.Front);
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
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(283f, 273f, 294f, 315f, 0.3f);
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("flut_egg", Grid.SceneLayer.Front);
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
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(283f, 273f, 294f, 315f, 0.3f);
		entityType.prefab.AddButcherable(new string[] { "Meat" });
		AquaticReproducer aquaticReproducer = entityType.prefab.UpdateComponentRequirement<AquaticReproducer>(true);
		aquaticReproducer.EggPrefabTag = new Tag("FlutEgg");
		aquaticReproducer.SpawnEgg = true;
		entityType.prefab.UpdateComponentRequirement<Storage>(true);
		entityType.prefab.UpdateComponentRequirement<Operational>(true);
		ElementConverter elementConverter = entityType.prefab.UpdateComponentRequirement<ElementConverter>(true);
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(null, 0.25f, SimHashes.Fertilizer, 0f, false, 0f, 0f)
		};
		elementConverter.conversionInterval = 150f;
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("flut_single", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
	}

	private void CreateGlom(Db modifier_set)
	{
		EntityType entityType = this.CreateCommon("Glom", CREATURES.SPECIES.GLOM.NAME, true);
		KBatchedAnimController kbatchedAnimController = entityType.prefab.AddAnimController("glom", Grid.SceneLayer.Front);
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
		entityType.prefab.UpdateComponentRequirement<TemperatureVulnerable>(true).Configure(293f, 283f, 310f, 330f, 0.3f);
		elementEmitter.outputElement = new ElementConverter.OutputElement(null, 0f, SimHashes.ContaminatedOxygen, 0f, false, 0f, 0f);
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
		entityType.prefab.UpdateComponentRequirement<InfraredVisualizer>(true);
		entityType.prefab.UpdateComponentRequirement<SaveLoadRoot>(true);
		entityType.prefab.UpdateComponentRequirement<SavedObject>(true);
		entityType.prefab.UpdateComponentRequirement<StateMachineController>(true);
		entityType.prefab.UpdateComponentRequirement<CharacterOverlay>(true);
		entityType.prefab.AddDescription(Strings.Get(new StringKey("STRINGS.CREATURES.SPECIES." + id.ToString().ToUpper() + ".DESC")));
		return entityType;
	}

	public EntityType groneHog;

	public EntityType groneHogMound;

	public EntityType Flut;

	public EntityType FlutEgg;

	public EntityType Haunt;

	public EntityType MysteryEgg;
}
