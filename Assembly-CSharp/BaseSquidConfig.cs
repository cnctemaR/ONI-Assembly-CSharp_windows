using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseSquidConfig
{
	public static GameObject CreatePrefab(string id, string base_trait_id, string name, string description, string anim_file, bool is_baby, string symbol_prefix, float warnLowTemp, float warnHighTemp, float lethalLowTemp, float lethalHighTemp)
	{
		float num = 200f;
		int num2 = (is_baby ? 1 : 2);
		EffectorValues tier = DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim(is_baby ? anim_file : "squid_build_kanim");
		string text = "idle_loop";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Creatures;
		int num3 = 1;
		int num4 = num2;
		EffectorValues effectorValues = tier;
		float num5 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, description, num, anim, text, sceneLayer, num3, num4, effectorValues, default(EffectorValues), SimHashes.Creature, null, num5);
		if (!is_baby)
		{
			KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
			kboxCollider2D.offset = new Vector2f(0f, kboxCollider2D.offset.y);
		}
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.SwimmingCreature, false);
		component.AddTag(GameTags.Creatures.Swimmer, false);
		component.AddTag(GameTags.Creatures.SquidFriend, false);
		Trait trait = Db.Get().CreateTrait(base_trait_id, name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, SquidTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -SquidTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		gameObject.AddWeapon(2f, 3f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "squid_build_kanim", null, FactionManager.FactionID.Prey, base_trait_id, is_baby ? "SwimmerNavGrid" : "SwimmerNavGrid1x2", NavType.Swim, 32, 2f, "SquidMeat", 12f, false, false, warnLowTemp, warnHighTemp, lethalLowTemp, lethalHighTemp);
		ThreatMonitor.Def def = gameObject.AddOrGetDef<ThreatMonitor.Def>();
		def.fleethresholdState = Health.HealthState.Dead;
		def.friendlyCreatureTags = new Tag[] { GameTags.Creatures.SquidFriend };
		def.maxSearchDistance = 12;
		def.offsets = CrabTuning.DEFEND_OFFSETS;
		MilkProductionMonitor.Def def2 = gameObject.AddOrGetDef<MilkProductionMonitor.Def>();
		def2.element = SimHashes.Ink;
		def2.CaloriesPerCycle = SquidTuning.WELLFED_CALORIES_PER_CYCLE;
		def2.Capacity = SquidTuning.INK_CAPACITY;
		def2.effectId = "SquidWellFed";
		def2.fullStatusItem = Db.Get().CreatureStatusItems.InkFull;
		KAnimFile anim2 = Assets.GetAnim("squid_emotes_kanim");
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def
			{
				getLandAnim = new Func<FallStates.Instance, string>(BaseSquidConfig.GetLandAnim)
			}, true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FlopStates.Def(), true, -1)
			.Add(new DefendStates.Def
			{
				preAnim = "attack_pre",
				attackAnim = "attack",
				pstAnim = "attack_pst",
				specialAttackAction = new Action<GameObject, GameObject>(BaseSquidConfig.InkAttack)
			}, true, -1)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def(), true, -1)
			.Add(new PoopStates.Def(anim2, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, false), true, -1)
			.Add(new MoveToLureStates.Def(), true, -1)
			.Add(new CritterCondoStates.Def
			{
				fgLayer = CritterCondo.CreatureFGLayerType.SquidLayer
			}, !is_baby, -1)
			.Add(new CritterEmoteStates.Def(anim2), true, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		CreatureFallMonitor.Def def3 = gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		def3.canSwim = true;
		def3.checkHead = true;
		gameObject.AddOrGetDef<FlopMonitor.Def>();
		gameObject.AddOrGetDef<FishOvercrowdingMonitor.Def>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[] { GameTags.Creatures.FishTrapLure };
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.SquidSpecies, symbol_prefix);
		CritterCondoInteractMontior.Def def4 = gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>();
		def4.requireCavity = false;
		def4.condoPrefabTag = "UnderwaterCritterCondo";
		Tag poop_ELEMENT = SquidTuning.POOP_ELEMENT;
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add("TubeWorm");
		Diet diet = new Diet(new List<Diet.Info>
		{
			new Diet.Info(hashSet, poop_ELEMENT, SquidTuning.CALORIES_PER_GROWTH_EATEN, SquidTuning.GROWTH_TO_PRODUCT_EFFICIENCY, null, 0f, false, Diet.Info.FoodType.EatPlantDirectly, false, null)
		}.ToArray());
		CreatureCalorieMonitor.Def def5 = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def5.diet = diet;
		def5.minConsumedCaloriesBeforePooping = SquidTuning.STANDARD_CALORIES_PER_CYCLE;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num6 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["Squid"];
		pickupable.sortOrder = num6;
		return gameObject;
	}

	private static string GetLandAnim(FallStates.Instance smi)
	{
		if (smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation())
		{
			return "idle_loop";
		}
		return "flop_loop";
	}

	private static void InkAttack(GameObject attacker, GameObject target)
	{
		if (target == null)
		{
			return;
		}
		MilkProductionMonitor.Instance smi = attacker.GetSMI<MilkProductionMonitor.Instance>();
		if (smi.MilkAmount < 10f)
		{
			return;
		}
		Grid.PosToCell(attacker);
		int num = Grid.PosToCell(target);
		int num2 = Grid.CellBelow(num);
		if (!Grid.IsValidCell(num2) || Grid.Solid[num2])
		{
			num2 = num;
		}
		if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
		{
			PrimaryElement component = attacker.GetComponent<PrimaryElement>();
			smi.RemoveMilkFromAmount(10f);
			SimMessages.AddRemoveSubstance(num2, SimHashes.Ink, CellEventLogger.Instance.ElementEmitted, 5f, component.Temperature, byte.MaxValue, 0, true, -1);
			Element element = ElementLoader.FindElementByHash(SimHashes.Ink);
			Vector3 vector = Grid.CellToPosCCC(num2, Grid.SceneLayer.FXFront);
			OccupyArea component2 = target.GetComponent<OccupyArea>();
			if (component2 != null)
			{
				vector = component2.GetExtents().GetCentrePosition();
			}
			GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.SquidAttackId), vector, Grid.SceneLayer.FXFront, null, 0).SetActive(true);
			PopFX popFX = PopFXManager.Instance.SpawnFX(Def.GetUISprite(element, "ui", false).first, null, global::STRINGS.CREATURES.SPECIES.SQUID.INK_PUNCH, null, vector, 1.5f, true, false, false);
			if (popFX != null)
			{
				popFX.SetIconTint(element.substance.colour);
			}
		}
	}

	public const string EMOTION_FILE_NAME = "squid_emotes_kanim";
}
