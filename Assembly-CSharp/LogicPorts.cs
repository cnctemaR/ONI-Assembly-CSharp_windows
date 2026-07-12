using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/LogicPorts")]
public class LogicPorts : KMonoBehaviour, IGameObjectEffectDescriptor, IRenderEveryTick
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.isPhysical = component == null || component is BuildingComplete;
		if (!this.isPhysical && !(component is BuildingUnderConstruction))
		{
			OverlayScreen instance = OverlayScreen.Instance;
			instance.OnOverlayChanged = (Action<HashedString>)Delegate.Combine(instance.OnOverlayChanged, new Action<HashedString>(this.OnOverlayChanged));
			this.OnOverlayChanged(OverlayScreen.Instance.mode);
			this.CreateVisualizers();
			SimAndRenderScheduler.instance.Add(this, false);
			return;
		}
		if (this.isPhysical)
		{
			this.UpdateMissingWireIcon();
			this.CreatePhysicalPorts(false);
			return;
		}
		this.CreateVisualizers();
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(this.OnOverlayChanged));
		this.DestroyVisualizers();
		if (this.isPhysical)
		{
			this.DestroyPhysicalPorts();
		}
		base.OnCleanUp();
	}

	public void RenderEveryTick(float dt)
	{
		this.CreateVisualizers();
	}

	public void HackRefreshVisualizers()
	{
		this.CreateVisualizers();
	}

	private void CreateVisualizers()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		bool flag = num != this.cell;
		this.cell = num;
		if (!flag)
		{
			Rotatable component = base.GetComponent<Rotatable>();
			if (component != null)
			{
				Orientation orientation = component.GetOrientation();
				flag = orientation != this.orientation;
				this.orientation = orientation;
			}
		}
		if (!flag)
		{
			return;
		}
		this.DestroyVisualizers();
		if (this.outputPortInfo != null)
		{
			this.outputPorts = new List<ILogicUIElement>();
			for (int i = 0; i < this.outputPortInfo.Length; i++)
			{
				LogicPorts.Port port = this.outputPortInfo[i];
				LogicPortVisualizer logicPortVisualizer = new LogicPortVisualizer(this.GetActualCell(port.cellOffset), port.spriteType);
				this.outputPorts.Add(logicPortVisualizer);
				Game.Instance.logicCircuitManager.AddVisElem(logicPortVisualizer);
			}
		}
		if (this.inputPortInfo != null)
		{
			this.inputPorts = new List<ILogicUIElement>();
			for (int j = 0; j < this.inputPortInfo.Length; j++)
			{
				LogicPorts.Port port2 = this.inputPortInfo[j];
				LogicPortVisualizer logicPortVisualizer2 = new LogicPortVisualizer(this.GetActualCell(port2.cellOffset), port2.spriteType);
				this.inputPorts.Add(logicPortVisualizer2);
				Game.Instance.logicCircuitManager.AddVisElem(logicPortVisualizer2);
			}
		}
	}

	private void DestroyVisualizers()
	{
		if (this.outputPorts != null)
		{
			foreach (ILogicUIElement logicUIElement in this.outputPorts)
			{
				Game.Instance.logicCircuitManager.RemoveVisElem(logicUIElement);
			}
		}
		if (this.inputPorts != null)
		{
			foreach (ILogicUIElement logicUIElement2 in this.inputPorts)
			{
				Game.Instance.logicCircuitManager.RemoveVisElem(logicUIElement2);
			}
		}
	}

	private void CreatePhysicalPorts(bool forceCreate = false)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (num == this.cell && !forceCreate)
		{
			return;
		}
		this.cell = num;
		this.DestroyVisualizers();
		if (this.outputPortInfo != null)
		{
			this.outputPorts = new List<ILogicUIElement>();
			for (int i = 0; i < this.outputPortInfo.Length; i++)
			{
				LogicPorts.Port info2 = this.outputPortInfo[i];
				LogicEventSender logicEventSender = new LogicEventSender(info2.id, this.GetActualCell(info2.cellOffset), delegate(int new_value, int prev_value)
				{
					if (this != null)
					{
						this.OnLogicValueChanged(info2.id, new_value, prev_value);
					}
				}, new Action<int, bool>(this.OnLogicNetworkConnectionChanged), info2.spriteType);
				this.outputPorts.Add(logicEventSender);
				Game.Instance.logicCircuitManager.AddVisElem(logicEventSender);
				Game.Instance.logicCircuitSystem.AddToNetworks(logicEventSender.GetLogicUICell(), logicEventSender, true);
			}
			if (this.serializedOutputValues != null && this.serializedOutputValues.Length == this.outputPorts.Count)
			{
				for (int j = 0; j < this.outputPorts.Count; j++)
				{
					(this.outputPorts[j] as LogicEventSender).SetValue(this.serializedOutputValues[j]);
				}
			}
			else
			{
				for (int k = 0; k < this.outputPorts.Count; k++)
				{
					(this.outputPorts[k] as LogicEventSender).SetValue(0);
				}
			}
		}
		this.serializedOutputValues = null;
		if (this.inputPortInfo != null)
		{
			this.inputPorts = new List<ILogicUIElement>();
			for (int l = 0; l < this.inputPortInfo.Length; l++)
			{
				LogicPorts.Port info = this.inputPortInfo[l];
				LogicEventHandler logicEventHandler = new LogicEventHandler(this.GetActualCell(info.cellOffset), delegate(int new_value, int prev_value)
				{
					if (this != null)
					{
						this.OnLogicValueChanged(info.id, new_value, prev_value);
					}
				}, new Action<int, bool>(this.OnLogicNetworkConnectionChanged), info.spriteType);
				this.inputPorts.Add(logicEventHandler);
				Game.Instance.logicCircuitManager.AddVisElem(logicEventHandler);
				Game.Instance.logicCircuitSystem.AddToNetworks(logicEventHandler.GetLogicUICell(), logicEventHandler, true);
			}
		}
	}

	private bool ShowMissingWireIcon()
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		if (this.outputPortInfo != null)
		{
			for (int i = 0; i < this.outputPortInfo.Length; i++)
			{
				LogicPorts.Port port = this.outputPortInfo[i];
				if (port.requiresConnection)
				{
					int portCell = this.GetPortCell(port.id);
					if (logicCircuitManager.GetNetworkForCell(portCell) == null)
					{
						return true;
					}
				}
			}
		}
		if (this.inputPortInfo != null)
		{
			for (int j = 0; j < this.inputPortInfo.Length; j++)
			{
				LogicPorts.Port port2 = this.inputPortInfo[j];
				if (port2.requiresConnection)
				{
					int portCell2 = this.GetPortCell(port2.id);
					if (logicCircuitManager.GetNetworkForCell(portCell2) == null)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void OnMove()
	{
		this.DestroyPhysicalPorts();
		this.CreatePhysicalPorts(false);
	}

	private void OnLogicNetworkConnectionChanged(int cell, bool connected)
	{
		this.UpdateMissingWireIcon();
	}

	private void UpdateMissingWireIcon()
	{
		LogicCircuitManager.ToggleNoWireConnected(this.ShowMissingWireIcon(), base.gameObject);
	}

	private void DestroyPhysicalPorts()
	{
		if (this.outputPorts != null)
		{
			foreach (ILogicUIElement logicUIElement in this.outputPorts)
			{
				ILogicEventSender logicEventSender = (ILogicEventSender)logicUIElement;
				Game.Instance.logicCircuitSystem.RemoveFromNetworks(logicEventSender.GetLogicCell(), logicEventSender, true);
			}
		}
		if (this.inputPorts != null)
		{
			for (int i = 0; i < this.inputPorts.Count; i++)
			{
				LogicEventHandler logicEventHandler = this.inputPorts[i] as LogicEventHandler;
				if (logicEventHandler != null)
				{
					Game.Instance.logicCircuitSystem.RemoveFromNetworks(logicEventHandler.GetLogicCell(), logicEventHandler, true);
				}
			}
		}
	}

	private void OnLogicValueChanged(HashedString port_id, int new_value, int prev_value)
	{
		if (base.gameObject != null)
		{
			base.gameObject.Trigger(-801688580, new LogicValueChanged
			{
				portID = port_id,
				newValue = new_value,
				prevValue = prev_value
			});
		}
	}

	private int GetActualCell(CellOffset offset)
	{
		Rotatable component = base.GetComponent<Rotatable>();
		if (component != null)
		{
			offset = component.GetRotatedCellOffset(offset);
		}
		return Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), offset);
	}

	public bool TryGetPortAtCell(int cell, out LogicPorts.Port port, out bool isInput)
	{
		foreach (LogicPorts.Port port2 in this.inputPortInfo)
		{
			if (this.GetActualCell(port2.cellOffset) == cell)
			{
				port = port2;
				isInput = true;
				return true;
			}
		}
		foreach (LogicPorts.Port port3 in this.outputPortInfo)
		{
			if (this.GetActualCell(port3.cellOffset) == cell)
			{
				port = port3;
				isInput = false;
				return true;
			}
		}
		port = default(LogicPorts.Port);
		isInput = false;
		return false;
	}

	public void SendSignal(HashedString port_id, int new_value)
	{
		if (this.outputPortInfo != null && this.outputPorts == null)
		{
			this.CreatePhysicalPorts(true);
		}
		foreach (ILogicUIElement logicUIElement in this.outputPorts)
		{
			LogicEventSender logicEventSender = (LogicEventSender)logicUIElement;
			if (logicEventSender.ID == port_id)
			{
				logicEventSender.SetValue(new_value);
				break;
			}
		}
	}

	public int GetPortCell(HashedString port_id)
	{
		foreach (LogicPorts.Port port in this.inputPortInfo)
		{
			if (port.id == port_id)
			{
				return this.GetActualCell(port.cellOffset);
			}
		}
		foreach (LogicPorts.Port port2 in this.outputPortInfo)
		{
			if (port2.id == port_id)
			{
				return this.GetActualCell(port2.cellOffset);
			}
		}
		return -1;
	}

	public int GetInputValue(HashedString port_id)
	{
		int num = 0;
		while (num < this.inputPortInfo.Length && this.inputPorts != null)
		{
			if (this.inputPortInfo[num].id == port_id)
			{
				LogicEventHandler logicEventHandler = this.inputPorts[num] as LogicEventHandler;
				if (logicEventHandler == null)
				{
					return 0;
				}
				return logicEventHandler.Value;
			}
			else
			{
				num++;
			}
		}
		return 0;
	}

	public int GetOutputValue(HashedString port_id)
	{
		for (int i = 0; i < this.outputPorts.Count; i++)
		{
			LogicEventSender logicEventSender = this.outputPorts[i] as LogicEventSender;
			if (logicEventSender == null)
			{
				return 0;
			}
			if (logicEventSender.ID == port_id)
			{
				return logicEventSender.GetLogicValue();
			}
		}
		return 0;
	}

	public bool IsPortConnected(HashedString port_id)
	{
		int portCell = this.GetPortCell(port_id);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell) != null;
	}

	private void OnOverlayChanged(HashedString mode)
	{
		if (mode == OverlayModes.Logic.ID)
		{
			base.enabled = true;
			this.CreateVisualizers();
			return;
		}
		base.enabled = false;
		this.DestroyVisualizers();
	}

	public LogicWire.BitDepth GetConnectedWireBitDepth(HashedString port_id)
	{
		LogicWire.BitDepth bitDepth = LogicWire.BitDepth.NumRatings;
		int portCell = this.GetPortCell(port_id);
		GameObject gameObject = Grid.Objects[portCell, 31];
		if (gameObject != null)
		{
			LogicWire component = gameObject.GetComponent<LogicWire>();
			if (component != null)
			{
				bitDepth = component.MaxBitDepth;
			}
		}
		return bitDepth;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		LogicPorts component = go.GetComponent<LogicPorts>();
		if (component != null)
		{
			if (component.inputPortInfo != null && component.inputPortInfo.Length != 0)
			{
				Descriptor descriptor = new Descriptor(UI.LOGIC_PORTS.INPUT_PORTS, UI.LOGIC_PORTS.INPUT_PORTS_TOOLTIP, Descriptor.DescriptorType.Effect, false);
				list.Add(descriptor);
				foreach (LogicPorts.Port port in component.inputPortInfo)
				{
					string text = string.Format(UI.LOGIC_PORTS.INPUT_PORT_TOOLTIP, port.activeDescription, port.inactiveDescription);
					descriptor = new Descriptor(port.description, text, Descriptor.DescriptorType.Effect, false);
					descriptor.IncreaseIndent();
					list.Add(descriptor);
				}
			}
			if (component.outputPortInfo != null && component.outputPortInfo.Length != 0)
			{
				Descriptor descriptor2 = new Descriptor(UI.LOGIC_PORTS.OUTPUT_PORTS, UI.LOGIC_PORTS.OUTPUT_PORTS_TOOLTIP, Descriptor.DescriptorType.Effect, false);
				list.Add(descriptor2);
				foreach (LogicPorts.Port port2 in component.outputPortInfo)
				{
					string text2 = string.Format(UI.LOGIC_PORTS.OUTPUT_PORT_TOOLTIP, port2.activeDescription, port2.inactiveDescription);
					descriptor2 = new Descriptor(port2.description, text2, Descriptor.DescriptorType.Effect, false);
					descriptor2.IncreaseIndent();
					list.Add(descriptor2);
				}
			}
		}
		return list;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		if (this.isPhysical && this.outputPorts != null)
		{
			this.serializedOutputValues = new int[this.outputPorts.Count];
			for (int i = 0; i < this.outputPorts.Count; i++)
			{
				LogicEventSender logicEventSender = this.outputPorts[i] as LogicEventSender;
				this.serializedOutputValues[i] = logicEventSender.GetLogicValue();
			}
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		this.serializedOutputValues = null;
	}

	[SerializeField]
	public LogicPorts.Port[] outputPortInfo;

	[SerializeField]
	public LogicPorts.Port[] inputPortInfo;

	public List<ILogicUIElement> outputPorts;

	public List<ILogicUIElement> inputPorts;

	private int cell = -1;

	private Orientation orientation = Orientation.NumRotations;

	[Serialize]
	private int[] serializedOutputValues;

	private bool isPhysical;

	[Serializable]
	public struct Port
	{
		public Port(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon, LogicPortSpriteType sprite_type, bool display_custom_name = false)
		{
			this.id = id;
			this.cellOffset = cell_offset;
			this.description = description;
			this.activeDescription = activeDescription;
			this.inactiveDescription = inactiveDescription;
			this.requiresConnection = show_wire_missing_icon;
			this.spriteType = sprite_type;
			this.displayCustomName = display_custom_name;
		}

		public static LogicPorts.Port InputPort(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon = false, bool display_custom_name = false)
		{
			return new LogicPorts.Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.Input, display_custom_name);
		}

		public static LogicPorts.Port OutputPort(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon = false, bool display_custom_name = false)
		{
			return new LogicPorts.Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.Output, display_custom_name);
		}

		public static LogicPorts.Port RibbonInputPort(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon = false, bool display_custom_name = false)
		{
			return new LogicPorts.Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.RibbonInput, display_custom_name);
		}

		public static LogicPorts.Port RibbonOutputPort(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon = false, bool display_custom_name = false)
		{
			return new LogicPorts.Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.RibbonOutput, display_custom_name);
		}

		public HashedString id;

		public CellOffset cellOffset;

		public string description;

		public string activeDescription;

		public string inactiveDescription;

		public bool requiresConnection;

		public LogicPortSpriteType spriteType;

		public bool displayCustomName;
	}
}
