using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RustDeoxidizer : StateMachineComponent<RustDeoxidizer.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
		Tutorial.Instance.oxygenGenerators.Add(base.gameObject);
	}

	protected override void OnCleanUp()
	{
		Tutorial.Instance.oxygenGenerators.Remove(base.gameObject);
		base.OnCleanUp();
	}

	private bool RoomForPressure
	{
		get
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			num = Grid.CellAbove(num);
			return !GameUtil.FloodFillCheck<RustDeoxidizer>(new Func<int, RustDeoxidizer, bool>(RustDeoxidizer.OverPressure), this, num, 3, true, true);
		}
	}

	private static bool OverPressure(int cell, RustDeoxidizer rustDeoxidizer)
	{
		return Grid.Mass[cell] > rustDeoxidizer.maxMass;
	}

	[SerializeField]
	public float maxMass = 2.5f;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ElementConverter emitter;

	[MyCmpReq]
	private Operational operational;

	private MeterController meter;

	public class StatesInstance : GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.GameInstance
	{
		public StatesInstance(RustDeoxidizer smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disabled;
			this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (RustDeoxidizer.StatesInstance smi) => !smi.master.operational.IsOperational);
			this.disabled.EventTransition(GameHashes.OperationalChanged, this.waiting, (RustDeoxidizer.StatesInstance smi) => smi.master.operational.IsOperational);
			this.waiting.Enter("Waiting", delegate(RustDeoxidizer.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).EventTransition(GameHashes.OnStorageChange, this.converting, (RustDeoxidizer.StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting(false));
			this.converting.Enter("Ready", delegate(RustDeoxidizer.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Transition(this.waiting, (RustDeoxidizer.StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll(), UpdateRate.SIM_200ms).Transition(this.overpressure, (RustDeoxidizer.StatesInstance smi) => !smi.master.RoomForPressure, UpdateRate.SIM_200ms);
			this.overpressure.Enter("OverPressure", delegate(RustDeoxidizer.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk, null).Transition(this.converting, (RustDeoxidizer.StatesInstance smi) => smi.master.RoomForPressure, UpdateRate.SIM_200ms);
		}

		public GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State disabled;

		public GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State waiting;

		public GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State converting;

		public GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State overpressure;
	}
}
