using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Refrigerator : KMonoBehaviour, IUserControlledCapacity, IGameObjectEffectDescriptor, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		this.filteredStorage = new FilteredStorage(this, new Tag[] { GameTags.MarkedForCompost }, this.filterTint, this.noFilterTint, this);
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
	}

	protected override void OnSpawn()
	{
		this.operational.SetActive(this.operational.IsOperational, false);
		base.GetComponent<KAnimControllerBase>().Play("off", KAnim.PlayMode.Once, 1f, 0f);
		this.filteredStorage.FilterChanged();
		this.temperatureAdjuster = new SimulatedTemperatureAdjuster(this.simulatedInternalTemperature, this.simulatedInternalHeatCapacity, this.simulatedThermalConductivity, base.GetComponent<Storage>());
	}

	protected override void OnCleanUp()
	{
		this.filteredStorage.CleanUp();
		this.temperatureAdjuster.CleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		bool isOperational = this.operational.IsOperational;
		this.operational.SetActive(isOperational, false);
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

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return this.GetDescriptors(def.BuildingComplete);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return SimulatedTemperatureAdjuster.GetDescriptors(this.simulatedInternalTemperature);
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
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	[SerializeField]
	public Color noFilterTint = Color.white;

	[SerializeField]
	public Color filterTint = Color.white;

	[SerializeField]
	public float simulatedInternalTemperature = 277.15f;

	[SerializeField]
	public float simulatedInternalHeatCapacity = 400f;

	[SerializeField]
	public float simulatedThermalConductivity = 1000f;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	private FilteredStorage filteredStorage;

	private SimulatedTemperatureAdjuster temperatureAdjuster;
}
