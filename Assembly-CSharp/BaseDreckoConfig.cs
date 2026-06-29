using System;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseDreckoConfig
{
	public static GameObject BaseDrecko(string id, string name, string desc, string anim_file, string trait_id, bool is_baby, string symbol_override_prefix, float warnLowTemp, float warnHighTemp)
	{
		float num = 400f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		EffectorValues tier = DECOR.BONUS.TIER0;
		float num2 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, anim, text, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, num2);
		gameObject.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Creatures.GroundBased);
		string text2 = "DreckoNavGrid";
		GameObject gameObject2 = gameObject;
		FactionManager.FactionID factionID = FactionManager.FactionID.Pest;
		string text3 = text2;
		num2 = 1f;
		string text4 = "Meat";
		int num3 = 2;
		float num4 = warnHighTemp + 20f;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject2, factionID, trait_id, text3, NavType.Floor, 32, num2, text4, num3, true, false, 30f, warnLowTemp, warnHighTemp, warnLowTemp - 20f, num4);
		if (!string.IsNullOrEmpty(symbol_override_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByPrefix(Assets.GetAnim(anim_file), symbol_override_prefix, 0);
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<Capturable>();
		gameObject.AddOrGet<FloorSwitchActivator>();
		gameObject.AddOrGetDef<TrappedMonitor.Def>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<RanchableMonitor.Def>();
		gameObject.AddOrGet<LoopingSounds>();
		ThreatMonitor.Def def = gameObject.AddOrGetDef<ThreatMonitor.Def>();
		def.fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		string text5 = id + "_Preview";
		EntityTemplates.CreateAndRegisterPreview(text5, Assets.GetAnim("drecko_kanim"), "idle_loop", ObjectLayer.NumLayers, 1, 1);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, string.Format(global::STRINGS.CREATURES.BAGGED_NAME_FMT, name), string.Format(global::STRINGS.CREATURES.BAGGED_DESC_FMT, name), Assets.GetAnim("creature_sack_kanim"), "object", new Tag(text5), true);
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new TrappedStates.Def(), true)
			.Add(new FallStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DrowningStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new FleeStates.Def(), true)
			.Add(new AttackStates.Def(), !is_baby)
			.PushInterruptGroup()
			.Add(new GrowUpStates.Def(), true)
			.Add(new RanchedStates.Def(), true)
			.Add(new LayEggStates.Def(), true)
			.Add(new EatStates.Def(), true)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true)
			.Add(new CallAdultStates.Def(), true)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.DreckoSpecies, symbol_override_prefix);
		return gameObject;
	}
}
