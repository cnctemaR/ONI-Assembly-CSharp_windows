using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AlgaeDistillery : StateMachineComponent<AlgaeDistillery.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	[SerializeField]
	public Tag emitTag;

	[SerializeField]
	public float emitMass;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ElementConverter emitter;

	[MyCmpReq]
	private Operational operational;

	public class StatesInstance : GameStateMachine<AlgaeDistillery.States, AlgaeDistillery.StatesInstance, AlgaeDistillery, object>.GameInstance
	{
		public StatesInstance(AlgaeDistillery smi)
			: base(smi)
		{
		}

		public void TryEmit()
		{
			Storage storage = base.smi.master.storage;
			GameObject gameObject = storage.FindFirst(base.smi.master.emitTag);
			if (gameObject != null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component.Mass >= base.master.emitMass)
				{
					storage.Drop(gameObject, true);
				}
			}
		}
	}

	public class States : GameStateMachine<AlgaeDistillery.States, AlgaeDistillery.StatesInstance, AlgaeDistillery>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disabled;
			this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (AlgaeDistillery.StatesInstance smi) => !smi.master.operational.IsOperational);
			this.disabled.EventTransition(GameHashes.OperationalChanged, this.waiting, (AlgaeDistillery.StatesInstance smi) => smi.master.operational.IsOperational);
			this.waiting.Enter("Waiting", delegate(AlgaeDistillery.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).EventTransition(GameHashes.OnStorageChange, this.converting, (AlgaeDistillery.StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			this.converting.Enter("Ready", delegate(AlgaeDistillery.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Transition(this.waiting, (AlgaeDistillery.StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll(), UpdateRate.SIM_200ms).EventHandler(GameHashes.OnStorageChange, delegate(AlgaeDistillery.StatesInstance smi)
			{
				smi.TryEmit();
			});
		}

		public GameStateMachine<AlgaeDistillery.States, AlgaeDistillery.StatesInstance, AlgaeDistillery, object>.State disabled;

		public GameStateMachine<AlgaeDistillery.States, AlgaeDistillery.StatesInstance, AlgaeDistillery, object>.State waiting;

		public GameStateMachine<AlgaeDistillery.States, AlgaeDistillery.StatesInstance, AlgaeDistillery, object>.State converting;

		public GameStateMachine<AlgaeDistillery.States, AlgaeDistillery.StatesInstance, AlgaeDistillery, object>.State overpressure;
	}
}
