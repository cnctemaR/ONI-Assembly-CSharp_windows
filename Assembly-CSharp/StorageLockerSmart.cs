using System;

public class StorageLockerSmart : StorageLocker
{
	protected override void OnPrefabInit()
	{
		base.Initialize(true);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.ports = base.gameObject.GetComponent<LogicPorts>();
		base.Subscribe(-1697596308, new Action<object>(this.UpdateLogicCircuitCB));
		base.Subscribe(-592767678, new Action<object>(this.UpdateLogicCircuitCB));
		this.UpdateLogicCircuit();
	}

	private void UpdateLogicCircuitCB(object data)
	{
		this.UpdateLogicCircuit();
	}

	private void UpdateLogicCircuit()
	{
		bool flag = this.filteredStorage.IsFull();
		bool isOperational = this.operational.IsOperational;
		bool flag2 = flag && isOperational;
		this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, (!flag2) ? 0 : 1);
		this.filteredStorage.SetLogicMeter(flag2);
	}

	public override float UserMaxCapacity
	{
		get
		{
			return base.UserMaxCapacity;
		}
		set
		{
			base.UserMaxCapacity = value;
			this.UpdateLogicCircuit();
		}
	}

	[MyCmpGet]
	private LogicPorts ports;

	[MyCmpGet]
	private Operational operational;
}
