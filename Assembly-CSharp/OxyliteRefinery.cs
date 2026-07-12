using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class OxyliteRefinery : StateMachineComponent<OxyliteRefinery.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	[MyCmpAdd]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	public Tag emitTag;

	public float emitMass;

	public Vector3 dropOffset;

	public class StatesInstance : GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.GameInstance
	{
		public StatesInstance(OxyliteRefinery smi)
			: base(smi)
		{
		}

		public void TryEmit()
		{
			Storage storage = base.smi.master.storage;
			GameObject gameObject = storage.FindFirst(base.smi.master.emitTag);
			if (gameObject != null && gameObject.GetComponent<PrimaryElement>().Mass >= base.master.emitMass)
			{
				Vector3 vector = base.transform.GetPosition() + base.master.dropOffset;
				vector.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
				gameObject.transform.SetPosition(vector);
				storage.Drop(gameObject, true);
			}
		}
	}

	public class States : GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disabled;
			this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (OxyliteRefinery.StatesInstance smi) => !smi.master.operational.IsOperational);
			this.disabled.EventTransition(GameHashes.OperationalChanged, this.waiting, (OxyliteRefinery.StatesInstance smi) => smi.master.operational.IsOperational);
			this.waiting.EventTransition(GameHashes.OnStorageChange, this.converting, (OxyliteRefinery.StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting(false));
			this.converting.Enter(delegate(OxyliteRefinery.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit(delegate(OxyliteRefinery.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).Transition(this.waiting, (OxyliteRefinery.StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll(), UpdateRate.SIM_200ms)
				.EventHandler(GameHashes.OnStorageChange, delegate(OxyliteRefinery.StatesInstance smi)
				{
					smi.TryEmit();
				});
		}

		public GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.State disabled;

		public GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.State waiting;

		public GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.State converting;
	}
}
