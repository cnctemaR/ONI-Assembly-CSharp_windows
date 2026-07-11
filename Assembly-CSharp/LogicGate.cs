using System;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicGate : LogicGateBase, ILogicEventSender, ILogicNetworkConnection
{
	protected override void OnSpawn()
	{
		this.inputOne = new LogicEventHandler(base.InputCellOne, new Action<int>(this.UpdateState), null, LogicPortSpriteType.Input);
		if (base.RequiresTwoInputs)
		{
			this.inputTwo = new LogicEventHandler(base.InputCellTwo, new Action<int>(this.UpdateState), null, LogicPortSpriteType.Input);
		}
		base.Subscribe<LogicGate>(774203113, LogicGate.OnBuildingBrokenDelegate);
		base.Subscribe<LogicGate>(-1735440190, LogicGate.OnBuildingFullyRepairedDelegate);
		BuildingHP component = base.GetComponent<BuildingHP>();
		if (component == null || !component.IsBroken)
		{
			this.Connect();
		}
	}

	protected override void OnCleanUp()
	{
		this.cleaningUp = true;
		this.Disconnect();
		base.Unsubscribe<LogicGate>(774203113, LogicGate.OnBuildingBrokenDelegate, false);
		base.Unsubscribe<LogicGate>(-1735440190, LogicGate.OnBuildingFullyRepairedDelegate, false);
		base.OnCleanUp();
	}

	private void OnBuildingBroken(object data)
	{
		this.Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		this.Connect();
	}

	private void Connect()
	{
		if (!this.connected)
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
			this.connected = true;
			int outputCell = base.OutputCell;
			logicCircuitSystem.AddToNetworks(outputCell, this, true);
			this.output = new LogicPortVisualizer(outputCell, LogicPortSpriteType.Output);
			logicCircuitManager.AddVisElem(this.output);
			int inputCellOne = base.InputCellOne;
			logicCircuitSystem.AddToNetworks(inputCellOne, this.inputOne, true);
			logicCircuitManager.AddVisElem(this.inputOne);
			if (base.RequiresTwoInputs)
			{
				int inputCellTwo = base.InputCellTwo;
				logicCircuitSystem.AddToNetworks(inputCellTwo, this.inputTwo, true);
				logicCircuitManager.AddVisElem(this.inputTwo);
			}
			this.RefreshAnimation();
		}
	}

	private void Disconnect()
	{
		if (this.connected)
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
			this.connected = false;
			int outputCell = base.OutputCell;
			logicCircuitSystem.RemoveFromNetworks(outputCell, this, true);
			logicCircuitManager.RemoveVisElem(this.output);
			this.output = null;
			int inputCellOne = base.InputCellOne;
			logicCircuitSystem.RemoveFromNetworks(inputCellOne, this.inputOne, true);
			logicCircuitManager.RemoveVisElem(this.inputOne);
			this.inputOne = null;
			if (base.RequiresTwoInputs)
			{
				int inputCellTwo = base.InputCellTwo;
				logicCircuitSystem.RemoveFromNetworks(inputCellTwo, this.inputTwo, true);
				logicCircuitManager.RemoveVisElem(this.inputTwo);
				this.inputTwo = null;
			}
			this.RefreshAnimation();
		}
	}

	private void UpdateState(int new_value)
	{
		if (this.cleaningUp)
		{
			return;
		}
		int value = this.inputOne.Value;
		int num = ((this.inputTwo != null) ? this.inputTwo.Value : 0);
		this.outputValue = 0;
		switch (this.op)
		{
		case LogicGateBase.Op.And:
			this.outputValue = value & num;
			break;
		case LogicGateBase.Op.Or:
			this.outputValue = value | num;
			break;
		case LogicGateBase.Op.Not:
			this.outputValue = ((value == 0) ? 1 : 0);
			break;
		case LogicGateBase.Op.Xor:
			this.outputValue = value ^ num;
			break;
		case LogicGateBase.Op.CustomSingle:
			this.outputValue = this.GetCustomValue(value, num);
			break;
		}
		this.RefreshAnimation();
	}

	public virtual void LogicTick()
	{
	}

	protected virtual int GetCustomValue(int val1, int val2)
	{
		return val1;
	}

	public int GetPortValue(LogicGateBase.PortId port)
	{
		switch (port)
		{
		case LogicGateBase.PortId.InputOne:
			return this.inputOne.Value;
		case LogicGateBase.PortId.InputTwo:
			if (!base.RequiresTwoInputs)
			{
				return 0;
			}
			return this.inputTwo.Value;
		}
		return this.outputValue;
	}

	public bool GetPortConnected(LogicGateBase.PortId port)
	{
		if (port == LogicGateBase.PortId.InputTwo && !base.RequiresTwoInputs)
		{
			return false;
		}
		int num = base.PortCell(port);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(num) != null;
	}

	public void SetPortDescriptions(LogicGate.LogicGateDescriptions descriptions)
	{
		this.descriptions = descriptions;
	}

	public LogicGate.LogicGateDescriptions.Description GetPortDescription(LogicGateBase.PortId port)
	{
		switch (port)
		{
		case LogicGateBase.PortId.InputOne:
			if (this.descriptions.inputOne != null)
			{
				return this.descriptions.inputOne;
			}
			if (!base.RequiresTwoInputs)
			{
				return LogicGate.INPUT_ONE_SINGLE_DESCRIPTION;
			}
			return LogicGate.INPUT_ONE_DOUBLE_DESCRIPTION;
		case LogicGateBase.PortId.InputTwo:
			if (this.descriptions.inputTwo == null)
			{
				return LogicGate.INPUT_TWO_DESCRIPTION;
			}
			return this.descriptions.inputTwo;
		}
		return this.descriptions.output;
	}

	public int GetLogicValue()
	{
		return this.outputValue;
	}

	public int GetLogicCell()
	{
		return this.GetLogicUICell();
	}

	public int GetLogicUICell()
	{
		return base.OutputCell;
	}

	public bool IsLogicInput()
	{
		return false;
	}

	protected void RefreshAnimation()
	{
		if (this.cleaningUp)
		{
			return;
		}
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		int outputCell = base.OutputCell;
		if (!(Game.Instance.logicCircuitSystem.GetNetworkForCell(outputCell) is LogicCircuitNetwork))
		{
			component.Play("off", KAnim.PlayMode.Once, 1f, 0f);
			return;
		}
		if (base.RequiresTwoInputs)
		{
			component.Play("on_" + (this.inputOne.Value + this.inputTwo.Value * 2 + this.outputValue * 4).ToString(), KAnim.PlayMode.Once, 1f, 0f);
			return;
		}
		component.Play("on_" + (this.inputOne.Value + this.outputValue * 4).ToString(), KAnim.PlayMode.Once, 1f, 0f);
	}

	public void OnLogicNetworkConnectionChanged(bool connected)
	{
	}

	private static readonly LogicGate.LogicGateDescriptions.Description INPUT_ONE_SINGLE_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_NAME,
		active = UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_INACTIVE
	};

	private static readonly LogicGate.LogicGateDescriptions.Description INPUT_ONE_DOUBLE_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_ONE_NAME,
		active = UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_ONE_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_ONE_INACTIVE
	};

	private static readonly LogicGate.LogicGateDescriptions.Description INPUT_TWO_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_TWO_NAME,
		active = UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_TWO_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_TWO_INACTIVE
	};

	private LogicGate.LogicGateDescriptions descriptions;

	private const bool IS_CIRCUIT_ENDPOINT = true;

	private bool connected;

	protected bool cleaningUp;

	[Serialize]
	protected int outputValue;

	private LogicEventHandler inputOne;

	private LogicEventHandler inputTwo;

	private LogicPortVisualizer output;

	private static readonly EventSystem.IntraObjectHandler<LogicGate> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<LogicGate>(delegate(LogicGate component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicGate> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<LogicGate>(delegate(LogicGate component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	public class LogicGateDescriptions
	{
		public LogicGate.LogicGateDescriptions.Description inputOne;

		public LogicGate.LogicGateDescriptions.Description inputTwo;

		public LogicGate.LogicGateDescriptions.Description output;

		public class Description
		{
			public string name;

			public string active;

			public string inactive;
		}
	}
}
