using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ScaleGrowthMonitor : GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.growing;
		this.root.Enter(delegate(ScaleGrowthMonitor.Instance smi)
		{
			ScaleGrowthMonitor.UpdateScales(smi, 0f);
		}).Update(new Action<ScaleGrowthMonitor.Instance, float>(ScaleGrowthMonitor.UpdateScales), UpdateRate.SIM_1000ms, false);
		this.growing.DefaultState(this.growing.growing).Transition(this.fullyGrown, new StateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.Transition.ConditionCallback(ScaleGrowthMonitor.AreScalesFullyGrown), UpdateRate.SIM_1000ms);
		this.growing.growing.Transition(this.growing.stunted, GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.Not(new StateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.Transition.ConditionCallback(ScaleGrowthMonitor.IsInCorrectAtmosphere)), UpdateRate.SIM_1000ms).Enter(new StateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.State.Callback(ScaleGrowthMonitor.ApplyModifier)).Exit(new StateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.State.Callback(ScaleGrowthMonitor.RemoveModifier));
		GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.State state = this.growing.stunted.Transition(this.growing.growing, new StateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.Transition.ConditionCallback(ScaleGrowthMonitor.IsInCorrectAtmosphere), UpdateRate.SIM_1000ms);
		string text = CREATURES.STATUSITEMS.STUNTED_SCALE_GROWTH.NAME;
		string text2 = CREATURES.STATUSITEMS.STUNTED_SCALE_GROWTH.TOOLTIP;
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Info;
		NotificationType notificationType = NotificationType.Neutral;
		bool flag = false;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, main);
		this.fullyGrown.ToggleBehaviour(GameTags.Creatures.ScalesGrown, (ScaleGrowthMonitor.Instance smi) => smi.HasTag(GameTags.Creatures.CanMolt), null).Transition(this.growing, GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.Not(new StateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.Transition.ConditionCallback(ScaleGrowthMonitor.AreScalesFullyGrown)), UpdateRate.SIM_1000ms);
	}

	private static bool IsInCorrectAtmosphere(ScaleGrowthMonitor.Instance smi)
	{
		if (smi.def.targetAtmosphere == (SimHashes)0)
		{
			return true;
		}
		int num = Grid.PosToCell(smi);
		return Grid.IsValidCell(num) && Grid.Element[num].id == smi.def.targetAtmosphere;
	}

	private static bool AreScalesFullyGrown(ScaleGrowthMonitor.Instance smi)
	{
		return smi.scaleGrowth.value >= smi.scaleGrowth.GetMax();
	}

	private static void ApplyModifier(ScaleGrowthMonitor.Instance smi)
	{
		smi.scaleGrowth.deltaAttribute.Add(smi.scaleGrowthModifier);
	}

	private static void RemoveModifier(ScaleGrowthMonitor.Instance smi)
	{
		smi.scaleGrowth.deltaAttribute.Remove(smi.scaleGrowthModifier);
	}

	private static void UpdateScales(ScaleGrowthMonitor.Instance smi, float dt)
	{
		int num = (int)((float)smi.def.levelCount * smi.scaleGrowth.value / 100f);
		if (smi.currentScaleLevel != num)
		{
			KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
			for (int i = 0; i < ScaleGrowthMonitor.SCALE_SYMBOL_NAMES.Length; i++)
			{
				bool flag = i <= num - 1;
				component.SetSymbolVisiblity(ScaleGrowthMonitor.SCALE_SYMBOL_NAMES[i], flag);
			}
			smi.currentScaleLevel = num;
		}
	}

	public ScaleGrowthMonitor.GrowingState growing;

	public GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.State fullyGrown;

	private AttributeModifier scaleGrowthModifier;

	private static HashedString[] SCALE_SYMBOL_NAMES = new HashedString[] { "scale_0", "scale_1", "scale_2", "scale_3", "scale_4" };

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.ScaleGrowth.Id);
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			List<Descriptor> list = new List<Descriptor>();
			if (this.targetAtmosphere == (SimHashes)0)
			{
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.SCALE_GROWTH.Replace("{Item}", this.itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(this.dropMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{Time}", GameUtil.GetFormattedCycles(1f / this.defaultGrowthRate, "F1", false)), UI.BUILDINGEFFECTS.TOOLTIPS.SCALE_GROWTH.Replace("{Item}", this.itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(this.dropMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{Time}", GameUtil.GetFormattedCycles(1f / this.defaultGrowthRate, "F1", false)), Descriptor.DescriptorType.Effect, false));
			}
			else
			{
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.SCALE_GROWTH_ATMO.Replace("{Item}", this.itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(this.dropMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{Time}", GameUtil.GetFormattedCycles(1f / this.defaultGrowthRate, "F1", false))
					.Replace("{Atmosphere}", this.targetAtmosphere.CreateTag().ProperName()), UI.BUILDINGEFFECTS.TOOLTIPS.SCALE_GROWTH_ATMO.Replace("{Item}", this.itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(this.dropMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{Time}", GameUtil.GetFormattedCycles(1f / this.defaultGrowthRate, "F1", false))
					.Replace("{Atmosphere}", this.targetAtmosphere.CreateTag().ProperName()), Descriptor.DescriptorType.Effect, false));
			}
			return list;
		}

		public int levelCount;

		public float defaultGrowthRate;

		public SimHashes targetAtmosphere;

		public Tag itemDroppedOnShear;

		public float dropMass;
	}

	public class GrowingState : GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.State
	{
		public GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.State growing;

		public GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.State stunted;
	}

	public new class Instance : GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>.GameInstance, IShearable
	{
		public Instance(IStateMachineTarget master, ScaleGrowthMonitor.Def def)
			: base(master, def)
		{
			this.scaleGrowth = Db.Get().Amounts.ScaleGrowth.Lookup(base.gameObject);
			this.scaleGrowth.value = this.scaleGrowth.GetMax();
			this.scaleGrowthModifier = new AttributeModifier(this.scaleGrowth.amount.deltaAttribute.Id, def.defaultGrowthRate * 100f, CREATURES.MODIFIERS.SCALE_GROWTH_RATE.NAME, false, false, true);
		}

		public bool IsFullyGrown()
		{
			return this.currentScaleLevel == base.def.levelCount;
		}

		public void Shear()
		{
			this.scaleGrowth.value = 0f;
			ScaleGrowthMonitor.UpdateScales(this, 0f);
		}

		public global::Tuple<Tag, float> GetItemDroppedOnShear()
		{
			return new global::Tuple<Tag, float>(base.def.itemDroppedOnShear, base.def.dropMass);
		}

		public AmountInstance scaleGrowth;

		public AttributeModifier scaleGrowthModifier;

		public int currentScaleLevel = -1;
	}
}
