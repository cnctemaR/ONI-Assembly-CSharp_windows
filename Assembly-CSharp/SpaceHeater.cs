using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpaceHeater : StateMachineComponent<SpaceHeater.StatesInstance>
{
	public float TargetTemperature
	{
		get
		{
			return this.targetTemperature;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetLiquidHeater()
	{
		this.heatLiquid = true;
	}

	private SpaceHeater.MonitorState MonitorHeating(float dt)
	{
		this.monitorCells.Clear();
		int num = Grid.PosToCell(base.transform.position);
		GameUtil.GetNonSolidCells(num, this.radius, this.monitorCells);
		int num2 = 0;
		float num3 = 0f;
		for (int i = 0; i < this.monitorCells.Count; i++)
		{
			if (Grid.Cell[this.monitorCells[i]].mass > this.minimumCellMass && ((Grid.Element[this.monitorCells[i]].IsGas && !this.heatLiquid) || (Grid.Element[this.monitorCells[i]].IsLiquid && this.heatLiquid)))
			{
				num2++;
				num3 += Grid.Temperature[this.monitorCells[i]];
			}
		}
		SpaceHeater.MonitorState monitorState;
		if (num2 == 0)
		{
			monitorState = ((!this.heatLiquid) ? SpaceHeater.MonitorState.NotEnoughGas : SpaceHeater.MonitorState.NotEnoughLiquid);
		}
		else
		{
			bool flag = num3 / (float)num2 >= this.targetTemperature;
			if (flag)
			{
				monitorState = SpaceHeater.MonitorState.TooHot;
			}
			else
			{
				monitorState = SpaceHeater.MonitorState.ReadyToHeat;
			}
		}
		return monitorState;
	}

	public float targetTemperature = 308.15f;

	public float minimumCellMass = 0f;

	public int radius = 2;

	[SerializeField]
	private bool heatLiquid = false;

	[MyCmpReq]
	private Operational operational;

	private const float UPDATE_INTERVAL = 5f;

	private List<int> monitorCells = new List<int>();

	public class StatesInstance : GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.GameInstance
	{
		public StatesInstance(SpaceHeater master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.offline;
			base.serializable = false;
			this.statusItemUnderMassLiquid = new StatusItem("statusItemUnderMassLiquid", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, 30718);
			this.statusItemUnderMassGas = new StatusItem("statusItemUnderMassGas", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, 30718);
			this.statusItemOverTemp = new StatusItem("statusItemOverTemp", BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, 30718);
			this.statusItemOverTemp.resolveStringCallback = delegate(string str, object obj)
			{
				SpaceHeater.StatesInstance statesInstance = (SpaceHeater.StatesInstance)obj;
				return string.Format(str, GameUtil.GetFormattedTemperature(statesInstance.master.TargetTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			};
			this.offline.EventTransition(GameHashes.OperationalChanged, this.online, (SpaceHeater.StatesInstance smi) => smi.master.operational.IsOperational);
			this.online.EventTransition(GameHashes.OperationalChanged, this.offline, (SpaceHeater.StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(this.online.heating).ToggleSchedulePeriodic("spaceheater_online", 5f, delegate(SpaceHeater.StatesInstance smi)
			{
				switch (smi.master.MonitorHeating(5f))
				{
				case SpaceHeater.MonitorState.ReadyToHeat:
					smi.GoTo(this.online.heating);
					break;
				case SpaceHeater.MonitorState.TooHot:
					smi.GoTo(this.online.overtemp);
					break;
				case SpaceHeater.MonitorState.NotEnoughLiquid:
					smi.GoTo(this.online.undermassliquid);
					break;
				case SpaceHeater.MonitorState.NotEnoughGas:
					smi.GoTo(this.online.undermassgas);
					break;
				}
			});
			this.online.heating.Enter(delegate(SpaceHeater.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit(delegate(SpaceHeater.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			});
			this.online.undermassliquid.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemUnderMassLiquid, null);
			this.online.undermassgas.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemUnderMassGas, null);
			this.online.overtemp.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemOverTemp, null);
		}

		public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State offline;

		public SpaceHeater.States.OnlineStates online;

		private StatusItem statusItemUnderMassLiquid;

		private StatusItem statusItemUnderMassGas;

		private StatusItem statusItemOverTemp;

		public class OnlineStates : GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State
		{
			public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State heating;

			public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State overtemp;

			public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State undermassliquid;

			public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State undermassgas;
		}
	}

	private enum MonitorState
	{
		ReadyToHeat,
		TooHot,
		NotEnoughLiquid,
		NotEnoughGas
	}
}
