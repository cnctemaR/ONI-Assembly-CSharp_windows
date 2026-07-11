using System;
using KSerialization;
using UnityEngine;

public class StorageLocker : KMonoBehaviour, IUserControlledCapacity
{
	protected override void OnPrefabInit()
	{
		this.Initialize(false);
	}

	protected void Initialize(bool use_logic_meter)
	{
		base.OnPrefabInit();
		this.log = new LoggerFS("StorageLocker", 35);
		this.filteredStorage = new FilteredStorage(this, null, null, this, use_logic_meter, Db.Get().ChoreTypes.StorageFetch);
		base.Subscribe<StorageLocker>(-905833192, StorageLocker.OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		this.filteredStorage.FilterChanged();
		if (!this.lockerName.IsNullOrWhiteSpace())
		{
			this.SetName(this.lockerName);
		}
	}

	protected override void OnCleanUp()
	{
		this.filteredStorage.CleanUp();
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		StorageLocker component = gameObject.GetComponent<StorageLocker>();
		if (component == null)
		{
			return;
		}
		this.UserMaxCapacity = component.UserMaxCapacity;
	}

	public virtual float UserMaxCapacity
	{
		get
		{
			return Mathf.Min(this.userMaxCapacity, base.GetComponent<Storage>().capacityKg);
		}
		set
		{
			this.userMaxCapacity = value;
			this.filteredStorage.FilterChanged();
		}
	}

	public float AmountStored
	{
		get
		{
			return base.GetComponent<Storage>().MassStored();
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
			return base.GetComponent<Storage>().capacityKg;
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

	public void SetName(string name)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		base.name = name;
		this.lockerName = name;
		if (component != null)
		{
			component.SetName(name);
		}
		base.gameObject.name = name;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	private LoggerFS log;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	[Serialize]
	public string lockerName = "";

	protected FilteredStorage filteredStorage;

	private static readonly EventSystem.IntraObjectHandler<StorageLocker> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<StorageLocker>(delegate(StorageLocker component, object data)
	{
		component.OnCopySettings(data);
	});
}
