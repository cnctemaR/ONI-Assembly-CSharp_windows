using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class IceMachine : StateMachineComponent<IceMachine.StatesInstance>
{
	public void SetStorages(Storage waterStorage, Storage iceStorage)
	{
		this.waterStorage = waterStorage;
		this.iceStorage = iceStorage;
	}

	private bool CanMakeIce()
	{
		bool flag = this.waterStorage != null && this.waterStorage.GetMassAvailable(SimHashes.Water) >= 0.1f;
		bool flag2 = this.iceStorage != null && this.iceStorage.IsFull();
		return flag && !flag2;
	}

	private void MakeIce(IceMachine.StatesInstance smi, float dt)
	{
		float num = this.heatRemovalRate * dt / (float)this.waterStorage.items.Count;
		foreach (GameObject gameObject in this.waterStorage.items)
		{
			GameUtil.DeltaThermalEnergy(gameObject.GetComponent<PrimaryElement>(), -num, smi.master.targetTemperature);
		}
		for (int i = this.waterStorage.items.Count; i > 0; i--)
		{
			GameObject gameObject2 = this.waterStorage.items[i - 1];
			if (gameObject2 && gameObject2.GetComponent<PrimaryElement>().Temperature < gameObject2.GetComponent<PrimaryElement>().Element.lowTemp)
			{
				PrimaryElement component = gameObject2.GetComponent<PrimaryElement>();
				this.waterStorage.AddOre(component.Element.lowTempTransitionTarget, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, false, true);
				this.waterStorage.ConsumeIgnoringDisease(gameObject2);
			}
		}
		smi.UpdateIceState();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	[MyCmpGet]
	private Operational operational;

	public Storage waterStorage;

	public Storage iceStorage;

	public float targetTemperature;

	public float heatRemovalRate;

	private static StatusItem iceStorageFullStatusItem;

	public class StatesInstance : GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.GameInstance
	{
		public StatesInstance(IceMachine smi)
			: base(smi)
		{
			this.meter = new MeterController(base.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_OL", "meter_frame", "meter_fill" });
			this.UpdateMeter();
			base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		}

		private void OnStorageChange(object data)
		{
			this.UpdateMeter();
		}

		public void UpdateMeter()
		{
			this.meter.SetPositionPercent(Mathf.Clamp01(base.smi.master.iceStorage.MassStored() / base.smi.master.iceStorage.Capacity()));
		}

		public void UpdateIceState()
		{
			bool flag = false;
			for (int i = base.smi.master.waterStorage.items.Count; i > 0; i--)
			{
				GameObject gameObject = base.smi.master.waterStorage.items[i - 1];
				if (gameObject && gameObject.GetComponent<PrimaryElement>().Temperature <= base.smi.master.targetTemperature)
				{
					flag = true;
				}
			}
			base.sm.doneFreezingIce.Set(flag, this);
		}

		private MeterController meter;

		public Chore emptyChore;
	}

	public class States : GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			base.serializable = true;
			this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (IceMachine.StatesInstance smi) => smi.master.operational.IsOperational);
			this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.off, (IceMachine.StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(this.on.waiting);
			this.on.waiting.EventTransition(GameHashes.OnStorageChange, this.on.working_pre, (IceMachine.StatesInstance smi) => smi.master.CanMakeIce());
			this.on.working_pre.Enter(delegate(IceMachine.StatesInstance smi)
			{
				smi.UpdateIceState();
			}).PlayAnim("working_pre").OnAnimQueueComplete(this.on.working);
			this.on.working.QueueAnim("working_loop", true, null).Update("UpdateWorking", delegate(IceMachine.StatesInstance smi, float dt)
			{
				smi.master.MakeIce(smi, dt);
			}, UpdateRate.SIM_200ms, false).ParamTransition<bool>(this.doneFreezingIce, this.on.working_pst, GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.IsTrue)
				.Enter(delegate(IceMachine.StatesInstance smi)
				{
					smi.master.operational.SetActive(true, false);
					smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(true, "Working");
				})
				.Exit(delegate(IceMachine.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
					smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(false, "Done Working");
				});
			this.on.working_pst.Exit(new StateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State.Callback(this.DoTransfer)).PlayAnim("working_pst").OnAnimQueueComplete(this.on);
		}

		private void DoTransfer(IceMachine.StatesInstance smi)
		{
			for (int i = smi.master.waterStorage.items.Count - 1; i >= 0; i--)
			{
				GameObject gameObject = smi.master.waterStorage.items[i];
				if (gameObject && gameObject.GetComponent<PrimaryElement>().Temperature <= smi.master.targetTemperature)
				{
					smi.master.waterStorage.Transfer(gameObject, smi.master.iceStorage, false, true);
				}
			}
			smi.UpdateMeter();
		}

		public StateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.BoolParameter doneFreezingIce;

		public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State off;

		public IceMachine.States.OnStates on;

		public class OnStates : GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State
		{
			public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State waiting;

			public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State working_pre;

			public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State working;

			public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State working_pst;
		}
	}
}
