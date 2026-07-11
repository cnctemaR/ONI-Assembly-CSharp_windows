using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseDreckoConfig
{
	public static GameObject BaseDrecko(string id, string name, string desc, string anim_file, string trait_id, bool is_baby, string symbol_override_prefix, float warnLowTemp, float warnHighTemp)
	{
		float num = 200f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, (warnLowTemp + warnHighTemp) / 2f);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker, false);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		string text = "DreckoNavGrid";
		if (is_baby)
		{
			text = "DreckoBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, trait_id, text, NavType.Floor, 32, 1f, "Meat", 2, true, false, warnLowTemp, warnHighTemp, warnLowTemp - 20f, warnHighTemp + 20f);
		if (!string.IsNullOrEmpty(symbol_override_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_override_prefix, null, 0);
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new GrowUpStates.Def(), true)
			.Add(new TrappedStates.Def(), true)
			.Add(new IncubatingStates.Def(), true)
			.Add(new BaggedStates.Def(), true)
			.Add(new FallStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DrowningStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new FleeStates.Def(), true)
			.Add(new AttackStates.Def(), !is_baby)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true)
			.Add(new RanchedStates.Def(), true)
			.Add(new LayEggStates.Def(), true)
			.Add(new EatStates.Def(), true)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true)
			.Add(new CallAdultStates.Def(), true)
			.PopInterruptGroup()
			.Add(new IdleStates.Def
			{
				customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseDreckoConfig.CustomIdleAnim)
			}, true);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.DreckoSpecies, symbol_override_prefix);
		return gameObject;
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
}
