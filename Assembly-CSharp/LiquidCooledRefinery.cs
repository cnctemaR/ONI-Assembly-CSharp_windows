using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LiquidCooledRefinery : Refinery
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.meter_coolant = new MeterController(component, "meter_target", "meter_coolant", Meter.Offset.Infront, Vector3.zero, null);
		this.meter_metal = new MeterController(component, "meter_target_metal", "meter_metal", Meter.Offset.Infront, Vector3.zero, null);
		this.meter_metal.SetPositionPercent(1f);
		this.smi = new LiquidCooledRefinery.StatesInstance(this);
		this.smi.StartSM();
		this.workable.OnWorkTickActions = delegate(Worker worker, float dt)
		{
			float percentComplete = this.workable.GetPercentComplete();
			this.meter_metal.SetPositionPercent(percentComplete);
		};
	}

	public bool HasEnoughCoolant()
	{
		float amountAvailable = this.inStorage.GetAmountAvailable(this.coolantTag);
		return amountAvailable >= this.minCoolantMass;
	}

	private void OnStorageChange(object data)
	{
		float amountAvailable = this.inStorage.GetAmountAvailable(this.coolantTag);
		this.operational.SetFlag(LiquidCooledRefinery.enoughCoolant, amountAvailable >= this.minCoolantMass);
		float capacityKG = this.conduitConsumer.capacityKG;
		float num = Mathf.Clamp01(amountAvailable / capacityKG);
		if (this.meter_coolant != null)
		{
			this.meter_coolant.SetPositionPercent(num);
		}
	}

	protected override List<GameObject> CompleteOrder(Refinery.UserOrder completed_order)
	{
		List<GameObject> list = base.CompleteOrder(completed_order);
		PrimaryElement component = list[0].GetComponent<PrimaryElement>();
		component.Temperature = this.outputTemperature;
		float num = GameUtil.CalculateEnergyDeltaForElementChange(component.Element.specificHeatCapacity, component.Mass, component.Element.highTemp, this.outputTemperature);
		this.inStorage.Transfer(this.outStorage, this.coolantTag, this.minCoolantMass, false, true);
		ListPool<GameObject, LiquidCooledRefinery>.PooledList pooledList = ListPool<GameObject, LiquidCooledRefinery>.Allocate();
		this.outStorage.Find(this.coolantTag, pooledList);
		foreach (GameObject gameObject in pooledList)
		{
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			if (component2.Mass != 0f)
			{
				float num2 = component2.Mass / this.minCoolantMass;
				float num3 = -num * num2 * this.thermalFudge;
				float num4 = GameUtil.CalculateTemperatureChange(component2.Element.specificHeatCapacity, component2.Mass, num3);
				float temperature = component2.Temperature;
				component2.Temperature += num4;
			}
		}
		pooledList.Recycle();
		return list;
	}

	public override List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> descriptors = base.GetDescriptors(def);
		descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.COOLANT, this.coolantTag.ProperName(), GameUtil.GetFormattedMass(this.minCoolantMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.COOLANT, this.coolantTag.ProperName(), GameUtil.GetFormattedMass(this.minCoolantMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		return descriptors;
	}

	public override List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
	{
		List<Descriptor> list = base.AdditionalEffectsForRecipe(recipe);
		GameObject prefab = Assets.GetPrefab(recipe.results[0].material);
		PrimaryElement component = prefab.GetComponent<PrimaryElement>();
		PrimaryElement primaryElement = this.inStorage.FindFirstWithMass(this.coolantTag);
		string text = UI.BUILDINGEFFECTS.TOOLTIPS.REFINEMENT_ENERGY_HAS_COOLANT;
		if (primaryElement == null)
		{
			GameObject prefab2 = Assets.GetPrefab(GameTags.Water);
			primaryElement = prefab2.GetComponent<PrimaryElement>();
			text = UI.BUILDINGEFFECTS.TOOLTIPS.REFINEMENT_ENERGY_NO_COOLANT;
		}
		float num = -GameUtil.CalculateEnergyDeltaForElementChange(component.Element.specificHeatCapacity, recipe.results[0].amount, component.Element.highTemp, this.outputTemperature);
		float num2 = GameUtil.CalculateTemperatureChange(primaryElement.Element.specificHeatCapacity, this.minCoolantMass, num * this.thermalFudge);
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.REFINEMENT_ENERGY, GameUtil.GetFormattedJoules(num, "F1", GameUtil.TimeSlice.None)), string.Format(text, GameUtil.GetFormattedJoules(num, "F1", GameUtil.TimeSlice.None), primaryElement.GetProperName(), GameUtil.GetFormattedTemperature(num2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true)), Descriptor.DescriptorType.Effect, false));
		return list;
	}

	[MyCmpReq]
	private ConduitConsumer conduitConsumer;

	public static Operational.Flag enoughCoolant = new Operational.Flag("enoughCoolant", Operational.Flag.Type.Functional);

	public Tag coolantTag;

	public float minCoolantMass = 100f;

	public float thermalFudge = 0.8f;

	public float outputTemperature = 313.15f;

	private MeterController meter_coolant;

	private MeterController meter_metal;

	private LiquidCooledRefinery.StatesInstance smi;

	public class StatesInstance : GameStateMachine<LiquidCooledRefinery.States, LiquidCooledRefinery.StatesInstance, LiquidCooledRefinery, object>.GameInstance
	{
		public StatesInstance(LiquidCooledRefinery master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<LiquidCooledRefinery.States, LiquidCooledRefinery.StatesInstance, LiquidCooledRefinery>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			if (LiquidCooledRefinery.States.waitingForCoolantStatus == null)
			{
				LiquidCooledRefinery.States.waitingForCoolantStatus = new StatusItem("waitingForCoolantStatus", BUILDING.STATUSITEMS.ENOUGH_COOLANT.NAME, BUILDING.STATUSITEMS.ENOUGH_COOLANT.TOOLTIP, "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, 63486);
				LiquidCooledRefinery.States.waitingForCoolantStatus.resolveStringCallback = delegate(string str, object obj)
				{
					LiquidCooledRefinery liquidCooledRefinery = (LiquidCooledRefinery)obj;
					return string.Format(str, liquidCooledRefinery.coolantTag.ProperName(), GameUtil.GetFormattedMass(liquidCooledRefinery.minCoolantMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				};
			}
			default_state = this.waiting_for_coolant;
			this.waiting_for_coolant.ToggleStatusItem(LiquidCooledRefinery.States.waitingForCoolantStatus, (LiquidCooledRefinery.StatesInstance smi) => smi.master).EventTransition(GameHashes.OnStorageChange, this.ready, (LiquidCooledRefinery.StatesInstance smi) => smi.master.HasEnoughCoolant());
			this.ready.EventTransition(GameHashes.OnStorageChange, this.waiting_for_coolant, (LiquidCooledRefinery.StatesInstance smi) => !smi.master.HasEnoughCoolant());
		}

		public static StatusItem waitingForCoolantStatus;

		public GameStateMachine<LiquidCooledRefinery.States, LiquidCooledRefinery.StatesInstance, LiquidCooledRefinery, object>.State waiting_for_coolant;

		public GameStateMachine<LiquidCooledRefinery.States, LiquidCooledRefinery.StatesInstance, LiquidCooledRefinery, object>.State ready;
	}
}
