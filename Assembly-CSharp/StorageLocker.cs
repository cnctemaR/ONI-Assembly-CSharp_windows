using System;
using KSerialization;
using STRINGS;
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
		this.filteredStorage = new FilteredStorage(this, null, null, this, use_logic_meter, Db.Get().ChoreTypes.Fetch);
		base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
	}

	protected override void OnSpawn()
	{
		base.Subscribe(1088293757, new Action<object>(this.OnToggleClosed));
		this.filteredStorage.FilterChanged();
	}

	protected override void OnCleanUp()
	{
		this.filteredStorage.CleanUp();
	}

	private void OnToggleClosed(object data)
	{
		BuildingEnabledButton component = base.GetComponent<BuildingEnabledButton>();
		bool flag = component != null && !component.IsEnabled;
		this.filteredStorage.SetEnabled(!flag);
		Game.Instance.userMenu.Refresh(base.gameObject);
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
			GameUtil.MassUnit massUnit = GameUtil.massUnit;
			LocString locString;
			if (massUnit != GameUtil.MassUnit.Pounds)
			{
				if (massUnit != GameUtil.MassUnit.Kilograms)
				{
				}
				locString = UI.UNITSUFFIXES.MASS.KILOGRAM;
			}
			else
			{
				locString = UI.UNITSUFFIXES.MASS.POUND;
			}
			return locString;
		}
	}

	private LoggerFS log;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	protected FilteredStorage filteredStorage;
}
