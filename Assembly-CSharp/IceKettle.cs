using System;
using System.Collections.Generic;
using Klei;
using STRINGS;
using UnityEngine;

public class IceKettle : GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.noOperational;
		this.root.EventHandlerTransition(GameHashes.WorkableStartWork, this.inUse, (IceKettle.Instance smi, object obj) => true).EventHandler(GameHashes.OnStorageChange, delegate(IceKettle.Instance smi)
		{
			smi.UpdateMeter();
		});
		this.noOperational.TagTransition(GameTags.Operational, this.operational, false);
		this.operational.TagTransition(GameTags.Operational, this.noOperational, true).DefaultState(this.operational.idle);
		this.operational.idle.PlayAnim(IceKettle.IDEL_ANIM_STATE).DefaultState(this.operational.idle.waitingForSolids);
		this.operational.idle.waitingForSolids.ToggleStatusItem(Db.Get().BuildingStatusItems.KettleInsuficientSolids, null).EventTransition(GameHashes.OnStorageChange, this.operational.idle.waitingForSpaceInLiquidTank, (IceKettle.Instance smi) => IceKettle.HasEnoughSolidsToMelt(smi));
		this.operational.idle.waitingForSpaceInLiquidTank.ToggleStatusItem(Db.Get().BuildingStatusItems.KettleInsuficientLiquidSpace, null).EventTransition(GameHashes.OnStorageChange, this.operational.idle.notEnoughFuel, new StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.Transition.ConditionCallback(IceKettle.LiquidTankHasCapacityForNextBatch));
		this.operational.idle.notEnoughFuel.ToggleStatusItem(Db.Get().BuildingStatusItems.KettleInsuficientFuel, null).EventTransition(GameHashes.OnStorageChange, this.operational.melting, new StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.Transition.ConditionCallback(IceKettle.CanMeltNextBatch));
		this.operational.melting.Toggle("Operational Active State", new StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State.Callback(IceKettle.SetOperationalActiveStatesTrue), new StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State.Callback(IceKettle.SetOperationalActiveStatesFalse)).DefaultState(this.operational.melting.entering);
		this.operational.melting.entering.PlayAnim(IceKettle.BOILING_PRE_ANIM_NAME, KAnim.PlayMode.Once).OnAnimQueueComplete(this.operational.melting.working);
		this.operational.melting.working.ToggleStatusItem(Db.Get().BuildingStatusItems.KettleMelting, null).DefaultState(this.operational.melting.working.idle).PlayAnim(IceKettle.BOILING_LOOP_ANIM_NAME, KAnim.PlayMode.Loop);
		this.operational.melting.working.idle.ParamTransition<float>(this.MeltingTimer, this.operational.melting.working.complete, new StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.Parameter<float>.Callback(IceKettle.IsDoneMelting)).Update(new Action<IceKettle.Instance, float>(IceKettle.MeltingTimerUpdate), UpdateRate.SIM_200ms, false);
		this.operational.melting.working.complete.Enter(new StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State.Callback(IceKettle.ResetMeltingTimer)).Enter(new StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State.Callback(IceKettle.MeltNextBatch)).EnterTransition(this.operational.melting.working.idle, new StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.Transition.ConditionCallback(IceKettle.CanMeltNextBatch))
			.EnterTransition(this.operational.melting.exit, GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.Not(new StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.Transition.ConditionCallback(IceKettle.CanMeltNextBatch)));
		this.operational.melting.exit.PlayAnim(IceKettle.BOILING_PST_ANIM_NAME, KAnim.PlayMode.Once).OnAnimQueueComplete(this.operational.idle);
		this.inUse.EventHandlerTransition(GameHashes.WorkableStopWork, this.noOperational, (IceKettle.Instance smi, object obj) => true).ScheduleGoTo(new Func<IceKettle.Instance, float>(IceKettle.GetInUseTimeout), this.noOperational);
	}

	public static void SetOperationalActiveStatesTrue(IceKettle.Instance smi)
	{
		smi.operational.SetActive(true, false);
	}

	public static void SetOperationalActiveStatesFalse(IceKettle.Instance smi)
	{
		smi.operational.SetActive(false, false);
	}

	public static float GetInUseTimeout(IceKettle.Instance smi)
	{
		return smi.InUseWorkableDuration + 1f;
	}

	public static void ResetMeltingTimer(IceKettle.Instance smi)
	{
		smi.sm.MeltingTimer.Set(0f, smi, false);
	}

	public static bool HasEnoughSolidsToMelt(IceKettle.Instance smi)
	{
		return smi.HasAtLeastOneBatchOfSolidsWaitingToMelt;
	}

	public static bool LiquidTankHasCapacityForNextBatch(IceKettle.Instance smi)
	{
		return smi.LiquidTankHasCapacityForNextBatch;
	}

	public static bool HasEnoughFuelForNextBacth(IceKettle.Instance smi)
	{
		return smi.HasEnoughFuelUnitsToMeltNextBatch;
	}

	public static bool CanMeltNextBatch(IceKettle.Instance smi)
	{
		return smi.HasAtLeastOneBatchOfSolidsWaitingToMelt && IceKettle.LiquidTankHasCapacityForNextBatch(smi) && IceKettle.HasEnoughFuelForNextBacth(smi);
	}

	public static bool IsDoneMelting(IceKettle.Instance smi, float timePassed)
	{
		return timePassed >= smi.MeltDurationPerBatch;
	}

	public static void MeltingTimerUpdate(IceKettle.Instance smi, float dt)
	{
		float num = smi.sm.MeltingTimer.Get(smi);
		smi.sm.MeltingTimer.Set(num + dt, smi, false);
	}

	public static void MeltNextBatch(IceKettle.Instance smi)
	{
		smi.MeltNextBatch();
	}

	public static string LIQUID_METER_TARGET_NAME = "kettle_meter_target";

	public static string LIQUID_METER_ANIM_NAME = "meter_kettle";

	public static string IDEL_ANIM_STATE = "on";

	public static string BOILING_PRE_ANIM_NAME = "boiling_pre";

	public static string BOILING_LOOP_ANIM_NAME = "boiling_loop";

	public static string BOILING_PST_ANIM_NAME = "boiling_pst";

	private const float InUseTimeout = 5f;

	public GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State noOperational;

	public IceKettle.OperationalStates operational;

	public GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State inUse;

	public StateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.FloatParameter MeltingTimer;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			string text = string.Format(UI.BUILDINGEFFECTS.KETTLE_MELT_RATE, GameUtil.GetFormattedMass(this.KGMeltedPerSecond, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			string text2 = string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.KETTLE_MELT_RATE, GameUtil.GetFormattedMass(this.KGToMeltPerBatch, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(this.TargetTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			Descriptor descriptor = new Descriptor(text, text2, Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor);
			return list;
		}

		public SimHashes exhaust_tag;

		public Tag targetElementTag;

		public Tag fuelElementTag;

		public float KGToMeltPerBatch;

		public float KGMeltedPerSecond;

		public float TargetTemperature;

		public float EnergyPerUnitOfLumber;

		public float ExhaustMassPerUnitOfLumber;
	}

	public class WorkingStates : GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State
	{
		public GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State idle;

		public GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State complete;
	}

	public class MeltingStates : GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State
	{
		public GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State entering;

		public IceKettle.WorkingStates working;

		public GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State exit;
	}

	public class IdleStates : GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State
	{
		public GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State notEnoughFuel;

		public GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State waitingForSolids;

		public GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State waitingForSpaceInLiquidTank;
	}

	public class OperationalStates : GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.State
	{
		public IceKettle.MeltingStates melting;

		public IceKettle.IdleStates idle;
	}

	public new class Instance : GameStateMachine<IceKettle, IceKettle.Instance, IStateMachineTarget, IceKettle.Def>.GameInstance
	{
		public float CurrentTemperatureOfSolidsStored
		{
			get
			{
				if (this.kettleStorage.MassStored() <= 0f)
				{
					return 0f;
				}
				return this.kettleStorage.items[0].GetComponent<PrimaryElement>().Temperature;
			}
		}

		public float MeltDurationPerBatch
		{
			get
			{
				return base.def.KGToMeltPerBatch / base.def.KGMeltedPerSecond;
			}
		}

		public float FuelUnitsAvailable
		{
			get
			{
				return this.fuelStorage.MassStored();
			}
		}

		public bool HasAtLeastOneBatchOfSolidsWaitingToMelt
		{
			get
			{
				return this.kettleStorage.MassStored() >= base.def.KGToMeltPerBatch;
			}
		}

		public bool HasEnoughFuelUnitsToMeltNextBatch
		{
			get
			{
				return this.kettleStorage.MassStored() <= 0f || this.FuelUnitsAvailable >= this.FuelRequiredForNextBratch;
			}
		}

		public bool LiquidTankHasCapacityForNextBatch
		{
			get
			{
				return this.outputStorage.RemainingCapacity() >= base.def.KGToMeltPerBatch;
			}
		}

		public float LiquidTankCapacity
		{
			get
			{
				return this.outputStorage.capacityKg;
			}
		}

		public float LiquidStored
		{
			get
			{
				return this.outputStorage.MassStored();
			}
		}

		public float FuelRequiredForNextBratch
		{
			get
			{
				return this.GetUnitsOfFuelRequiredToMelt(this.elementToMelt, base.def.KGToMeltPerBatch, this.CurrentTemperatureOfSolidsStored);
			}
		}

		public float InUseWorkableDuration
		{
			get
			{
				return this.dupeWorkable.workTime;
			}
		}

		public Instance(IStateMachineTarget master, IceKettle.Def def)
			: base(master, def)
		{
			this.elementToMelt = ElementLoader.GetElement(def.targetElementTag);
			this.LiquidMeter = new MeterController(this.animController, IceKettle.LIQUID_METER_TARGET_NAME, IceKettle.LIQUID_METER_ANIM_NAME, Meter.Offset.UserSpecified, Grid.SceneLayer.BuildingFront, Array.Empty<string>());
			Storage[] components = base.gameObject.GetComponents<Storage>();
			this.fuelStorage = components[0];
			this.kettleStorage = components[1];
			this.outputStorage = components[2];
		}

		public override void StartSM()
		{
			base.StartSM();
			this.UpdateMeter();
		}

		public void UpdateMeter()
		{
			this.LiquidMeter.SetPositionPercent(this.outputStorage.MassStored() / this.outputStorage.capacityKg);
		}

		public void MeltNextBatch()
		{
			if (!this.HasAtLeastOneBatchOfSolidsWaitingToMelt)
			{
				return;
			}
			PrimaryElement component = this.kettleStorage.FindFirst(base.def.targetElementTag).GetComponent<PrimaryElement>();
			float num = Mathf.Min(this.GetUnitsOfFuelRequiredToMelt(this.elementToMelt, base.def.KGToMeltPerBatch, component.Temperature), this.FuelUnitsAvailable);
			float num2 = 0f;
			float num3 = 0f;
			SimUtil.DiseaseInfo diseaseInfo;
			this.kettleStorage.ConsumeAndGetDisease(this.elementToMelt.id.CreateTag(), base.def.KGToMeltPerBatch, out num2, out diseaseInfo, out num3);
			this.outputStorage.AddElement(this.elementToMelt.highTempTransitionTarget, num2, base.def.TargetTemperature, diseaseInfo.idx, diseaseInfo.count, false, true);
			float temperature = this.fuelStorage.FindFirst(base.def.fuelElementTag).GetComponent<PrimaryElement>().Temperature;
			this.fuelStorage.ConsumeIgnoringDisease(base.def.fuelElementTag, num);
			float num4 = num * base.def.ExhaustMassPerUnitOfLumber;
			Element element = ElementLoader.FindElementByHash(base.def.exhaust_tag);
			SimMessages.AddRemoveSubstance(Grid.PosToCell(base.gameObject), element.id, null, num4, temperature, byte.MaxValue, 0, true, -1);
		}

		public float GetUnitsOfFuelRequiredToMelt(Element elementToMelt, float massToMelt_KG, float elementToMelt_initialTemperature)
		{
			if (!elementToMelt.IsSolid)
			{
				return -1f;
			}
			float num = massToMelt_KG * elementToMelt.specificHeatCapacity * elementToMelt_initialTemperature;
			float targetTemperature = base.def.TargetTemperature;
			return (massToMelt_KG * elementToMelt.specificHeatCapacity * targetTemperature - num) / base.def.EnergyPerUnitOfLumber;
		}

		private Storage fuelStorage;

		private Storage kettleStorage;

		private Storage outputStorage;

		private Element elementToMelt;

		private MeterController LiquidMeter;

		[MyCmpGet]
		public Operational operational;

		[MyCmpGet]
		private IceKettleWorkable dupeWorkable;

		[MyCmpGet]
		private KBatchedAnimController animController;
	}
}
