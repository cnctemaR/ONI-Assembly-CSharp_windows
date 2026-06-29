using System;
using TUNING;
using UnityEngine;

public static class BaseLightBugConfig
{
	public static GameObject BaseLightBug(string id, string name, string desc, string anim_file, string traitId, Color lightColor, EffectorValues decor, bool is_baby, string symbolOverridePrefix = null)
	{
		float num = 50f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, anim, text, Grid.SceneLayer.Creatures, 1, 1, decor, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		FactionManager.FactionID factionID = FactionManager.FactionID.Prey;
		string text2 = "FlyerNavGrid1x1";
		NavType navType = NavType.Hover;
		num = 2f;
		string text3 = "Meat";
		int num2 = 0;
		float freezing_ = CREATURES.TEMPERATURE.FREEZING_1;
		float hot_ = CREATURES.TEMPERATURE.HOT_1;
		float hot_2 = CREATURES.TEMPERATURE.HOT_2;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject2, factionID, traitId, text2, navType, 32, num, text3, num2, true, true, 30f, freezing_, hot_, CREATURES.TEMPERATURE.FREEZING_2, hot_2);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByPrefix(Assets.GetAnim(anim_file), symbolOverridePrefix, 0);
		}
		gameObject.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Creatures.Flyer);
		gameObject.AddOrGet<NotCapturable>();
		gameObject.AddOrGet<LoopingSounds>();
		LureableMonitor.Def def = gameObject.AddOrGetDef<LureableMonitor.Def>();
		def.lures = new Tag[] { GameTags.Phosphorite };
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		if (is_baby)
		{
			KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
			component.animWidth = 0.5f;
			component.animHeight = 0.5f;
		}
		if (lightColor != Color.black)
		{
			Light2D light2D = gameObject.AddOrGet<Light2D>();
			light2D.Color = lightColor;
			light2D.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
			light2D.Range = 5f;
			light2D.Angle = 0f;
			light2D.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
			light2D.Offset = LIGHT2D.LIGHTBUG_OFFSET;
			light2D.shape = LightShape.Circle;
			light2D.drawOverlay = true;
		}
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new TrappedStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new DrowningStates.Def(), true)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true)
			.Add(new GrowUpStates.Def(), true)
			.Add(new RanchedStates.Def(), true)
			.Add(new LayEggStates.Def(), true)
			.Add(new EatStates.Def(), true)
			.Add(new MoveToLureStates.Def(), true)
			.Add(new CallAdultStates.Def(), true)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.LightBugSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static GameObject SetupDiet(GameObject prefab, TagBits consumedBits, Tag producedTag, float caloriesPerKg)
	{
		Diet.Info[] array = new Diet.Info[]
		{
			new Diet.Info(consumedBits, producedTag, caloriesPerKg, 1f, null, 0f)
		};
		Diet diet = new Diet(array);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		SolidConsumerMonitor.Def def2 = prefab.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
		return prefab;
	}

	public static void SetupLoopingSounds(GameObject inst)
	{
		LoopingSounds component = inst.GetComponent<LoopingSounds>();
		component.AddLoopingSoundUpdater();
		component.StartSound(GlobalAssets.GetSound("ShineBug_wings_LP", false), component.transform.GetPosition());
	}
}
