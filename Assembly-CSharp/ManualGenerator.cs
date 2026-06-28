using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ManualGenerator : Workable, IBatteryRefillControl
{
	private ManualGenerator()
	{
		this.showProgressBar = false;
	}

	public float BatteryRefillPercent
	{
		get
		{
			return this.batteryRefillPercent;
		}
		set
		{
			this.batteryRefillPercent = value;
		}
	}

	public bool IsPowered
	{
		get
		{
			return this.operational.IsActive;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.Subscribe(824508782, new Action<object>(this.OnActiveChanged));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.GeneratingPower;
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		EnergyGenerator.EnsureStatusItemAvailable();
	}

	protected void OnActiveChanged(object is_active)
	{
		if (this.operational.IsActive)
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.ManualGeneratorChargingUp, null);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.SetWorkTime(float.PositiveInfinity);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.HideSymbols(true, ManualGenerator.symbol_names);
		Building component2 = base.GetComponent<Building>();
		this.powerCell = component2.GetPowerOutputCell();
		this.OnActiveChanged(null);
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_generatormanual_kanim") };
		this.smi = new ManualGenerator.GeneratePowerSM.Instance(this);
		this.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		this.smi.StopSM("cleanup");
		base.OnCleanUp();
	}

	private void SimUpdate(float dt)
	{
		if (this.operational.IsActive)
		{
			this.generator.GenerateJoules(this.generator.WattageRating * dt, false);
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, this.generator);
		}
		else
		{
			this.generator.ResetJoules();
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.GeneratorOffline, null);
			if (this.operational.IsOperational)
			{
				CircuitManager circuitManager = Game.Instance.circuitManager;
				if (circuitManager == null)
				{
					return;
				}
				ushort circuitID = circuitManager.GetCircuitID(this.powerCell);
				bool flag = circuitManager.HasBatteries(circuitID);
				bool flag2 = (flag && circuitManager.GetMinBatteryPercentFullOnCircuit(circuitID) < this.batteryRefillPercent) || (!flag && circuitManager.HasConsumers(circuitID));
				if (flag2)
				{
					if (this.chore == null && this.smi.GetCurrentState() == this.smi.sm.on)
					{
						this.chore = new WorkChore<ManualGenerator>(Db.Get().ChoreTypes.GeneratePower, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true);
					}
				}
				else if (this.chore != null)
				{
					this.chore.Cancel("No refill needed");
					this.chore = null;
				}
				this.selectable.ToggleStatusItem(EnergyGenerator.BatteriesSufficientlyFull, !flag2, null);
			}
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.operational.SetActive(true, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		CircuitManager circuitManager = Game.Instance.circuitManager;
		bool flag = false;
		if (circuitManager != null)
		{
			ushort circuitID = circuitManager.GetCircuitID(this.powerCell);
			bool flag2 = circuitManager.HasBatteries(circuitID);
			flag = (flag2 && circuitManager.GetMinBatteryPercentFullOnCircuit(circuitID) < 1f) || (!flag2 && circuitManager.HasConsumers(circuitID));
		}
		AttributeLevels component = worker.GetComponent<AttributeLevels>();
		if (component != null)
		{
			component.AddExperience(Db.Get().Attributes.Athletics.Id, dt);
		}
		return !flag;
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.operational.SetActive(false, false);
		if (this.chore != null && this.generator.PercentFull >= this.batteryRefillPercent)
		{
			this.chore.Cancel("Full enough");
			this.chore = null;
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.operational.SetActive(false, false);
		this.chore = null;
	}

	private void OnOperationalChanged(object data)
	{
		if (!this.buildingEnabledButton.IsEnabled)
		{
			this.generator.ResetJoules();
		}
	}

	private const float batteryStopRunningPercent = 1f;

	[SerializeField]
	[Serialize]
	private float batteryRefillPercent = 0.5f;

	[MyCmpReq]
	private Generator generator;

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private BuildingEnabledButton buildingEnabledButton;

	private Chore chore;

	private int powerCell;

	private ManualGenerator.GeneratePowerSM.Instance smi;

	private static readonly KAnimHashedString[] symbol_names = new KAnimHashedString[]
	{
		new KAnimHashedString("meter"),
		new KAnimHashedString("meter_target"),
		new KAnimHashedString("meter_fill"),
		new KAnimHashedString("meter_frame"),
		new KAnimHashedString("meter_light"),
		new KAnimHashedString("meter_tubing")
	};

	public class GeneratePowerSM : GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			base.serializable = true;
			this.off.EventTransition(GameHashes.OperationalChanged, this.on, (ManualGenerator.GeneratePowerSM.Instance smi) => smi.master.GetComponent<Operational>().IsOperational);
			this.on.EventTransition(GameHashes.OperationalChanged, this.off, (ManualGenerator.GeneratePowerSM.Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, this.working.pre, (ManualGenerator.GeneratePowerSM.Instance smi) => smi.master.GetComponent<Operational>().IsActive);
			this.working.DefaultState(this.working.pre);
			this.working.pre.PlayAnim("working_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.working.loop);
			this.working.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.ActiveChanged, this.working.pst, (ManualGenerator.GeneratePowerSM.Instance smi) => this.masterTarget.Get(smi) != null && !smi.master.GetComponent<Operational>().IsActive);
			this.working.pst.PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.off);
		}

		public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State off;

		public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State on;

		public ManualGenerator.GeneratePowerSM.WorkingStates working;

		public class WorkingStates : GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State
		{
			public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State pre;

			public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State loop;

			public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State pst;
		}

		public new class Instance : GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.GameInstance
		{
			public Instance(IStateMachineTarget master)
				: base(master)
			{
			}
		}
	}
}
