using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicPorts : KMonoBehaviour, IEffectDescriptor, IRenderEveryTick
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.isPhysical = base.GetComponent<BuildingComplete>() != null;
		bool flag = !this.isPhysical && base.GetComponent<BuildingUnderConstruction>() == null;
		if (flag)
		{
			OverlayScreen instance = OverlayScreen.Instance;
			instance.OnOverlayChanged = (Action<HashedString>)Delegate.Combine(instance.OnOverlayChanged, new Action<HashedString>(this.OnOverlayChanged));
			this.OnOverlayChanged(OverlayScreen.Instance.mode);
			this.CreateVisualizers();
			SimAndRenderScheduler.instance.Add(this, false);
		}
		else if (this.isPhysical)
		{
			this.UpdateMissingWireIcon();
			this.CreatePhysicalPorts();
		}
		else
		{
			this.CreateVisualizers();
		}
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

	private void CreatePhysicalPorts()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (num == this.cell)
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
				LogicEventSender logicEventSender = new LogicEventSender(info2.id, this.GetActualCell(info2.cellOffset), delegate(int new_value)
				{
					if (this != null)
					{
						this.OnLogicValueChanged(info2.id, new_value);
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
					LogicEventSender logicEventSender2 = this.outputPorts[j] as LogicEventSender;
					logicEventSender2.SetValue(this.serializedOutputValues[j]);
				}
			}
		}
		this.serializedOutputValues = null;
		if (this.inputPortInfo != null)
		{
			this.inputPorts = new List<ILogicUIElement>();
			for (int k = 0; k < this.inputPortInfo.Length; k++)
			{
				LogicPorts.Port info = this.inputPortInfo[k];
				LogicEventHandler logicEventHandler = new LogicEventHandler(this.GetActualCell(info.cellOffset), delegate(int new_value)
				{
					if (this != null)
					{
						this.OnLogicValueChanged(info.id, new_value);
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

	private void OnLogicNetworkConnectionChanged(int cell, bool connected)
	{
		this.UpdateMissingWireIcon();
	}

	private void UpdateMissingWireIcon()
	{
		bool flag = this.ShowMissingWireIcon();
		LogicCircuitManager.ToggleNoWireConnected(flag, base.gameObject);
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

	private void OnLogicValueChanged(HashedString port_id, int new_value)
	{
		if (base.gameObject != null)
		{
			base.gameObject.Trigger(-801688580, new LogicValueChanged
			{
				portID = port_id,
				newValue = new_value
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
		int num = Grid.PosToCell(base.transform.GetPosition());
		return Grid.OffsetCell(num, offset);
	}

	public void SendSignal(HashedString port_id, int new_value)
	{
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
		for (int i = 0; i < this.inputPortInfo.Length; i++)
		{
			if (this.inputPortInfo[i].id == port_id)
			{
				LogicEventHandler logicEventHandler = this.inputPorts[i] as LogicEventHandler;
				return logicEventHandler.Value;
			}
		}
		return 0;
	}

	public int GetOutputValue(HashedString port_id)
	{
		for (int i = 0; i < this.outputPorts.Count; i++)
		{
			LogicEventSender logicEventSender = this.outputPorts[i] as LogicEventSender;
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
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(portCell);
		return networkForCell != null;
	}

	private void OnOverlayChanged(HashedString mode)
	{
		if (mode == OverlayModes.Logic.ID)
		{
			base.enabled = true;
			this.CreateVisualizers();
		}
		else
		{
			base.enabled = false;
			this.DestroyVisualizers();
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = null;
		LogicPorts component = def.BuildingComplete.GetComponent<LogicPorts>();
		if (component != null)
		{
			if (component.inputPortInfo != null && component.inputPortInfo.Length > 0)
			{
				string text = string.Empty;
				string text2 = ((component.inputPortInfo.Length != 1) ? "\n\t\t" : string.Empty);
				foreach (LogicPorts.Port port in component.inputPortInfo)
				{
					text = text + text2 + port.description;
				}
				string empty = string.Empty;
				string text3 = string.Format(UI.LOGIC_PORTS.INPUT_PORTS, text);
				Descriptor descriptor = default(Descriptor);
				descriptor.SetupDescriptor(text3, empty, Descriptor.DescriptorType.Effect);
				if (list == null)
				{
					list = new List<Descriptor>();
				}
				list.Add(descriptor);
			}
			if (component.outputPortInfo != null && component.outputPortInfo.Length > 0)
			{
				string text4 = string.Empty;
				string text5 = ((component.outputPortInfo.Length != 1) ? "\n\t\t" : string.Empty);
				foreach (LogicPorts.Port port2 in component.outputPortInfo)
				{
					text4 = text4 + text5 + port2.description;
				}
				string empty2 = string.Empty;
				string text6 = string.Format(UI.LOGIC_PORTS.OUTPUT_PORTS, text4);
				Descriptor descriptor2 = default(Descriptor);
				descriptor2.SetupDescriptor(text6, empty2, Descriptor.DescriptorType.Effect);
				if (list == null)
				{
					list = new List<Descriptor>();
				}
				list.Add(descriptor2);
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
		public Port(HashedString id, CellOffset cell_offset, LocString description, bool show_wire_missing_icon, LogicPortSpriteType sprite_type)
		{
			this.id = id;
			this.cellOffset = cell_offset;
			this.description = description;
			this.requiresConnection = show_wire_missing_icon;
			this.spriteType = sprite_type;
		}

		public static LogicPorts.Port InputPort(HashedString id, CellOffset cell_offset, LocString description, bool show_wire_missing_icon = false)
		{
			return new LogicPorts.Port(id, cell_offset, description, show_wire_missing_icon, LogicPortSpriteType.Input);
		}

		public static LogicPorts.Port OutputPort(HashedString id, CellOffset cell_offset, LocString description, bool show_wire_missing_icon = false)
		{
			return new LogicPorts.Port(id, cell_offset, description, show_wire_missing_icon, LogicPortSpriteType.Output);
		}

		public HashedString id;

		public CellOffset cellOffset;

		public LocString description;

		public bool requiresConnection;

		public LogicPortSpriteType spriteType;
	}
}
