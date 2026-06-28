using System;
using Klei;
using KSerialization;
using UnityEngine;

public class Turbine : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		BuildingDef def = base.GetComponent<BuildingComplete>().Def;
		Rotatable component = base.GetComponent<Rotatable>();
		this.srcCells = new int[def.WidthInCells];
		this.destCells = new int[def.WidthInCells];
		int num = Grid.PosToCell(this);
		for (int i = 0; i < def.WidthInCells; i++)
		{
			int num2 = i - (def.WidthInCells - 1) / 2;
			this.srcCells[i] = Grid.OffsetCell(num, component.GetRotatedCellOffset(new CellOffset(num2, -1)));
			this.destCells[i] = Grid.OffsetCell(num, component.GetRotatedCellOffset(new CellOffset(num2, def.HeightInCells)));
		}
		this.smi = new Turbine.Instance(this);
		this.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		if (this.smi != null)
		{
			this.smi.StopSM("cleanup");
		}
		base.OnCleanUp();
	}

	private void Pump(float dt)
	{
		float num = this.pumpKGRate * dt / (float)this.srcCells.Length;
		foreach (int num2 in this.srcCells)
		{
			HandleVector<Game.ComplexCallbackInfo>.Handle handle = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(new Action<object>(this.OnSimConsume)));
			SimMessages.ConsumeMass(num2, this.srcElem, num, 1, handle.index);
		}
	}

	private void OnSimConsume(object data)
	{
		Sim.MassConsumptionCallback massConsumptionCallback = (Sim.MassConsumptionCallback)data;
		if (massConsumptionCallback.mass > 0f)
		{
			this.storedTemperature = SimUtil.CalculateFinalTemperature(this.storedMass, this.storedTemperature, massConsumptionCallback.mass, massConsumptionCallback.temperature);
			this.storedMass += massConsumptionCallback.mass;
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(this.diseaseIdx, this.diseaseCount, massConsumptionCallback.diseaseIdx, massConsumptionCallback.diseaseCount);
			this.diseaseIdx = diseaseInfo.idx;
			this.diseaseCount = diseaseInfo.count;
			if (this.storedMass > this.minEmitMass)
			{
				float num = this.storedMass / (float)this.destCells.Length;
				int num2 = this.diseaseCount / this.destCells.Length;
				float num3 = this.storedTemperature + this.destTempDelta;
				foreach (int num4 in this.destCells)
				{
					SimMessages.AddRemoveSubstance(num4, (int)massConsumptionCallback.removedElemIdx, CellEventLogger.Instance.WallPumpSimUpdate, num, num3, this.diseaseIdx, num2, -1);
				}
				this.storedMass = 0f;
				this.storedTemperature = 0f;
				this.diseaseIdx = byte.MaxValue;
				this.diseaseCount = 0;
			}
		}
	}

	public static void InitializeStatusItems()
	{
		Turbine.outputBlockedStatusItem = new StatusItem("TURBINE_BLOCKED_OUTPUT", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 14334);
		Turbine.spinningUpStatusItem = new StatusItem("TURBINE_SPINNING_UP", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.Good, false, SimViewMode.None, true, 14334);
		Turbine.insufficientMassStatusItem = new StatusItem("TURBINE_INSUFFICIENT_MASS", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, true, 14334);
		Turbine.insufficientMassStatusItem.resolveTooltipCallback = delegate(string str, object data)
		{
			Turbine turbine = (Turbine)data;
			str = str.Replace("{MASS}", GameUtil.GetFormattedMass(turbine.srcMinMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			str = str.Replace("{ELEMENT}", ElementLoader.FindElementByHash(turbine.srcElem).name);
			str = str.Replace("{TEMPERATURE}", GameUtil.GetFormattedTemperature(turbine.srcMinTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			return str;
		};
	}

	public SimHashes srcElem;

	public float srcMinMass;

	public float srcMinTemp;

	public float destMaxMass;

	public float destTempDelta;

	public float minEmitMass;

	public float maxRPM;

	public float rpmAcceleration;

	public float rpmDeceleration;

	public float minGenerationRPM;

	public float pumpKGRate;

	[Serialize]
	private float storedMass;

	[Serialize]
	private float storedTemperature;

	[Serialize]
	private byte diseaseIdx = byte.MaxValue;

	[Serialize]
	private int diseaseCount;

	[MyCmpGet]
	private Generator generator;

	[Serialize]
	private float currentRPM;

	private int[] srcCells;

	private int[] destCells;

	private Turbine.Instance smi;

	private static StatusItem insufficientMassStatusItem;

	private static StatusItem outputBlockedStatusItem;

	private static StatusItem spinningUpStatusItem;

	public class States : GameStateMachine<Turbine.States, Turbine.Instance, Turbine>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			Turbine.InitializeStatusItems();
			default_state = this.inoperational;
			this.root.EventTransition(GameHashes.OperationalChanged, this.inoperational, (Turbine.Instance smi) => !smi.master.GetComponent<Operational>().IsOperational);
			this.inoperational.EventTransition(GameHashes.OperationalChanged, this.operational.outputBlocked, (Turbine.Instance smi) => smi.master.GetComponent<Operational>().IsOperational);
			this.operational.DefaultState(this.operational.outputBlocked).Update(delegate(Turbine.Instance smi)
			{
				smi.CheckOperational();
			});
			this.operational.outputBlocked.ToggleStatusItem((Turbine.Instance smi) => Turbine.outputBlockedStatusItem, (Turbine.Instance smi) => smi.master);
			this.operational.insufficientMass.ToggleStatusItem((Turbine.Instance smi) => Turbine.insufficientMassStatusItem, (Turbine.Instance smi) => smi.master);
			this.active.DefaultState(this.active.spinningUp).Update(delegate(Turbine.Instance smi)
			{
				smi.CheckActive();
			});
			this.active.blocked.ToggleStatusItem((Turbine.Instance smi) => Turbine.outputBlockedStatusItem, (Turbine.Instance smi) => smi.master);
			this.active.spinningUp.ToggleStatusItem((Turbine.Instance smi) => Turbine.spinningUpStatusItem, (Turbine.Instance smi) => smi.master);
			this.active.generating.Update(delegate(Turbine.Instance smi)
			{
				smi.master.Pump(smi.dt);
			});
		}

		public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State inoperational;

		public Turbine.States.OperationalStates operational;

		public Turbine.States.ActiveStates active;

		public class OperationalStates : GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State
		{
			public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State outputBlocked;

			public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State insufficientMass;
		}

		public class ActiveStates : GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State
		{
			public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State blocked;

			public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State spinningUp;

			public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State generating;
		}
	}

	public class Instance : GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.GameInstance
	{
		public Instance(Turbine master)
			: base(master)
		{
		}

		public void CheckOperational()
		{
			if (this.IsOutputBlocked())
			{
				base.smi.GoTo(base.sm.operational.outputBlocked);
				return;
			}
			bool flag;
			bool flag2;
			this.GetInputState(out flag, out flag2);
			if (!flag || !flag2)
			{
				base.smi.GoTo(base.sm.operational.insufficientMass);
			}
			else
			{
				base.smi.GoTo(base.sm.active);
			}
		}

		public void CheckActive()
		{
			bool flag = false;
			bool flag2 = false;
			if (!this.IsOutputBlocked())
			{
				this.GetInputState(out flag, out flag2);
			}
			float num = ((!flag || !flag2) ? base.master.rpmDeceleration : base.master.rpmAcceleration);
			base.master.currentRPM = Mathf.Clamp(base.master.currentRPM + this.dt * num, 0f, base.master.maxRPM);
			StateMachine.BaseState currentState = base.smi.GetCurrentState();
			if (base.master.currentRPM > base.master.minGenerationRPM)
			{
				if (currentState != base.sm.active.generating)
				{
					base.smi.GoTo(base.sm.active.generating);
				}
				base.smi.master.generator.GenerateJoules(base.smi.master.generator.WattageRating * this.dt, false);
			}
			else if (currentState != base.sm.active.spinningUp)
			{
				base.smi.GoTo(base.sm.active.spinningUp);
			}
			if (base.master.currentRPM <= 0f)
			{
				base.master.currentRPM = 0f;
				if (currentState != base.sm.operational)
				{
					base.smi.GoTo(base.sm.operational);
				}
			}
		}

		private bool IsOutputBlocked()
		{
			bool flag = false;
			foreach (int num in base.master.destCells)
			{
				if ((!Grid.Element[num].IsVacuum && Grid.Element[num].id != base.master.srcElem) || Grid.Cell[num].mass > base.master.destMaxMass)
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		private void GetInputState(out bool sufficient_mass, out bool sufficient_energy)
		{
			sufficient_mass = true;
			sufficient_energy = true;
			foreach (int num in base.master.srcCells)
			{
				if (Grid.Element[num].id != base.master.srcElem || Grid.Cell[num].mass < base.master.srcMinMass)
				{
					sufficient_mass = false;
				}
				if (Grid.Cell[num].temperature < base.master.srcMinTemp)
				{
					sufficient_energy = false;
				}
			}
		}
	}
}
