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
		base.Subscribe<StorageLockerSmart>(-1697596308, StorageLockerSmart.UpdateLogicCircuitCBDelegate);
		base.Subscribe<StorageLockerSmart>(-592767678, StorageLockerSmart.UpdateLogicCircuitCBDelegate);
		this.UpdateLogicAndActiveState();
	}

	private void UpdateLogicCircuitCB(object data)
	{
		this.UpdateLogicAndActiveState();
	}

	private void UpdateLogicAndActiveState()
	{
		bool flag = this.filteredStorage.IsFull();
		bool isOperational = this.operational.IsOperational;
		bool flag2 = flag && isOperational;
		this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, flag2 ? 1 : 0);
		this.filteredStorage.SetLogicMeter(flag2);
		this.operational.SetActive(isOperational, false);
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
			this.UpdateLogicAndActiveState();
		}
	}

	[MyCmpGet]
	private LogicPorts ports;

	[MyCmpGet]
	private Operational operational;

	private static readonly EventSystem.IntraObjectHandler<StorageLockerSmart> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<StorageLockerSmart>(delegate(StorageLockerSmart component, object data)
	{
		component.UpdateLogicCircuitCB(data);
	});
}
