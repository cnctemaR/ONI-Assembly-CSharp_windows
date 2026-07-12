using System;
using KSerialization;
using UnityEngine;

public class RemoteWorkerCapacitor : StateMachineComponent<RemoteWorkerCapacitor.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public float ApplyDeltaEnergy(float delta)
	{
		float num = this.charge;
		this.charge = Mathf.Clamp(this.charge + delta, 0f, 60f);
		return this.charge - num;
	}

	public float ChargeRatio
	{
		get
		{
			return this.charge / 60f;
		}
	}

	public float Charge
	{
		get
		{
			return this.charge;
		}
	}

	public bool IsLowPower
	{
		get
		{
			return this.charge < 12f;
		}
	}

	public bool IsOutOfPower
	{
		get
		{
			return this.charge < float.Epsilon;
		}
	}

	[Serialize]
	private float charge;

	public const float LOW_LEVEL = 12f;

	public const float POWER_USE_RATE_J_PER_S = -0.1f;

	public const float POWER_CHARGE_RATE_J_PER_S = 7.5f;

	public const float CAPACITY_J = 60f;

	public class StatesInstance : GameStateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.GameInstance
	{
		public StatesInstance(RemoteWorkerCapacitor master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.InitializeStates(out default_state);
			default_state = this.ok;
			this.root.ToggleStatusItem(Db.Get().DuplicantStatusItems.RemoteWorkerCapacitorStatus, (RemoteWorkerCapacitor.StatesInstance smi) => smi.master);
			this.ok.Transition(this.out_of_power, new StateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.Transition.ConditionCallback(RemoteWorkerCapacitor.States.IsOutOfPower), UpdateRate.SIM_200ms).Transition(this.low_power, new StateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.Transition.ConditionCallback(RemoteWorkerCapacitor.States.IsLowPower), UpdateRate.SIM_200ms);
			this.low_power.Transition(this.out_of_power, new StateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.Transition.ConditionCallback(RemoteWorkerCapacitor.States.IsOutOfPower), UpdateRate.SIM_200ms).Transition(this.ok, new StateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.Transition.ConditionCallback(RemoteWorkerCapacitor.States.IsOkForPower), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.RemoteWorkerLowPower, null);
			this.out_of_power.Transition(this.low_power, new StateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.Transition.ConditionCallback(RemoteWorkerCapacitor.States.IsLowPower), UpdateRate.SIM_200ms).Transition(this.ok, new StateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.Transition.ConditionCallback(RemoteWorkerCapacitor.States.IsOkForPower), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.RemoteWorkerOutOfPower, null);
		}

		public static bool IsOkForPower(RemoteWorkerCapacitor.StatesInstance smi)
		{
			return !smi.master.IsLowPower;
		}

		public static bool IsLowPower(RemoteWorkerCapacitor.StatesInstance smi)
		{
			return smi.master.IsLowPower && !smi.master.IsOutOfPower;
		}

		public static bool IsOutOfPower(RemoteWorkerCapacitor.StatesInstance smi)
		{
			return smi.master.IsOutOfPower;
		}

		private GameStateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.State ok;

		private GameStateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.State low_power;

		private GameStateMachine<RemoteWorkerCapacitor.States, RemoteWorkerCapacitor.StatesInstance, RemoteWorkerCapacitor, object>.State out_of_power;
	}
}
