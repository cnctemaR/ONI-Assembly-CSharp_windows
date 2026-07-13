using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolarPanel : Generator
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.cachedCell = Grid.PosToCell(this);
		base.Subscribe<SolarPanel>(824508782, SolarPanel.OnActiveChangedDelegate);
		this.smi = new SolarPanel.StatesInstance(this);
		this.smi.StartSM();
		this.accumulator = Game.Instance.accumulators.Add("Element", this);
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
	}

	protected override void OnCleanUp()
	{
		this.smi.StopSM("cleanup");
		Game.Instance.accumulators.Remove(this.accumulator);
		base.OnCleanUp();
	}

	protected void OnActiveChanged(object data)
	{
		StatusItem statusItem = (((Operational)data).IsActive ? Db.Get().BuildingStatusItems.Wattage : Db.Get().BuildingStatusItems.GeneratorOffline);
		this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, statusItem, this);
	}

	private void UpdateStatusItem()
	{
		this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.Wattage, false);
		if (this.statusHandle == Guid.Empty)
		{
			this.statusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.SolarPanelWattage, this);
			return;
		}
		if (this.statusHandle != Guid.Empty)
		{
			this.selectable.ReplaceStatusItem(this.statusHandle, Db.Get().BuildingStatusItems.SolarPanelWattage, this);
		}
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		ushort circuitID = base.CircuitID;
		this.operational.SetFlag(Generator.wireConnectedFlag, circuitID != ushort.MaxValue);
		if (!this.operational.IsOperational)
		{
			return;
		}
		float num = 0f;
		foreach (CellOffset cellOffset in this.solarCellOffsets)
		{
			int num2 = Grid.LightIntensity[Grid.OffsetCell(this.cachedCell, cellOffset)];
			num += (float)num2 * 0.00053f;
		}
		this.operational.SetActive(num > 0f, false);
		num = Mathf.Clamp(num, 0f, 380f);
		Game.Instance.accumulators.Accumulate(this.accumulator, num * dt);
		if (num > 0f)
		{
			num *= dt;
			num = Mathf.Max(num, 1f * dt);
			base.GenerateJoules(num, false);
		}
		this.meter.SetPositionPercent(Game.Instance.accumulators.GetAverageRate(this.accumulator) / 380f);
		this.UpdateStatusItem();
	}

	public float CurrentWattage
	{
		get
		{
			return Game.Instance.accumulators.GetAverageRate(this.accumulator);
		}
	}

	private MeterController meter;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private SolarPanel.StatesInstance smi;

	private Guid statusHandle;

	private int cachedCell;

	private CellOffset[] solarCellOffsets = new CellOffset[]
	{
		new CellOffset(-3, 2),
		new CellOffset(-2, 2),
		new CellOffset(-1, 2),
		new CellOffset(0, 2),
		new CellOffset(1, 2),
		new CellOffset(2, 2),
		new CellOffset(3, 2),
		new CellOffset(-3, 1),
		new CellOffset(-2, 1),
		new CellOffset(-1, 1),
		new CellOffset(0, 1),
		new CellOffset(1, 1),
		new CellOffset(2, 1),
		new CellOffset(3, 1)
	};

	private static readonly EventSystem.IntraObjectHandler<SolarPanel> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<SolarPanel>(delegate(SolarPanel component, object data)
	{
		component.OnActiveChanged(data);
	});

	public class StatesInstance : GameStateMachine<SolarPanel.States, SolarPanel.StatesInstance, SolarPanel, object>.GameInstance
	{
		public StatesInstance(SolarPanel master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<SolarPanel.States, SolarPanel.StatesInstance, SolarPanel>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.DoNothing();
		}

		public GameStateMachine<SolarPanel.States, SolarPanel.StatesInstance, SolarPanel, object>.State idle;
	}
}
