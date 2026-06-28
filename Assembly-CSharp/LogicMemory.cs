using System;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicMemory : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		if (LogicMemory.infoStatusItem == null)
		{
			LogicMemory.infoStatusItem = new StatusItem("StoredValue", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			LogicMemory.infoStatusItem.resolveStringCallback = new Func<string, object, string>(LogicMemory.ResolveInfoStatusItemString);
		}
		base.Subscribe(-801688580, new Action<object>(this.OnLogicValueChanged));
	}

	public void OnLogicValueChanged(object data)
	{
		if (((LogicValueChanged)data).portID == LogicMemory.WRITE_PORT_ID)
		{
			int inputValue = this.ports.GetInputValue(LogicMemory.VALUE_PORT_ID);
			this.ports.SendSignal(LogicMemory.READ_PORT_ID, inputValue);
		}
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		LogicMemory logicMemory = (LogicMemory)data;
		int outputValue = logicMemory.ports.GetOutputValue(LogicMemory.READ_PORT_ID);
		return string.Format(BUILDINGS.PREFABS.LOGICMEMORY.STATUS_ITEM_VALUE, outputValue);
	}

	[MyCmpGet]
	private LogicPorts ports;

	private static StatusItem infoStatusItem;

	public static readonly HashedString VALUE_PORT_ID = new HashedString("LogicMemoryValue");

	public static readonly HashedString WRITE_PORT_ID = new HashedString("LogicMemoryWrite");

	public static readonly HashedString READ_PORT_ID = new HashedString("LogicMemoryRead");
}
