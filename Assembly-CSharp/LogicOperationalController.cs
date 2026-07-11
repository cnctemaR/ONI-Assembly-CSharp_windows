using System;
using STRINGS;

public class LogicOperationalController : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-801688580, new Action<object>(this.OnLogicValueChanged));
		if (LogicOperationalController.infoStatusItem == null)
		{
			LogicOperationalController.infoStatusItem = new StatusItem("LogicOperationalInfo", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			LogicOperationalController.infoStatusItem.resolveStringCallback = new Func<string, object, string>(LogicOperationalController.ResolveInfoStatusItemString);
		}
		this.CheckWireState();
	}

	private LogicCircuitNetwork GetNetwork()
	{
		LogicPorts component = base.GetComponent<LogicPorts>();
		int portCell = component.GetPortCell(LogicOperationalController.PORT_ID);
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		return logicCircuitManager.GetNetworkForCell(portCell);
	}

	private LogicCircuitNetwork CheckWireState()
	{
		LogicCircuitNetwork network = this.GetNetwork();
		int num = ((network == null) ? this.unNetworkedValue : network.OutputValue);
		base.GetComponent<Operational>().SetFlag(LogicOperationalController.logicOperationalFlag, num > 0);
		return network;
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		LogicOperationalController logicOperationalController = (LogicOperationalController)data;
		Operational component = logicOperationalController.GetComponent<Operational>();
		return (!component.GetFlag(LogicOperationalController.logicOperationalFlag)) ? BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_DISABLED : BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_ENABLED;
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

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
