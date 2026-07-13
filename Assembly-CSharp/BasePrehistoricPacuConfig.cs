using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BasePrehistoricPacuConfig
{
	public static GameObject CreatePrefab(string id, string base_trait_id, string name, string description, string anim_file, bool is_baby, string symbol_prefix, float warnLowTemp, float warnHighTemp, float lethalLowTemp, float lethalHighTemp)
	{
		float num = 200f;
		int num2 = (is_baby ? 1 : 2);
		int num3 = (is_baby ? 1 : 2);
		EffectorValues tier = DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim(is_baby ? anim_file : "paculacanth_build_kanim");
		string text = "idle_loop";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Creatures;
		int num4 = num2;
		int num5 = num3;
		EffectorValues effectorValues = tier;
		float num6 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, description, num, anim, text, sceneLayer, num4, num5, effectorValues, default(EffectorValues), SimHashes.Creature, null, num6);
		if (!is_baby)
		{
			KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
			kboxCollider2D.offset = new Vector2f(0f, kboxCollider2D.offset.y);
		}
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.SwimmingCreature, false);
		component.AddTag(GameTags.Creatures.Swimmer, false);
		Trait trait = Db.Get().CreateTrait(base_trait_id, name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PrehistoricPacuTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -PrehistoricPacuTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		gameObject.AddComponent<Movable>();
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "paculacanth_build_kanim", null, FactionManager.FactionID.Prey, base_trait_id, is_baby ? "SwimmerNavGrid" : "SwimmerGrid2x2", NavType.Swim, 32, 2f, "PrehistoricPacuFillet", 12f, false, false, warnLowTemp, warnHighTemp, lethalLowTemp, lethalHighTemp);
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def
			{
				getLandAnim = new Func<FallStates.Instance, string>(BasePrehistoricPacuConfig.GetLandAnim)
			}, true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FlopStates.Def(), true, -1)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "lay_egg_pre", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true, -1)
			.Add(new MoveToLureStates.Def(), true, -1)
			.Add(new CritterCondoStates.Def(), !is_baby, -1)
			.Add(new CritterEmoteStates.Def(Assets.GetAnim("paculacanth_emotes_kanim")), true, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		CreatureFallMonitor.Def def = gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		def.canSwim = true;
		def.checkHead = true;
		gameObject.AddOrGetDef<FlopMonitor.Def>();
		gameObject.AddOrGetDef<FishOvercrowdingMonitor.Def>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.PrehistoricPacuSpecies, symbol_prefix);
		CritterCondoInteractMontior.Def def2 = gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>();
		def2.requireCavity = false;
		def2.condoPrefabTag = "UnderwaterCritterCondo";
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add("Pacu");
		hashSet.Add("PacuCleaner");
		hashSet.Add("PacuTropical");
		HashSet<Tag> hashSet2 = new HashSet<Tag>();
		hashSet2.Add("FishMeat");
		Diet diet = new Diet(new List<Diet.Info>
		{
			new Diet.Info(hashSet, PrehistoricPacuTuning.POOP_ELEMENT, BasePrehistoricPacuConfig.CALORIES_PER_KG_OF_PACU, 60f / PacuTuning.MASS, null, 0f, false, Diet.Info.FoodType.EatPrey, false, null),
			new Diet.Info(hashSet2, PrehistoricPacuTuning.POOP_ELEMENT, BasePrehistoricPacuConfig.CALORIES_PER_KG_OF_PACU_MEAT, 60f, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null)
		}.ToArray());
		CreatureCalorieMonitor.Def def3 = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def3.diet = diet;
		def3.minConsumedCaloriesBeforePooping = BasePrehistoricPacuConfig.CALORIES_PER_KG_OF_PACU * 60f;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		if (!string.IsNullOrEmpty(symbol_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_prefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num7 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["PrehistoricPacu"];
		pickupable.sortOrder = num7;
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

	private static float CALORIES_PER_KG_OF_PACU = PrehistoricPacuTuning.STANDARD_CALORIES_PER_CYCLE / 1f / PacuTuning.MASS;

	private static float CALORIES_PER_KG_OF_PACU_MEAT = PrehistoricPacuTuning.STANDARD_CALORIES_PER_CYCLE / 1f;
}
