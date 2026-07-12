using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RefrigeratorController : GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		this.inoperational.EventTransition(GameHashes.OperationalChanged, this.operational, new StateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.Transition.ConditionCallback(this.IsOperational));
		this.operational.DefaultState(this.operational.steady).EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.Not(new StateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.Transition.ConditionCallback(this.IsOperational))).Enter(delegate(RefrigeratorController.StatesInstance smi)
		{
			smi.operational.SetActive(true, false);
		})
			.Exit(delegate(RefrigeratorController.StatesInstance smi)
			{
				smi.operational.SetActive(false, false);
			});
		this.operational.cooling.Update("Cooling exhaust", delegate(RefrigeratorController.StatesInstance smi, float dt)
		{
			smi.ApplyCoolingExhaust(dt);
		}, UpdateRate.SIM_200ms, true).UpdateTransition(this.operational.steady, new Func<RefrigeratorController.StatesInstance, float, bool>(this.AllFoodCool), UpdateRate.SIM_4000ms, true).ToggleStatusItem(Db.Get().BuildingStatusItems.FridgeCooling, (RefrigeratorController.StatesInstance smi) => smi, Db.Get().StatusItemCategories.Main);
		this.operational.steady.Update("Cooling exhaust", delegate(RefrigeratorController.StatesInstance smi, float dt)
		{
			smi.ApplySteadyExhaust(dt);
		}, UpdateRate.SIM_200ms, true).UpdateTransition(this.operational.cooling, new Func<RefrigeratorController.StatesInstance, float, bool>(this.AnyWarmFood), UpdateRate.SIM_4000ms, true).ToggleStatusItem(Db.Get().BuildingStatusItems.FridgeSteady, (RefrigeratorController.StatesInstance smi) => smi, Db.Get().StatusItemCategories.Main)
			.Enter(delegate(RefrigeratorController.StatesInstance smi)
			{
				smi.SetEnergySaver(true);
			})
			.Exit(delegate(RefrigeratorController.StatesInstance smi)
			{
				smi.SetEnergySaver(false);
			});
	}

	private bool AllFoodCool(RefrigeratorController.StatesInstance smi, float dt)
	{
		foreach (GameObject gameObject in smi.storage.items)
		{
			if (!(gameObject == null))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (!(component == null) && component.Mass >= 0.01f && component.Temperature >= smi.def.simulatedInternalTemperature + smi.def.activeCoolingStopBuffer)
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool AnyWarmFood(RefrigeratorController.StatesInstance smi, float dt)
	{
		foreach (GameObject gameObject in smi.storage.items)
		{
			if (!(gameObject == null))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (!(component == null) && component.Mass >= 0.01f && component.Temperature >= smi.def.simulatedInternalTemperature + smi.def.activeCoolingStartBuffer)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsOperational(RefrigeratorController.StatesInstance smi)
	{
		return smi.operational.IsOperational;
	}

	public GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State inoperational;

	public RefrigeratorController.OperationalStates operational;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			list.AddRange(SimulatedTemperatureAdjuster.GetDescriptors(this.simulatedInternalTemperature));
			Descriptor descriptor = default(Descriptor);
			string formattedHeatEnergy = GameUtil.GetFormattedHeatEnergy(this.coolingHeatKW * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATGENERATED, formattedHeatEnergy), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED, formattedHeatEnergy), Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
			return list;
		}

		public float activeCoolingStartBuffer = 2f;

		public float activeCoolingStopBuffer = 0.1f;

		public float simulatedInternalTemperature = 274.15f;

		public float simulatedInternalHeatCapacity = 400f;

		public float simulatedThermalConductivity = 1000f;

		public float powerSaverEnergyUsage;

		public float coolingHeatKW;

		public float steadyHeatKW;
	}

	public class OperationalStates : GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State
	{
		public GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State cooling;

		public GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State steady;
	}

	public class StatesInstance : GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, RefrigeratorController.Def def)
			: base(master, def)
		{
			this.temperatureAdjuster = new SimulatedTemperatureAdjuster(def.simulatedInternalTemperature, def.simulatedInternalHeatCapacity, def.simulatedThermalConductivity, this.storage);
			this.structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		}

		protected override void OnCleanUp()
		{
			this.temperatureAdjuster.CleanUp();
			base.OnCleanUp();
		}

		public float GetSaverPower()
		{
			return base.def.powerSaverEnergyUsage;
		}

		public float GetNormalPower()
		{
			return base.GetComponent<EnergyConsumer>().WattsNeededWhenActive;
		}

		public void SetEnergySaver(bool energySaving)
		{
			EnergyConsumer component = base.GetComponent<EnergyConsumer>();
			if (energySaving)
			{
				component.BaseWattageRating = this.GetSaverPower();
				return;
			}
			component.BaseWattageRating = this.GetNormalPower();
		}

		public void ApplyCoolingExhaust(float dt)
		{
			GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, base.def.coolingHeatKW * dt, BUILDING.STATUSITEMS.OPERATINGENERGY.FOOD_TRANSFER, dt);
		}

		public void ApplySteadyExhaust(float dt)
		{
			GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, base.def.steadyHeatKW * dt, BUILDING.STATUSITEMS.OPERATINGENERGY.FOOD_TRANSFER, dt);
		}

		[MyCmpReq]
		public Operational operational;

		[MyCmpReq]
		public Storage storage;

		private HandleVector<int>.Handle structureTemperature;

		private SimulatedTemperatureAdjuster temperatureAdjuster;
	}
}
