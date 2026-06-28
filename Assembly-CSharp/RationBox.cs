using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class RationBox : KMonoBehaviour, IUserControlledCapacity
{
	protected override void OnPrefabInit()
	{
		this.filteredStorage = new FilteredStorage(this, new Tag[] { GameTags.MarkedForCompost }, this.filterTint, this.noFilterTint, this);
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
	}

	protected override void OnSpawn()
	{
		Operational component = base.GetComponent<Operational>();
		component.SetActive(component.IsOperational, false);
		this.handle = GameScheduler.Instance.SchedulePeriodic(base.name, 0.5f, new Action<object>(this.UpdatePreservationStatusItems), null, null, 0f, null);
		this.filteredStorage.FilterChanged();
	}

	protected override void OnCleanUp()
	{
		this.filteredStorage.CleanUp();
		this.handle.ClearScheduler();
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

	private void UpdatePreservationStatusItems(object data)
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

	public LocString CapacityUnits
	{
		get
		{
			GameUtil.MassUnit massUnit = GameUtil.massUnit;
			if (massUnit != GameUtil.MassUnit.Kilograms)
			{
				if (massUnit == GameUtil.MassUnit.Pounds)
				{
					return UI.UNITSUFFIXES.MASS.POUND;
				}
			}
			return UI.UNITSUFFIXES.MASS.KILOGRAM;
		}
	}

	[MyCmpReq]
	private Storage storage;

	[SerializeField]
	public Color noFilterTint = Color.white;

	[SerializeField]
	public Color filterTint = Color.white;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	private SchedulerHandle handle;

	private FilteredStorage filteredStorage;
}
