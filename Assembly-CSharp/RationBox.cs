using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class RationBox : KMonoBehaviour, IUserControlledCapacity, IRender1000ms
{
	protected override void OnPrefabInit()
	{
		this.filteredStorage = new FilteredStorage(this, null, new Tag[] { GameTags.MarkedForCompost }, this, false, Db.Get().ChoreTypes.FoodFetch);
		base.Subscribe<RationBox>(-592767678, RationBox.OnOperationalChangedDelegate);
		base.Subscribe<RationBox>(-905833192, RationBox.OnCopySettingsDelegate);
		WorldInventory.Instance.Discover("FieldRation".ToTag(), GameTags.Edible);
	}

	protected override void OnSpawn()
	{
		Operational component = base.GetComponent<Operational>();
		component.SetActive(component.IsOperational, false);
		this.filteredStorage.FilterChanged();
	}

	protected override void OnCleanUp()
	{
		this.filteredStorage.CleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		Operational component = base.GetComponent<Operational>();
		component.SetActive(component.IsOperational, false);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		RationBox component = gameObject.GetComponent<RationBox>();
		if (component == null)
		{
			return;
		}
		this.UserMaxCapacity = component.UserMaxCapacity;
	}

	public void Render1000ms(float dt)
	{
		Rottable.SetStatusItems(base.GetComponent<KSelectable>(), Rottable.IsRefrigerated(base.gameObject), Rottable.AtmosphereQuality(base.gameObject));
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

	[MyCmpReq]
	private Storage storage;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	private FilteredStorage filteredStorage;

	private static readonly EventSystem.IntraObjectHandler<RationBox> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<RationBox>(delegate(RationBox component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RationBox> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<RationBox>(delegate(RationBox component, object data)
	{
		component.OnCopySettings(data);
	});
}
