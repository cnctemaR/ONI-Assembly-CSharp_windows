using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseChameleonConfig
{
	public static GameObject BaseChameleon(string id, string name, string desc, string anim_file, string trait_id, bool is_baby, string symbol_override_prefix, float warnLowTemp, float warnHighTemp, float lethalLowTemp, float lethalHighTemp)
	{
		float num = 50f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Creatures;
		int num2 = 1;
		int num3 = 1;
		EffectorValues effectorValues = tier;
		float num4 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, anim, text, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, null, num4);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker, false);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		string text2 = "DreckoNavGrid";
		if (is_baby)
		{
			text2 = "DreckoBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "chameleo_build_kanim", null, FactionManager.FactionID.Pest, trait_id, text2, NavType.Floor, 32, 1f, "Meat", 0.5f, true, false, warnLowTemp, warnHighTemp, lethalLowTemp, lethalHighTemp);
		if (!string.IsNullOrEmpty(symbol_override_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_override_prefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num5 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["Drecko"];
		pickupable.sortOrder = num5;
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		if (!is_baby)
		{
			ShakeHarvestMonitor.Def def = gameObject.AddOrGetDef<ShakeHarvestMonitor.Def>();
			def.cooldownDuration = 150f;
			def.harvestablePlants.Add("DewDripperPlant");
			def.radius = 10;
		}
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), !is_baby, -1)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new ShakeHarvestStates.Def(), !is_baby, -1)
			.Add(new DrinkMilkStates.Def
			{
				shouldBeBehindMilkTank = is_baby
			}, true, -1)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true, -1)
			.Add(new CallAdultStates.Def(), is_baby, -1)
			.Add(new CritterCondoStates.Def(), !is_baby, -1)
			.Add(new CritterEmoteStates.Def(Assets.GetAnim("chameleo_emotes_kanim")), true, -1)
			.PopInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.ChameleonSpecies, symbol_override_prefix);
		return gameObject;
	}
}
