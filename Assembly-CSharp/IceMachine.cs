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
		return this.waterStorage != null && !this.waterStorage.IsEmpty();
	}

	private void MakeIce(IceMachine.StatesInstance smi, float dt)
	{
		float num = (this.energyConsumption - this.energyWaste) * dt / (float)this.waterStorage.items.Count;
		foreach (GameObject gameObject in this.waterStorage.items)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			GameUtil.DeltaThermalEnergy(component, -num);
		}
		for (int i = this.waterStorage.items.Count; i > 0; i--)
		{
			GameObject gameObject2 = this.waterStorage.items[i - 1];
			if (gameObject2 && gameObject2.GetComponent<PrimaryElement>().Temperature < gameObject2.GetComponent<PrimaryElement>().Element.lowTemp)
			{
				PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
				this.waterStorage.AddOre(component2.Element.lowTempTransitionTarget, component2.Mass, component2.Temperature, component2.DiseaseIdx, component2.DiseaseCount, false, true);
				this.waterStorage.ConsumeIgnoringDisease(gameObject2);
			}
		}
		for (int j = this.waterStorage.items.Count; j > 0; j--)
		{
			GameObject gameObject3 = this.waterStorage.items[j - 1];
			if (gameObject3 && gameObject3.GetComponent<PrimaryElement>().Temperature <= this.targetTemperature)
			{
				this.waterStorage.Transfer(gameObject3, this.iceStorage, true, true);
			}
		}
		smi.UpdateIceState();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.deliveryComponents = base.GetComponents<ManualDeliveryKG>();
		base.smi.StartSM();
	}

	[MyCmpGet]
	private Operational operational;

	private ManualDeliveryKG[] deliveryComponents;

	public Storage waterStorage;

	public Storage iceStorage;

	public float targetTemperature;

	public float energyConsumption;

	public float energyWaste;

	public class StatesInstance : GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.GameInstance
	{
		public StatesInstance(IceMachine smi)
			: base(smi)
		{
		}

		public void UpdateIceState()
		{
			base.sm.shouldDropIce.Set(!base.smi.master.iceStorage.IsEmpty(), this);
		}
	}

	public class States : GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (IceMachine.StatesInstance smi) => smi.master.operational.IsOperational);
			this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.off, (IceMachine.StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(this.on.waiting);
			this.on.waiting.EventTransition(GameHashes.OnStorageChange, this.on.working_pre, (IceMachine.StatesInstance smi) => smi.master.CanMakeIce());
			this.on.working_pre.Enter(delegate(IceMachine.StatesInstance smi)
			{
				smi.UpdateIceState();
			}).PlayAnim("working_pre").OnAnimQueueComplete(this.on.working);
			this.on.working.Enter(delegate(IceMachine.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
				smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(true, "Working");
			}).QueueAnim("working_loop", true, null).Update("UpdateWorking", delegate(IceMachine.StatesInstance smi, float dt)
			{
				smi.master.MakeIce(smi, dt);
			}, UpdateRate.SIM_200ms, false)
				.ParamTransition<bool>(this.shouldDropIce, this.on.working_pst, GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.IsTrue)
				.Exit(delegate(IceMachine.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
					smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(false, "Done Working");
				});
			this.on.working_pst.Exit(delegate(IceMachine.StatesInstance smi)
			{
				Storage iceStorage = smi.master.iceStorage;
				Vector3 vector = new Vector3(1f, 0f, 0f);
				iceStorage.DropAll(false, false, vector, true);
			}).PlayAnim("working_pst").OnAnimQueueComplete(this.on.waiting);
		}

		public StateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.BoolParameter shouldDropIce;

		public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State off;

		public IceMachine.States.OnStates on;

		private static readonly HashedString[] FULL_ANIMS = new HashedString[] { "working_pst", "off" };

		public class OnStates : GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State
		{
			public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State waiting;

			public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State working_pre;

			public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State working;

			public GameStateMachine<IceMachine.States, IceMachine.StatesInstance, IceMachine, object>.State working_pst;
		}
	}
}
