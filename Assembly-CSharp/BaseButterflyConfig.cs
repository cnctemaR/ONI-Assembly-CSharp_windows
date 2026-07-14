using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public static class BaseButterflyConfig
{
	public static GameObject BaseButterfly(string id, string name, string desc, string anim_file, string traitId, string symbolOverridePrefix = null)
	{
		float num = 5f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, "pollinator_build_kanim", null, FactionManager.FactionID.Pest, traitId, "FlyerNavGrid1x1", NavType.Hover, 32, 2f, "ButterflyPlantSeed", 1f, true, true, 283.15f, 318.15f, 233.15f, 353.15f);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num2 = CREATURES.SORTING.CRITTER_ORDER["Butterfly"];
		pickupable.sortOrder = num2;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Flyer, false);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<SubmergedMonitor.Def>();
		gameObject.AddOrGetDef<PollinateMonitor.Def>().radius = 10;
		gameObject.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = CREATURES.SPACE_REQUIREMENTS.TIER2;
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[]
		{
			GameTags.Algae,
			GameTags.Creatures.FlyersLure
		};
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		KAnimFile anim = Assets.GetAnim("pollinator_emotes_kanim");
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new TrappedStates.Def(), true, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), true, -1)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new ApproachBehaviourStates.Def(PollinateMonitor.ID, GameTags.Creatures.WantsToPollinate)
			{
				preAnim = "pollinate_pre",
				loopAnim = "pollinate_loop",
				pstAnim = "pollinate_pst"
			}, true, -1)
			.Add(new CritterEmoteStates.Def(anim), true, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.ButterflySpecies, symbolOverridePrefix);
		return gameObject;
	}

	public const string EMOTION_FILE_NAME = "pollinator_emotes_kanim";
}
