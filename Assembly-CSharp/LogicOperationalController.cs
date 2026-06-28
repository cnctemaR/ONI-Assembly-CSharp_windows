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
			LogicOperationalController.infoStatusItem = new StatusItem("LogicOperationalInfo", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
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
		Operational operational = (Operational)data;
		return (!operational.IsOperational) ? BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_DISABLED : BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_ENABLED;
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == LogicOperationalController.PORT_ID)
		{
			LogicCircuitNetwork logicCircuitNetwork = this.CheckWireState();
			base.GetComponent<KSelectable>().ToggleStatusItem(LogicOperationalController.infoStatusItem, logicCircuitNetwork != null && logicValueChanged.newValue > 0, base.GetComponent<Operational>());
		}
	}

	public static readonly HashedString PORT_ID = "LogicOperational";

	public int unNetworkedValue = 1;

	private static Operational.Flag logicOperationalFlag = new Operational.Flag("LogicOperational", Operational.Flag.Type.Requirement);

	private static StatusItem infoStatusItem;
}
