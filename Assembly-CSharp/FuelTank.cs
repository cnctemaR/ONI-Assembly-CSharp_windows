using System;
using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

public class FuelTank : Storage, IUserControlledCapacity
{
	public bool IsSuspended
	{
		get
		{
			return this.isSuspended;
		}
	}

	public float UserMaxCapacity
	{
		get
		{
			return this.targetFillMass;
		}
		set
		{
			this.targetFillMass = value;
			this.capacityKg = this.targetFillMass;
			ConduitConsumer component = base.GetComponent<ConduitConsumer>();
			if (component != null)
			{
				component.capacityKG = this.targetFillMass;
			}
			ManualDeliveryKG component2 = base.GetComponent<ManualDeliveryKG>();
			if (component2 != null)
			{
				component2.capacity = (component2.refillMass = this.targetFillMass);
			}
			base.Trigger(-945020481, this);
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
			return 900f;
		}
	}

	public float AmountStored
	{
		get
		{
			return base.MassStored();
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

	public Tag FuelType
	{
		get
		{
			return this.fuelType;
		}
		set
		{
			this.fuelType = value;
			if (this.storageFilters == null)
			{
				this.storageFilters = new List<Tag>();
			}
			this.storageFilters.Add(this.fuelType);
			ManualDeliveryKG component = base.GetComponent<ManualDeliveryKG>();
			if (component != null)
			{
				component.requestedItemTag = this.fuelType;
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		base.gameObject.Subscribe(1366341636, new Action<object>(this.OnReturn));
		this.UserMaxCapacity = this.UserMaxCapacity;
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
		base.Subscribe(-1697596308, delegate(object data)
		{
			this.meter.SetPositionPercent(base.MassStored() / this.capacityKg);
		});
	}

	public void FillTank()
	{
		RocketEngine rocketEngine = null;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
		{
			rocketEngine = gameObject.GetComponent<RocketEngine>();
			if (rocketEngine != null && rocketEngine.mainEngine)
			{
				break;
			}
		}
		if (rocketEngine != null)
		{
			base.AddLiquid(ElementLoader.GetElementID(rocketEngine.fuelTag), this.targetFillMass - base.MassStored(), ElementLoader.GetElement(rocketEngine.fuelTag).defaultValues.temperature, 0, 0, false, true);
		}
		else
		{
			global::Debug.LogWarning("Fuel tank couldn't find rocket engine", null);
		}
	}

	private void OnReturn(object data)
	{
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			Util.KDestroyGameObject(this.items[i]);
		}
		this.items.Clear();
	}

	private bool isSuspended;

	private MeterController meter;

	[Serialize]
	public float targetFillMass = BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];

	[SerializeField]
	private Tag fuelType;

	public float minimumLaunchMass = BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];
}
