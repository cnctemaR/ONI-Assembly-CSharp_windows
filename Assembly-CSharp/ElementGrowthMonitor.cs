using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ElementGrowthMonitor : GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.growing;
		this.root.Enter(delegate(ElementGrowthMonitor.Instance smi)
		{
			ElementGrowthMonitor.UpdateGrowth(smi, 0f);
		}).Update(new Action<ElementGrowthMonitor.Instance, float>(ElementGrowthMonitor.UpdateGrowth), UpdateRate.SIM_1000ms, false).EventHandler(GameHashes.EatSolidComplete, delegate(ElementGrowthMonitor.Instance smi, object data)
		{
			smi.OnEatSolidComplete(data);
		});
		this.growing.DefaultState(this.growing.growing).Transition(this.fullyGrown, new StateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.Transition.ConditionCallback(ElementGrowthMonitor.IsFullyGrown), UpdateRate.SIM_1000ms).TagTransition(this.HungryTags, this.halted, false);
		this.growing.growing.Transition(this.growing.stunted, GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.Not(new StateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.Transition.ConditionCallback(ElementGrowthMonitor.IsConsumedInTemperatureRange)), UpdateRate.SIM_1000ms).ToggleStatusItem(Db.Get().CreatureStatusItems.ElementGrowthGrowing, null).Enter(new StateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.State.Callback(ElementGrowthMonitor.ApplyModifier))
			.Exit(new StateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.State.Callback(ElementGrowthMonitor.RemoveModifier));
		this.growing.stunted.Transition(this.growing.growing, new StateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.Transition.ConditionCallback(ElementGrowthMonitor.IsConsumedInTemperatureRange), UpdateRate.SIM_1000ms).ToggleStatusItem(Db.Get().CreatureStatusItems.ElementGrowthStunted, null).Enter(new StateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.State.Callback(ElementGrowthMonitor.ApplyModifier))
			.Exit(new StateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.State.Callback(ElementGrowthMonitor.RemoveModifier));
		this.halted.TagTransition(this.HungryTags, this.growing, true).ToggleStatusItem(Db.Get().CreatureStatusItems.ElementGrowthHalted, null);
		this.fullyGrown.ToggleStatusItem(Db.Get().CreatureStatusItems.ElementGrowthComplete, null).ToggleBehaviour(GameTags.Creatures.ScalesGrown, (ElementGrowthMonitor.Instance smi) => smi.HasTag(GameTags.Creatures.CanMolt), null).Transition(this.growing, GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.Not(new StateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.Transition.ConditionCallback(ElementGrowthMonitor.IsFullyGrown)), UpdateRate.SIM_1000ms);
	}

	private static bool IsConsumedInTemperatureRange(ElementGrowthMonitor.Instance smi)
	{
		return smi.lastConsumedTemperature == 0f || (smi.lastConsumedTemperature >= smi.def.minTemperature && smi.lastConsumedTemperature <= smi.def.maxTemperature);
	}

	private static bool IsFullyGrown(ElementGrowthMonitor.Instance smi)
	{
		return smi.elementGrowth.value >= smi.elementGrowth.GetMax();
	}

	private static void ApplyModifier(ElementGrowthMonitor.Instance smi)
	{
		if (smi.IsInsideState(smi.sm.growing.growing))
		{
			smi.elementGrowth.deltaAttribute.Add(smi.growingGrowthModifier);
			return;
		}
		if (smi.IsInsideState(smi.sm.growing.stunted))
		{
			smi.elementGrowth.deltaAttribute.Add(smi.stuntedGrowthModifier);
		}
	}

	private static void RemoveModifier(ElementGrowthMonitor.Instance smi)
	{
		smi.elementGrowth.deltaAttribute.Remove(smi.growingGrowthModifier);
		smi.elementGrowth.deltaAttribute.Remove(smi.stuntedGrowthModifier);
	}

	private static void UpdateGrowth(ElementGrowthMonitor.Instance smi, float dt)
	{
		int num = (int)((float)smi.def.levelCount * smi.elementGrowth.value / 100f);
		if (smi.currentGrowthLevel != num)
		{
			KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
			for (int i = 0; i < ElementGrowthMonitor.GROWTH_SYMBOL_NAMES.Length; i++)
			{
				bool flag = i == num - 1;
				component.SetSymbolVisiblity(ElementGrowthMonitor.GROWTH_SYMBOL_NAMES[i], flag);
			}
			smi.currentGrowthLevel = num;
		}
	}

	public Tag[] HungryTags = new Tag[] { GameTags.Creatures.Hungry };

	public GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.State halted;

	public ElementGrowthMonitor.GrowingState growing;

	public GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.State fullyGrown;

	private static HashedString[] GROWTH_SYMBOL_NAMES = new HashedString[] { "del_ginger1", "del_ginger2", "del_ginger3", "del_ginger4", "del_ginger5" };

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.ElementGrowth.Id);
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			return new List<Descriptor>
			{
				new Descriptor(UI.BUILDINGEFFECTS.SCALE_GROWTH_TEMP.Replace("{Item}", this.itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(this.dropMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{Time}", GameUtil.GetFormattedCycles(1f / this.defaultGrowthRate, "F1", false))
					.Replace("{TempMin}", GameUtil.GetFormattedTemperature(this.minTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))
					.Replace("{TempMax}", GameUtil.GetFormattedTemperature(this.maxTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), UI.BUILDINGEFFECTS.TOOLTIPS.SCALE_GROWTH_TEMP.Replace("{Item}", this.itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(this.dropMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{Time}", GameUtil.GetFormattedCycles(1f / this.defaultGrowthRate, "F1", false))
					.Replace("{TempMin}", GameUtil.GetFormattedTemperature(this.minTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))
					.Replace("{TempMax}", GameUtil.GetFormattedTemperature(this.maxTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Effect, false)
			};
		}

		public int levelCount;

		public float defaultGrowthRate;

		public Tag itemDroppedOnShear;

		public float dropMass;

		public float minTemperature;

		public float maxTemperature;
	}

	public class GrowingState : GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.State
	{
		public GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.State growing;

		public GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.State stunted;
	}

	public new class Instance : GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>.GameInstance, IShearable
	{
		public Instance(IStateMachineTarget master, ElementGrowthMonitor.Def def)
			: base(master, def)
		{
			this.elementGrowth = Db.Get().Amounts.ElementGrowth.Lookup(base.gameObject);
			this.elementGrowth.value = this.elementGrowth.GetMax();
			this.growingGrowthModifier = new AttributeModifier(this.elementGrowth.amount.deltaAttribute.Id, def.defaultGrowthRate * 100f, CREATURES.MODIFIERS.ELEMENT_GROWTH_RATE.NAME, false, false, true);
			this.stuntedGrowthModifier = new AttributeModifier(this.elementGrowth.amount.deltaAttribute.Id, def.defaultGrowthRate * 20f, CREATURES.MODIFIERS.ELEMENT_GROWTH_RATE.NAME, false, false, true);
		}

		public void OnEatSolidComplete(object data)
		{
			KPrefabID kprefabID = (KPrefabID)data;
			if (kprefabID == null)
			{
				return;
			}
			PrimaryElement component = kprefabID.GetComponent<PrimaryElement>();
			this.lastConsumedElement = component.ElementID;
			this.lastConsumedTemperature = component.Temperature;
		}

		public bool IsFullyGrown()
		{
			return this.currentGrowthLevel == base.def.levelCount;
		}

		public void Shear()
		{
			PrimaryElement component = base.smi.GetComponent<PrimaryElement>();
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(base.def.itemDroppedOnShear), null, null);
			gameObject.transform.SetPosition(Grid.CellToPosCCC(Grid.CellLeft(Grid.PosToCell(this)), Grid.SceneLayer.Ore));
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			component2.Temperature = component.Temperature;
			component2.Mass = base.def.dropMass;
			component2.AddDisease(component.DiseaseIdx, component.DiseaseCount, "Shearing");
			gameObject.SetActive(true);
			Vector2 vector = new Vector2(global::UnityEngine.Random.Range(-1f, 1f) * 1f, global::UnityEngine.Random.value * 2f + 2f);
			if (GameComps.Fallers.Has(gameObject))
			{
				GameComps.Fallers.Remove(gameObject);
			}
			GameComps.Fallers.Add(gameObject, vector);
			this.elementGrowth.value = 0f;
			ElementGrowthMonitor.UpdateGrowth(this, 0f);
		}

		public AmountInstance elementGrowth;

		public AttributeModifier growingGrowthModifier;

		public AttributeModifier stuntedGrowthModifier;

		public int currentGrowthLevel = -1;

		[Serialize]
		public SimHashes lastConsumedElement;

		[Serialize]
		public float lastConsumedTemperature;
	}
}
