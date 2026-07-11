using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Refrigerator : KMonoBehaviour, IUserControlledCapacity, IEffectDescriptor, IGameObjectEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		this.filteredStorage = new FilteredStorage(this, null, new Tag[] { GameTags.MarkedForCompost }, this, true, Db.Get().ChoreTypes.FoodFetch);
	}

	protected override void OnSpawn()
	{
		this.operational.SetActive(this.operational.IsOperational, false);
		base.GetComponent<KAnimControllerBase>().Play("off", KAnim.PlayMode.Once, 1f, 0f);
		this.filteredStorage.FilterChanged();
		this.temperatureAdjuster = new SimulatedTemperatureAdjuster(this.simulatedInternalTemperature, this.simulatedInternalHeatCapacity, this.simulatedThermalConductivity, base.GetComponent<Storage>());
		this.UpdateLogicCircuit();
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
		base.Subscribe(-1697596308, new Action<object>(this.UpdateLogicCircuitCB));
		base.Subscribe(-592767678, new Action<object>(this.UpdateLogicCircuitCB));
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

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private LogicPorts ports;

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
