using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Electrolyzer : StateMachineComponent<Electrolyzer.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.GetComponent<Storage>().choreType = Db.Get().ChoreTypes.FetchCritical;
	}

	protected override void OnSpawn()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (this.hasMeter)
		{
			this.meter = new MeterController(component, "U2H_meter_target", "meter", Meter.Offset.Behind, new Vector3(-0.4f, 0.5f, -0.1f), new string[] { "U2H_meter_target", "U2H_meter_tank", "U2H_meter_waterbody", "U2H_meter_level" });
		}
		base.smi.StartSM();
	}

	private void SimUpdate(float dt)
	{
		if (this.hasMeter)
		{
			float num = Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg);
			this.meter.SetPositionPercent(num);
		}
	}

	private bool RoomForPressure
	{
		get
		{
			int num = Grid.PosToCell(base.transform.position);
			num = Grid.CellAbove(num);
			return !GameUtil.FloodFillCheck(new Func<int, bool>(this.OverPressure), num, 3, true, true);
		}
	}

	public bool OverPressure(int cell)
	{
		return Grid.Cell[cell].mass > this.maxMass;
	}

	[SerializeField]
	public float maxMass = 2.5f;

	[SerializeField]
	public bool hasMeter = true;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ElementConverter emitter;

	[MyCmpReq]
	private Operational operational;

	private MeterController meter;

	public class StatesInstance : GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.GameInstance
	{
		public StatesInstance(Electrolyzer smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disabled;
			this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (Electrolyzer.StatesInstance smi) => !smi.master.operational.IsOperational);
			this.disabled.EventTransition(GameHashes.OperationalChanged, this.waiting, (Electrolyzer.StatesInstance smi) => smi.master.operational.IsOperational);
			this.waiting.Enter("Waiting", delegate(Electrolyzer.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).EventTransition(GameHashes.OnStorageChange, this.converting, (Electrolyzer.StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			this.converting.Enter("Ready", delegate(Electrolyzer.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Transition(this.waiting, (Electrolyzer.StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll()).Transition(this.overpressure, (Electrolyzer.StatesInstance smi) => !smi.master.RoomForPressure);
			this.overpressure.Enter("OverPressure", delegate(Electrolyzer.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk, null).Transition(this.converting, (Electrolyzer.StatesInstance smi) => smi.master.RoomForPressure);
		}

		public GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State disabled;

		public GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State waiting;

		public GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State converting;

		public GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State overpressure;
	}
}
