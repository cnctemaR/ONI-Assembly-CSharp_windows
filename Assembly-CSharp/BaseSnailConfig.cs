using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BaseSnailConfig
{
	public static GameObject CreatePrefab(string id, string baseTraitId, string name, string description, string animFile, bool isBaby, string symbolPrefix, float warnLowTemp, float warnHighTemp, float lethalLowTemp, float lethalHighTemp, string moltPrefabId)
	{
		float mass = SnailTuning.MASS;
		EffectorValues tier = DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim(isBaby ? animFile : "snail_build_kanim");
		string text = "idle_loop";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Creatures;
		int num = 1;
		int num2 = 1;
		EffectorValues effectorValues = tier;
		float num3 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, description, mass, anim, text, sceneLayer, num, num2, effectorValues, default(EffectorValues), SimHashes.Creature, null, num3);
		gameObject.AddTag(GameTags.Amphibious);
		gameObject.AddTag(GameTags.Creatures.Walker);
		BaseSnailConfig.ConfigureTraits(baseTraitId, name, !isBaby);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, false, true, true);
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, animFile, isBaby ? null : "snail_build_kanim", symbolPrefix, FactionManager.FactionID.Prey, baseTraitId, isBaby ? "WalkerBabyNavGrid" : "WalkerNavGrid1x1NoJump", NavType.Floor, 16, 0.25f, "Meat", 0.5f, false, false, warnLowTemp, warnHighTemp, lethalLowTemp, lethalHighTemp);
		ChoreTable.Builder builder = BaseSnailConfig.CreateChoreTable(isBaby, animFile);
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.SnailSpecies, symbolPrefix);
		if (!isBaby)
		{
			BaseSnailConfig.UsesCondo(gameObject);
			BaseSnailConfig.DropsMolt(gameObject, moltPrefabId);
			MoistureMonitor.Def def = gameObject.AddOrGetDef<MoistureMonitor.Def>();
			def.lubricant = SimHashes.Mucus;
			def.onDryLandModifier = SnailTuning.MUCUS_PER_CYCLE_DRY_LAND_BONUS / 600f;
			def.lubricantTemperatureKelvin = 311.15f;
			gameObject.AddOrGetDef<DesiccationMonitor.Def>();
		}
		gameObject.AddOrGet<Pickupable>().sortOrder = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["Snail"];
		return gameObject;
	}

	private static void DropsMolt(GameObject prefab, string prefabId)
	{
		MoltDropperMonitor.Def def = prefab.AddOrGetDef<MoltDropperMonitor.Def>();
		def.onGrowDropID = prefabId;
		def.massToDrop = 10f;
		def.isReadyToMolt = new Func<MoltDropperMonitor.Instance, bool>(BaseSnailConfig.IsReadyToMolt);
	}

	public static bool IsReadyToMolt(MoltDropperMonitor.Instance smi)
	{
		return BaseSnailConfig.IsValidTimeToDropMolt(smi) && BaseSnailConfig.IsValidDropCell(smi) && !smi.prefabID.HasTag(GameTags.Creatures.Hungry) && smi.prefabID.HasTag(GameTags.Creatures.Happy);
	}

	public static bool IsValidTimeToDropMolt(MoltDropperMonitor.Instance smi)
	{
		return !smi.spawnedThisCycle && (smi.timeOfLastDrop <= 0f || GameClock.Instance.GetTime() - smi.timeOfLastDrop > 600f);
	}

	public static void OnSpawn(GameObject inst)
	{
		Navigator component = inst.GetComponent<Navigator>();
		component.transitionDriver.overrideLayers.Add(new SadSnailTransitionLayer(component));
	}

	private static void UsesCondo(GameObject prefab)
	{
		CritterCondoInteractMontior.Def def = prefab.AddOrGetDef<CritterCondoInteractMontior.Def>();
		def.requireCavity = false;
		def.condoPrefabTag = "CritterCondo";
	}

	private static ChoreTable.Builder CreateChoreTable(bool isBaby, string animFile)
	{
		ChoreTable.Builder builder = new ChoreTable.Builder();
		KAnimFile anim = Assets.GetAnim(animFile);
		builder.Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), isBaby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), isBaby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), true, -1)
			.PushInterruptGroup()
			.Add(new MucusSecretionStates.Def(), true, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !isBaby, -1)
			.Add(new LayEggStates.Def(), !isBaby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def
			{
				shouldBeBehindMilkTank = true
			}, true, -1)
			.Add(new PoopStates.Def(anim, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, false), true, -1)
			.Add(new CallAdultStates.Def(), isBaby, -1)
			.Add(new CritterCondoStates.Def(), !isBaby, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def
			{
				customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseSnailConfig.CustomIdleAnim)
			}, true, -1);
		return builder;
	}

	private static void ConfigureTraits(string baseTraitId, string name, bool isAdult)
	{
		Trait trait = Db.Get().CreateTrait(baseTraitId, name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, SnailTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -SnailTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name, false, false, true));
		if (isAdult)
		{
			trait.Add(new AttributeModifier(Db.Get().Amounts.Moisture.deltaAttribute.Id, SnailTuning.DEFAULT_DRYING_RATE, name, false, false, true));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Mucus.deltaAttribute.Id, SnailTuning.MUCUS_PER_CYCLE / 600f, global::STRINGS.CREATURES.MODIFIERS.MUCUS.BASE_RATE, false, false, true));
		}
	}

	private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
	{
		DesiccationMonitor.Instance smi2 = smi.GetSMI<DesiccationMonitor.Instance>();
		return (smi2 == null || !smi2.IsDesiccating()) ? "idle_loop" : "idle_loop_sad";
	}

	public static bool IsValidDropCell(MoltDropperMonitor.Instance smi)
	{
		return Grid.IsValidCell(Grid.PosToCell(smi.transform.GetPosition()));
	}

	public static Diet.Info[] SaltToDirtDiet()
	{
		return new Diet.Info[]
		{
			new Diet.Info(new HashSet<Tag> { SimHashes.Salt.CreateTag() }, SimHashes.Dirt.CreateTag(), SnailTuning.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null)
		};
	}

	public static Diet.Info[] SulfurToObsidianDiet()
	{
		return new Diet.Info[]
		{
			new Diet.Info(new HashSet<Tag> { SimHashes.Sulfur.CreateTag() }, SimHashes.Obsidian.CreateTag(), SnailTuning.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null)
		};
	}

	private const string IDLE_LOOP = "idle_loop";

	private const string IDLE_LOOP_SAD = "idle_loop_sad";
}
