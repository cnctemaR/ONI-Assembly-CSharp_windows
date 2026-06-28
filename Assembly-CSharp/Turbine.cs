using System;
using Klei;
using KSerialization;
using UnityEngine;

public class Turbine : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.simEmitCBHandle = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(new Action<object>(this.OnSimEmitted)));
		BuildingDef def = base.GetComponent<BuildingComplete>().Def;
		this.srcCells = new int[def.WidthInCells];
		this.destCells = new int[def.WidthInCells];
		int num = Grid.PosToCell(this);
		for (int i = 0; i < def.WidthInCells; i++)
		{
			int num2 = i - (def.WidthInCells - 1) / 2;
			this.srcCells[i] = Grid.OffsetCell(num, new CellOffset(num2, -1));
			this.destCells[i] = Grid.OffsetCell(num, new CellOffset(num2, def.HeightInCells - 1));
			int num3 = Grid.OffsetCell(num, new CellOffset(num2, 0));
			SimMessages.SetCellProperties(num3, 7);
			Grid.Foundation[num3] = true;
			Grid.SetSolid(num3, true, CellEventLogger.Instance.SimCellOccupierForceSolid);
			Grid.RenderedByWorld[num3] = false;
			World.Instance.OnSolidChanged(num3);
			GameScenePartitioner.Instance.TriggerEvent(num3, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
		this.smi = new Turbine.Instance(this);
		this.smi.StartSM();
		this.CreateMeter();
	}

	private void CreateMeter()
	{
		this.meter = new MeterController(base.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_OL", "meter_frame", "meter_fill" });
		this.smi.UpdateMeter();
	}

	protected override void OnCleanUp()
	{
		if (this.smi != null)
		{
			this.smi.StopSM("cleanup");
		}
		BuildingDef def = base.GetComponent<BuildingComplete>().Def;
		int num = Grid.PosToCell(this);
		for (int i = 0; i < def.WidthInCells; i++)
		{
			int num2 = i - (def.WidthInCells - 1) / 2;
			int num3 = Grid.OffsetCell(num, new CellOffset(num2, 0));
			SimMessages.ClearCellProperties(num3, 7);
			Grid.Foundation[num3] = false;
			Grid.SetSolid(num3, false, CellEventLogger.Instance.SimCellOccupierForceSolid);
			Grid.RenderedByWorld[num3] = true;
			World.Instance.OnSolidChanged(num3);
			GameScenePartitioner.Instance.TriggerEvent(num3, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
		Game.Instance.complexCallbackManager.Release(this.simEmitCBHandle);
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
		Sim.MassConsumedCallback massConsumedCallback = (Sim.MassConsumedCallback)data;
		if (massConsumedCallback.mass > 0f)
		{
			this.storedTemperature = SimUtil.CalculateFinalTemperature(this.storedMass, this.storedTemperature, massConsumedCallback.mass, massConsumedCallback.temperature);
			this.storedMass += massConsumedCallback.mass;
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(this.diseaseIdx, this.diseaseCount, massConsumedCallback.diseaseIdx, massConsumedCallback.diseaseCount);
			this.diseaseIdx = diseaseInfo.idx;
			this.diseaseCount = diseaseInfo.count;
			if (this.storedMass > this.minEmitMass)
			{
				float num = this.storedMass / (float)this.destCells.Length;
				int num2 = this.diseaseCount / this.destCells.Length;
				foreach (int num3 in this.destCells)
				{
					SimMessages.EmitMass(num3, massConsumedCallback.elemIdx, num, this.emitTemperature, this.diseaseIdx, num2, this.simEmitCBHandle.index);
				}
				this.storedMass = 0f;
				this.storedTemperature = 0f;
				this.diseaseIdx = byte.MaxValue;
				this.diseaseCount = 0;
			}
		}
	}

	private void OnSimEmitted(object data)
	{
		Sim.MassEmittedCallback massEmittedCallback = (Sim.MassEmittedCallback)data;
		if (massEmittedCallback.suceeded != 1)
		{
			this.storedTemperature = SimUtil.CalculateFinalTemperature(this.storedMass, this.storedTemperature, massEmittedCallback.mass, massEmittedCallback.temperature);
			this.storedMass += massEmittedCallback.mass;
			if (massEmittedCallback.diseaseIdx != 255)
			{
				SimUtil.DiseaseInfo diseaseInfo = new SimUtil.DiseaseInfo
				{
					idx = this.diseaseIdx,
					count = this.diseaseCount
				};
				SimUtil.DiseaseInfo diseaseInfo2 = new SimUtil.DiseaseInfo
				{
					idx = massEmittedCallback.diseaseIdx,
					count = massEmittedCallback.diseaseCount
				};
				SimUtil.DiseaseInfo diseaseInfo3 = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo, diseaseInfo2);
				this.diseaseIdx = diseaseInfo3.idx;
				this.diseaseCount = diseaseInfo3.count;
			}
		}
	}

	public static void InitializeStatusItems()
	{
		Turbine.inputBlockedStatusItem = new StatusItem("TURBINE_BLOCKED_INPUT", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
		Turbine.outputBlockedStatusItem = new StatusItem("TURBINE_BLOCKED_OUTPUT", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
		Turbine.spinningUpStatusItem = new StatusItem("TURBINE_SPINNING_UP", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Good, false, SimViewMode.None, true, 63486);
		Turbine.activeStatusItem = new StatusItem("TURBINE_ACTIVE", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Good, false, SimViewMode.None, true, 63486);
		Turbine.activeStatusItem.resolveStringCallback = delegate(string str, object data)
		{
			Turbine turbine = (Turbine)data;
			str = string.Format(str, (int)turbine.currentRPM);
			return str;
		};
		Turbine.insufficientMassStatusItem = new StatusItem("TURBINE_INSUFFICIENT_MASS", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, true, 63486);
		Turbine.insufficientMassStatusItem.resolveTooltipCallback = delegate(string str, object data)
		{
			Turbine turbine2 = (Turbine)data;
			str = str.Replace("{MASS}", GameUtil.GetFormattedMass(turbine2.requiredMassFlowDifferential, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			str = str.Replace("{ELEMENT}", ElementLoader.FindElementByHash(turbine2.srcElem).name);
			return str;
		};
		Turbine.insufficientTemperatureStatusItem = new StatusItem("TURBINE_INSUFFICIENT_TEMPERATURE", "BUILDING", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, true, 63486);
		Turbine.insufficientTemperatureStatusItem.resolveStringCallback = new Func<string, object, string>(Turbine.ResolveStrings);
		Turbine.insufficientTemperatureStatusItem.resolveTooltipCallback = new Func<string, object, string>(Turbine.ResolveStrings);
	}

	private static string ResolveStrings(string str, object data)
	{
		Turbine turbine = (Turbine)data;
		str = str.Replace("{ELEMENT}", ElementLoader.FindElementByHash(turbine.srcElem).name);
		str = str.Replace("{TEMPERATURE}", GameUtil.GetFormattedTemperature(turbine.minActiveTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
		return str;
	}

	public SimHashes srcElem;

	public float requiredMassFlowDifferential = 3f;

	public float activePercent = 0.75f;

	public float minEmitMass;

	public float minActiveTemperature = 400f;

	public float emitTemperature = 300f;

	public float maxRPM;

	public float rpmAcceleration;

	public float rpmDeceleration;

	public float minGenerationRPM;

	public float pumpKGRate;

	private static readonly HashedString TINT_SYMBOL = new HashedString("meter_fill");

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

	private static StatusItem inputBlockedStatusItem;

	private static StatusItem outputBlockedStatusItem;

	private static StatusItem insufficientMassStatusItem;

	private static StatusItem insufficientTemperatureStatusItem;

	private static StatusItem activeStatusItem;

	private static StatusItem spinningUpStatusItem;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)7;

	private MeterController meter;

	private HandleVector<Game.ComplexCallbackInfo>.Handle simEmitCBHandle = HandleVector<Game.ComplexCallbackInfo>.InvalidHandle;

	public class States : GameStateMachine<Turbine.States, Turbine.Instance, Turbine>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			Turbine.InitializeStatusItems();
			default_state = this.operational;
			base.serializable = true;
			this.inoperational.EventTransition(GameHashes.OperationalChanged, this.operational.spinningUp, (Turbine.Instance smi) => smi.master.GetComponent<Operational>().IsOperational).QueueAnim("off", false, null).Enter(delegate(Turbine.Instance smi)
			{
				smi.master.currentRPM = 0f;
				smi.UpdateMeter();
			});
			this.operational.DefaultState(this.operational.spinningUp).EventTransition(GameHashes.OperationalChanged, this.inoperational, (Turbine.Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).Update("UpdateOperational", delegate(Turbine.Instance smi, float dt)
			{
				smi.UpdateState(dt);
			}, UpdateRate.SIM_200ms, false)
				.Exit(delegate(Turbine.Instance smi)
				{
					smi.DisableStatusItems();
				});
			this.operational.idle.QueueAnim("on", false, null);
			this.operational.spinningUp.ToggleStatusItem((Turbine.Instance smi) => Turbine.spinningUpStatusItem, (Turbine.Instance smi) => smi.master).QueueAnim("buildup", true, null);
			this.operational.active.Update("UpdateActive", delegate(Turbine.Instance smi, float dt)
			{
				smi.master.Pump(dt);
			}, UpdateRate.SIM_200ms, false).ToggleStatusItem((Turbine.Instance smi) => Turbine.activeStatusItem, (Turbine.Instance smi) => smi.master).Enter(delegate(Turbine.Instance smi)
			{
				smi.GetComponent<KAnimControllerBase>().Play(Turbine.States.ACTIVE_ANIMS, KAnim.PlayMode.Loop);
				smi.GetComponent<Operational>().SetActive(true, false);
			})
				.Exit(delegate(Turbine.Instance smi)
				{
					smi.master.GetComponent<Generator>().ResetJoules();
					smi.GetComponent<Operational>().SetActive(false, false);
				});
		}

		public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State inoperational;

		public Turbine.States.OperationalStates operational;

		private static readonly HashedString[] ACTIVE_ANIMS = new HashedString[] { "working_pre", "working_loop" };

		public class OperationalStates : GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State
		{
			public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State idle;

			public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State spinningUp;

			public GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.State active;
		}
	}

	public class Instance : GameStateMachine<Turbine.States, Turbine.Instance, Turbine, object>.GameInstance
	{
		public Instance(Turbine master)
			: base(master)
		{
		}

		public void UpdateState(float dt)
		{
			bool flag = this.CanSteamFlow(ref this.insufficientMass, ref this.insufficientTemperature);
			float num = ((!flag) ? (-base.master.rpmDeceleration) : base.master.rpmAcceleration);
			base.master.currentRPM = Mathf.Clamp(base.master.currentRPM + dt * num, 0f, base.master.maxRPM);
			this.UpdateMeter();
			this.UpdateStatusItems();
			StateMachine.BaseState currentState = base.smi.GetCurrentState();
			if (base.master.currentRPM >= base.master.minGenerationRPM)
			{
				if (currentState != base.sm.operational.active)
				{
					base.smi.GoTo(base.sm.operational.active);
				}
				base.smi.master.generator.GenerateJoules(base.smi.master.generator.WattageRating * dt, false);
			}
			else if (base.master.currentRPM > 0f)
			{
				if (currentState != base.sm.operational.spinningUp)
				{
					base.smi.GoTo(base.sm.operational.spinningUp);
				}
			}
			else if (currentState != base.sm.operational.idle)
			{
				base.smi.GoTo(base.sm.operational.idle);
			}
		}

		public void UpdateMeter()
		{
			if (base.master.meter != null)
			{
				float num = Mathf.Clamp01(base.master.currentRPM / base.master.maxRPM);
				base.master.meter.SetPositionPercent(num);
				base.master.meter.SetSymbolTint(Turbine.TINT_SYMBOL, (num < base.master.activePercent) ? Color.red : Color.green);
			}
		}

		private bool CanSteamFlow(ref bool insufficient_mass, ref bool insufficient_temperature)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = float.PositiveInfinity;
			this.isInputBlocked = false;
			for (int i = 0; i < base.master.srcCells.Length; i++)
			{
				int num4 = base.master.srcCells[i];
				float num5 = Grid.Mass[num4];
				if (Grid.Element[num4].id == base.master.srcElem)
				{
					num = Mathf.Max(num, num5);
				}
				float num6 = Grid.Temperature[num4];
				num2 = Mathf.Max(num2, num6);
				byte b = Grid.ElementIdx[num4];
				Element element = ElementLoader.elements[(int)b];
				if (element.IsLiquid || element.IsSolid)
				{
					this.isInputBlocked = true;
				}
			}
			this.isOutputBlocked = false;
			for (int j = 0; j < base.master.destCells.Length; j++)
			{
				int num7 = base.master.destCells[j];
				float num8 = Grid.Mass[num7];
				num3 = Mathf.Min(num3, num8);
				byte b2 = Grid.ElementIdx[num7];
				Element element2 = ElementLoader.elements[(int)b2];
				if (element2.IsLiquid || element2.IsSolid)
				{
					this.isOutputBlocked = true;
				}
			}
			insufficient_mass = num - num3 < base.master.requiredMassFlowDifferential;
			insufficient_temperature = num2 < base.master.minActiveTemperature;
			return !insufficient_mass && !insufficient_temperature;
		}

		public void UpdateStatusItems()
		{
			KSelectable component = base.GetComponent<KSelectable>();
			this.inputBlockedHandle = this.UpdateStatusItem(Turbine.inputBlockedStatusItem, this.isInputBlocked, this.inputBlockedHandle, component);
			this.outputBlockedHandle = this.UpdateStatusItem(Turbine.outputBlockedStatusItem, this.isOutputBlocked, this.outputBlockedHandle, component);
			this.insufficientMassHandle = this.UpdateStatusItem(Turbine.insufficientMassStatusItem, this.insufficientMass, this.insufficientMassHandle, component);
			this.insufficientTemperatureHandle = this.UpdateStatusItem(Turbine.insufficientTemperatureStatusItem, this.insufficientTemperature, this.insufficientTemperatureHandle, component);
		}

		private Guid UpdateStatusItem(StatusItem item, bool show, Guid current_handle, KSelectable ksel)
		{
			Guid guid = current_handle;
			if (show != (current_handle != Guid.Empty))
			{
				if (show)
				{
					guid = ksel.AddStatusItem(item, base.master);
				}
				else
				{
					guid = ksel.RemoveStatusItem(current_handle, false);
				}
			}
			return guid;
		}

		public void DisableStatusItems()
		{
			KSelectable component = base.GetComponent<KSelectable>();
			component.RemoveStatusItem(this.inputBlockedHandle, false);
			component.RemoveStatusItem(this.outputBlockedHandle, false);
			component.RemoveStatusItem(this.insufficientMassHandle, false);
			component.RemoveStatusItem(this.insufficientTemperatureHandle, false);
		}

		public bool isInputBlocked;

		public bool isOutputBlocked;

		public bool insufficientMass;

		public bool insufficientTemperature;

		private Guid inputBlockedHandle = Guid.Empty;

		private Guid outputBlockedHandle = Guid.Empty;

		private Guid insufficientMassHandle = Guid.Empty;

		private Guid insufficientTemperatureHandle = Guid.Empty;
	}
}
