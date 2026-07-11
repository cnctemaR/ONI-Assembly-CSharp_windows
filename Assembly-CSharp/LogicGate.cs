using System;
using KSerialization;

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
		int num = ((this.inputTwo == null) ? 0 : this.inputTwo.Value);
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
			this.outputValue = ((value != 0) ? 0 : 1);
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

	protected virtual int GetCustomValue(int val1, int val2)
	{
		return val1;
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
		}
		else if (base.RequiresTwoInputs)
		{
			component.Play("on_" + (this.inputOne.Value + this.inputTwo.Value * 2 + this.outputValue * 4).ToString(), KAnim.PlayMode.Once, 1f, 0f);
		}
		else
		{
			component.Play("on_" + (this.inputOne.Value + this.outputValue * 4).ToString(), KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	public void OnLogicNetworkConnectionChanged(bool connected)
	{
	}

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
}
