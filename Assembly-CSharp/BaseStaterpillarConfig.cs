using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BaseStaterpillarConfig
{
	public static GameObject BaseStaterpillar(string id, string name, string desc, string anim_file, string trait_id, bool is_baby, ObjectLayer conduitLayer, string connectorDefId, Tag inhaleTag, string symbolOverridePrefix = null, float warningLowTemperature = 283.15f, float warningHighTemperature = 293.15f, float lethalLowTemperature = 243.15f, float lethalHighTemperature = 343.15f, InhaleStates.Def inhaleDef = null)
	{
		float num = 200f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, Assets.GetAnim(is_baby ? anim_file : "caterpillar_build_kanim"), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Walker, false);
		gameObject.AddTag(GameTags.Amphibious);
		string text = "WalkerBabyNavGrid";
		if (!is_baby)
		{
			text = "DreckoNavGrid";
			gameObject.AddOrGetDef<ConduitSleepMonitor.Def>().conduitLayer = conduitLayer;
		}
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "caterpillar_build_kanim", symbolOverridePrefix, FactionManager.FactionID.Pest, trait_id, text, NavType.Floor, 32, 1f, "Meat", 2f, false, false, warningLowTemperature, warningHighTemperature, lethalLowTemperature, lethalHighTemperature);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num2 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["Staterpillar"];
		pickupable.sortOrder = num2;
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGet<LoopingSounds>();
		Staterpillar staterpillar = gameObject.AddOrGet<Staterpillar>();
		staterpillar.conduitLayer = conduitLayer;
		staterpillar.connectorDefId = connectorDefId;
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		KAnimFile anim = Assets.GetAnim("caterpillar_emotes_kanim");
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), !is_baby, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def
			{
				WaitCellOffset = 2
			}, !is_baby, -1)
			.PushInterruptGroup()
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def(), true, -1)
			.Add(new PoopStates.Def(anim, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, false), true, -1)
			.Add(inhaleDef, inhaleTag != Tag.Invalid, -1)
			.Add(new ConduitSleepStates.Def(), true, -1)
			.Add(new CallAdultStates.Def(), is_baby, -1)
			.Add(new CritterCondoStates.Def(), !is_baby, -1)
			.Add(new CritterEmoteStates.Def(anim), !is_baby, -1)
			.PopInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new IdleStates.Def
			{
				customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseStaterpillarConfig.CustomIdleAnim)
			}, true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.StaterpillarSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos)
	{
		Diet diet = new Diet(diet_infos.ToArray());
		prefab.AddOrGetDef<CreatureCalorieMonitor.Def>().diet = diet;
		prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		return prefab;
	}

	public static List<Diet.Info> RawMetalDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		List<SimHashes> list = new List<SimHashes> { SimHashes.FoolsGold };
		List<Diet.Info> list2 = new List<Diet.Info>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsSolid && element.materialCategory == GameTags.Metal && element.HasTag(GameTags.Ore) && !element.disabled && !list.Contains(element.id))
			{
				list2.Add(new Diet.Info(new HashSet<Tag>(new Tag[] { element.tag }), poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false, null));
			}
		}
		return list2;
	}

	public static List<Diet.Info> RefinedMetalDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsSolid && element.materialCategory == GameTags.RefinedMetal && !element.disabled)
			{
				list.Add(new Diet.Info(new HashSet<Tag>(new Tag[] { element.tag }), poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false, null));
			}
		}
		return list;
	}

	private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
	{
		CellOffset cellOffset = new CellOffset(0, -1);
		bool facing = smi.GetComponent<Facing>().GetFacing();
		NavType currentNavType = smi.GetComponent<Navigator>().CurrentNavType;
		if (currentNavType != NavType.Floor)
		{
			if (currentNavType == NavType.Ceiling)
			{
				cellOffset = (facing ? new CellOffset(1, 1) : new CellOffset(-1, 1));
			}
		}
		else
		{
			cellOffset = (facing ? new CellOffset(1, -1) : new CellOffset(-1, -1));
		}
		HashedString hashedString = "idle_loop";
		int num = Grid.OffsetCell(Grid.PosToCell(smi), cellOffset);
		if (Grid.IsValidCell(num) && !Grid.Solid[num])
		{
			pre_anim = "idle_loop_hang_pre";
			hashedString = "idle_loop_hang";
		}
		return hashedString;
	}

	public const string EMOTION_FILE_NAME = "caterpillar_emotes_kanim";
}
