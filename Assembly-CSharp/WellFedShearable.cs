using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class WellFedShearable : GameStateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.growing;
		this.root.Enter(delegate(WellFedShearable.Instance smi)
		{
			WellFedShearable.UpdateScales(smi, 0f);
		}).Enter(delegate(WellFedShearable.Instance smi)
		{
			if (smi.def.hideSymbols != null)
			{
				foreach (KAnimHashedString kanimHashedString in smi.def.hideSymbols)
				{
					smi.animController.SetSymbolVisiblity(kanimHashedString, false);
				}
			}
		}).Update(new Action<WellFedShearable.Instance, float>(WellFedShearable.UpdateScales), UpdateRate.SIM_1000ms, false)
			.EventHandler(GameHashes.CaloriesConsumed, delegate(WellFedShearable.Instance smi, object data)
			{
				smi.OnCaloriesConsumed(data);
			});
		this.growing.Enter(delegate(WellFedShearable.Instance smi)
		{
			WellFedShearable.UpdateScales(smi, 0f);
		}).DefaultState(this.growing.stalled).Transition(this.fullyGrown, new StateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.Transition.ConditionCallback(WellFedShearable.AreScalesFullyGrown), UpdateRate.SIM_1000ms);
		this.growing.stalled.ToggleCritterEmotion(Db.Get().CritterEmotions.WellFed, null).Transition(this.growing.growing, (WellFedShearable.Instance smi) => smi.HasWellFedEffect(), UpdateRate.SIM_1000ms);
		this.growing.growing.Transition(this.growing.stalled, GameStateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.Not((WellFedShearable.Instance smi) => smi.HasWellFedEffect()), UpdateRate.SIM_1000ms).ToggleCritterEmotion(Db.Get().CritterEmotions.WellFed, null);
		this.fullyGrown.Enter(delegate(WellFedShearable.Instance smi)
		{
			WellFedShearable.UpdateScales(smi, 0f);
		}).ToggleBehaviour(GameTags.Creatures.ScalesGrown, (WellFedShearable.Instance smi) => smi.HasTag(GameTags.Creatures.CanMolt), null).EventTransition(GameHashes.Molt, this.growing, GameStateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.Not(new StateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.Transition.ConditionCallback(WellFedShearable.AreScalesFullyGrown)))
			.Transition(this.growing, GameStateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.Not(new StateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.Transition.ConditionCallback(WellFedShearable.AreScalesFullyGrown)), UpdateRate.SIM_1000ms);
	}

	private static bool AreScalesFullyGrown(WellFedShearable.Instance smi)
	{
		return smi.scaleGrowth.value >= smi.scaleGrowth.GetMax();
	}

	private static void UpdateScales(WellFedShearable.Instance smi, float dt)
	{
		int num = (int)((float)smi.def.levelCount * smi.scaleGrowth.value / 100f);
		if (smi.currentScaleLevel != num)
		{
			for (int i = 0; i < smi.def.scaleGrowthSymbols.Length; i++)
			{
				bool flag = i <= num - 1;
				smi.animController.SetSymbolVisiblity(smi.def.scaleGrowthSymbols[i], flag);
			}
			smi.currentScaleLevel = num;
		}
	}

	public WellFedShearable.GrowingState growing;

	public GameStateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.State fullyGrown;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.ScaleGrowth.Id);
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			return new List<Descriptor>
			{
				new Descriptor(UI.BUILDINGEFFECTS.SCALE_GROWTH.Replace("{Item}", this.itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(this.dropMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{Time}", GameUtil.GetFormattedCycles(this.growthDurationCycles * 600f, "F1", false)), UI.BUILDINGEFFECTS.TOOLTIPS.SCALE_GROWTH_FED.Replace("{Item}", this.itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(this.dropMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{Time}", GameUtil.GetFormattedCycles(this.growthDurationCycles * 600f, "F1", false)), Descriptor.DescriptorType.Effect, false)
			};
		}

		public string effectId;

		public float caloriesPerCycle;

		public float growthDurationCycles;

		public int levelCount;

		public Tag itemDroppedOnShear;

		public float dropMass;

		public Tag requiredDiet = null;

		public KAnimHashedString[] scaleGrowthSymbols = WellFedShearable.Def.SCALE_SYMBOL_NAMES;

		public KAnimHashedString[] hideSymbols;

		public static KAnimHashedString[] SCALE_SYMBOL_NAMES = new KAnimHashedString[] { "scale_0", "scale_1", "scale_2", "scale_3", "scale_4" };
	}

	public class GrowingState : GameStateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.State
	{
		public GameStateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.State growing;

		public GameStateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.State stalled;
	}

	public new class Instance : GameStateMachine<WellFedShearable, WellFedShearable.Instance, IStateMachineTarget, WellFedShearable.Def>.GameInstance, IShearable
	{
		public Instance(IStateMachineTarget master, WellFedShearable.Def def)
			: base(master, def)
		{
			this.scaleGrowth = Db.Get().Amounts.ScaleGrowth.Lookup(base.gameObject);
			this.scaleGrowth.value = this.scaleGrowth.GetMax();
		}

		public bool IsFullyGrown()
		{
			return this.currentScaleLevel == base.def.levelCount;
		}

		public void OnCaloriesConsumed(object data)
		{
			CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = Boxed<CreatureCalorieMonitor.CaloriesConsumedEvent>.Unbox(data);
			if (base.def.requiredDiet != null && caloriesConsumedEvent.tag != base.def.requiredDiet)
			{
				return;
			}
			EffectInstance effectInstance = this.effects.Get(base.smi.def.effectId);
			if (effectInstance == null)
			{
				effectInstance = this.effects.Add(base.smi.def.effectId, true);
			}
			effectInstance.timeRemaining += caloriesConsumedEvent.calories / base.smi.def.caloriesPerCycle * 600f;
		}

		public bool HasWellFedEffect()
		{
			return this.effects.Get(base.def.effectId) != null;
		}

		public void Shear()
		{
			this.scaleGrowth.value = 0f;
			WellFedShearable.UpdateScales(this, 0f);
		}

		public global::Tuple<Tag, float> GetItemDroppedOnShear()
		{
			return new global::Tuple<Tag, float>(base.def.itemDroppedOnShear, base.def.dropMass);
		}

		[MyCmpGet]
		private Effects effects;

		[MyCmpGet]
		public KBatchedAnimController animController;

		public AmountInstance scaleGrowth;

		public int currentScaleLevel = -1;
	}
}
