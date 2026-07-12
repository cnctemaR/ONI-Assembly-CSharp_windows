using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Refrigerator")]
public class Refrigerator : KMonoBehaviour, IUserControlledCapacity
{
	protected override void OnPrefabInit()
	{
		this.filteredStorage = new FilteredStorage(this, new Tag[] { GameTags.Compostable }, this, true, Db.Get().ChoreTypes.FoodFetch);
	}

	protected override void OnSpawn()
	{
		base.GetComponent<KAnimControllerBase>().Play("off", KAnim.PlayMode.Once, 1f, 0f);
		base.GetComponent<FoodStorage>().FilteredStorage = this.filteredStorage;
		this.filteredStorage.FilterChanged();
		this.UpdateLogicCircuit();
		base.Subscribe<Refrigerator>(-905833192, Refrigerator.OnCopySettingsDelegate);
		base.Subscribe<Refrigerator>(-1697596308, Refrigerator.UpdateLogicCircuitCBDelegate);
		base.Subscribe<Refrigerator>(-592767678, Refrigerator.UpdateLogicCircuitCBDelegate);
	}

	protected override void OnCleanUp()
	{
		this.filteredStorage.CleanUp();
	}

	public bool IsActive()
	{
		return this.operational.IsActive;
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		Refrigerator component = gameObject.GetComponent<Refrigerator>();
		if (component == null)
		{
			return;
		}
		this.UserMaxCapacity = component.UserMaxCapacity;
	}

	public float UserMaxCapacity
	{
		get
		{
			return Mathf.Min(this.userMaxCapacity, this.storage.capacityKg);
		}
		set
		{
			this.userMaxCapacity = value;
			this.filteredStorage.FilterChanged();
			this.UpdateLogicCircuit();
		}
	}

	public float AmountStored
	{
		get
		{
			return this.storage.MassStored();
		}
	}

	public float MinCapacity
	{
		get
		{
			return 0f;
		}
	}

	public float MaxCapacity
	{
		get
		{
			return this.storage.capacityKg;
		}
	}

	public bool WholeValues
	{
		get
		{
			return false;
		}
	}

	public LocString CapacityUnits
	{
		get
		{
			return GameUtil.GetCurrentMassUnit(false);
		}
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
		this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, flag2 ? 1 : 0);
		this.filteredStorage.SetLogicMeter(flag2);
	}

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private LogicPorts ports;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	private FilteredStorage filteredStorage;

	private static readonly EventSystem.IntraObjectHandler<Refrigerator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Refrigerator>(delegate(Refrigerator component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Refrigerator> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<Refrigerator>(delegate(Refrigerator component, object data)
	{
		component.UpdateLogicCircuitCB(data);
	});
}
