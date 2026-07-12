using System;
using System.Collections.Generic;
using UnityEngine;

public class OilChanger : GameStateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.inoperational;
		this.root.EventHandler(GameHashes.OnStorageChange, new StateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State.Callback(OilChanger.UpdateStorageMeter));
		this.inoperational.PlayAnim("off").Enter(new StateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State.Callback(OilChanger.LED_Off)).Enter(new StateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State.Callback(OilChanger.UpdateStorageMeter))
			.TagTransition(GameTags.Operational, this.operational, false);
		this.operational.PlayAnim("on").Enter(new StateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State.Callback(OilChanger.UpdateStorageMeter)).TagTransition(GameTags.Operational, this.inoperational, true)
			.DefaultState(this.operational.oilNeeded);
		this.operational.oilNeeded.Enter(new StateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State.Callback(OilChanger.LED_Off)).ToggleStatusItem(Db.Get().BuildingStatusItems.WaitingForMaterials, null).EventTransition(GameHashes.OnStorageChange, this.operational.ready, new StateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.Transition.ConditionCallback(OilChanger.HasEnoughLubricant));
		this.operational.ready.Enter(new StateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State.Callback(OilChanger.LED_On)).ToggleChore(new Func<OilChanger.Instance, Chore>(OilChanger.CreateChore), this.operational.oilNeeded);
	}

	public static bool HasEnoughLubricant(OilChanger.Instance smi)
	{
		return smi.OilAmount >= smi.def.MIN_LUBRICANT_MASS_TO_WORK;
	}

	private static bool IsOperational(OilChanger.Instance smi)
	{
		return smi.IsOperational;
	}

	public static void UpdateStorageMeter(OilChanger.Instance smi)
	{
		smi.UpdateStorageMeter();
	}

	public static void LED_On(OilChanger.Instance smi)
	{
		smi.SetLEDState(true);
	}

	public static void LED_Off(OilChanger.Instance smi)
	{
		smi.SetLEDState(false);
	}

	private static WorkChore<OilChangerWorkableUse> CreateChore(OilChanger.Instance smi)
	{
		return new WorkChore<OilChangerWorkableUse>(Db.Get().ChoreTypes.OilChange, smi.master, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.personalNeeds, 5, false, true);
	}

	public const string STORAGE_METER_TARGET_NAME = "meter_target";

	public const string STORAGE_METER_ANIM_NAME = "meter";

	public const string LED_METER_TARGET_NAME = "light_target";

	public const string LED_METER_ANIM_ON_NAME = "light_on";

	public const string LED_METER_ANIM_OFF_NAME = "light_off";

	public GameStateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State inoperational;

	public OilChanger.OperationalStates operational;

	public class Def : StateMachine.BaseDef
	{
		public float MIN_LUBRICANT_MASS_TO_WORK = 200f;
	}

	public class OperationalStates : GameStateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State
	{
		public GameStateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State oilNeeded;

		public GameStateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.State ready;
	}

	public new class Instance : GameStateMachine<OilChanger, OilChanger.Instance, IStateMachineTarget, OilChanger.Def>.GameInstance, IFetchList
	{
		public bool IsOperational
		{
			get
			{
				return this.operational.IsOperational;
			}
		}

		public float OilAmount
		{
			get
			{
				return this.storage.GetMassAvailable(GameTags.LubricatingOil);
			}
		}

		public Instance(IStateMachineTarget master, OilChanger.Def def)
		{
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			Tag lubricatingOil = GameTags.LubricatingOil;
			dictionary[lubricatingOil] = 0f;
			this.remainingLubricationMass = dictionary;
			base..ctor(master, def);
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			this.storage = base.GetComponent<Storage>();
			this.operational = base.GetComponent<Operational>();
			this.oilStorageMeter = new MeterController(component, "meter_target", "meter", Meter.Offset.UserSpecified, Grid.SceneLayer.BuildingFront, Array.Empty<string>());
			this.readyLightMeter = new MeterController(component, "light_target", "light_off", Meter.Offset.UserSpecified, Grid.SceneLayer.BuildingFront, Array.Empty<string>());
		}

		public void SetLEDState(bool isOn)
		{
			string text = (isOn ? "light_on" : "light_off");
			this.readyLightMeter.meterController.Play(text, KAnim.PlayMode.Once, 1f, 0f);
		}

		public void UpdateStorageMeter()
		{
			float num = this.OilAmount / this.storage.capacityKg;
			this.oilStorageMeter.SetPositionPercent(num);
		}

		public Storage Destination
		{
			get
			{
				return this.storage;
			}
		}

		public float GetMinimumAmount(Tag tag)
		{
			return base.def.MIN_LUBRICANT_MASS_TO_WORK;
		}

		public Dictionary<Tag, float> GetRemaining()
		{
			this.remainingLubricationMass[GameTags.LubricatingOil] = Mathf.Clamp(base.def.MIN_LUBRICANT_MASS_TO_WORK - this.OilAmount, 0f, base.def.MIN_LUBRICANT_MASS_TO_WORK);
			return this.remainingLubricationMass;
		}

		public Dictionary<Tag, float> GetRemainingMinimum()
		{
			throw new NotImplementedException();
		}

		private Storage storage;

		private Operational operational;

		private MeterController oilStorageMeter;

		private MeterController readyLightMeter;

		private Dictionary<Tag, float> remainingLubricationMass;
	}
}
