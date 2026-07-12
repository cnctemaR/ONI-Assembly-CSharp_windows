using System;

public class Campfire : GameStateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.noOperational;
		this.noOperational.Enter(new StateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.State.Callback(Campfire.DisableHeatEmission)).TagTransition(GameTags.Operational, this.operational, false).PlayAnim("off", KAnim.PlayMode.Once);
		this.operational.TagTransition(GameTags.Operational, this.noOperational, true).DefaultState(this.operational.needsFuel);
		this.operational.needsFuel.Enter(new StateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.State.Callback(Campfire.DisableHeatEmission)).EventTransition(GameHashes.OnStorageChange, this.operational.working, new StateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.Transition.ConditionCallback(Campfire.HasFuel)).PlayAnim("off", KAnim.PlayMode.Once);
		this.operational.working.Enter(new StateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.State.Callback(Campfire.EnableHeatEmission)).EventTransition(GameHashes.OnStorageChange, this.operational.needsFuel, GameStateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.Not(new StateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.Transition.ConditionCallback(Campfire.HasFuel))).PlayAnim("on", KAnim.PlayMode.Loop)
			.Exit(new StateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.State.Callback(Campfire.DisableHeatEmission));
	}

	public static bool HasFuel(Campfire.Instance smi)
	{
		return smi.HasFuel;
	}

	public static void EnableHeatEmission(Campfire.Instance smi)
	{
		smi.EnableHeatEmission();
	}

	public static void DisableHeatEmission(Campfire.Instance smi)
	{
		smi.DisableHeatEmission();
	}

	public const string LIT_ANIM_NAME = "on";

	public const string UNLIT_ANIM_NAME = "off";

	public GameStateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.State noOperational;

	public Campfire.OperationalStates operational;

	public StateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.BoolParameter WarmAuraEnabled;

	public class Def : StateMachine.BaseDef
	{
		public Tag fuelTag;

		public float initialFuelMass;
	}

	public class OperationalStates : GameStateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.State
	{
		public GameStateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.State needsFuel;

		public GameStateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.State working;
	}

	public new class Instance : GameStateMachine<Campfire, Campfire.Instance, IStateMachineTarget, Campfire.Def>.GameInstance
	{
		public bool HasFuel
		{
			get
			{
				return this.storage.MassStored() > 0f;
			}
		}

		public bool IsAuraEnabled
		{
			get
			{
				return base.sm.WarmAuraEnabled.Get(this);
			}
		}

		public Instance(IStateMachineTarget master, Campfire.Def def)
			: base(master, def)
		{
		}

		public void EnableHeatEmission()
		{
			this.operational.SetActive(true, false);
			this.light.enabled = true;
			this.heater.EnableEmission = true;
			this.decorProvider.SetValues(CampfireConfig.DECOR_ON);
			this.decorProvider.Refresh();
		}

		public void DisableHeatEmission()
		{
			this.operational.SetActive(false, false);
			this.light.enabled = false;
			this.heater.EnableEmission = false;
			this.decorProvider.SetValues(CampfireConfig.DECOR_OFF);
			this.decorProvider.Refresh();
		}

		[MyCmpGet]
		public Operational operational;

		[MyCmpGet]
		public Storage storage;

		[MyCmpGet]
		public RangeVisualizer rangeVisualizer;

		[MyCmpGet]
		public Light2D light;

		[MyCmpGet]
		public DirectVolumeHeater heater;

		[MyCmpGet]
		public DecorProvider decorProvider;
	}
}
