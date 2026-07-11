using System;
using STRINGS;

public class LogicOperationalController : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<LogicOperationalController>(-801688580, LogicOperationalController.OnLogicValueChangedDelegate);
		if (LogicOperationalController.infoStatusItem == null)
		{
			LogicOperationalController.infoStatusItem = new StatusItem("LogicOperationalInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			LogicOperationalController.infoStatusItem.resolveStringCallback = new Func<string, object, string>(LogicOperationalController.ResolveInfoStatusItemString);
		}
		this.CheckWireState();
	}

	private LogicCircuitNetwork GetNetwork()
	{
		int portCell = base.GetComponent<LogicPorts>().GetPortCell(LogicOperationalController.PORT_ID);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
	}

	private LogicCircuitNetwork CheckWireState()
	{
		LogicCircuitNetwork network = this.GetNetwork();
		int num = ((network != null) ? network.OutputValue : this.unNetworkedValue);
		base.GetComponent<Operational>().SetFlag(LogicOperationalController.logicOperationalFlag, num > 0);
		return network;
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		return ((LogicOperationalController)data).GetComponent<Operational>().GetFlag(LogicOperationalController.logicOperationalFlag) ? BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_ENABLED : BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_DISABLED;
	}

	private void OnLogicValueChanged(object data)
	{
		if (((LogicValueChanged)data).portID == LogicOperationalController.PORT_ID)
		{
			LogicCircuitNetwork logicCircuitNetwork = this.CheckWireState();
			base.GetComponent<KSelectable>().ToggleStatusItem(LogicOperationalController.infoStatusItem, logicCircuitNetwork != null, this);
		}
	}

	public static readonly HashedString PORT_ID = "LogicOperational";

	public int unNetworkedValue = 1;

	private static readonly Operational.Flag logicOperationalFlag = new Operational.Flag("LogicOperational", Operational.Flag.Type.Requirement);

	private static StatusItem infoStatusItem;

	public static readonly LogicPorts.Port[] INPUT_PORTS_0_0 = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE, false, false) };

	public static readonly LogicPorts.Port[] INPUT_PORTS_0_1 = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE, false, false) };

	public static readonly LogicPorts.Port[] INPUT_PORTS_1_0 = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(1, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE, false, false) };

	public static readonly LogicPorts.Port[] INPUT_PORTS_1_1 = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(1, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE, false, false) };

	public static readonly LogicPorts.Port[] INPUT_PORTS_N1_0 = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(-1, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE, false, false) };

	private static readonly EventSystem.IntraObjectHandler<LogicOperationalController> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicOperationalController>(delegate(LogicOperationalController component, object data)
	{
		component.OnLogicValueChanged(data);
	});
}
