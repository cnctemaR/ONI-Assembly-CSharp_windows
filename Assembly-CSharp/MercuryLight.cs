using System;
using System.Collections.Generic;
using Klei;
using STRINGS;
using UnityEngine;

public class MercuryLight : GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.noOperational;
		this.noOperational.Enter(new StateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State.Callback(MercuryLight.SetOperationalActiveFlagOff)).ParamTransition<float>(this.Charge, this.noOperational.depleating, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsGTZero).ParamTransition<float>(this.Charge, this.noOperational.idle, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsLTEZero);
		this.noOperational.depleating.TagTransition(GameTags.Operational, this.operational, false).PlayAnim("depleating", KAnim.PlayMode.Loop).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingLight, null)
			.ToggleStatusItem(Db.Get().BuildingStatusItems.MercuryLight_Depleating, null)
			.ParamTransition<float>(this.Charge, this.noOperational.depleated, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsLTEZero)
			.Update(new Action<MercuryLight.Instance, float>(MercuryLight.DepleteUpdate), UpdateRate.SIM_200ms, false);
		this.noOperational.depleated.TagTransition(GameTags.Operational, this.operational, false).PlayAnim("on_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.noOperational.idle);
		this.noOperational.idle.TagTransition(GameTags.Operational, this.noOperational.exit, false).PlayAnim("off", KAnim.PlayMode.Once).ToggleStatusItem(Db.Get().BuildingStatusItems.MercuryLight_Depleated, null);
		this.noOperational.exit.PlayAnim("on_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.operational);
		this.operational.TagTransition(GameTags.Operational, this.noOperational, true).DefaultState(this.operational.darkness).Update(new Action<MercuryLight.Instance, float>(MercuryLight.ConsumeFuelUpdate), UpdateRate.SIM_200ms, false);
		this.operational.darkness.Enter(new StateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State.Callback(MercuryLight.SetOperationalActiveFlagOff)).ParamTransition<bool>(this.HasEnoughFuel, this.operational.light, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsTrue).ParamTransition<float>(this.Charge, this.operational.darkness.depleating, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsGTZero)
			.ParamTransition<float>(this.Charge, this.operational.darkness.idle, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsLTEZero);
		this.operational.darkness.depleating.PlayAnim("depleating", KAnim.PlayMode.Loop).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingLight, null).ToggleStatusItem(Db.Get().BuildingStatusItems.MercuryLight_Depleating, null)
			.ParamTransition<float>(this.Charge, this.operational.darkness.depleated, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsLTEZero)
			.Update(new Action<MercuryLight.Instance, float>(MercuryLight.DepleteUpdate), UpdateRate.SIM_200ms, false);
		this.operational.darkness.depleated.PlayAnim("on_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.operational.darkness.idle);
		this.operational.darkness.idle.PlayAnim("off", KAnim.PlayMode.Once).ToggleStatusItem(Db.Get().BuildingStatusItems.MercuryLight_Depleated, null).ParamTransition<float>(this.Charge, this.operational.darkness.depleating, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsGTZero);
		this.operational.light.Enter(new StateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State.Callback(MercuryLight.SetOperationalActiveFlagOn)).PlayAnim("on", KAnim.PlayMode.Loop).ParamTransition<bool>(this.HasEnoughFuel, this.operational.darkness, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsFalse)
			.ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingLight, null)
			.DefaultState(this.operational.light.charging);
		this.operational.light.charging.ToggleStatusItem(Db.Get().BuildingStatusItems.MercuryLight_Charging, null).ParamTransition<float>(this.Charge, this.operational.light.idle, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsGTEOne).Update(new Action<MercuryLight.Instance, float>(MercuryLight.ChargeUpdate), UpdateRate.SIM_200ms, false);
		this.operational.light.idle.ToggleStatusItem(Db.Get().BuildingStatusItems.MercuryLight_Charged, null).ParamTransition<float>(this.Charge, this.operational.light.charging, GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.IsLTOne);
	}

	public static void SetOperationalActiveFlagOn(MercuryLight.Instance smi)
	{
		smi.operational.SetActive(true, false);
	}

	public static void SetOperationalActiveFlagOff(MercuryLight.Instance smi)
	{
		smi.operational.SetActive(false, false);
	}

	public static void DepleteUpdate(MercuryLight.Instance smi, float dt)
	{
		smi.DepleteUpdate(dt);
	}

	public static void ChargeUpdate(MercuryLight.Instance smi, float dt)
	{
		smi.ChargeUpdate(dt);
	}

	public static void ConsumeFuelUpdate(MercuryLight.Instance smi, float dt)
	{
		smi.ConsumeFuelUpdate(dt);
	}

	private static Tag ELEMENT_TAG = SimHashes.Mercury.CreateTag();

	private const string ON_ANIM_NAME = "on";

	private const string ON_PRE_ANIM_NAME = "on_pre";

	private const string TRANSITION_TO_OFF_ANIM_NAME = "on_pst";

	private const string DEPLEATING_ANIM_NAME = "depleating";

	private const string OFF_ANIM_NAME = "off";

	private const string LIGHT_LEVEL_METER_TARGET_NAME = "meter_target";

	private const string LIGHT_LEVEL_METER_ANIM_NAME = "meter";

	public MercuryLight.Darknesstates noOperational;

	public MercuryLight.OperationalStates operational;

	public StateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.FloatParameter Charge;

	public StateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.BoolParameter HasEnoughFuel;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			string text = MercuryLight.ELEMENT_TAG.ProperName();
			List<Descriptor> list = new List<Descriptor>();
			Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, text, GameUtil.GetFormattedMass(this.FUEL_MASS_PER_SECOND, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, text, GameUtil.GetFormattedMass(this.FUEL_MASS_PER_SECOND, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false);
			list.Add(descriptor);
			return list;
		}

		public float MAX_LUX;

		public float TURN_ON_DELAY;

		public float FUEL_MASS_PER_SECOND;
	}

	public class LightStates : GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State
	{
		public GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State charging;

		public GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State idle;
	}

	public class Darknesstates : GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State
	{
		public GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State depleating;

		public GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State depleated;

		public GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State idle;

		public GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State exit;
	}

	public class OperationalStates : GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.State
	{
		public MercuryLight.LightStates light;

		public MercuryLight.Darknesstates darkness;
	}

	public new class Instance : GameStateMachine<MercuryLight, MercuryLight.Instance, IStateMachineTarget, MercuryLight.Def>.GameInstance
	{
		public bool HasEnoughFuel
		{
			get
			{
				return base.sm.HasEnoughFuel.Get(this);
			}
		}

		public int LuxLevel
		{
			get
			{
				return Mathf.FloorToInt(base.smi.ChargeLevel * base.def.MAX_LUX);
			}
		}

		public float ChargeLevel
		{
			get
			{
				return base.smi.sm.Charge.Get(this);
			}
		}

		public Instance(IStateMachineTarget master, MercuryLight.Def def)
			: base(master, def)
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			this.lightIntensityMeterController = new MeterController(component, "meter_target", "meter", Meter.Offset.NoChange, Grid.SceneLayer.Building, Array.Empty<string>());
		}

		public override void StartSM()
		{
			base.StartSM();
			this.SetChargeLevel(this.ChargeLevel);
		}

		public void DepleteUpdate(float dt)
		{
			float num = Mathf.Clamp(this.ChargeLevel - dt / base.def.TURN_ON_DELAY, 0f, 1f);
			this.SetChargeLevel(num);
		}

		public void ChargeUpdate(float dt)
		{
			float num = Mathf.Clamp(this.ChargeLevel + dt / base.def.TURN_ON_DELAY, 0f, 1f);
			this.SetChargeLevel(num);
		}

		public void SetChargeLevel(float value)
		{
			base.sm.Charge.Set(value, this, false);
			this.light.Lux = this.LuxLevel;
			this.light.FullRefresh();
			bool flag = this.ChargeLevel > 0f;
			if (this.light.enabled != flag)
			{
				this.light.enabled = flag;
			}
			this.lightIntensityMeterController.SetPositionPercent(value);
		}

		public void ConsumeFuelUpdate(float dt)
		{
			float num = base.def.FUEL_MASS_PER_SECOND * dt;
			if (this.storage.MassStored() < num)
			{
				base.sm.HasEnoughFuel.Set(false, this, false);
				return;
			}
			float num2;
			SimUtil.DiseaseInfo diseaseInfo;
			float num3;
			this.storage.ConsumeAndGetDisease(MercuryLight.ELEMENT_TAG, num, out num2, out diseaseInfo, out num3);
			base.sm.HasEnoughFuel.Set(true, this, false);
		}

		public bool CanRun()
		{
			return true;
		}

		[MyCmpGet]
		public Operational operational;

		[MyCmpGet]
		private Light2D light;

		[MyCmpGet]
		private Storage storage;

		[MyCmpGet]
		private ConduitConsumer conduitConsumer;

		private MeterController lightIntensityMeterController;
	}
}
